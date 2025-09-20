using DAL.IRepositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Model_New.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class VisitorReviewReportRepository : IVisitorReviewReportRepository
    {
        private readonly IDbConnection _connection;

        public VisitorReviewReportRepository(IConfiguration config)
        {
            _connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<VisitorLoginDurationDto>> GetTodayLoginDurationReportAsync()
        {
            var sql = @"
            SELECT 
                Id,
                UserId,
                Username,
                BlobUrl,
                CaptureDate,
                LogoutDate,
                COALESCE(LastActivity, LogoutDate, CaptureDate) AS LastActivityComputed,
                CAST(DATEDIFF(SECOND, CaptureDate, COALESCE(LastActivity, LogoutDate, CaptureDate)) / 3600 AS VARCHAR) + ' hour ' +
                CAST((DATEDIFF(SECOND, CaptureDate, COALESCE(LastActivity, LogoutDate, CaptureDate)) % 3600) / 60 AS VARCHAR) + ' min ' +
                CAST(DATEDIFF(SECOND, CaptureDate, COALESCE(LastActivity, LogoutDate, CaptureDate)) % 60 AS VARCHAR) + ' sec' AS Duration_HMS
            FROM Tbl_LogindetailsCapture
            WHERE CAST(CaptureDate AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY COALESCE(LastActivity, LogoutDate, CaptureDate) DESC;";

            return await _connection.QueryAsync<VisitorLoginDurationDto>(sql);
        }

        public async Task<IEnumerable<VisitorReportDto>> GetTodayReviewReportAsync()
        {
            var sql = "EXEC sp_InsertStoreVisitLog"; // Call the stored procedure

            return await _connection.QueryAsync<VisitorReportDto>(sql);
        }

        //public async Task<IEnumerable<VisitorReportDto>> GetTodayReviewReportAsync()
        //{
        //    var sql = @"
        //        SELECT  
        //            s.MrName,
        //            s.Rscode,
        //            s.SrName,
        //            s.SrCode,
        //            s.RouteName,
        //            s.OutletCode,
        //            s.OutletName,
        //            s.ChildParty,
        //            s.ServicingPLG,
        //            FORMAT(MIN(s.CreatedAt), 'hh:mm tt') AS ReviewStartTime,
        //            FORMAT(MAX(a.CreatedDate), 'hh:mm tt') AS ReviewEndTime
        //        FROM tbl_Savereview_details s
        //        LEFT JOIN TblStoreVisitAnswer a
        //               ON s.Rscode = a.Rscode
        //              AND s.SrCode = a.SrCode
        //              AND s.OutletCode = a.OutletCode
        //        WHERE CAST(s.CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
        //          AND s.MrName <> 'TestUserMR'
        //          AND CAST(a.CreatedDate AS DATE) = CAST(GETDATE() AS DATE)
        //        GROUP BY 
        //            s.MrName, s.Rscode, s.SrName, s.SrCode, s.RouteName,
        //            s.OutletCode, s.OutletName, s.ChildParty, s.ServicingPLG
        //        ORDER BY ReviewStartTime ASC";

        //    return await _connection.QueryAsync<VisitorReportDto>(sql);
        //}



        public class VisitorLoginDurationDto1
        {
            public string UserId { get; set; } = null!;
            public string EmpName { get; set; } = null!;
            public string EmpEmail { get; set; } = null!;
            public string Username { get; set; } = null!;
            public DateTime FirstLogin { get; set; }
            public DateTime LastActivity { get; set; }
            public string Duration_HMS { get; set; } = null!;
        }

        public async Task<IEnumerable<VisitorLoginDurationDto1>> GetTodayLoginDurationReportAsync1()
        {
            var sql = @"
        SELECT 

Top 2
            l.UserId,
            u.EmpName,
            u.EmpEmail,
            l.Username,
            MIN(l.CaptureDate) AS FirstLogin,
            MAX(COALESCE(l.LastActivity, l.LogoutDate, l.CaptureDate)) AS LastActivity,
            CAST(SUM(DATEDIFF(SECOND, l.CaptureDate, COALESCE(l.LastActivity, l.LogoutDate, l.CaptureDate))) / 3600 AS VARCHAR) + ' hour ' +
            CAST((SUM(DATEDIFF(SECOND, l.CaptureDate, COALESCE(l.LastActivity, l.LogoutDate, l.CaptureDate))) % 3600) / 60 AS VARCHAR) + ' min ' +
            CAST(SUM(DATEDIFF(SECOND, l.CaptureDate, COALESCE(l.LastActivity, l.LogoutDate, l.CaptureDate))) % 60 AS VARCHAR) + ' sec' AS Duration_HMS
        FROM Tbl_LogindetailsCapture l
        LEFT JOIN Tbl_User u ON l.UserId = u.EmpNo
        WHERE CAST(l.CaptureDate AS DATE) = CAST(GETDATE() AS DATE)
        GROUP BY l.UserId, u.EmpName, u.EmpEmail, l.Username
        ORDER BY LastActivity DESC;";

            return await _connection.QueryAsync<VisitorLoginDurationDto1>(sql);
        }


    }
}
