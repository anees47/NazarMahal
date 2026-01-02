using NazarMahal.Application.Interfaces;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.ResponseDto.UserResponseDto;

namespace NazarMahal.Infrastructure.Services
{
    public class NotificationService(IUserRepository userRepository, IEmailService emailService) : INotificationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEmailService _emailService = emailService;

        #region Appointment Emails

        public async Task SendAppointmentConfirmationEmail(Appointment appointment)
        {
                var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var userEmail = user != null ? user.Email : appointment.Email;
            var userList = await _userRepository.GetUserListByRoleId(RoleEnum.Admin.ToString());

            var emailMessage = GetAppointmentConfirmationTemplate(appointment, user);
            var emailRecipients = userList.Select(u => u.Email).ToList();
            emailRecipients.Add(userEmail);

            await _emailService.SendEmailDirectlyAsync(emailRecipients, "New Appointment Scheduled - Nazar Mahal", emailMessage);
        }

        public async Task SendAppointmentCancellationEmail(Appointment appointment)
        {
            var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var userEmail = user != null ? user.Email : appointment.Email;
            var userList = await _userRepository.GetUserListByRoleId(RoleEnum.Admin.ToString());

            var emailMessage = GetAppointmentCancellationTemplate(appointment, user);
            var emailRecipients = userList.Select(u => u.Email).ToList();
            emailRecipients.Add(userEmail);

            await _emailService.SendEmailDirectlyAsync(emailRecipients, "Appointment Cancelled - Nazar Mahal", emailMessage);
        }

        public async Task SendAppointmentCompletionEmail(Appointment appointment)
        {
            var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var userEmail = user != null ? user.Email : appointment.Email;
            var userList = await _userRepository.GetUserListByRoleId(RoleEnum.Admin.ToString());

            var emailMessage = GetAppointmentCompletionTemplate(appointment, user);
            var emailRecipients = userList.Select(u => u.Email).ToList();
            emailRecipients.Add(userEmail);

            await _emailService.SendEmailDirectlyAsync(emailRecipients, "Appointment Completed - Nazar Mahal", emailMessage);
        }

        public async Task SendAppointmentUpdateConfirmationEmail(Appointment appointment)
        {
            var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var userList = await _userRepository.GetUserListByRoleId(RoleEnum.Admin.ToString());

            var emailMessage = GetAppointmentUpdateTemplate(appointment, user);
            var emailRecipients = userList.Select(u => u.Email).ToList();
            emailRecipients.Add(user != null ? user.Email : appointment.Email);

            await _emailService.SendEmailDirectlyAsync(emailRecipients, "Appointment Updated - Nazar Mahal", emailMessage);
        }

        public async Task SendAppointmentReminderEmail(Appointment appointment)
        {
            var user = await _userRepository.GetUserByIdAsync(appointment.UserId);
            var userEmail = user != null ? user.Email : appointment.Email;

            var emailMessage = GetAppointmentReminderTemplate(appointment, user);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Appointment Reminder - Nazar Mahal", emailMessage);
        }

        #endregion

        #region Order Emails

        public async Task SendOrderConfirmationEmail(string userEmail, string orderNumber, decimal amount)
        {
            var emailMessage = GetOrderConfirmationTemplate(orderNumber, amount);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Order Confirmation - Nazar Mahal", emailMessage);
        }

        public async Task SendAdminNewOrderEmail(string orderNumber, decimal amount, string userEmail, string phoneNumber)
        {
            var adminUsers = await _userRepository.GetUserListByRoleId(RoleEnum.Admin.ToString());
            var recipients = adminUsers?.Select(u => u.Email).Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();
            if (!recipients.Any())
            {
                recipients.Add("support@nazarmahal.com");
            }

            var body = $@"New order placed.<br/><br/>
                <strong>Order Number:</strong> {orderNumber}<br/>
                <strong>Amount:</strong> {amount:C}<br/>
                <strong>User Email:</strong> {userEmail}<br/>
                <strong>Phone:</strong> {phoneNumber}<br/>
                <br/>Please verify payment proof (if provided by customer) and move the order to <strong>ReadyForPickup</strong> when ready.";
            await _emailService.SendEmailDirectlyAsync(recipients, $"New Order - {orderNumber}", body);
        }

