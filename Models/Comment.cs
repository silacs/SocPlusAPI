#pragma warning disable CS8618

namespace SocPlus.Models; 
public class Comment {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; }
    public bool IsReply { get; set; }
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    //Navigation Props
    public Post Post { get; set; }
    public User User { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; }
}
