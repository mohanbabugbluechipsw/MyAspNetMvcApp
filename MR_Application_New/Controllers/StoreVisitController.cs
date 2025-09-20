using DAL.IRepositories;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model_New.Models;
using Model_New.ViewModels;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Security.Claims;
using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MR_Application_New.Controllers
{
    public class StoreVisitController : Controller
    {
        private readonly IStoreVisitQuestionRepository _repo;
        private readonly ILogger<StoreVisitController> _logger;
        private readonly IConfiguration _configuration;

        public StoreVisitController(
            IStoreVisitQuestionRepository repo,
            ILogger<StoreVisitController> logger,
            IConfiguration configuration)
        {
            _repo = repo;
            _logger = logger;
            _configuration = configuration;
        }


        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;

            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var errorSql = @"
        INSERT INTO tbl_ErrorLog (
            ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
            ReviewId, ServerName, ApplicationName, CreatedAt
        )
        VALUES (
            @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
            @ReviewId, @ServerName, @ApplicationName, SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time'
        );";

            await connection.ExecuteAsync(errorSql, new
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ErrorCode = ex.HResult,
                ErrorMessage = ex.Message,
                StackTrace = ex.ToString(),
                ReviewId = reviewId,
                ServerName = Environment.MachineName,
                ApplicationName = "MR_Application_New"
            });
        }


        public async Task<IActionResult> PreVisit(string channelType)
        {
            _logger.LogInformation("Loading Pre-Visit form for ChannelType: {channelType}", channelType);

            var questions = await _repo.GetActiveQuestionsAsync("Pre-Visit", channelType);

            var model = new StorePreVisitFormModel
            {
                VisitId = int.Parse(DateTime.UtcNow.Ticks.ToString().Substring(10)),
                VisitType = "Pre-Visit",
                ChannelType = channelType,
                Answers = questions.Select(q => new StoreVisitAnswerModel
                {
                    QuestionId = q.QuestionId,
                    Text = q.Text
                }).ToList()
            };

            ViewBag.QuestionJson = JsonConvert.SerializeObject(model.Answers);
            _logger.LogInformation("Loaded {count} questions for ChannelType: {channelType}", model.Answers.Count, channelType);

            return View(model);
        }




        [HttpPost]
        [RequestSizeLimit(1_700_000_000)]
        public async Task<IActionResult> PreVisitJson([FromBody] StorePreVisitFormModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            string userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Unknown";
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";
            string userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

            _logger.LogInformation("Pre-Visit submitted by UserId: {UserId}, UserName: {UserName}, Role: {UserRole}",
                userId, userName, userRole);
            _logger.LogInformation("Received Pre-Visit data for VisitId: {VisitId}", model?.VisitId);

            if (HttpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogWarning("Client disconnected before PreVisitJson completed.");
                return StatusCode(499, "Client closed the connection.");
            }

            if (model == null || model.Answers == null || !model.Answers.Any())
            {
                _logger.LogWarning("Pre-Visit submission failed: No answers found.");
                return Json(new { success = false, message = "Invalid Pre-Visit data. No answers found." });
            }

            // Fetch context from database using ReviewId
            ReviewContextDto reviewContext;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                reviewContext = await connection.QueryFirstOrDefaultAsync<ReviewContextDto>(
                    @"SELECT Rscode, SrName, SrCode, RouteName, OutletCode, OutletName, 
                     OutletSubType, OutletAddress, ChildParty, ServicingPLG, 
                     Latitude, Longitude, DeviceInfo, DeviceType
              FROM tbl_Savereview_details 
              WHERE ReviewId = @ReviewId",
                    new { ReviewId = model.ReviewId });
            }

            if (reviewContext == null)
            {
                _logger.LogWarning("Review context not found for ReviewId: {ReviewId}", model.ReviewId);
                return Json(new
                {
                    success = false,
                    message = "Review context expired or invalid. Please restart the visit."
                });
            }

            // Validate required fields
            var validationResult = ValidateReviewContext(reviewContext);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Missing fields in review {ReviewId}: {Fields}",
                    model.ReviewId, validationResult.MissingFields);

                return Json(new
                {
                    success = false,
                    message = $"Missing context data: {validationResult.MissingFields}. Please restart visit."
                });
            }

            var visitGuid = Guid.NewGuid();


            try
            {
                var visitAnswers = new List<TblStoreVisitAnswer>();



                foreach (var ans in model.Answers)
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        _logger.LogWarning("Client disconnected during Pre-Visit processing.");
                        return StatusCode(499, "Client closed the connection.");
                    }

                    if (string.IsNullOrEmpty(ans.BlobUrl))
                    {
                        _logger.LogInformation("No BlobUrl provided for QuestionId: {QuestionId}, saving as null.", ans.QuestionId);
                    }

                    visitAnswers.Add(new TblStoreVisitAnswer
                    {
                        VisitId = model.VisitId,
                        QuestionId = ans.QuestionId,
                        VisitType = model.VisitType,
                        BlobUrl = string.IsNullOrEmpty(ans.BlobUrl) ? null : StripSas(ans.BlobUrl),
                        ChannelType = model.ChannelType,
                        IsNew = ans.IsNew,
                        CreatedDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time"),
                        Rscode = reviewContext.Rscode,
                        SrName = reviewContext.SrName,
                        SrCode = reviewContext.SrCode,
                        RouteName = reviewContext.RouteName,
                        OutletCode = reviewContext.OutletCode,
                        OutletName = reviewContext.OutletName,
                        OutletSubType = reviewContext.OutletSubType,
                        OutletAddress = reviewContext.OutletAddress,
                        ChildParty = reviewContext.ChildParty,
                        ServicingPLG = reviewContext.ServicingPLG,
                        Latitude = reviewContext.Latitude,
                        Longitude = reviewContext.Longitude,
                        DeviceInfo = reviewContext.DeviceInfo,
                        DeviceType = reviewContext.DeviceType,
                        UserId = userId,
                        UserName = userName,
                        UserEmail = userEmail,
                        RowGuid = visitGuid

                    });
                }

                await _repo.SaveVisitAnswersAsync(visitAnswers);
                _logger.LogInformation(
                    "PreVisitJson started | RowGuid: {RowGuid}, VisitId: {VisitId}, VisitType: {VisitType}, ChannelType: {ChannelType}, " +
                    "Rscode: {Rscode}, SrName: {SrName}, SrCode: {SrCode}, RouteName: {RouteName}, OutletCode: {OutletCode}, " +
                    "OutletName: {OutletName}, OutletSubType: {OutletSubType}, OutletAddress: {OutletAddress}, " +
                    "UserId: {UserId}, UserName: {UserName}, UserEmail: {UserEmail}",
                    visitGuid,
                    model.VisitId,
                    model.VisitType,
                    model.ChannelType,
                    reviewContext.Rscode,
                    reviewContext.SrName,
                    reviewContext.SrCode,
                    reviewContext.RouteName,
                    reviewContext.OutletCode,
                    reviewContext.OutletName,
                    reviewContext.OutletSubType,
                    reviewContext.OutletAddress,
                    userId,
                    userName,
                    userEmail
                );
                return Json(new { success = true, message = "Pre-Visit data saved successfully.", rowGuid = visitGuid });

                //return Json(new { success = true, message = "Pre-Visit data saved successfully." });
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex, nameof(PreVisitJson), model?.ReviewId);

                var errorDetails = $@"
