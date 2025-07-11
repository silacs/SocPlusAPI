#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class VerificationCode {
    [Key]
    [MaxLength(300)]
    public required string Email { get; init; }
    [MaxLength(6)]
    public required string Code { get; init; }
    public Guid UserId { get; init; }
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    //Navigation Props
    public User User { get; init; }
}
