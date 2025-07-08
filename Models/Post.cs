#pragma warning disable CS8618

namespace SocPlus.Models; 
public class Post {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public Visibility Visibility { get; set; }
    //Navigation Props
    public User User { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Vote> Votes { get; set; }
    public ICollection<Image> Images { get; set; }
}
public enum Visibility {
    Public,
    Friends,
    Private
}
