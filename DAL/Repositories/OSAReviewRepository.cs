using DAL.IRepositories;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Dapper; // Add this using directive
using Microsoft.EntityFrameworkCore; // For GetDbConnection()
using System.Data.Common; // For DbConnection


namespace DAL.Repositories
{
    public class OSAReviewRepository : IOSAReviewRepository
    {
        private readonly MrAppDbNewContext _context;
        private readonly IDbConnection _db; // Dapper works with IDbConnection

        public OSAReviewRepository(MrAppDbNewContext context)
        {
            _context = context;
            _db = _context.Database.GetDbConnection(); // Get the underlying connection
        }


        public async Task<List<EmployeeDropdownDto>> GetMRNames()
        {
            return await _context.TblUsers
                .OrderBy(u => u.EmpName)
                .Select(u => new EmployeeDropdownDto
                {
                    EmpNo = u.EmpNo,
                    EmpName = u.EmpName
                })
                .ToListAsync();
        }




        public async Task<List<string>> GetRSCodesByMRName(string mrName)
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync(); // Ensure connection is open

            return (await _db.QueryAsync<string>(
                "sp_GetRSCodesByMRName",
                new { mrName },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<string>> GetOutletTypes()
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            return (await _db.QueryAsync<string>(
                "sp_GetOutletTypes",
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<PagedResult<OSAReviewViewModel>> GetOSAReviewData(OSAReviewFilterModel filter)
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@RSCode", filter.RSCode);
            parameters.Add("@OutletType", filter.OutletType);
            parameters.Add("@FromDate", filter.FromDate);
            parameters.Add("@ToDate", filter.ToDate);
            parameters.Add("@PageNumber", filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize);

            using var multi = await _db.QueryMultipleAsync(
                "sp_GetOSAReviewData",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var items = (await multi.ReadAsync<OSAReviewViewModel>()).ToList();
            var totalCount = items.FirstOrDefault()?.TotalCount ?? 0;

            return new PagedResult<OSAReviewViewModel>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<int> CreateOSAReview(OSAReviewViewModel model)
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@RSCode", model.RSCode);
            parameters.Add("@PartyHLLCode", model.PartyHLLCode);
            parameters.Add("@OutletType", model.OutletType);
            parameters.Add("@QuId", model.QuId);
            parameters.Add("@Answer", model.Answer);
            parameters.Add("@ReviewDate", model.ReviewDate);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _db.ExecuteAsync(
                "sp_InsertOSAReview",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@Id");
        }

        public async Task UpdateOSAReview(OSAReviewViewModel model)
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@RSCode", model.RSCode);
            parameters.Add("@PartyHLLCode", model.PartyHLLCode);
            parameters.Add("@OutletType", model.OutletType);
            parameters.Add("@QuId", model.QuId);
            parameters.Add("@Answer", model.Answer);
            parameters.Add("@ReviewDate", model.ReviewDate);

            await _db.ExecuteAsync(
                "sp_UpdateOSAReview",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task DeleteOSAReview(int id)
        {
            if (_db.State != ConnectionState.Open)
                await _db.OpenAsync();

            await _db.ExecuteAsync(
                "sp_DeleteOSAReview",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }



      
    }
}