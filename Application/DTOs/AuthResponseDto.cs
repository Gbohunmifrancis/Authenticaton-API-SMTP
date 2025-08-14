namespace AuthAPI.Application.DTOs;


public class AuthResponseDto
{
   
    public string AccessToken { get; set; } = string.Empty;
    
    public string RefreshToken { get; set; } = string.Empty;

   
    public DateTime ExpiresAt { get; set; }

    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    
    public Guid Id { get; set; }

    
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    public bool IsEmailVerified { get; set; }
}