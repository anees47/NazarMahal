using Microsoft.Extensions.Configuration;
using NazarMahal.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace NazarMahal.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendEmailDirectlyAsync(List<string> toEmail, string subject, string message)
    {
        var apiKey = configuration["BrevoSettings:ApiKey"];
        var senderEmail = configuration["BrevoSettings:SenderEmail"];
        var senderName = configuration["BrevoSettings:SenderName"];
        foreach (var item in toEmail)
        {
            var emailContent = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = item, name = "Recipient" } },
                subject = subject,
                htmlContent = message
            };

            var jsonContent = JsonSerializer.Serialize(emailContent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                var response = await client.PostAsync("https://api.sendinblue.com/v3/smtp/email", content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to send email: {response.StatusCode}, {responseBody}");
                }
                else
                {
                    Console.WriteLine("Email Sent Successfully");
                }
            }
        }

    }

}
