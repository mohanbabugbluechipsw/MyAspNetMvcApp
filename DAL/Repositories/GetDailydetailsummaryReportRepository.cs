


//using DAL.IRepositories;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using Model_New.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DAL.Repositories
//{
//    public class GetDailydetailsummaryReportRepository : IGetDailydetailsummaryReportRepository
//    {
//        private readonly IConfiguration _configuration;

//        public GetDailydetailsummaryReportRepository(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<List<Tbl_DetailsummaryReportDto>> GetOutletStatusAsync(string mrName, DateTime startDate, DateTime endDate)
//        {
//            var result = new List<Tbl_DetailsummaryReportDto>();

//            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//            {
//                using (SqlCommand cmd = new SqlCommand("Tbl__GetdetailsummaryDailyStatus", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@MrName", mrName);
//                    cmd.Parameters.AddWithValue("@StartDate", startDate);
//                    cmd.Parameters.AddWithValue("@EndDate", endDate);

//                    await conn.OpenAsync();

//                    using (var reader = await cmd.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            result.Add(new Tbl_DetailsummaryReportDto
//                            {
//                                RS_Code = reader["Rscode"].ToString(),
//                                OutletCode = reader["OutletCode"].ToString(),
//                                SrName = reader["SrName"].ToString(),
//                                SrCode = reader["SrCode"].ToString(),
//                                RouteName = reader["RouteName"].ToString(),
//                                ChildParty = reader["ChildParty"].ToString(),
//                                ServicingPLG = reader["ServicingPLG"].ToString(),
//                                StartTime = reader["StartTime"]?.ToString(),
//                                EndTime = reader["EndTime"]?.ToString(),
//                                Status = reader["Status"].ToString(),
//                                TotalMinutes = reader["TotalMinutes"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TotalMinutes"]),
//                                TotalTimeSpent = reader["TotalTimeSpent"]?.ToString(),
//                                Rn = reader["rn"] == DBNull.Value ? 0 : Convert.ToInt32(reader["rn"])
//                            });
//                        }
//                    }

//                }
//            }

//            return result;
//        }

//        public async Task<List<Tbl_DetailsummaryReportDto>> GetFilteredDataAsync(
//            string mrName,
//            string startDate,
//            string endDate,
//            string sortColumn,
//            string sortOrder)
//        {
//            DateTime fromDate = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
//            DateTime toDate = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

//            var data = await GetOutletStatusAsync(mrName, fromDate, toDate);

//            IEnumerable<Tbl_DetailsummaryReportDto> sortedData = sortColumn switch
//            {
//                "RS_Code" => sortOrder == "asc" ? data.OrderBy(r => r.RS_Code) : data.OrderByDescending(r => r.RS_Code),
//                "OutletCode" => sortOrder == "asc" ? data.OrderBy(r => r.OutletCode) : data.OrderByDescending(r => r.OutletCode),
//                "Status" => sortOrder == "asc" ? data.OrderBy(r => r.Status) : data.OrderByDescending(r => r.Status),
//                "StartTime" => sortOrder == "asc" ? data.OrderBy(r => r.StartTime) : data.OrderByDescending(r => r.StartTime),
//                "EndTime" => sortOrder == "asc" ? data.OrderBy(r => r.EndTime) : data.OrderByDescending(r => r.EndTime),

//            };

//            return sortedData.ToList();
//        }
//    }
//}


using DAL.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GetDailydetailsummaryReportRepository : IGetDailydetailsummaryReportRepository
    {
        private readonly IConfiguration _configuration;

        public GetDailydetailsummaryReportRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Tbl_DetailsummaryReportDto>> GetOutletStatusAsync(string mrName, DateTime startDate, DateTime endDate)
        {
            var result = new List<Tbl_DetailsummaryReportDto>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand("Tbl__GetdetailsummaryDailyStatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MrName", mrName);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new Tbl_DetailsummaryReportDto
                        {
                            RS_Code = reader["Rscode"]?.ToString(),
                            OutletCode = reader["OutletCode"]?.ToString(),
                            MrName = reader["MrName"]?.ToString(),
                            OutletName = reader["OutletName"]?.ToString(),
                            SrCode = reader["SrCode"]?.ToString(),
                            RouteName = reader["RouteName"]?.ToString(),
                            ChildParty = reader["ChildParty"]?.ToString(),
                            ServicingPLG = reader["ServicingPLG"]?.ToString(),
                            StartDate = reader["StartDate"]?.ToString(),
                            StartTime = reader["StartTime"]?.ToString(),
                            EndDate = reader["EndDate"]?.ToString(),
                            EndTime = reader["EndTime"]?.ToString(),
                            Status = reader["Status"]?.ToString(),
                            TotalTimeSpent = reader["TotalTime"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<Tbl_DetailsummaryReportDto>> GetFilteredDataAsync(
            string mrName,
            string startDate,
            string endDate,
            string sortColumn,
            string sortOrder)
        {
            DateTime fromDate = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var data = await GetOutletStatusAsync(mrName, fromDate, toDate);

            IEnumerable<Tbl_DetailsummaryReportDto> sortedData = sortColumn switch
            {
                "RS_Code" => sortOrder == "asc" ? data.OrderBy(r => r.RS_Code) : data.OrderByDescending(r => r.RS_Code),
                "OutletCode" => sortOrder == "asc" ? data.OrderBy(r => r.OutletCode) : data.OrderByDescending(r => r.OutletCode),
                "Status" => sortOrder == "asc" ? data.OrderBy(r => r.Status) : data.OrderByDescending(r => r.Status),
                "StartTime" => sortOrder == "asc" ? data.OrderBy(r => r.StartTime) : data.OrderByDescending(r => r.StartTime),
                "EndTime" => sortOrder == "asc" ? data.OrderBy(r => r.EndTime) : data.OrderByDescending(r => r.EndTime),
                _ => data
            };

            return sortedData.ToList();
        }
    }
}
