using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IGetDailydetailsummaryReportRepository
    {
        Task<List<Tbl_DetailsummaryReportDto>> GetOutletStatusAsync(string mrCode, DateTime monthStart, DateTime monthEnd);


        Task<List<Tbl_DetailsummaryReportDto>> GetFilteredDataAsync(
          string mrCode,
          string monthStart,
          string monthEnd,
          string sortColumn,
          string sortOrder);
    
}
}
