using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IReportRepository
    {
        Task<VisitStatusChartViewModel> GetVisitStatusSummaryAsync(string mrName, DateTime startDate, DateTime endDate);

    }
}
