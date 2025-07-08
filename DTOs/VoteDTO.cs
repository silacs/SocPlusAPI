using SocPlus.Models;

namespace SocPlus.DTOs; 
public class VoteDTO {
    public string UserId { get; set; }
    public string PostId { get; set; }
    public bool? Positive { get; set; }
    public static explicit operator Vote(VoteDTO dto) {
        return new Vote {
            UserId = Guid.Parse(dto.UserId),
            PostId = Guid.Parse(dto.PostId),
            Positive = dto.Positive ?? false,
        };
    }
}
