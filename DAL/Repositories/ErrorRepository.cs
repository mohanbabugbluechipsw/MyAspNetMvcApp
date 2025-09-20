

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DAL.IRepositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Model_New.Models;

namespace DAL.Repositories
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly string _connectionString;

        public ErrorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public async Task AddErrorAsync(ApplicationErrorLog errorLog)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                //string sql = @"INSERT INTO TblAppError_log 
                //       (AppErrorId, Message, StackTrace, Endpoint, OccurredAt, HttpMethod, UserAgent, UserIP, Status, RSCode, OutletCode, ReviewDetailsJson)
                //       VALUES 
                //       (@AppErrorId, @Message, @StackTrace, @Endpoint, @OccurredAt, @HttpMethod, @UserAgent, @UserIP, @Status, @RSCode, @OutletCode, @ReviewDetailsJson)";

                string sql = @"INSERT INTO TblAppError_log 
               (AppErrorId, Message, StackTrace, Endpoint, OccurredAt, HttpMethod, UserAgent, UserIP, Status, RSCode, OutletCode, ReviewDetailsJson)
               VALUES 
               (@AppErrorId, @Message, @StackTrace, @Endpoint, @OccurredAt, @HttpMethod, @UserAgent, @UserIP, @Status, @RSCode, @OutletCode, @ReviewDetailsJson)";




                await db.ExecuteAsync(sql, new
                {
                    AppErrorId = errorLog.ErrorLogId,
                    Message = errorLog.ErrorMessage,
                    StackTrace = errorLog.StackTrace,
                    Endpoint = errorLog.Endpoint,
                    OccurredAt = errorLog.LoggedAt,
                    HttpMethod = errorLog.HttpMethod,
                    UserAgent = errorLog.UserAgent,
                    UserIP = errorLog.UserIP,
                    Status = errorLog.Status ?? "New",
                    RSCode = errorLog.RSCode,
                    OutletCode = errorLog.OutletCode,
                    ReviewDetailsJson = errorLog.ReviewDetailsJson
                });
            }
        }



        public async Task<ApplicationErrorLog> GetErrorByIdAsync(Guid errorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM TblAppError_log WHERE AppErrorId = @ErrorLogId";

                return await db.QueryFirstOrDefaultAsync<ApplicationErrorLog>(sql, new { ErrorLogId = errorId });
            }
        }

        public async Task UpdateErrorAsync(ApplicationErrorLog errorLog)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE TblAppError_log 
                               SET Status = @Status
                               WHERE AppErrorId = @AppErrorId";

                await db.ExecuteAsync(sql, new
                {
                    Status = errorLog.Status,
                    AppErrorId = errorLog.ErrorLogId
                });
            }
        }

        public async Task<List<ApplicationErrorLog>> GetErrorsByStatusAsync(string[] statuses)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM TblAppError_log WHERE Status IN @Statuses";

                var result = await db.QueryAsync<ApplicationErrorLog>(sql, new { Statuses = statuses });

                return result.ToList();
            }
        }


        public async Task<List<ApplicationErrorLog>> GetPendingErrorsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TblAppError_log WHERE Status IN ('Pending', 'In Progress')";

                var errors = await connection.QueryAsync<ApplicationErrorLog>(query);
                return errors.ToList();
            }
        }

    


    }
}
