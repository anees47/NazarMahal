namespace NazarMahal.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailDirectlyAsync(List<string> toEmail, string subject, string message);
    }
}

