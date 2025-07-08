#pragma warning disable CS8618
namespace SocPlus.Models; 
public class Vote {
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public bool Positive { get; set; }
    public DateTimeOffset VoteDate { get; set; } = DateTimeOffset.UtcNow;
    //Navigation Props
    public User User { get; set; }
    public Post Post { get; set; }
}
