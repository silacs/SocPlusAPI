#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace SocPlus.Models; 
public class Image {
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string FileName { get; set; }
    //Navigation Props
    public Post Post { get; set; }
}
