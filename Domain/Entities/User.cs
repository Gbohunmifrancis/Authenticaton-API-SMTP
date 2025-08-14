using System.ComponentModel.DataAnnotations;
using Enforca.Domain.Entities;

namespace AuthAPI.Domain.Entities;

public class User
{
  
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

   
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
     
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; } = false;

   
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();
}