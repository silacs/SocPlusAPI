using System.ComponentModel.DataAnnotations;

namespace SocPlus.DTOs; 
public class SendCodeDTO {
    [RegularExpression(@"^[\w.+-]+@[a-zA-Z\d.-]+\.[a-zA-Z]{2,}$"), MaxLength(100)]
    public required string Email { get; set; }
}
