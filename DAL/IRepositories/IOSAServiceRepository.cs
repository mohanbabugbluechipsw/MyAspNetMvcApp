using Microsoft.AspNetCore.Mvc.Rendering;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IOSAServiceRepository
    {
        Task<FilterViewModel> GetFilterDataAsync();
        Task<List<SelectListItem>> GetRsCodesByMrIdAsync(string mrId);
        Task<List<OSADetailViewModel>> GetOSADataRawAsync(string mrId, string rsCode, string outletType, DateTime from, DateTime to);
    }
}
