#pragma warning disable CS8618
namespace SocPlus.Models;

public class Follow {
    public Guid FollowerId { get; set; }
    public Guid UserId { get; set; }
    //Navigation Props
    public User Follower;
    public User User;
}