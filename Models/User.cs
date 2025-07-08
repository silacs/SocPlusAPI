#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models {
    [Index(nameof(Email), IsUnique = true)]
    public class User {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTimeOffset RegistrationDate { get; set; } = DateTimeOffset.UtcNow;
        public bool Verified { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.User;
        //Navigation Props
        public ICollection<Post> Posts { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
    public enum UserRole {
        User,
        Admin
    }
}
