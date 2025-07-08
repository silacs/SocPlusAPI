#pragma warning disable CS8618

namespace SocPlus.Models; 
public class JwtConfig {
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessHours { get; set; }
}
