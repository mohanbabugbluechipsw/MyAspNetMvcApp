using DAL.IRepositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ReportRepository: IReportRepository
    {

        private readonly MrAppDbNewContext _context;


        public ReportRepository(MrAppDbNewContext context)
        {
            _context = context;
        }

        //public async Task<VisitStatusChartViewModel> GetVisitStatusSummaryAsync(string mrName, DateTime startDate, DateTime endDate)
        //{
        //    var start = startDate.Date;
        //    var end = endDate.Date;

        //    var query = await (from p in _context.tbl_SRMaster_Datas
        //                       join m in _context.tbl_MRSrMappings on p.RS_Code equals m.Rs_code
        //                       where m.MrName == mrName
        //                       join v in (
        //                           from t in _context.tblcapturedatalog
        //                           where t.UploadedAt.Date >= start && t.UploadedAt.Date <= end
        //                           group t by t.Outlet into g
        //                           select new { Outlet = g.Key }
        //                       ) on p.Party_HUL_Code equals v.Outlet into visitJoin
        //                       from v in visitJoin.DefaultIfEmpty()
        //                       join c in (
        //                           from t in _context.tbl_FSWSdetails
        //                           where t.CreatedAt.Date >= start && t.CreatedAt.Date <= end
        //                           group t by t.Outlet_code into g
        //                           select new { Outlet_code = g.Key }
        //                       ) on p.Party_HUL_Code equals c.Outlet_code into completeJoin
        //                       from c in completeJoin.DefaultIfEmpty()
        //                       select new
        //                       {
        //                           Completed = c != null,
        //                           Visited = c == null && v != null,
        //                           NotVisited = c == null && v == null
        //                       }).ToListAsync();

        //    return new VisitStatusChartViewModel
        //    {
        //        TotalCount = query.Count,
        //        CompletedCount = query.Count(x => x.Completed),
        //        VisitedCount = query.Count(x => x.Visited),
        //        NotVisitedCount = query.Count(x => x.NotVisited)
        //    };
        //}


    //    public async Task<VisitStatusChartViewModel> GetVisitStatusSummaryAsync(string mrName, DateTime startDate, DateTime endDate)
    //    {
    //        var sql = "EXEC sp_GetVisitStatusSummary @MrName, @StartDate, @EndDate";

    //        var parameters = new[]
    //        {
    //    new SqlParameter("@MrName", SqlDbType.NVarChar) { Value = mrName },
    //    new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate },
    //    new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate }
    //};

    //        var result = await _context.VisitStatusChartViewModel
    //            .FromSqlRaw(sql, parameters)
    //            .AsNoTracking()
    //            .FirstOrDefaultAsync();

    //        return result;
    //    }


        public async Task<VisitStatusChartViewModel> GetVisitStatusSummaryAsync(string mrName, DateTime startDate, DateTime endDate)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                var parameters = new { MrName = mrName, StartDate = startDate, EndDate = endDate };
                var result = await connection.QueryFirstOrDefaultAsync<VisitStatusChartViewModel>(
                    "sp_GetVisitStatusSummary", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }



    }
}
