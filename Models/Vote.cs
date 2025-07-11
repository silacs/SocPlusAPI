#pragma warning disable CS8618
namespace SocPlus.Models; 
public class Vote {
    public Guid UserId { get; init; }
    public Guid PostId { get; init; }
    public bool Positive { get; init; }
    public DateTimeOffset VoteDate { get; init; } = DateTimeOffset.UtcNow;
    //Navigation Props
    public User User { get; init; }
    public Post Post { get; init; }
}
