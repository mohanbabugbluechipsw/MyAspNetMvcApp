using iTextSharp.xmp;
using Microsoft.AspNetCore.Http;
using Model_New.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    //// Core/Interfaces/IErrorRepository.cs
    //public interface IErrorRepository
    //{
    //    Task<TblAppError> AddAsync(TblAppError error);
    //    Task MarkAsResolvedAsync(Guid errorId);
    //    Task<IEnumerable<TblAppError>> GetUnresolvedErrorsAsync();
    //    Task<IEnumerable<TblAppError>> GetAllErrorsAsync();
    //    Task<int> GetUnresolvedErrorCountAsync();
    //    Task<TblAppError> GetByIdAsync(Guid errorId);
    //}


    public interface IErrorRepository
    {
        Task AddErrorAsync(ApplicationErrorLog errorLog);



        Task<ApplicationErrorLog> GetErrorByIdAsync(Guid errorId);

        // ✅ New: Update error (for status change)
        Task UpdateErrorAsync(ApplicationErrorLog errorLog);

        Task<List<ApplicationErrorLog>> GetErrorsByStatusAsync(string[] statuses);

        Task<List<ApplicationErrorLog>> GetPendingErrorsAsync();




    }



}
