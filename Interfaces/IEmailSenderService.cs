namespace GundamStore.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

}

