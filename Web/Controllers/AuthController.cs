using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using AuthAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await authService.RegisterAsync(registerDto);
            logger.LogInformation("User registration successful for email: {Email}", registerDto.Email);
            
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Registration successful",
                Data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Registration failed for email {Email}: {Error}", registerDto.Email, ex.Message);
            return Conflict(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during registration for email: {Email}", registerDto.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An unexpected error occurred"
            });
        }
    }

   
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        try
        {
            var result = await authService.VerifyEmailAsync(verifyEmailDto);
            logger.LogInformation("Email verification successful for: {Email}", verifyEmailDto.Email);
            
            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Email verified successfully",
                Data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Email verification failed for {Email}: {Error}", verifyEmailDto.Email, ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during email verification for: {Email}", verifyEmailDto.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An unexpected error occurred"
            });
        }
    }

    
    [HttpPost("resend-code")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeRequest request)
    {
        try
        {
            var result = await authService.ResendVerificationCodeAsync(request.Email);
            logger.LogInformation("Verification code resent to: {Email}", request.Email);
            
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Verification code sent",
                Data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("wait"))
        {
            logger.LogWarning("Resend code rate limited for {Email}: {Error}", request.Email, ex.Message);
            return StatusCode(429, new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Resend code failed for {Email}: {Error}", request.Email, ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during code resend for: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An unexpected error occurred"
            });
        }
    }

    
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await authService.LoginAsync(loginDto);
            logger.LogInformation("Login successful for: {Email}", loginDto.Email);
            
            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Login failed for {Email}: {Error}", loginDto.Email, ex.Message);
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for: {Email}", loginDto.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An unexpected error occurred"
            });
        }
    }
}

public class ResendCodeRequest
{
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}


public class ApiResponse<T>
{
    
    public bool Success { get; set; }
    
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}