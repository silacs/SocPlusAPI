using SocPlus.Models;

namespace SocPlus.DTOs; 
public class CommentDTO {
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; }
    public bool isReply { get; set; }
    public DateTimeOffset Created { get; set; }
    public UserDTO User { get; set; }
    public static explicit operator CommentDTO(Comment com) {
        return new CommentDTO {
            Id = com.Id,
            PostId = com.PostId,
            UserId = com.UserId,
            ParentCommentId = com.ParentCommentId,
            Content = com.Content,
            isReply = com.IsReply,
            Created = com.Created,
            User = (UserDTO)com.User
        };
    }
}
