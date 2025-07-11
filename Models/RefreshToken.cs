#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class RefreshToken {
    [Key]
    public Guid Jti { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    [MaxLength(256)]
    public required string Token { get; init; }
    public DateTime Iat { get; init; } = DateTime.UtcNow;
    //Navigation Props
    public User User { get; init; }
}
