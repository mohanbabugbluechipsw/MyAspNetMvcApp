using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IStoreVisitQuestionRepository
    {
        //Task<IEnumerable<StoreVisitQuestion>> GetAllActiveQuestionsAsync();


        Task<IEnumerable<StoreVisitQuestion>> GetActiveQuestionsAsync(string visitType, string channelType);


        Task SaveVisitAnswersAsync(List<TblStoreVisitAnswer> answers);


        Task SaveOutletViewAnswersAsync(List<TblOutletViewAnswer> answers);


        //Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionIdsAsync( string channelType);
        Task<List<QuestionLinkDto>> GetRequiredPostVisitQuestionIdsAsync(string channelType, string preVisitGuid);
        Task SaveErrorLogAsync(object log);

    }
}
