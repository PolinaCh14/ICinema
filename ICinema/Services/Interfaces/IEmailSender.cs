using ICinema.Models;

namespace ICinema.Services.Interfaces
{
    public interface IEmailSender
    {
        public Task SendEmail(string toEmail, string subject, string message);
    }
}
