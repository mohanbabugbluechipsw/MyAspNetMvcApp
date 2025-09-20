using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IErrorService
    {
        Task<Guid> HandleExceptionAsync(Exception exception, HttpContext context);
        Task<bool> UpdateErrorStatusAsync(Guid id, string status);
    }
}
