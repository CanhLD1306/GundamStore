using MimeKit;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GundamStore.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be null or empty", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Email body cannot be null or empty", nameof(body));

            if (_smtpSettings == null)
                throw new InvalidOperationException("SMTP settings must be configured.");

            if (string.IsNullOrWhiteSpace(_smtpSettings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_smtpSettings.SenderName))
                throw new InvalidOperationException("Sender email and name must be configured in SMTP settings.");

            try
            {
                using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(toEmail));
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                throw new InvalidOperationException($"SMTP error while sending email: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while sending email: {ex.Message}", ex);
            }
        }
    }
}
