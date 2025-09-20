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

        public class OSAReviewService : IOSAReviewService
        {
            private readonly IOSAReviewRepository _repository;

            public OSAReviewService(IOSAReviewRepository repository)
            {
                _repository = repository;
            }

        public Task<List<EmployeeDropdownDto>> GetMRNames() => _repository.GetMRNames();


        public Task<List<string>> GetRSCodesByMRName(string mrName) => _repository.GetRSCodesByMRName(mrName);
            public Task<List<string>> GetOutletTypes() => _repository.GetOutletTypes();
            public Task<PagedResult<OSAReviewViewModel>> GetOSAReviewData(OSAReviewFilterModel filter) => _repository.GetOSAReviewData(filter);
            public Task<int> CreateOSAReview(OSAReviewViewModel model) => _repository.CreateOSAReview(model);
            public Task UpdateOSAReview(OSAReviewViewModel model) => _repository.UpdateOSAReview(model);
            public Task DeleteOSAReview(int id) => _repository.DeleteOSAReview(id);
        }
    
}
