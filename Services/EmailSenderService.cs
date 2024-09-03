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
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));

            if (string.IsNullOrEmpty(_smtpSettings.SenderEmail))
                throw new ArgumentException("Sender email cannot be null or empty", nameof(_smtpSettings.SenderEmail));

            if (string.IsNullOrEmpty(_smtpSettings.SenderName))
                throw new ArgumentException("Sender name cannot be null or empty", nameof(_smtpSettings.SenderName));

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

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                // Log and handle SMTP exceptions
                throw new InvalidOperationException("SMTP Error: " + smtpEx.Message, smtpEx);
            }
            catch (FormatException formatEx)
            {
                // Log and handle format exceptions
                throw new InvalidOperationException("Invalid Email Format: " + formatEx.Message, formatEx);
            }
            catch (Exception ex)
            {
                // Log and handle other exceptions
                throw new InvalidOperationException("Failed to send email: " + ex.Message, ex);
            }
        }
    }
}
