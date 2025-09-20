using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New;

using Microsoft.Extensions.Logging;



namespace DAL.Repositories
{






    public class MRDataService : IMRDataService
    {
        private readonly MrAppDbNewContext _context;
        private readonly ILogger<MRDataService> _logger; // ✅ Inject Logger

        public MRDataService(MrAppDbNewContext context, ILogger<MRDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<Tbl_capturedata_log>> GetMRDataBySrCodeAsync(string srCode, string srCodeName, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.tblcapturedatalog
                    .Where(x => x.SrCode == srCode && x.SrCodeName == srCodeName);

                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(x => x.UploadedAt >= fromDate && x.UploadedAt <= toDate);
                }

                query = query.OrderByDescending(x => x.UploadedAt);

                return await PaginatedList<Tbl_capturedata_log>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetMRDataBySrCodeAsync: {ex.Message}", ex);
                return new PaginatedList<Tbl_capturedata_log>(new List<Tbl_capturedata_log>(), 0, pageNumber, pageSize); // Return empty list on error
            }
        }

        public async Task<PaginatedList<Tbl_capturedata_log>> GetMRDataByMREmpAsync(string mrId, string mrEmpNo, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            try
            {
                if (!int.TryParse(mrId, out int parsedMrId))
                {
                    _logger.LogWarning($"Invalid MRID: {mrId}");
                    return new PaginatedList<Tbl_capturedata_log>(new List<Tbl_capturedata_log>(), 0, pageNumber, pageSize);
                }

                var query = _context.tblcapturedatalog
                    .Where(x => x.MRID ==Convert.ToInt32( mrId )&& x.MREmpNo == mrEmpNo);

                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(x => x.UploadedAt >= fromDate && x.UploadedAt <= toDate);
                }

                query = query.OrderByDescending(x => x.UploadedAt);

                return await PaginatedList<Tbl_capturedata_log>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetMRDataByMREmpAsync: {ex.Message}", ex);
                return new PaginatedList<Tbl_capturedata_log>(new List<Tbl_capturedata_log>(), 0, pageNumber, pageSize); // Return empty list on error
            }
        }
    }


}
