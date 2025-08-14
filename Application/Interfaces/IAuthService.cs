using AuthAPI.Application.DTOs;

namespace Application.Interfaces;


public interface IAuthService
{
   
    Task<string> RegisterAsync(RegisterDto registerDto);

  
    Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto);

  
    Task<string> ResendVerificationCodeAsync(string email);

    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}