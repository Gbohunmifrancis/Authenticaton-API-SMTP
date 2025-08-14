using System.Security.Cryptography;
using Application.Interfaces;
using AuthAPI.Application.DTOs;
using AuthAPI.Domain.Entities;
using AuthAPI.Domain.Interfaces;
using Domain.Interfaces;
using Enforca.Domain.Entities;

namespace Application.Services;


public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    
    public AuthService(
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository,
        IEmailService emailService,
        IJwtService jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _verificationCodeRepository = verificationCodeRepository;
        _emailService = emailService;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc/>
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        // Create new user
        var user = new User
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email.ToLowerInvariant()
        };

        // Hash password
        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        // Save user
        user = await _userRepository.AddAsync(user);

        // Generate and send verification code
        await GenerateAndSendVerificationCodeAsync(user);

        return "Registration successful. Please check your email for verification code.";
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        var verificationCode = await _verificationCodeRepository
            .GetByEmailAndCodeAsync(verifyEmailDto.Email.ToLowerInvariant(), verifyEmailDto.VerificationCode);

        if (verificationCode == null)
        {
            throw new InvalidOperationException("Invalid verification code.");
        }

        if (verificationCode.IsUsed)
        {
            throw new InvalidOperationException("Verification code has already been used.");
        }

        if (verificationCode.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Verification code has expired.");
        }

        // Mark code as used
        verificationCode.IsUsed = true;
        await _verificationCodeRepository.UpdateAsync(verificationCode);

        // Mark user as verified
        var user = await _userRepository.GetByIdAsync(verificationCode.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        user.IsEmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Generate JWT token
        var (accessToken, refreshToken, expiresAt) = _jwtService.GenerateTokens(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified
            }
        };
    }

    /// <inheritdoc/>
    public async Task<string> ResendVerificationCodeAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLowerInvariant());
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.IsEmailVerified)
        {
            throw new InvalidOperationException("Email is already verified.");
        }

        // Check cooldown period (1 minute)
        var lastCodeSentTime = await _verificationCodeRepository.GetLastCodeSentTimeAsync(user.Id);
        if (lastCodeSentTime.HasValue && DateTime.UtcNow - lastCodeSentTime.Value < TimeSpan.FromMinutes(1))
        {
            throw new InvalidOperationException("Please wait before requesting a new verification code.");
        }

        await GenerateAndSendVerificationCodeAsync(user);

        return "Verification code sent successfully.";
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLowerInvariant());
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Verify password
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Check if email is verified
        if (!user.IsEmailVerified)
        {
            throw new UnauthorizedAccessException("Please verify your email before logging in.");
        }

        // Generate JWT token
        var (accessToken, refreshToken, expiresAt) = _jwtService.GenerateTokens(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified
            }
        };
    }

    /// <summary>
    /// Generates a secure 6-digit verification code and sends it via email
    /// </summary>
    /// <param name="user">The user to send the code to</param>
    private async Task GenerateAndSendVerificationCodeAsync(User user)
    {
        // Generate secure 6-digit code
        var code = GenerateSecureVerificationCode();

        // Create verification code entity
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10) // 10 minutes expiry
        };

        // Save verification code
        await _verificationCodeRepository.AddAsync(verificationCode);

        // Send email
        await _emailService.SendVerificationCodeAsync(user.Email, user.FullName, code);
    }


    private static string GenerateSecureVerificationCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var randomNumber = Math.Abs(BitConverter.ToInt32(bytes, 0));
        return (randomNumber % 900000 + 100000).ToString();
    }
}