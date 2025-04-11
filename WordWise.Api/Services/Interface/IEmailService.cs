using WordWise.Api.Models.Domain;

namespace WordWise.Api.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(ExtendedIdentityUser user);
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
        Task SendPasswordResetEmailAsync(string email, string username, string resetLink);
    }
}
