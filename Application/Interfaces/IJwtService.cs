using AuthAPI.Domain.Entities;

namespace Application.Interfaces;


public interface IJwtService
{
    (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateTokens(User user);

  
    Guid? ValidateToken(string token);
}