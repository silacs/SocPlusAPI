#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class Comment {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid PostId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? ParentCommentId { get; init; }
    
    [MaxLength(5000)]
    public string Content { get; init; }
    public bool IsReply { get; init; }
    public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    //Navigation Props
    public Post Post { get; init; }
    public User User { get; init; }
    public Comment ParentComment { get; init; }
    public ICollection<Comment> Replies { get; init; }
}
