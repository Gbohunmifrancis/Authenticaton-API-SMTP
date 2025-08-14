using AuthAPI.Domain.Entities;
using Enforca.Domain.Entities;

public interface IVerificationCodeRepository
{
    
    Task<VerificationCode> AddAsync(VerificationCode verificationCode);

    Task<VerificationCode?> GetLatestByUserIdAsync(Guid userId);
    Task<VerificationCode?> GetByEmailAndCodeAsync(string email, string code);
    Task<VerificationCode> UpdateAsync(VerificationCode verificationCode);
    Task<DateTime?> GetLastCodeSentTimeAsync(Guid userId);
}