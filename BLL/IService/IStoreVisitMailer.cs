using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IStoreVisitMailer
    {
         Task SendDailyReports();

        //Task SendEmailAsync(string to, string cc, string subject, string body, string csvContent = null, string fileName = null);


    }
}