        public async Task SendOrderStatusUpdateEmail(string userEmail, string orderNumber, string status)
        {
            var emailMessage = GetOrderStatusUpdateTemplate(orderNumber, status);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, $"Order {status} - Nazar Mahal", emailMessage);
        }

        public async Task SendOrderCancellationEmail(string userEmail, string orderNumber, decimal refundAmount)
        {
            var emailMessage = GetOrderCancellationTemplate(orderNumber, refundAmount);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Order Cancelled - Nazar Mahal", emailMessage);
        }

        #endregion

        #region Account Emails

        public async Task SendAccountConfirmationEmail(List<string> toEmails, string confirmationLink)
        {
            var emailMessage = GetAccountConfirmationTemplate(confirmationLink);
            await _emailService.SendEmailDirectlyAsync(toEmails, "Confirm Your Email - Nazar Mahal", emailMessage);
        }

        public async Task SendPasswordResetEmail(string userEmail, string resetLink, string userName)
        {
            var emailMessage = GetPasswordResetTemplate(resetLink, userName);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Password Reset Request - Nazar Mahal", emailMessage);
        }

        public async Task SendPasswordResetConfirmationEmail(string userEmail)
        {
            var emailMessage = GetPasswordResetConfirmationTemplate();
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Password Reset Successful - Nazar Mahal", emailMessage);
        }

        public async Task SendWelcomeEmail(string userEmail, string userName)
        {
            var emailMessage = GetWelcomeEmailTemplate(userName);
            await _emailService.SendEmailDirectlyAsync(new List<string> { userEmail }, "Welcome to Nazar Mahal", emailMessage);
        }

        #endregion

        #region Email Templates

        // Helper method to format TimeSpan to 12-hour format
        private string FormatTime(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var period = hours >= 12 ? "PM" : "AM";
            var displayHours = hours > 12 ? hours - 12 : (hours == 0 ? 12 : hours);
            return $"{displayHours:D2}:{minutes:D2} {period}";
        }

        private string GetAppointmentConfirmationTemplate(Appointment appointment, UserResponseDto? user)
        {
            var userName = user?.FullName ?? appointment.FullName;
            var userEmail = user?.Email ?? appointment.Email;
            var userPhone = appointment.PhoneNumber;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .appointment-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .btn {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 10px 0; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>??</div>
            <h1>Appointment Confirmed!</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your appointment has been successfully scheduled</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>Thank you for scheduling an appointment with <strong>Nazar Mahal</strong>. We're excited to see you!</p>
            
            <div class='appointment-details'>
                <h3 style='margin-top: 0; color: #667eea;'>Appointment Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Appointment Type:</span>
                    <span class='detail-value'>{appointment.AppointmentType}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Date:</span>
                    <span class='detail-value'>{appointment.AppointmentDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Time:</span>
                    <span class='detail-value'>{FormatTime(appointment.AppointmentTime)}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Status:</span>
                    <span class='detail-value' style='color: #28a745; font-weight: 600;'>{appointment.AppointmentStatus}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Reason for Visit:</span>
                    <span class='detail-value'>{appointment.ReasonForVisit}</span>
                </div>
                {(string.IsNullOrEmpty(appointment.AdditionalNotes) ? "" : $@"
                <div class='detail-row'>
                    <span class='detail-label'>Additional Notes:</span>
                    <span class='detail-value'>{appointment.AdditionalNotes}</span>
                </div>")}
            </div>

            <div style='background: #e8f4f8; padding: 15px; border-left: 4px solid #667eea; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Important Reminders:</strong>
                <ul style='margin: 10px 0 0 0; padding-left: 20px;'>
                    <li>Please arrive 10 minutes before your scheduled time</li>
                    <li>Bring a valid ID and any relevant medical documents</li>
                    <li>If you need to reschedule or cancel, please contact us at least 24 hours in advance</li>
                </ul>
            </div>

            <p style='margin-top: 30px;'>If you have any questions or need to make changes to your appointment, please don't hesitate to contact us.</p>
            
            <p>We look forward to serving you!</p>
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetAppointmentCancellationTemplate(Appointment appointment, UserResponseDto? user)
        {
            var userName = user?.FullName ?? appointment.FullName;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .appointment-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>?</div>
            <h1>Appointment Cancelled</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your appointment has been cancelled</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>This email confirms that your appointment has been <strong>cancelled</strong>.</p>
            
            <div class='appointment-details'>
                <h3 style='margin-top: 0; color: #f5576c;'>Cancelled Appointment Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Appointment Type:</span>
                    <span class='detail-value'>{appointment.AppointmentType}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Original Date:</span>
                    <span class='detail-value'>{appointment.AppointmentDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Original Time:</span>
                    <span class='detail-value'>{appointment.AppointmentTime:hh\\:mm tt}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Status:</span>
                    <span class='detail-value' style='color: #dc3545; font-weight: 600;'>Cancelled</span>
                </div>
            </div>

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Need to Reschedule?</strong>
                <p style='margin: 10px 0 0 0;'>We'd be happy to help you find a new appointment time that works for you. Simply contact us or book a new appointment online.</p>
            </div>

            <p>If you have any questions or need assistance, please don't hesitate to reach out to us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetAppointmentCompletionTemplate(Appointment appointment, UserResponseDto? user)
        {
            var userName = user?.FullName ?? appointment.FullName;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .appointment-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>?</div>
            <h1>Appointment Completed</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Thank you for visiting Nazar Mahal</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>Thank you for visiting <strong>Nazar Mahal</strong>! We hope you had a great experience with us.</p>
            
            <div class='appointment-details'>
                <h3 style='margin-top: 0; color: #11998e;'>Completed Appointment Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Appointment Type:</span>
                    <span class='detail-value'>{appointment.AppointmentType}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Date:</span>
                    <span class='detail-value'>{appointment.AppointmentDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Time:</span>
                    <span class='detail-value'>{FormatTime(appointment.AppointmentTime)}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Status:</span>
                    <span class='detail-value' style='color: #28a745; font-weight: 600;'>Completed</span>
                </div>
            </div>

            <div style='background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Follow-Up Care:</strong>
                <p style='margin: 10px 0 0 0;'>If you have any questions or concerns following your appointment, please don't hesitate to contact us. We're here to help!</p>
            </div>

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px;'>
                <strong>? We Value Your Feedback!</strong>
                <p style='margin: 10px 0 0 0;'>Your feedback helps us improve our services. Please take a moment to share your experience with us.</p>
            </div>

            <p>We look forward to serving you again in the future!</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetAppointmentUpdateTemplate(Appointment appointment, UserResponseDto? user)
        {
            var userName = user?.FullName ?? appointment.FullName;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .appointment-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>??</div>
            <h1>Appointment Updated</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your appointment details have been updated</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>This email confirms that your appointment has been <strong>successfully updated</strong>.</p>
            
            <div class='appointment-details'>
                <h3 style='margin-top: 0; color: #4facfe;'>Updated Appointment Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Appointment Type:</span>
                    <span class='detail-value'>{appointment.AppointmentType}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Date:</span>
                    <span class='detail-value'>{appointment.AppointmentDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Time:</span>
                    <span class='detail-value'>{FormatTime(appointment.AppointmentTime)}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Status:</span>
                    <span class='detail-value' style='color: #28a745; font-weight: 600;'>{appointment.AppointmentStatus}</span>
                </div>
            </div>

            <p>Please make note of your updated appointment details. If you have any questions, please don't hesitate to contact us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetAppointmentReminderTemplate(Appointment appointment, UserResponseDto? user)
        {
            var userName = user?.FullName ?? appointment.FullName;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .appointment-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>?</div>
            <h1>Appointment Reminder</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>You have an upcoming appointment</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>This is a friendly reminder that you have an appointment with <strong>Nazar Mahal</strong>.</p>
            
            <div class='appointment-details'>
                <h3 style='margin-top: 0; color: #fa709a;'>Your Upcoming Appointment</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Date:</span>
                    <span class='detail-value'>{appointment.AppointmentDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Time:</span>
                    <span class='detail-value'>{FormatTime(appointment.AppointmentTime)}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Type:</span>
                    <span class='detail-value'>{appointment.AppointmentType}</span>
                </div>
            </div>

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Don't Forget:</strong>
                <ul style='margin: 10px 0 0 0; padding-left: 20px;'>
                    <li>Arrive 10 minutes early</li>
                    <li>Bring a valid ID</li>
                    <li>Bring any relevant documents</li>
                </ul>
            </div>

            <p>We look forward to seeing you!</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetOrderConfirmationTemplate(string orderNumber, decimal amount)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>???</div>
            <h1>Order Confirmed!</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Thank you for your purchase</p>
        </div>
        <div class='content'>
            <p>Dear Valued Customer,</p>
            <p>Thank you for your order with <strong>Nazar Mahal</strong>! We're excited to serve you.</p>
            
            <div class='order-details'>
                <h3 style='margin-top: 0; color: #667eea;'>Order Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Order Number:</span>
                    <span class='detail-value'><strong>{orderNumber}</strong></span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Order Date:</span>
                    <span class='detail-value'>{DateTime.Now:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Total Amount:</span>
                    <span class='detail-value' style='font-size: 20px; color: #28a745;'><strong>Rs. {amount:N2}</strong></span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Status:</span>
                    <span class='detail-value' style='color: #28a745; font-weight: 600;'>Confirmed</span>
                </div>
            </div>

            <div style='background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; border-radius: 4px;'>
                <strong>?? What's Next?</strong>
                <p style='margin: 10px 0 0 0;'>Your order is being processed and will be prepared for delivery. You'll receive another email with tracking information once your order ships.</p>
            </div>

            <p>If you have any questions about your order, please don't hesitate to contact us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetOrderStatusUpdateTemplate(string orderNumber, string status)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>??</div>
            <h1>Order Status Update</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your order status has been updated</p>
        </div>
        <div class='content'>
            <p>Dear Valued Customer,</p>
            <p>This email confirms that your order status has been updated.</p>
            
            <div class='order-details'>
                <h3 style='margin-top: 0; color: #4facfe;'>Order Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Order Number:</span>
                    <span class='detail-value'><strong>{orderNumber}</strong></span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Updated Status:</span>
                    <span class='detail-value' style='color: #17a2b8; font-weight: 600; font-size: 18px;'>{status}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Update Date:</span>
                    <span class='detail-value'>{DateTime.Now:dddd, MMMM dd, yyyy}</span>
                </div>
            </div>

            <p>If you have any questions about your order, please don't hesitate to contact us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetOrderCancellationTemplate(string orderNumber, decimal refundAmount)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eee; }}
        .detail-row:last-child {{ border-bottom: none; }}
        .detail-label {{ font-weight: 600; color: #666; }}
        .detail-value {{ color: #333; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>?</div>
            <h1>Order Cancelled</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your order has been cancelled</p>
        </div>
        <div class='content'>
            <p>Dear Valued Customer,</p>
            <p>This email confirms that your order has been <strong>cancelled</strong>.</p>
            
            <div class='order-details'>
                <h3 style='margin-top: 0; color: #f5576c;'>Cancelled Order Details</h3>
                <div class='detail-row'>
                    <span class='detail-label'>Order Number:</span>
                    <span class='detail-value'><strong>{orderNumber}</strong></span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Cancellation Date:</span>
                    <span class='detail-value'>{DateTime.Now:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Refund Amount:</span>
                    <span class='detail-value' style='font-size: 20px; color: #28a745;'><strong>Rs. {refundAmount:N2}</strong></span>
                </div>
            </div>

            <div style='background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Refund Information:</strong>
                <p style='margin: 10px 0 0 0;'>Your refund of <strong>Rs. {refundAmount:N2}</strong> will be processed within 5-7 business days and credited back to your original payment method.</p>
            </div>

            <p>If you have any questions about this cancellation or your refund, please don't hesitate to contact us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetAccountConfirmationTemplate(string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .btn {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>??</div>
            <h1>Confirm Your Email</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Welcome to Nazar Mahal</p>
        </div>
        <div class='content'>
            <p>Dear User,</p>
            <p>Thank you for signing up with <strong>Nazar Mahal</strong>! To complete your registration, please confirm your email address by clicking the button below.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{confirmationLink}' class='btn'>Confirm Email Address</a>
            </div>

            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all; color: #667eea;'>{confirmationLink}</p>

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Important:</strong>
                <p style='margin: 10px 0 0 0;'>This confirmation link will expire in 24 hours. If you didn't create an account with us, please ignore this email.</p>
            </div>

            <p>If you have any questions, please don't hesitate to contact us.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplate(string resetLink, string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .btn {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: 600; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 5px; }}
        .info-box {{ background: #e3f2fd; border-left: 4px solid #2196F3; padding: 15px; margin: 20px 0; border-radius: 5px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>&#128274;</div>
            <h1>Password Reset Request</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Reset your Nazar Mahal account password</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>We received a request to reset the password for your <strong>Nazar Mahal</strong> account.</p>
            
            <div class='info-box'>
                <p style='margin: 0; font-size: 14px;'><strong>Important:</strong> This link will expire in <strong>10 minutes</strong> for security reasons.</p>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' class='btn'>Reset Password</a>
            </div>
            
            <p style='font-size: 14px; color: #666;'>Or copy and paste this link into your browser:</p>
            <p style='font-size: 12px; background: #fff; padding: 10px; border-radius: 5px; word-break: break-all; border: 1px solid #ddd;'>{resetLink}</p>
            
            <div class='warning'>
                <p style='margin: 0; font-size: 14px;'><strong>Didn't request this?</strong> If you didn't request a password reset, please ignore this email or contact us immediately at <strong>support&#64;nazarmahal.com</strong>. Your account is still secure.</p>
            </div>

            <p style='font-size: 14px; color: #666; margin-top: 20px;'>This is an automated email. Please do not reply to this message.</p>
        </div>
        <div class='footer'>
            <p>&#169; 2025 Nazar Mahal. All rights reserved.</p>
            <p>Your vision, our expertise</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetConfirmationTemplate()
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>?</div>
            <h1>Password Reset Successful</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>Your password has been changed</p>
        </div>
        <div class='content'>
            <p>Dear User,</p>
            <p>This email confirms that your password for your <strong>Nazar Mahal</strong> account has been successfully changed.</p>
            
            <div style='background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Security Information:</strong>
                <p style='margin: 10px 0 0 0;'>Your password was changed on {DateTime.Now:dddd, MMMM dd, yyyy 'at' hh:mm tt}.</p>
            </div>

            <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px;'>
                <strong>?? Important:</strong>
                <p style='margin: 10px 0 0 0;'>If you did not make this change, please contact us immediately at <strong>info@nazarmahal.com</strong>.</p>
            </div>

            <p>You can now log in to your account with your new password.</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}
        .icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='icon'>??</div>
            <h1>Welcome to Nazar Mahal!</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>We're excited to have you</p>
        </div>
        <div class='content'>
            <p>Dear <strong>{userName}</strong>,</p>
            <p>Welcome to <strong>Nazar Mahal</strong>! We're thrilled to have you as part of our community.</p>
            
            <div style='background: #e8f4f8; padding: 15px; border-left: 4px solid #667eea; margin: 20px 0; border-radius: 4px;'>
                <strong>?? What's Next?</strong>
                <ul style='margin: 10px 0 0 0; padding-left: 20px;'>
                    <li>Browse our extensive collection of eyewear</li>
                    <li>Schedule an appointment for an eye examination</li>
                    <li>Explore our latest deals and promotions</li>
                    <li>Stay updated with our newsletter</li>
                </ul>
            </div>

            <p>If you have any questions or need assistance, our team is here to help you every step of the way.</p>
            
            <p>Thank you for choosing Nazar Mahal. We look forward to serving you!</p>
            
            <p style='margin-bottom: 0;'>
                Best regards,<br>
                <strong>The Nazar Mahal Team</strong>
            </p>
        </div>
        <div class='footer'>
            <p><strong>Contact Information:</strong></p>
            <p>Email: info@nazarmahal.com | Phone: +92 XXX XXXXXXX</p>
            <p style='font-size: 12px; margin-top: 20px;'>© {DateTime.Now.Year} Nazar Mahal. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        #endregion
    }
}
