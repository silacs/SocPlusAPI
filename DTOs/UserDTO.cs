using SocPlus.Models;

namespace SocPlus.DTOs; 
public class UserDTO {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
    public bool Verified { get; set; }
    public required string Role { get; set; }
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
