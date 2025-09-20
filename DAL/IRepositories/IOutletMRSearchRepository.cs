using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IOutletMRSearchRepository
    {
        Task<List<RSOutletSearchModel>> GetOutletSummaryAsync(string rsCode, string rsName, DateTime fromDate, DateTime toDate);
    }
}
