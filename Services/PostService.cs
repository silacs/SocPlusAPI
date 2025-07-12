using Microsoft.EntityFrameworkCore;
using SocPlus.Data;
using SocPlus.DTOs;
using SocPlus.Models;
using SocPlus.Utilities;

namespace SocPlus.Services; 
public class PostService {
    private string FilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Images");
    private readonly SocPlusDbContext _context;
    public PostService(SocPlusDbContext context) {
        _context = context;
        Directory.CreateDirectory(FilePath);
    }
    public async Task<Result> DeleteFollow(string followerId, string userId) {
        var user = await _context.Follows
            .Where(f => f.FollowerId == followerId.ToGuid())
            .FirstOrDefaultAsync(f => f.UserId == userId.ToGuid());
        if (user is null) return new Result("User", true);
        _context.Follows.Remove(user);
        await _context.SaveChangesAsync();
        return new Result("Successfully unfollowed user");
    }

    public async Task<Result> GetFriends(string userId) {
        var userGuid = userId.ToGuid();
        var friends = 
            from f1 in _context.Follows.Include(f => f.User)
            join f2 in _context.Follows
                on f1.UserId equals f2.FollowerId
            where f1.FollowerId == userGuid && f2.UserId == userGuid
            select f1.User;
        return new Result(await friends.ToListAsync());
    }
    public async Task<Result> GetFollowings(string userId) {
        var followers = await Task.Run(() => _context.Follows.Include(f => f.User).Where(f => userId.ToGuid() == f.FollowerId).ToList());
        return followers.Count > 0 ? new Result(followers.Select(f => (UserDTO)f.User)) : new Result(followers);
    }
    public async Task<Result> GetFollowers(string userId) {
        var followers = await Task.Run(() => _context.Follows.Include(f => f.Follower).Where(f => userId.ToGuid() == f.UserId).ToList());
        return followers.Count > 0 ? new Result(followers.Select(f => (UserDTO)f.Follower)) : new Result(followers);
    }
    public async Task<Result> GetPosts(string? userId, PaginationDTO dto) {
        List<User> friends = userId is null ? [] : await (
            from f1 in _context.Follows.Include(f => f.User)
            join f2 in _context.Follows
                on f1.UserId equals f2.FollowerId
            where f1.FollowerId == userId.ToGuid() && f2.UserId == userId.ToGuid()
            select f1.User
        ).ToListAsync();
        
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Images)
            .Where(p => p.Visibility == Visibility.Public || (p.Visibility == Visibility.Friends && friends.Contains(p.User)))
            .OrderByDescending(p => p.Created)
            .Paginate(dto.Page, dto.PageSize)
            .ToListAsync();
        var paginatedResult = new PaginatedResult<PostDTO> {
            Page = dto.Page,
            PageSize = dto.PageSize,
            Skip = (dto.Page - 1) * dto.PageSize,
            Total = posts.Count,
            Data = posts.Select(p => (PostDTO)p)
        };
        return new Result(paginatedResult);
    }
    public async Task<Result> GetComments(string postId, string? userId, PaginationDTO dto) {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId.ToGuid() && c.ParentCommentId == null)
            .OrderByDescending(c => c.Created)
            .ThenByDescending(c => userId != null && c.UserId == userId.ToGuid())
            .Paginate(dto.Page, dto.PageSize)
            .ToListAsync();
        return new Result(comments.Select(p => (CommentDTO)p));
    }
    public async Task<Result> GetVotes(string postId, string? userId) {
        var votes = _context.Votes.Where(v => v.PostId == postId.ToGuid());
        var goodVotes = votes.CountAsync(v => v.Positive);
        var badVotes = votes.CountAsync(v => !v.Positive);
        var userVote = userId is null ? null : (await votes.FirstOrDefaultAsync(v => v.UserId == userId.ToGuid()))?.Positive;
        await Task.WhenAll(goodVotes, badVotes);
        return new Result(new {
            goodVotes = goodVotes.Result,
            badVotes = badVotes.Result,
            userVote = userVote
        });
    }
    public async Task<Result> GetReplies(string commentId) {
        var comment = await _context.Comments
            .Include(c => c.Replies
                .OrderByDescending(r => r.Created))
            .FirstOrDefaultAsync(c => c.Id == commentId.ToGuid());
        return comment is null 
            ? new Result(new Error("Comment", "Comment not found")) { ErrorCode = 404 } 
            : new Result(comment.Replies);
    }
    public async Task<Result> AddComment(AddCommentDTO dto) {
        var post = await _context.Posts.FindAsync(dto.PostId.ToGuid());
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        if (dto.ParentCommentId is not null) {
            var parentComment = await _context.Comments.FindAsync(dto.ParentCommentId.ToGuid());
            if (parentComment is null) return new Result(new Error("ParentCommentId", "Parent Comment doesn't exist"));
        }
        await _context.Comments.AddAsync((Comment)dto);
        await _context.SaveChangesAsync();
        return new Result("Successfully added comment");
    }
    public async Task<Result> AddVote(VoteDTO dto) {
        var post = await _context.Posts.FindAsync(dto.PostId.ToGuid());
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        var vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == dto.UserId.ToGuid() && v.PostId == dto.PostId.ToGuid());
        if (vote is not null) _context.Votes.Remove(vote);
        if (dto.Positive is null) {
            await _context.SaveChangesAsync();
            return new Result("Successfully removed vote");
        }
        await _context.Votes.AddAsync((Vote)dto);
        await _context.SaveChangesAsync();
        return new Result("Successfully added vote");
    }
    public async Task<Result> AddFollow(string followerId, string userId) {
        var user = await _context.Users.FindAsync(userId.ToGuid());
        if (user is null) return new Result("User", true);
        await _context.Follows.AddAsync(
            new Follow {
                FollowerId = followerId.ToGuid(),
                UserId = userId.ToGuid()
            }
        );
        await _context.SaveChangesAsync();
        return new Result("Successfully followed user");
    }
    public async Task<Result> UploadPost(string userId, PostUploadDTO postDTO) {
        var post = new Post() { 
            UserId = Guid.Parse(userId),
            Text = postDTO.Text,
            Visibility = postDTO.Visibility
        };
        
        if (postDTO.Images?.Length == 0 || postDTO.Images is null) {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return new Result("Successfully uploaded post");
        }

        List<Image> filenames = []; 
        foreach (var image in postDTO.Images) {
            if (!image.ContentType.StartsWith(@"image/", StringComparison.OrdinalIgnoreCase)) {
                _ = Cleanup();
                return new Result(new Error("Image", $"Invalid Image: {image.FileName}"));
            }

            var filename = $"{Guid.NewGuid().ToString()}.{image.FileName.Split('.').Last()}";
            filenames.Add(new Image {
                PostId = post.Id,
                FileName = filename
            });

            await using var stream = new FileStream(Path.Combine(FilePath, filename), FileMode.Create);
            await image.CopyToAsync(stream);
        }
        await _context.Posts.AddAsync(post);
        await _context.Images.AddRangeAsync(filenames);
        await _context.SaveChangesAsync();
        return new Result("Successfully uploaded post");

        async Task Cleanup() {
            await Task.Run(() => {
                filenames.ForEach((file) => {
                    try { File.Delete(Path.Combine(FilePath, file.FileName)); }
                    catch { Console.WriteLine($"Couldn't delete {file.FileName}"); }
                });
            });
        }
    }
    public async Task<Result> DeletePost(string userId, string postId) {
        var post = await _context.Posts.FindAsync(postId.ToGuid());
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        if (post.UserId != userId.ToGuid()) return new Result(new Error("UserId", "You are not the owner of this post")) {
            ErrorCode = 403
        };
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return new Result("Successfully deleted post");
    }
    public async Task<Result> DeleteComment(string userId, string commentId) {
        var comment = await _context.Comments.FindAsync(commentId.ToGuid());
        if (comment is null) return new Result(new Error("CommentId", "Comment doesn't exist"));
        if (comment.UserId != userId.ToGuid()) return new Result(new Error("UserId", "You are not the owner of this post")) {
            ErrorCode = 403
        };
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return new Result("Successfully deleted comment");
    }
    public async Task<Result> GetImage(string filename) {
        var image = await _context.Images.Include(i => i.Post).FirstOrDefaultAsync(i => i.FileName == filename);
        if (image is null) return new Result(new Error("Filename", "Image not found")) { ErrorCode = 404 };
        if (image.Post.Visibility != Visibility.Public) 
            return new Result(new Error("Forbidden", "Forbidden")) { ErrorCode = 403 };
        return File.Exists(Path.Combine(FilePath, filename)) 
            ? new Result(await File.ReadAllBytesAsync(Path.Combine(FilePath, filename))) 
            : new Result(new Error("Filename", "Image not found")) { ErrorCode = 404 };
    }
}
