using DAL.IRepositories;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IOSAReviewService
    {



        Task<List<string>> GetRSCodesByMRName(string mrName);
        Task<List<string>> GetOutletTypes();
        Task<PagedResult<OSAReviewViewModel>> GetOSAReviewData(OSAReviewFilterModel filter);
        Task<int> CreateOSAReview(OSAReviewViewModel model);
        Task UpdateOSAReview(OSAReviewViewModel model);
        Task DeleteOSAReview(int id);

        Task<List<EmployeeDropdownDto>> GetMRNames();

    }
}
