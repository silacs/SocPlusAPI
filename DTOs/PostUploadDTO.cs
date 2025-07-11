using SocPlus.Models;

namespace SocPlus.DTOs; 
public class PostUploadDTO {
    public required string Text { get; set; }
    public IFormFile[]? Images { get; set; }
    public Visibility Visibility { get; set; }
}
