using Model_New.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DAL.Repositories.VisitorReviewReportRepository;

namespace DAL.IRepositories
{
    public interface IVisitorReviewReportRepository
    {
        Task<IEnumerable<VisitorReportDto>> GetTodayReviewReportAsync();

        Task<IEnumerable<VisitorLoginDurationDto>> GetTodayLoginDurationReportAsync();


        Task<IEnumerable<VisitorLoginDurationDto1>> GetTodayLoginDurationReportAsync1();
    }
}
