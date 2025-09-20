using BLL.IService;
using DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReminderJobService : IReminderJobService
    {
        private readonly IErrorRepository _errorRepository;
        private readonly IEmailService _emailService;

        public ReminderJobService(IErrorRepository errorRepository, IEmailService emailService)
        {
            _errorRepository = errorRepository;
            _emailService = emailService;
        }

        public async Task SendReminderEmailsAsync()
        {
            var pendingErrors = await _errorRepository.GetPendingErrorsAsync();

            if (pendingErrors != null && pendingErrors.Any())
            {
                foreach (var error in pendingErrors)
                {
                    await _emailService.SendReminderEmailAsync(error);
                }
            }
        }
    }

}
