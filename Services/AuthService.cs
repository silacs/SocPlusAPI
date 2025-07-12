using Microsoft.EntityFrameworkCore;
using SocPlus.Data;
using SocPlus.DTOs;
using SocPlus.Models;
using SocPlus.Utilities;
using System.Security.Cryptography;

namespace SocPlus.Services; 
public class AuthService(SocPlusDbContext context, MailService mail, JwtConfig config) {
    public async Task<Result> GetUser(string id, bool self = false) {
        var user = await context.Users.FindAsync(id.ToGuid());
        if (user is null) return new Result(new Error("Id", "Couldn't find an user with that id")) { ErrorCode = 404 };
        var userDTO = (UserDTO)user;
        userDTO.Email = self ? userDTO.Email : "hidden";
        return new Result(userDTO);
    }
    public async Task<Result> DeleteUser(string id) {
        var user = await context.Users.FindAsync(id.ToGuid());
        if (user is null) return new Result(new Error("Id", "Couldn't find an user with that id")) { ErrorCode = 404 };
        var userVotes = context.Votes.Where(v => v.UserId == user.Id);
        context.Users.Remove(user);
        context.Votes.RemoveRange(userVotes);
        await context.SaveChangesAsync();
        return new Result("Successfully deleted user");
    }
    public async Task<Result> Signup(SignupDTO signup) {
        signup.Email = signup.Email.Trim().ToLower();
        signup.Username = signup.Username.Trim().ToLower();
        signup.Password = BCrypt.Net.BCrypt.HashPassword(signup.Password);
        var users = await Task.Run(() => context.Users.Where(u => u.Email == signup.Email || u.Username == signup.Username));
        List<Error> errors = [];
        if (users.Any(u => u.Email == signup.Email && u.Verified)) errors.Add(new Error("Email", "Email is already taken"));
        if (users.Any(u => u.Username == signup.Username)) errors.Add(new Error("Username", "Username is already taken"));
        if (errors.Any()) return new Result(errors.ToArray());
        
        context.Users.RemoveRange(users);
        var code = await GenerateCode();
        var user = (User)signup;
        await context.Users.AddAsync(user);
        await context.VerificationCodes.AddAsync(new VerificationCode {
            Email = signup.Email,
            Code = code,
            UserId = user.Id
        });
        await context.SaveChangesAsync();
        _ = mail.SendCode(code, signup.Email);
        return new Result("Successfully signed up. See the verification code on your email");
    }
    public async Task<Result> Login(LoginDTO login) {
        login.Email = login.Email.Trim().ToLower();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == login.Email || u.Username == login.Email);
        if (user is null) return new Result(new Error("Credentials", "Incorrect username or password"));
        if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash)) return new Result(
                new Error("Credentials", "Incorrect username or password")
            );
        if (!user.Verified) return new Result(new Error("Username", "This user is not verified"));
        return new Result(new TokensDTO {
            AccessToken = user.GenerateAccessToken(config),
            RefreshToken = await user.GenerateRefreshToken(context)
        });
    }
    public async Task<Result> Refresh(RefreshDTO refresh) {
        var token = await context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == refresh.RefreshToken);
        if (token is null) return new Result(new Error("RefreshToken", "Invalid refresh token"));
        return new Result(new {
            AccessToken = token.User.GenerateAccessToken(config)
        });
    }
    public async Task<Result> Verify(VerifyDTO verify) {
        verify.Email = verify.Email.Trim().ToLower();
        var code = await context.VerificationCodes
            .Include(c => c.User)
            .OrderByDescending(c => c.SentAt)
            .FirstOrDefaultAsync(c => c.Email == verify.Email);
        if (code is null) return new Result(new Error("Code", "Invalid Code"));
        if (code.Code != verify.Code) return new Result(new Error("Code", "Invalid Code"));
        if (code.User.Verified) return new Result(new Error("Email", "Email is already verified"));
        code.User.Verified = true;
        context.VerificationCodes.RemoveRange(context.VerificationCodes.Where(c => c.Email == verify.Email));
        await context.SaveChangesAsync();
        return new Result("Successfully verified email");
    }
    public async Task<bool> UserExistsAsync(string userId) {
        var user = await context.Users.FindAsync(userId.ToGuid());
        return user is not null;
    }
    public async Task<Result> SendCode(SendCodeDTO codeDTO) {
        codeDTO.Email = codeDTO.Email.Trim().ToLower();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == codeDTO.Email);
        if (user is null) return new Result("We'll send a code if that email is registered");
        if (user.Verified) return new Result(new Error("Email", "Email is already verified"));
        context.VerificationCodes.RemoveRange(context.VerificationCodes.Where(c => c.Email == codeDTO.Email));
        var code = await GenerateCode();
        await context.VerificationCodes.AddAsync(new VerificationCode {
            Code = code,
            Email = codeDTO.Email,
            UserId = user.Id,
        });
        await context.SaveChangesAsync();
        _ = mail.SendCode(code, codeDTO.Email);
        return new Result("We'll send a code if that email is registered");
    }
    private static async Task<string> GenerateCode() {
        var code = await Task.Run(() => RandomNumberGenerator.GetInt32(0, 1000000));
        return code.ToString("D6");
    }
}
