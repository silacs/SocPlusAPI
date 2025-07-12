using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocPlus.Data;
using SocPlus.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocPlus.Utilities; 
public static class Extensions {
    public static string ToCSVColumn(this IEnumerable<string> strings) {
        var i = 0;
        var sb = new StringBuilder();
        var strs = strings.ToList();
        foreach (var str in strs) {
            if (i == strs.Count - 1) {
                sb.Append($"\n  {str}\n");
            }
            else {
                sb.Append($"\n  {str},");
            }
            i++;
        }
        return sb.ToString();
    }
    public static string ToCSV(this IEnumerable<string> strings) {
        var i = 0;
        var sb = new StringBuilder();
        foreach (var str in strings) {
            if (i == 0) {
                sb.Append(str);
            }
            else {
                sb.Append(", " + str);
            }
            i++;
        }
        return sb.ToString();
    }
    public static string Capitalize(this string toCapitalize) {
        return toCapitalize.ToUpper()[0] + toCapitalize.Substring(1).ToLower();
    }
    public static ActionResult ToActionResult(this Result result) {
        if (result.Success) {
            return new OkObjectResult(result.Value);
        } else {
            Dictionary<string, string[]> errors = new Dictionary<string, string[]>();
            foreach (var item in result.Errors) {
                errors.Add(item.Name, item.Errors);
            }
            return new ObjectResult(new { errors }) {
                StatusCode = result.ErrorCode
            };
        }
    }
    public static string GenerateAccessToken(this User user, JwtConfig config) {
        var key = new SymmetricSecurityKey(Convert.FromHexString(config.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config.Issuer,
            audience: config.Audience,
            claims: [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64),
                new Claim("username", user.Username),
                new Claim("email", user.Email),
                new Claim("verified", user.Verified.ToString(), ClaimValueTypes.Boolean),
                new Claim("role", user.Role.ToString())
            ],
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(config.AccessHours),
            signingCredentials: creds
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    public static async Task<string> GenerateRefreshToken(this User user, SocPlusDbContext context) {
        var token = RandomNumberGenerator.GetHexString(128, true);
        _ = await context.RefreshTokens.AddAsync(new RefreshToken {
            UserId = user.Id,
            Token = token
        });
        await context.SaveChangesAsync();
        return token;
    }

    public static Guid ToGuid(this string id) {
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize) {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 0;
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
