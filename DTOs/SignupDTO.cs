using SocPlus.Models;
using System.ComponentModel.DataAnnotations;

namespace SocPlus.DTOs; 
public class SignupDTO {
    [MinLength(2), MaxLength(100)]
    public required string Name { get; set; }

    [MinLength(2), MaxLength(100)]
    public required string Surname { get; set; }
    
    [RegularExpression(@"^[\w.+-]+@[a-zA-Z\d.-]+\.[a-zA-Z]{2,}$"), MaxLength(100)]
    public required string Email { get; set; }
    
    [MinLength(2), MaxLength(100), RegularExpression(@"^\w+$")]
    public required string Username { get; set; }
    
    [MinLength(8), MaxLength(100)]
    public required string Password { get; set; }

    public static explicit operator User(SignupDTO signupDTO) {
        return new User {
            Name = signupDTO.Name,
            Surname = signupDTO.Surname,
            Email = signupDTO.Email,
            Username = signupDTO.Username,
            PasswordHash = signupDTO.Password,
        };
    }
}
