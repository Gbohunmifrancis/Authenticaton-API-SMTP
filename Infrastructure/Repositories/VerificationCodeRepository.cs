using Enforca.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VerificationCodeRepository(ApplicationDbContext context) : IVerificationCodeRepository
{
    public async Task<VerificationCode> AddAsync(VerificationCode verificationCode)
    {
        context.VerificationCodes.Add(verificationCode);
        await context.SaveChangesAsync();
        return verificationCode;
    }

    
    public async Task<VerificationCode?> GetLatestByUserIdAsync(Guid userId)
    {
        return await context.VerificationCodes
            .Where(vc => vc.UserId == userId && !vc.IsUsed)
            .OrderByDescending(vc => vc.CreatedAt)
            .FirstOrDefaultAsync();
    }

 
    public async Task<VerificationCode?> GetByEmailAndCodeAsync(string email, string code)
    {
        return await context.VerificationCodes
            .Include(vc => vc.User)
            .FirstOrDefaultAsync(vc => vc.User.Email == email.ToLowerInvariant() && vc.Code == code);
    }

    
    public async Task<VerificationCode> UpdateAsync(VerificationCode verificationCode)
    {
        context.VerificationCodes.Update(verificationCode);
        await context.SaveChangesAsync();
        return verificationCode;
    }
    
    public async Task<DateTime?> GetLastCodeSentTimeAsync(Guid userId)
    {
        var lastCode = await context.VerificationCodes
            .Where(vc => vc.UserId == userId)
            .OrderByDescending(vc => vc.CreatedAt)
            .FirstOrDefaultAsync();

        return lastCode?.CreatedAt;
    }
}