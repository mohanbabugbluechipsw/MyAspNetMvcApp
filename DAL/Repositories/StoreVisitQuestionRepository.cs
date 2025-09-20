//using DAL.IRepositories;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Model_New.Models;
//using Model_New.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DAL.Repositories
//{
//    public class StoreVisitQuestionRepository : IStoreVisitQuestionRepository
//    {
//        private readonly IDbConnection _db;

//        private const int CommandTimeoutSeconds = 1800; // Set timeout to 2 minutes


//        public StoreVisitQuestionRepository(IConfiguration config)
//        {
//            _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
//        }

//        public async Task<IEnumerable<StoreVisitQuestion>> GetActiveQuestionsAsync(string visitType, string channelType)
//        {
//            var sql = @"SELECT * 
//                FROM tbl_Store_visit_Questions 
//                WHERE IsActive = 1 
//                  AND VisitType = @VisitType 
//                  AND ChannelType = @ChannelType";

//            return await _db.QueryAsync<StoreVisitQuestion>(sql, new { VisitType = visitType, ChannelType = channelType }, commandTimeout: CommandTimeoutSeconds);
//        }








//        public async Task SaveVisitAnswersAsync(List<TblStoreVisitAnswer> answers)
//{
//    var sql = @"
//INSERT INTO TblStoreVisitAnswer 
//(
//    VisitId, QuestionId, VisitType, BlobUrl, CreatedDate,
//    Rscode, SrName, SrCode, RouteName, OutletCode, OutletName, OutletSubType,
//    OutletAddress, ChildParty, ServicingPLG, Latitude, Longitude, DeviceInfo, DeviceType,
//    UserId, UserName, UserEmail,
//    ChannelType, IsNew
//)
//VALUES 
//(
//    @VisitId, @QuestionId, @VisitType, @BlobUrl, @CreatedDate,
//    @Rscode, @SrName, @SrCode, @RouteName, @OutletCode, @OutletName, @OutletSubType,
//    @OutletAddress, @ChildParty, @ServicingPLG, @Latitude, @Longitude, @DeviceInfo, @DeviceType,
//    @UserId, @UserName, @UserEmail,
//    @ChannelType, @IsNew
//);";

//    await _db.ExecuteAsync(sql, answers, commandTimeout: CommandTimeoutSeconds);
//}






//        public async Task SaveOutletViewAnswersAsync(List<TblOutletViewAnswer> answers)
//        {
//            var sql = @"
//INSERT INTO TblOutletViewAnswer
//(
//    OutletCode, VisitType, Format, OptionType, OptionName, IsSelected, CreatedDate,
//    Rscode, SrName, SrCode, RouteName, OutletName, OutletSubType, OutletAddress, ChildParty, ServicingPLG,UserId,UserName,UserEmail
//)
//VALUES
//(
//    @OutletCode, @VisitType, @Format, @OptionType, @OptionName, @IsSelected, @CreatedDate,
//    @Rscode, @SrName, @SrCode, @RouteName, @OutletName, @OutletSubType, @OutletAddress, @ChildParty, @ServicingPLG,@UserId,@UserName,@UserEmail
//);";

//            if (_db.State != ConnectionState.Open)
//            {
//                await _db.OpenAsync();
//            }

//            using (var transaction = _db.BeginTransaction())
//            {
//                try
//                {
//                    await _db.ExecuteAsync(sql, answers, transaction, commandTimeout: CommandTimeoutSeconds);
//                    transaction.Commit();
//                }
//                catch (Exception ex)
//                {
//                    transaction.Rollback();
//                    throw new Exception("Error saving Outlet View Answers", ex);
//                }
//            }
//        }


//        public async Task<List<int>> GetRequiredPostVisitQuestionIdsAsync(string outletCode, string userId, string channelType)
//        {
//            var sql = @"
//        SELECT DISTINCT q.PostVisitQuestionId
//        FROM TblStoreVisitAnswer a
//        INNER JOIN Tbl_QuestionLink q ON a.QuestionId = q.PreVisitQuestionId
//        WHERE a.IsNew = 1
//          AND a.VisitType = 'Pre-Visit'
//          AND a.OutletCode = @OutletCode
//          AND a.UserId = @UserId
//          AND a.ChannelType = @ChannelType
//          AND CAST(a.CreatedDate AS DATE) = CAST(GETDATE() AS DATE)
//    ";

//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var result = await connection.QueryAsync<int>(sql, new
//                {
//                    OutletCode = outletCode,
//                    UserId = userId,
//                    ChannelType = channelType
//                });

