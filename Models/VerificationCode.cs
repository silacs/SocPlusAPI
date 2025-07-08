#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class VerificationCode {
    [Key]
    public string Email { get; set; }
    public string Code { get; set; }
    public Guid UserId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    //Navigation Props
    public User User { get; set; }
}
