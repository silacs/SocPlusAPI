#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class Image {
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    
    [MaxLength(50)]
    public required string FileName { get; init; }
    //Navigation Props
    public Post Post { get; init; }
}