//                return result.ToList();
//            }
//        }




//    }

//}

using DAL.IRepositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StoreVisitQuestionRepository : IStoreVisitQuestionRepository
    {
        private readonly IDbConnection _db;
        private const int CommandTimeoutSeconds = 1800;

        public StoreVisitQuestionRepository(IConfiguration config)
        {
            _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<StoreVisitQuestion>> GetActiveQuestionsAsync(string visitType, string channelType)
        {
            var sql = @"SELECT * 
                        FROM tbl_Store_visit_Questions 
                        WHERE IsActive = 1 
                          AND VisitType = @VisitType 
                          AND ChannelType = @ChannelType";

            return await _db.QueryAsync<StoreVisitQuestion>(
                sql,
                new { VisitType = visitType, ChannelType = channelType },
                commandTimeout: CommandTimeoutSeconds);
        }

        public async Task SaveVisitAnswersAsync(List<TblStoreVisitAnswer> answers)
        {
            var sql = @"
INSERT INTO TblStoreVisitAnswer 
(
    VisitId, QuestionId, VisitType, BlobUrl, CreatedDate,
    Rscode, SrName, SrCode, RouteName, OutletCode, OutletName, OutletSubType,
    OutletAddress, ChildParty, ServicingPLG, Latitude, Longitude, DeviceInfo, DeviceType,
    UserId, UserName, UserEmail,
    ChannelType, IsNew,RowGuid
)
VALUES 
(
    @VisitId, @QuestionId, @VisitType, @BlobUrl, @CreatedDate,
    @Rscode, @SrName, @SrCode, @RouteName, @OutletCode, @OutletName, @OutletSubType,
    @OutletAddress, @ChildParty, @ServicingPLG, @Latitude, @Longitude, @DeviceInfo, @DeviceType,
    @UserId, @UserName, @UserEmail,
    @ChannelType, @IsNew,@RowGuid
);";

            await _db.ExecuteAsync(sql, answers, commandTimeout: CommandTimeoutSeconds);
        }

        public async Task SaveOutletViewAnswersAsync(List<TblOutletViewAnswer> answers)
        {
            var sql = @"
INSERT INTO TblOutletViewAnswer
(
    OutletCode, VisitType, Format, OptionType, OptionName, IsSelected, CreatedDate,
    Rscode, SrName, SrCode, RouteName, OutletName, OutletSubType, OutletAddress, ChildParty, ServicingPLG,
    UserId, UserName, UserEmail
)
VALUES
(
    @OutletCode, @VisitType, @Format, @OptionType, @OptionName, @IsSelected, @CreatedDate,
    @Rscode, @SrName, @SrCode, @RouteName, @OutletName, @OutletSubType, @OutletAddress, @ChildParty, @ServicingPLG,
    @UserId, @UserName, @UserEmail
);";

            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    await _db.ExecuteAsync(sql, answers, transaction, commandTimeout: CommandTimeoutSeconds);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error saving Outlet View Answers", ex);
                }
            }
        }

        //        public async Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionLinksAsync(string channelType)
        //        {
        //            var sql = @"
        //SELECT DISTINCT 
        //    q.PostVisitQuestionId,
        //    a.OutletCode,
        //    a.UserId
        //FROM TblStoreVisitAnswer a
        //INNER JOIN Tbl_QuestionLink q ON a.QuestionId = q.PreVisitQuestionId
        //WHERE a.IsNew = 1
        //  AND a.VisitType = 'Pre-Visit'
        //  AND a.ChannelType = @ChannelType
        //  AND CAST(a.CreatedDate AS DATE) = CAST(GETDATE() AS DATE);";

        //            var result = await _db.QueryAsync<QuestionLinkDto>(
        //                sql,
        //                new { ChannelType = channelType },
        //                commandTimeout: CommandTimeoutSeconds);

        //            return result.ToList();
        //        //        }

        //        public async Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionIdsAsync(string channelType)
        //        {
        //            var sql = @"
        //SELECT DISTINCT 
        //    q.PostVisitQuestionId,
        //    a.OutletCode,
        //    a.UserId
        //FROM TblStoreVisitAnswer a
        //INNER JOIN Tbl_QuestionLink q ON a.QuestionId = q.PreVisitQuestionId
        //WHERE a.IsNew = 1
        //  AND a.VisitType = 'Pre-Visit'
        //  AND a.ChannelType = @ChannelType
        //  AND CAST(a.CreatedDate AS DATE) = CAST(GETDATE() AS DATE);";

        //            var result = await _db.QueryAsync<QuestionLinkDto>(
        //                sql,
        //                new { ChannelType = channelType },
        //                commandTimeout: CommandTimeoutSeconds);

        //            return result.ToList();
        //        }



        //        public async Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionIdsAsync(string channelType)
        //        {
        //            var sql = @"
        //WITH LatestReview AS (
        //    SELECT Rscode, SrCode, RouteName, OutletCode, MrName
        //    FROM tbl_Savereview_details
        //    WHERE CreatedAt = (SELECT MAX(CreatedAt) FROM tbl_Savereview_details)
        //),
        //LatestAnswer AS (
        //    SELECT 
        //        q.PostVisitQuestionId,
        //        a.OutletCode,
        //        a.UserId,
        //        a.CreatedDate,
        //a.IsNew ,
        //        ROW_NUMBER() OVER (PARTITION BY q.PostVisitQuestionId ORDER BY a.CreatedDate DESC) AS rn
        //    FROM TblStoreVisitAnswer a
        //    JOIN Tbl_QuestionLink q ON a.QuestionId = q.PreVisitQuestionId
        //    JOIN LatestReview r ON 
        //        a.RsCode = r.Rscode AND
        //        a.SrCode = r.SrCode AND
        //        a.RouteName = r.RouteName AND
        //        a.OutletCode = r.OutletCode AND
        //        a.UserName = r.MrName
        //    WHERE a.IsNew = 1
        //      AND a.VisitType = 'Pre-Visit'
        //      AND a.ChannelType = @channelType
        //      AND CAST(a.CreatedDate AS DATE) = CAST(GETDATE() AS DATE)
        //)
        //SELECT PostVisitQuestionId, OutletCode, UserId,  CreatedDate,IsNew
        //FROM LatestAnswer
        //WHERE rn = 1;
        //";

        //            var result = await _db.QueryAsync<QuestionLinkDto>(
        //                sql,
        //                new { ChannelType = channelType },
        //                commandTimeout: CommandTimeoutSeconds);

        //            return result.ToList();
        //        }



        public async Task SaveErrorLogAsync(object log)
        {
            try
            {
                var sql = @"
            INSERT INTO TblAppError_log
            (AppErrorId, Message, StackTrace, Endpoint, OccurredAt, HttpMethod, 
             UserAgent, UserIP, Status, ResolvedAt, Username, UserRoles, RSCode, OutletCode, ReviewDetailsJson)
            VALUES
            (@AppErrorId, @Message, @StackTrace, @Endpoint, @OccurredAt, @HttpMethod, 
             @UserAgent, @UserIP, @Status, @ResolvedAt, @Username, @UserRoles, @RSCode, @OutletCode, @ReviewDetailsJson)";

                await _db.ExecuteAsync(sql, log, commandTimeout: CommandTimeoutSeconds);
            }
            catch (Exception ex)
            {
                // fallback logging if DB insert fails
                Console.WriteLine($"Failed to insert error log: {ex.Message}");
            }
        }




        public async Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionIdsAsync(string channelType, string preVisitGuid)
        {
            var sql = @"
WITH LatestReview AS (
    SELECT Rscode, SrCode, RouteName, OutletCode, MrName
    FROM tbl_Savereview_details
    WHERE CreatedAt = (SELECT MAX(CreatedAt) FROM tbl_Savereview_details)
),
LatestAnswer AS (
    SELECT 
        q.PostVisitQuestionId,
        a.OutletCode,
        a.UserId,
        a.CreatedDate,
        a.IsNew,
        ROW_NUMBER() OVER (PARTITION BY q.PostVisitQuestionId ORDER BY a.CreatedDate DESC) AS rn
    FROM TblStoreVisitAnswer a
    JOIN Tbl_QuestionLink q ON a.QuestionId = q.PreVisitQuestionId
    JOIN LatestReview r ON 
        a.RsCode = r.Rscode AND
        a.SrCode = r.SrCode AND
        a.RouteName = r.RouteName AND
        a.OutletCode = r.OutletCode AND
        a.UserName = r.MrName
    WHERE a.IsNew = 1
      AND a.VisitType = 'Pre-Visit'
      AND a.ChannelType = @channelType
      AND a.RowGuid = @preVisitGuid
)
SELECT PostVisitQuestionId, OutletCode, UserId, CreatedDate, IsNew
FROM LatestAnswer
WHERE rn = 1;
";

            var result = await _db.QueryAsync<QuestionLinkDto>(
                sql,
                new { ChannelType = channelType, PreVisitGuid = preVisitGuid },
                commandTimeout: CommandTimeoutSeconds);

            return result.ToList();
        }







    }
}
