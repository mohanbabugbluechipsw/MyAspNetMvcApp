using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class OSAService : IOSAServiceRepository
    {
        private readonly MrAppDbNewContext _context;

        public OSAService(MrAppDbNewContext context)
        {
            _context = context;
        }

        //public async Task<FilterViewModel> GetFilterDataAsync()
        //{

        //    var MrNames= await _context.TblUsers.Select(x=>new SelectListItem {Value= x.EmpNo.ToString(), Text= x.EmpName }).DistinctBy(x=> new { x.Value, x.Text }).ToListAsync();


        //    var OutletTypes=await _context.tbl_ChannelHierarchy_Masters.Select(x=>new SelectListItem {Value= x.Id.ToString(),Text= x.Element_Code }).DistinctBy(x => new { x.Value, x.Text}).ToListAsync(); 



        //    return new FilterViewModel
        //    {
        //        MrNames = MrNames,
        //        OutletTypes = OutletTypes
        //    };
        //}




        public async Task<FilterViewModel> GetFilterDataAsync()
        {
            // Get MR list and remove duplicates after fetching
            var mrList = await _context.TblUsers
                .Select(x => new SelectListItem
                {
                    Value = x.EmpNo.ToString(),
                    Text = x.EmpName
                })
                .ToListAsync();

            mrList = mrList.DistinctBy(x => new { x.Value, x.Text }).ToList();

            // Get OutletTypes with one item per unique Element_Code
            var outletList = await _context.tbl_ChannelHierarchy_Masters
                .GroupBy(x => x.Element_Code)
                .Select(g => new SelectListItem
                {
                    Value = g.First().Id.ToString(), // or use Min() if you prefer
                    Text = g.Key
                })
                .ToListAsync();

            return new FilterViewModel
            {
                MrNames = mrList,
                OutletTypes = outletList
            };
        }







        public async Task<List<SelectListItem>> GetRsCodesByMrIdAsync(string mrId)
        {
            if (string.IsNullOrEmpty(mrId) || !int.TryParse(mrId, out int mrEmpNo))
                return new List<SelectListItem>();

            return await _context.tbl_MRSrMappings
                .Where(x => x.MrEmpNo == mrEmpNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Rs_code.ToString(),
                    Text = x.Rs_code
                })
                .ToListAsync();
        }

        //public async Task<List<OSADetailViewModel>> GetOSADataRawAsync(string mrId, string rsCode, string outletType, DateTime fromdate, DateTime todate)
        //{
        //    // LINQ query to fetch data
        //    var result = await (from a in _context.tbl_OSA_ReviewAnswer_MR
        //                        join q in _context.Tbl_OSA_Questions on a.QuestionId equals q.Id
        //                        where a.RSCODE == rsCode.ToString()  // Convert rsCode to string for comparison
        //                              && a.EmpNo == mrId.ToString()   // Convert mrId to string for comparison
        //                              && a.OutletType == outletType.ToString()  // Convert outletType to string for comparison
        //                              && a.SubmittedDate >= fromdate
        //                      && a.SubmittedDate < todate
        //                        select new OSADetailViewModel
        //                        {
        //                            RsCode = a.RSCODE,
        //                            PartyCode = a.PartyHLLCode,
        //                            QuestionText = q.QuestionText,
        //                            Answer = a.Answer,
        //                            AnsweredDate = a.SubmittedDate,
        //                            QuestionOutletType = a.OutletType
        //                        }).ToListAsync();

        //    return result;
        //}


        public async Task<List<OSADetailViewModel>> GetOSADataRawAsync(string mrId, string rsCode, string outletType, DateTime fromdate, DateTime todate)
        {
            var query = from a in _context.tbl_OSA_ReviewAnswer_MR
                        join q in _context.Tbl_OSA_Questions on a.QuestionId equals q.Id
                        where a.SubmittedDate >= fromdate && a.SubmittedDate < todate
                        select new { a, q };

            if (!string.IsNullOrEmpty(rsCode))
            {
                query = query.Where(x => x.a.RSCODE == rsCode);
            }

            if (!string.IsNullOrEmpty(mrId))
            {
                query = query.Where(x => x.a.EmpNo == mrId);
            }

            if (!string.IsNullOrEmpty(outletType))
            {
                query = query.Where(x => x.a.OutletType == outletType);
            }

            var result = await query.Select(x => new OSADetailViewModel
            {
                RsCode = x.a.RSCODE,
                PartyCode = x.a.PartyHLLCode,
                QuestionText = x.q.QuestionText,
                Answer = x.a.Answer,
                AnsweredDate = x.a.SubmittedDate,
                QuestionOutletType = x.a.OutletType
            }).ToListAsync();

            return result;
        }








    }

}
