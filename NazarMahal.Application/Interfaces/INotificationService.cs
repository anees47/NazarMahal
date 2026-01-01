using NazarMahal.Core.Entities;

namespace NazarMahal.Application.Interfaces
{
    public interface INotificationService
    {
        // Appointment Emails
        Task SendAppointmentConfirmationEmail(Appointment appointment);
        Task SendAppointmentCancellationEmail(Appointment appointment);
        Task SendAppointmentCompletionEmail(Appointment appointment);
        Task SendAppointmentUpdateConfirmationEmail(Appointment appointment);
        Task SendAppointmentReminderEmail(Appointment appointment);

        // Order Emails
        Task SendOrderConfirmationEmail(string userEmail, string orderNumber, decimal amount);
        Task SendAdminNewOrderEmail(string orderNumber, decimal amount, string userEmail, string phoneNumber);
        Task SendOrderStatusUpdateEmail(string userEmail, string orderNumber, string status);
        Task SendOrderCancellationEmail(string userEmail, string orderNumber, decimal refundAmount);

        // Account Emails
        Task SendAccountConfirmationEmail(List<string> toEmails, string confirmationLink);
        Task SendPasswordResetEmail(string userEmail, string resetLink, string userName);
        Task SendPasswordResetConfirmationEmail(string userEmail);
        Task SendWelcomeEmail(string userEmail, string userName);
    }
}

