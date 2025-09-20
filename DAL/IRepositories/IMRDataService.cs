using Model_New;
using Model_New.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAL.IRepositories
{
    public interface IMRDataService
    {
        Task<PaginatedList<Tbl_capturedata_log>> GetMRDataBySrCodeAsync(string srCode, string srCodeName, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);

        Task<PaginatedList<Tbl_capturedata_log>> GetMRDataByMREmpAsync(string mrId, string mrEmpNo, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
    }
}
