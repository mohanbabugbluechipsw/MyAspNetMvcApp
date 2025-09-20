using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IOutletMRSearchService
    {
        Task<List<RSOutletSearchModel>> GetOutletSummaryAsync(string rsCode, string rsName, DateTime fromDate, DateTime toDate);
    }
}
