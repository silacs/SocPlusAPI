using SocPlus.Models;

namespace SocPlus.DTOs; 
public class PostDTO {
    public Guid Id { get; set; }
    public required UserDTO User { get; set; }
    public required string Text { get; set; }
    public required string[] Images { get; set; }
    public DateTimeOffset Created { get; set; }
    public Visibility Visibility { get; set; }
    public static explicit operator PostDTO(Post post) {
        return new PostDTO {
            Id = post.Id,
            User = (UserDTO)post.User,
            Text = post.Text,
            Images = post.Images.Select(i => i.FileName).ToArray(),
            Visibility = post.Visibility,
            Created = post.Created,
        };
    }
}