Error while saving Pre-Visit data.
RowGuid: {visitGuid},
VisitId: {model.VisitId},
VisitType: {model.VisitType},
ChannelType: {model.ChannelType},
Rscode: {reviewContext?.Rscode},
SrName: {reviewContext?.SrName},
SrCode: {reviewContext?.SrCode},
RouteName: {reviewContext?.RouteName},
OutletCode: {reviewContext?.OutletCode},
OutletName: {reviewContext?.OutletName},
OutletSubType: {reviewContext?.OutletSubType},
OutletAddress: {reviewContext?.OutletAddress},
UserId: {userId},
UserName: {userName},
UserEmail: {userEmail}";

                _logger.LogError(ex, errorDetails);

                return Json(new
                {
                    success = false,
                    message = errorDetails, // send full dynamic message back
                    error = ex.Message      // exception message (stack kept in logs)
                });
            }
        }


        public async Task<IActionResult> PostVisit(string channelType)
        {
            _logger.LogInformation("Loading Post-Visit form for ChannelType: {ChannelType}", channelType);

            var questions = await _repo.GetActiveQuestionsAsync("Post-Visit", channelType);

            var model = new StorePostVisitFormModel
            {
                VisitId = DateTime.UtcNow.Ticks.ToString().Substring(10),
                VisitType = "Post-Visit",
                ChannelType = channelType,
                Answers = questions.Select(q => new StorePostVisitAnswer
                {
                    QuestionId = q.QuestionId,
                    Text = q.Text
                }).ToList()
            };

            ViewBag.QuestionJson = JsonConvert.SerializeObject(model.Answers);
            _logger.LogInformation("Loaded {count} questions for ChannelType: {channelType}", model.Answers.Count, channelType);

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(1_700_000_000)]
        public async Task<IActionResult> PostVisitJson([FromBody] StorePostVisitFormModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            string userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
            string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
            string userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

            _logger.LogInformation("Post-Visit submitted by UserId: {UserId}, UserName: {UserName}, Role: {UserRole}",
                userId, userName, userRole);

            if (model == null || model.Answers == null || !model.Answers.Any())
            {
                _logger.LogWarning("Post-Visit submission failed: No answers found.");
                return BadRequest(new { message = "Invalid Post-Visit data. No answers found." });
            }

            if (!int.TryParse(model.VisitId, out int visitId))
            {
                _logger.LogWarning("Invalid VisitId format: {VisitId}", model.VisitId);
                return BadRequest(new { message = "Invalid VisitId format." });
            }

            // Fetch context from database using ReviewId
            ReviewContextDto reviewContext;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                reviewContext = await connection.QueryFirstOrDefaultAsync<ReviewContextDto>(
                    @"SELECT Rscode, SrName, SrCode, RouteName, OutletCode, OutletName, 
                     OutletSubType, OutletAddress, ChildParty, ServicingPLG, 
                     Latitude, Longitude, DeviceInfo, DeviceType
              FROM tbl_Savereview_details 
              WHERE ReviewId = @ReviewId",
                    new { ReviewId = model.ReviewId });
            }

            if (reviewContext == null)
            {
                _logger.LogWarning("Review context not found for ReviewId: {ReviewId}", model.ReviewId);
                return BadRequest(new
                {
                    message = "Review context expired or invalid. Please restart the visit."
                });
            }

            // Validate required fields
            var validationResult = ValidateReviewContext(reviewContext);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Missing fields in review {ReviewId}: {Fields}",
                    model.ReviewId, validationResult.MissingFields);

                return BadRequest(new
                {
                    message = $"Missing context data: {validationResult.MissingFields}. Please restart visit."
                });
            }

            var visitAnswers = new List<TblStoreVisitAnswer>();
            foreach (var ans in model.Answers)
            {
                if (string.IsNullOrEmpty(ans.BlobUrl))
                {
                    _logger.LogWarning("BlobUrl missing for QuestionId: {QuestionId}", ans.QuestionId);
                    return BadRequest(new { message = $"Photo missing for question: {ans.QuestionId}" });
                }

                visitAnswers.Add(new TblStoreVisitAnswer
                {
                    VisitId = visitId,
                    QuestionId = ans.QuestionId,
                    VisitType = model.VisitType,
                    BlobUrl = StripSas(ans.BlobUrl),
                    CreatedDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "India Standard Time"),
                    Rscode = reviewContext.Rscode,
                    SrName = reviewContext.SrName,
                    SrCode = reviewContext.SrCode,
                    RouteName = reviewContext.RouteName,
                    OutletCode = reviewContext.OutletCode,
                    OutletName = reviewContext.OutletName,
                    OutletSubType = reviewContext.OutletSubType,
                    OutletAddress = reviewContext.OutletAddress,
                    ChildParty = reviewContext.ChildParty,
                    ServicingPLG = reviewContext.ServicingPLG,
                    Latitude = reviewContext.Latitude,
                    Longitude = reviewContext.Longitude,
                    DeviceInfo = reviewContext.DeviceInfo,
                    DeviceType = reviewContext.DeviceType ??= "Unknown",
                 UserId = userId,
                    UserName = userName,
                    UserEmail = userEmail,
                    ChannelType = model.ChannelType,
                    IsNew = ans.IsNew,
                    RowGuid = model.RowGuid
                });
            }

            try
            {
                await _repo.SaveVisitAnswersAsync(visitAnswers);
                _logger.LogInformation("Post-Visit data saved for VisitId: {VisitId}", model.VisitId);
                return Ok(new { message = "Post-Visit data saved successfully." });
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex, nameof(PostVisitJson), model?.ReviewId);

                _logger.LogError(ex, "Error saving Post-Visit data for VisitId: {VisitId}", model.VisitId);
                return StatusCode(500, new { message = "Error saving Post-Visit answers.", error = ex.Message });
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetLinkedPostVisitQuestions(string channelType, string preVisitGuid)
        {
            var loggedInUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User?.Identity?.Name;
            var userRoles = string.Join(",", User?.FindAll(ClaimTypes.Role)?.Select(r => r.Value) ?? Enumerable.Empty<string>());

            if (string.IsNullOrWhiteSpace(loggedInUserId))
                return Unauthorized(new { Message = "User authentication required" });

            try
            {
                var questionLinks = await _repo.GetRequiredPostVisitQuestionIdsAsync(channelType, preVisitGuid);

                var filtered = questionLinks
                    .Where(x => string.Equals(x.UserId, loggedInUserId, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Log info about the result before returning
                _logger.LogInformation(
                    "Fetched {Count} linked post-visit questions for UserId={UserId}, ChannelType={ChannelType}, PreVisitGuid={PreVisitGuid}",
                    filtered.Count,
                    loggedInUserId,
                    channelType,
                    preVisitGuid
                );

                return Ok(filtered);

            }
            catch (Exception ex)
            {


                await HandleErrorAsync(ex, nameof(GetLinkedPostVisitQuestions));

                var log = new
                {
                    AppErrorId = Guid.NewGuid(),
                    Message = ex.Message,
                    StackTrace = ex.ToString(),
                    Endpoint = HttpContext?.Request?.Path.Value,
                    OccurredAt = DateTime.UtcNow,
                    HttpMethod = HttpContext?.Request?.Method,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    UserIP = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    Status = "500",
                    ResolvedAt = (DateTime?)null,
                    Username = User?.Identity?.Name,
                    UserRoles = string.Join(",", User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value) ?? new List<string>()),
                    RSCode = "", // you can fill later if available
                    OutletCode = "", // fill from logic if needed
                    ReviewDetailsJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        ChannelType = channelType,
                        PreVisitGuid = preVisitGuid,
                        UserId = loggedInUserId
                    })
                };

                await _repo.SaveErrorLogAsync(log);

                _logger.LogError(ex, "Error fetching linked questions. User={UserId}, ChannelType={ChannelType}, PreVisitGuid={PreVisitGuid}",
                    loggedInUserId, channelType, preVisitGuid);

                return StatusCode(500, new { Message = "Error retrieving questions" });
            }

        }

       

        public IActionResult OutletView() => View();
        public IActionResult VisitCompleted() => View();

        #region Helper Methods
        private static string StripSas(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url);
                return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
            }
            catch
            {
                return url;
            }
        }

        private ValidationResult ValidateReviewContext(ReviewContextDto context)
        {
            var requiredProps = new Dictionary<string, string>
            {
                [nameof(context.Rscode)] = context.Rscode,
                [nameof(context.SrName)] = context.SrName,
                [nameof(context.SrCode)] = context.SrCode,
                [nameof(context.RouteName)] = context.RouteName,
                [nameof(context.OutletCode)] = context.OutletCode,
                [nameof(context.OutletName)] = context.OutletName,
                [nameof(context.OutletSubType)] = context.OutletSubType,
                [nameof(context.OutletAddress)] = context.OutletAddress,
                [nameof(context.ChildParty)] = context.ChildParty,
                [nameof(context.ServicingPLG)] = context.ServicingPLG,
                [nameof(context.DeviceInfo)] = context.DeviceInfo,
                //[nameof(context.DeviceType)] = context.DeviceType
            };

            var missingFields = requiredProps
                .Where(f => string.IsNullOrWhiteSpace(f.Value))
                .Select(f => f.Key)
                .ToList();

            return new ValidationResult
            {
                IsValid = !missingFields.Any(),
                MissingFields = string.Join(", ", missingFields)
            };
        }
        #endregion

        #region Models and DTOs
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string MissingFields { get; set; }
        }

        public class StorePreVisitFormModel
        {
            public int VisitId { get; set; }
            public string VisitType { get; set; }
            public string ChannelType { get; set; }
            public List<StoreVisitAnswerModel> Answers { get; set; }
            public Guid ReviewId { get; set; }
        }

        public class StorePostVisitFormModel
        {
            public string VisitId { get; set; }
            public string VisitType { get; set; }
            public string ChannelType { get; set; }
            public List<StorePostVisitAnswer> Answers { get; set; }
            public Guid ReviewId { get; set; }
            public Guid RowGuid { get; set; }



        }

        public class StoreVisitAnswerModel
        {
            public int QuestionId { get; set; }
            public string Text { get; set; }
            public string BlobUrl { get; set; }
            public int IsNew { get; set; }
        }

        public class StorePostVisitAnswer
        {
            public int QuestionId { get; set; }
            public string Text { get; set; }
            public string BlobUrl { get; set; }
            public int IsNew { get; set; }
        }

        public class ReviewContextDto
        {
            public string Rscode { get; set; }
            public string SrName { get; set; }
            public string SrCode { get; set; }
            public string RouteName { get; set; }
            public string OutletCode { get; set; }
            public string OutletName { get; set; }
            public string OutletSubType { get; set; }
            public string OutletAddress { get; set; }
            public string ChildParty { get; set; }
            public string ServicingPLG { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string DeviceInfo { get; set; }
            public string DeviceType { get; set; }
        }
        #endregion
    }
}