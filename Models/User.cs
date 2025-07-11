#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SocPlus.Models {
    [Index(nameof(Email), IsUnique = true)]
    public class User {
        public Guid Id { get; init; } = Guid.NewGuid();
        [MaxLength(100)]
        public required string Name { get; init; }
        [MaxLength(100)]
        public required string Surname { get; init; }
        [MaxLength(300)]
        public required string Email { get; init; }
        [MaxLength(100)]
        public required string Username { get; init; }
        [MaxLength(256)]
        public required string PasswordHash { get; init; }
        public DateTimeOffset RegistrationDate { get; init; } = DateTimeOffset.UtcNow;
        public bool Verified { get; set; } = false;
        public UserRole Role { get; init; } = UserRole.User;
        //Navigation Props
        public ICollection<Post> Posts { get; init; }
        public ICollection<Vote> Votes { get; init; }
        public ICollection<Comment> Comments { get; init; }
    }
    public enum UserRole {
        User,
        Admin
    }
}
