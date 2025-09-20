////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////using DAL.IRepositories;
////using Dapper;
////using System.Data;
////using System.Data.SqlClient;
////using Model_New.ViewModels;

////namespace DAL.Repositories
////{
////    public class StoreVisitRepository : IStoreVisitRepository
////    {
////        private readonly string _connectionString;

////        public StoreVisitRepository(string connectionString)
////        {
////            _connectionString = connectionString;
////        }

////        public async Task RunInsertProcedureAsync()
////        {
////            using var connection = new SqlConnection(_connectionString);
////            await connection.ExecuteAsync("[sp_InsertStoreVisitLog]", commandType: CommandType.StoredProcedure);
////        }

////        public async Task<IEnumerable<StoreVisitLogDto>> GetTodayVisitLogsAsync()
////        {
////            using var connection = new SqlConnection(_connectionString);
////            return await connection.QueryAsync<StoreVisitLogDto>(@"

////   SELECT l.*, dr.Rs_Email, dr.Rs_Name
//// FROM tbl_StoreVisitLog l
//// JOIN tbl_Distributor_Regions dr ON l.Rscode = dr.RS_Code
//// WHERE l.TargetDate = CAST(GETDATE() AS DATE) AND dr.Rs_Email IS NOT NULL
////            ");
////        }
////    }
////}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DAL.IRepositories;
//using Dapper;
//using System.Data;
//using System.Data.SqlClient;
//using Model_New.ViewModels;
//using Microsoft.Extensions.Configuration;

//namespace DAL.Repositories
//{
//    public class StoreVisitRepository : IStoreVisitRepository
//    {
//        private readonly string _connectionString;
//        private readonly IConfiguration _configuration;

//        public StoreVisitRepository(string connectionString, IConfiguration configuration)
//        {
//            _connectionString = connectionString;
//            _configuration = configuration;
//        }

//        public async Task RunInsertProcedureAsync()
//        {
//            try
//            {
//                using var connection = new SqlConnection(_connectionString);
//                await connection.OpenAsync();

//                var command = new CommandDefinition(
//                    commandText: "[sp_InsertStoreVisitLog]",
//                    commandType: CommandType.StoredProcedure,
//                    commandTimeout: 7200 // 2 hours
//                );

//                await connection.ExecuteAsync(command);
//            }
//            catch (Exception ex)
//            {
//                await HandleErrorAsync(ex, "RunInsertProcedureAsync");
//                throw; // rethrow so calling code knows it failed
//            }
//        }

//        public async Task<IEnumerable<StoreVisitLogDto>> GetTodayVisitLogsAsync()
//        {
//            try
//            {
//                using var connection = new SqlConnection(_connectionString);
//                await connection.OpenAsync();

//                var command = new CommandDefinition(
//                    commandText: @"
//                        SELECT l.*, dr.Rs_Email, dr.Rs_Name
//                        FROM tbl_StoreVisitLog l
//                        JOIN tbl_Distributor_Regions dr ON l.Rscode = dr.RS_Code
//                        WHERE l.TargetDate = CAST(GETDATE() AS DATE) AND dr.Rs_Email IS NOT NULL
//                    ",
//                    commandType: CommandType.Text,
//                    commandTimeout: 7200 // 2 hours
//                );

//                return await connection.QueryAsync<StoreVisitLogDto>(command);
//            }
//            catch (Exception ex)
//            {
//                await HandleErrorAsync(ex, "GetTodayVisitLogsAsync");
//                return Enumerable.Empty<StoreVisitLogDto>(); // return empty if error
//            }
//        }

//        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
//        {
//            try
//            {
//                using var connection = new SqlConnection(_connectionString);
//                await connection.OpenAsync();

//                var errorSql = @"
//                    INSERT INTO tbl_ErrorLog (
//                        ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
//                        ReviewId, ServerName, ApplicationName, CreatedAt
//                    )
//                    VALUES (
//                        @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
//                        @ReviewId, @ServerName, @ApplicationName, SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time'
//                    );";

//                await connection.ExecuteAsync(errorSql, new
//                {
//                    ControllerName = "StoreVisitRepository",
//                    ActionName = actionName,
//                    ErrorCode = ex.HResult,
//                    ErrorMessage = ex.Message,
//                    StackTrace = ex.ToString(),
//                    ReviewId = reviewId,
//                    ServerName = Environment.MachineName,
//                    ApplicationName = "MR_Application_New"
//                });
//            }
//            catch
//            {
//                // Ignore logging failures to avoid infinite loops
//            }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.IRepositories;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Model_New.ViewModels;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories
{
    public class StoreVisitRepository : IStoreVisitRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public StoreVisitRepository(string connectionString, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _configuration = configuration;
        }

        public async Task RunInsertProcedureAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new CommandDefinition(
                    commandText: "[sp_InsertStoreVisitLog]",
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 7200 // 2 hours
                );

                await connection.ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "RunInsertProcedureAsync");
                throw; // rethrow so calling code knows it failed
            }
        }

        public async Task<IEnumerable<StoreVisitLogDto>> GetTodayVisitLogsAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new CommandDefinition(
                    commandText: @"
                        SELECT l.*, dr.Rs_Email, dr.Rs_Name
                        FROM tbl_StoreVisitLog l
                        JOIN tbl_Distributor_Regions dr ON l.Rscode = dr.RS_Code
                        WHERE l.TargetDate = CAST(GETDATE() AS DATE) AND dr.Rs_Email IS NOT NULL
                    ",
                    commandType: CommandType.Text,
                    commandTimeout: 7200 // 2 hours
                );

                return await connection.QueryAsync<StoreVisitLogDto>(command);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "GetTodayVisitLogsAsync");
                return Enumerable.Empty<StoreVisitLogDto>(); // return empty if error
            }
        }

        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
        {
            try
            {
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
                    ControllerName = "StoreVisitRepository",
                    ActionName = actionName,
                    ErrorCode = ex.HResult,
                    ErrorMessage = ex.Message,
                    StackTrace = ex.ToString(),
                    ReviewId = reviewId,
                    ServerName = Environment.MachineName,
                    ApplicationName = "MR_Application_New"
                });
            }
            catch
            {
                // Ignore logging failures to avoid infinite loops
            }
        }
    }
}

