using Microsoft.AspNetCore.Mvc.Rendering;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IOSAReviewRepository
    {

        Task<List<string>> GetRSCodesByMRName(string mrName);
        Task<List<string>> GetOutletTypes();
        Task<PagedResult<OSAReviewViewModel>> GetOSAReviewData(OSAReviewFilterModel filter);
        Task<int> CreateOSAReview(OSAReviewViewModel model);
        Task UpdateOSAReview(OSAReviewViewModel model);
        Task DeleteOSAReview(int id);

        Task<List<EmployeeDropdownDto>> GetMRNames(); // Add this method


    }


    public class EmployeeDropdownDto
    {
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
    }
}
