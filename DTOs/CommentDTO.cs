using SocPlus.Models;

namespace SocPlus.DTOs; 
public class CommentDTO {
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public required string Content { get; set; }
    public bool IsReply { get; set; }
    public DateTimeOffset Created { get; set; }
    public required UserDTO User { get; set; }
    public static explicit operator CommentDTO(Comment com) {
        return new CommentDTO {
            Id = com.Id,
            PostId = com.PostId,
            UserId = com.UserId,
            ParentCommentId = com.ParentCommentId,
            Content = com.Content,
            IsReply = com.IsReply,
            Created = com.Created,
            User = (UserDTO)com.User
        };
    }
}
