#pragma warning disable CS8618
using Microsoft.AspNetCore.Mvc;
using SocPlus.Models;

namespace SocPlus.DTOs; 
public class PostUploadDTO {
    public string Text { get; set; }
    public IFormFile[]? Images { get; set; }
    public Visibility Visibility { get; set; }
}
