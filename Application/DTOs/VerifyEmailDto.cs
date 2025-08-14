using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Application.DTOs;

public class VerifyEmailDto
{
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

  
    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be exactly 6 digits")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must contain only digits")]
    public string VerificationCode { get; set; } = string.Empty;
}