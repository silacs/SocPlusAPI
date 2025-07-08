#pragma warning disable CS8618
using SocPlus.Models;
using System.ComponentModel.DataAnnotations;

namespace SocPlus.DTOs; 
public class UserDTO {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
    public bool Verified { get; set; }
    public string Role { get; set; }
    public static explicit operator UserDTO(User user) {
        return new UserDTO {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Username = user.Username,
            RegistrationDate = user.RegistrationDate,
            Verified = user.Verified,
            Role = user.Role.ToString()
        };
    }
}
