using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SysMarkModerno.Helpers;

namespace SysMarkModerno.Services
{
    public class EmailService : IEmailService
    {
        public async Task<(bool Success, string ErrorMessage)> SendEmailAsync(string subject, string body, IEnumerable<string> additionalRecipients = null)
        {
            var config = ConfigurationHelper.ObtenerConfiguracionEmail();
            if (config == null || string.IsNullOrEmpty(config.SmtpServer))
            {
                return (false, "Configuración de correo no encontrada.");
            }
            
            var allRecipients = new HashSet<string>();
            if (config.ToEmails != null)
            {
                foreach (var email in config.ToEmails) allRecipients.Add(email);
            }
            if (additionalRecipients != null)
            {
                foreach (var email in additionalRecipients) 
                {
                    if (!string.IsNullOrWhiteSpace(email)) allRecipients.Add(email);
                }
            }

            if (allRecipients.Count == 0)
            {
                return (false, "No hay destinatarios configurados.");
            }

            StringBuilder errors = new StringBuilder();
            bool atLeastOneSuccess = false;

            foreach (var to in allRecipients)
            {
                var result = await SendEmailAsync(to, subject, body);
                if (result.Success)
                {
                    atLeastOneSuccess = true;
                }
                else
                {
                    errors.AppendLine($"{to}: {result.ErrorMessage}");
                }
            }
            
            return atLeastOneSuccess ? (true, string.Empty) : (false, errors.ToString());
        }

        public async Task<(bool Success, string ErrorMessage)> SendEmailAsync(string to, string subject, string body)
        {
            return await SendEmailInternalAsync(to, subject, body, null);
        }

        public async Task<(bool Success, string ErrorMessage)> SendCalendarInviteAsync(string subject, string body, DateTime startTime, DateTime endTime, string location, IEnumerable<string> additionalRecipients = null)
        {
            var config = ConfigurationHelper.ObtenerConfiguracionEmail();
            if (config == null) return (false, "Configuración inválida.");

            var allRecipients = new HashSet<string>();
            if (config.ToEmails != null)
            {
                foreach (var email in config.ToEmails) allRecipients.Add(email);
            }
            if (additionalRecipients != null)
            {
                foreach (var email in additionalRecipients) 
                {
                    if (!string.IsNullOrWhiteSpace(email)) allRecipients.Add(email);
                }
            }

            if (allRecipients.Count == 0) return (false, "No hay destinatarios.");

            var icsContent = GenerateIcsContent(subject, startTime, endTime, location, body);
            
            StringBuilder errors = new StringBuilder();
            bool atLeastOneSuccess = false;

            foreach (var to in allRecipients)
            {
                var result = await SendEmailInternalAsync(to, subject, body, icsContent);
                if (result.Success)
                {
                    atLeastOneSuccess = true;
                }
                else
                {
                    errors.AppendLine($"{to}: {result.ErrorMessage}");
                }
            }
            return atLeastOneSuccess ? (true, string.Empty) : (false, errors.ToString());
        }

        private async Task<(bool Success, string ErrorMessage)> SendEmailInternalAsync(string to, string subject, string body, string icsContent)
        {
            var config = ConfigurationHelper.ObtenerConfiguracionEmail();
            if (config == null || string.IsNullOrEmpty(config.SmtpServer))
                return (false, "Falta configuración SMTP.");

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(config.FromEmail, config.FromName);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;

                // Si es HTML, lo agregamos como un AlternateView para asegurar que se renderice
                // incluso si hay otros adjuntos o vistas (como el calendario).
                var htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");
                message.AlternateViews.Add(htmlView);

                if (!string.IsNullOrEmpty(icsContent))
                {
                    var contentType = new ContentType("text/calendar");
                    contentType.Parameters.Add("method", "REQUEST");
                    contentType.Parameters.Add("name", "status.ics");
                    
                    var av = AlternateView.CreateAlternateViewFromString(icsContent, contentType);
                    message.AlternateViews.Add(av);
                }
                else
                {
                    // Si no hay calendario, también seteamos el body para clientes muy antiguos
                    message.Body = body;
                    message.IsBodyHtml = true;
                }

                using var client = new SmtpClient(config.SmtpServer, config.SmtpPort);
                client.EnableSsl = config.UseSsl;
                client.Timeout = 10000; // 10 seconds timeout
                
                if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
                {
                    client.Credentials = new NetworkCredential(config.Username, config.Password);
                }

                await client.SendMailAsync(message);
                return (true, string.Empty);
            }
            catch (SmtpException smtpEx)
            {
                return (false, $"SMTP Error: {smtpEx.Message} (Status: {smtpEx.StatusCode})");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        private string GenerateIcsContent(string subject, DateTime startTime, DateTime endTime, string location, string description)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//SysMark//NONSGML v1.0//EN");
            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{Guid.NewGuid()}");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTSTART:{startTime:yyyyMMddTHHmmss}");
            sb.AppendLine($"DTEND:{endTime:yyyyMMddTHHmmss}");
            sb.AppendLine($"SUMMARY:{subject}");
            sb.AppendLine($"LOCATION:{location}");
            sb.AppendLine($"DESCRIPTION:{description}");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
    }
}
