using System;
using System.Threading.Tasks;

namespace SysMarkModerno.Services
{
using System.Collections.Generic;

    public interface IEmailService
    {
        Task<(bool Success, string ErrorMessage)> SendEmailAsync(string subject, string body, IEnumerable<string> additionalRecipients = null);
        Task<(bool Success, string ErrorMessage)> SendEmailAsync(string to, string subject, string body);
        Task<(bool Success, string ErrorMessage)> SendCalendarInviteAsync(string subject, string body, DateTime startTime, DateTime endTime, string location, IEnumerable<string> additionalRecipients = null);
    }
}
