//using BLL.IService;
//using DAL.IRepositories;
//using Model_New.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BLL.IService;

//using BLL.IService; // Make sure this using exists

//namespace BLL.Services
//{
//    public class StoreVisitQuestionService : IStoreVisitQuestionService
//    {
//        private readonly IStoreVisitQuestionRepository _repository;

//        public StoreVisitQuestionService(IStoreVisitQuestionRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<IEnumerable<StoreVisitQuestion>> LoadActiveQuestionsAsync()
//        {
//            return await _repository.GetAllActiveQuestionsAsync();
//        }
//    }
//}

