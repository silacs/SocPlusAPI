using SocPlus.Models;

namespace SocPlus.DTOs; 
public class AddCommentDTO {
    public string UserId { get; set; }
    public string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public string Content { get; set; }
    public bool IsReply { get; set; }
    public static explicit operator Comment(AddCommentDTO dto) {
        return new Comment {
            PostId = Guid.Parse(dto.PostId),
            UserId = Guid.Parse(dto.UserId),
            ParentCommentId = dto.ParentCommentId is null ? null : Guid.Parse(dto.ParentCommentId),
            Content = dto.Content,
            IsReply = dto.IsReply,
        };
    }
}
