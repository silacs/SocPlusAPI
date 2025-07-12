#pragma warning disable CS8618
namespace SocPlus.Models;

public class Follow {
    /// <summary>
    /// The Id of the user who is following the owner of UserId
    /// </summary>
    public Guid FollowerId { get; init; }
    /// <summary>
    /// The Id of the user who is getting followed by the owner of FollowerId
    /// </summary>
    public Guid UserId { get; init; }
    //Navigation Props
    public User Follower { get; init; }
    public User User { get; init; }
}