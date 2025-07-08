#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class RefreshToken {
    [Key]
    public Guid Jti { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Iat { get; set; } = DateTime.UtcNow;
    //Navigation Props
    public User User { get; set; }
}
