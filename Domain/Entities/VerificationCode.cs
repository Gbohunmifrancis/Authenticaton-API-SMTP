using System.ComponentModel.DataAnnotations;
using AuthAPI.Domain.Entities;

namespace Enforca.Domain.Entities;
public class VerificationCode
{
    
    public Guid Id { get; set; } = Guid.NewGuid();

    
    public Guid UserId { get; set; }

   
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}