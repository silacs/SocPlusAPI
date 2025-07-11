namespace SocPlus.Models; 
public class JwtConfig {
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessHours { get; init; }
}
