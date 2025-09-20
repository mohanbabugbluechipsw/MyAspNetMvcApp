using DAL.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class OutletMRSearchRepository : IOutletMRSearchRepository
    {
        private readonly IConfiguration _configuration;

        public OutletMRSearchRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<RSOutletSearchModel>> GetOutletSummaryAsync(string rsCode, string rsName, DateTime fromDate, DateTime toDate)
        {
            var result = new List<RSOutletSearchModel>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("Tbl__GetOutletSummary_ByRS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RsCode", rsCode);
                    cmd.Parameters.AddWithValue("@RsName", rsName);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new RSOutletSearchModel
                            {
                                RS_Code = reader["RS_Code"].ToString(),
                                RS_Name = reader["RS_Name"].ToString(),
                                MrName = reader["MrName"].ToString(),
                                OutletCode = reader["OutletCode"].ToString(),
                                PartyName = reader["PartyName"].ToString(),
                                PrimaryChannel = reader["PrimaryChannel"].ToString(),
                                SecondaryChannel_Code = reader["SecondaryChannel_Code"].ToString(),
                                Element_Code = reader["Element_Code"].ToString(),
                                Element_Description = reader["Element_Description"].ToString(),
                                Status = reader["Status"].ToString(),
                                StartTime = reader["StartTime"].ToString(),
                                EndTime = reader["EndTime"].ToString(),
                                Date = reader["Date"].ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
