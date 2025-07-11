using SocPlus.Models;

namespace SocPlus.DTOs; 
public class AddCommentDTO {
    public required string UserId { get; set; }
    public required string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public required string Content { get; set; }
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
