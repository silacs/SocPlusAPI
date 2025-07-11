#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class Post {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    [MaxLength(5000)]
    public string Text { get; init; }
    public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    public Visibility Visibility { get; init; }
    //Navigation Props
    public User User { get; init; }
    public ICollection<Comment> Comments { get; init; }
    public ICollection<Vote> Votes { get; init; }
    public ICollection<Image> Images { get; init; }
}
public enum Visibility {
    Public,
    Friends,
    Private
}
