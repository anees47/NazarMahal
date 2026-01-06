using NazarMahal.Core.Entities;

namespace NazarMahal.Application.Interfaces
{
    public interface INotificationService
    {
        // Appointment Emails - Production Workflow
        Task SendAppointmentScheduledEmail(Appointment appointment);       // When customer books (Status: Scheduled)
        Task SendAppointmentConfirmedEmail(Appointment appointment);       // When admin confirms (Status: Confirmed)
        Task SendAppointmentCompletionEmail(Appointment appointment);      // When admin marks complete (Status: Completed)
        Task SendAppointmentCancellationEmail(Appointment appointment);    // When admin cancels (Status: Cancelled)
        Task SendAppointmentUpdateConfirmationEmail(Appointment appointment); // When admin updates details
        Task SendAppointmentReminderEmail(Appointment appointment);        // Reminder before appointment
        Task SendAppointmentConfirmationEmail(Appointment appointment);

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

