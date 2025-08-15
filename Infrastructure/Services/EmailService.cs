using System.Net;
using System.Net.Mail;
using AuthAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;


public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;


    
    public async Task SendVerificationCodeAsync(string email, string fullName, string verificationCode)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SMTP");
            var fromEmail = smtpSettings["FromEmail"] ?? throw new InvalidOperationException("SMTP FromEmail not configured");
            var fromName = smtpSettings["FromName"] ?? "Authentication API";

            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"] ?? "587"))
            {
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
            };

            var subject = "Email Verification Code";
            var body = GenerateVerificationEmailBody(fullName, verificationCode);

            var message = new MailMessage(new MailAddress(fromEmail, fromName), new MailAddress(email))
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
            logger.LogInformation("Verification code sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification code to {Email}", email);
            throw new InvalidOperationException("Failed to send verification email. Please try again later.");
        }
    }

    
    private static string GenerateVerificationEmailBody(string fullName, string verificationCode)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Email Verification</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                    .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                    .header {{ text-align: center; margin-bottom: 30px; }}
                    .code {{ font-size: 32px; font-weight: bold; text-align: center; color: #007bff; letter-spacing: 4px; margin: 20px 0; padding: 15px; background-color: #f8f9fa; border-radius: 4px; }}
                    .footer {{ margin-top: 30px; font-size: 12px; color: #666; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Email Verification</h1>
                    </div>
                    <p>Hello {fullName},</p>
                    <p>Thank you for registering! Please use the following verification code to complete your registration:</p>
                    <div class='code'>{verificationCode}</div>
                    <p>This code will expire in 10 minutes.</p>
                    <p>If you didn't request this verification, please ignore this email.</p>
                    <div class='footer'>
                        <p>This is an automated message, please do not reply.</p>
                    </div>
                </div>
            </body>
            </html>";
    }
}