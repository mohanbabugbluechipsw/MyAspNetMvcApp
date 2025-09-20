using BLL.IService;
using DAL.IRepositories;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OutletMRSearchService : IOutletMRSearchService
    {
        private readonly IOutletMRSearchRepository _repository;

        public OutletMRSearchService(IOutletMRSearchRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RSOutletSearchModel>> GetOutletSummaryAsync(string rsCode, string rsName, DateTime fromDate, DateTime toDate)
        {
            return await _repository.GetOutletSummaryAsync(rsCode, rsName, fromDate, toDate);
        }
    }
}
