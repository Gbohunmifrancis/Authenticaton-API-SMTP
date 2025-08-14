namespace AuthAPI.Domain.Interfaces;

public interface IEmailService
{
 
    Task SendVerificationCodeAsync(string email, string fullName, string verificationCode);
}