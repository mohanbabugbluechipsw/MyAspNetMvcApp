//using DAL.IRepositories;
//using Model_New.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DAL.Repositories
//{
//    public class VisitQuestionRepository : IVisitQuestionRepository
//    {

//        private readonly IDbConnection _db;



//        public VisitQuestionRepository(IDbConnection db)
//        {
//            _db = db;
//        }
//        public Task<Dictionary<int, string>> GetAnswersByVisitIdAsync(Guid visitId, bool isPreVisit)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> HasPreVisitAnswersAsync(Guid visitId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SaveAnswersAsync(Guid visitId, IEnumerable<> answers, bool isPreVisit)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
