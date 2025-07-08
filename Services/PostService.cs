using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using SocPlus.Data;
using SocPlus.DTOs;
using SocPlus.Models;

namespace SocPlus.Services; 
public class PostService {
    private string FilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Images");
    private readonly SocPlusDbContext _context;
    public PostService(SocPlusDbContext context) {
        _context = context;
        Directory.CreateDirectory(FilePath);
    }
    public async Task<Result> GetPosts(string? userId) {
        var posts = await _context.Posts.Include(p => p.User).Include(p => p.Images).Where(p => p.Visibility == Visibility.Public).OrderByDescending(p => p.Created).ToListAsync();
        return new Result(posts.Select(p => (PostDTO)p));
    }
    public async Task<Result> GetComments(string postId, string? userId = null) {
        var comments = await Task.Run(() => _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == Guid.Parse(postId) && c.ParentCommentId == null)
            .OrderByDescending(c => c.Created)
            .ToList());
        return new Result(comments.Select(p => (CommentDTO)p));
    }
    public async Task<Result> GetVotes(string postId, string? userId) {
        var votes = await Task.Run(() => _context.Votes.Where(v => v.PostId == Guid.Parse(postId)));
        var goodVotes = votes.Where(v => v.Positive).Count();
        var badVotes = votes.Count() - goodVotes;
        bool? userVote = userId is null ? null : (await votes.FirstOrDefaultAsync(v => v.UserId == Guid.Parse(userId)))?.Positive;
        return new Result(new {
            goodVotes,
            badVotes,
            userVote
        });
    }
    public async Task<Result> AddComment(AddCommentDTO dto) {
        var post = await _context.Posts.FindAsync(Guid.Parse(dto.PostId));
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        if (dto.ParentCommentId is not null) {
            var parentComment = await _context.Comments.FindAsync(Guid.Parse(dto.ParentCommentId));
            if (parentComment is null) return new Result(new Error("ParentCommentId", "Parent Comment doesn't exist"));
        }
        await _context.Comments.AddAsync((Comment)dto);
        await _context.SaveChangesAsync();
        return new Result("Successfully added comment");
    }
    public async Task<Result> AddVote(VoteDTO dto) {
        var post = await _context.Posts.FindAsync(Guid.Parse(dto.PostId));
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        var vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == Guid.Parse(dto.UserId) && v.PostId == Guid.Parse(dto.PostId));
        if (vote is not null) _context.Votes.Remove(vote);
        if (dto.Positive is null) {
            await _context.SaveChangesAsync();
            return new Result("Successfully removed vote");
        };
        await _context.Votes.AddAsync((Vote)dto);
        await _context.SaveChangesAsync();
        return new Result("Successfully added vote");
    }
    public async Task<Result> DeletePost(string userId, string postId) {
        var post = _context.Posts.Find(Guid.Parse(postId));
        if (post is null) return new Result(new Error("PostId", "Post doesn't exist"));
        if (post.UserId != Guid.Parse(userId)) return new Result(new Error("UserId", "You are not the owner of this post")) {
            ErrorCode = 403
        };
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return new Result("Successfully deleted post");
    }
    public async Task<Result> UploadPost(string userId, PostUploadDTO postdto) {
        var post = new Post() { 
            UserId = Guid.Parse(userId),
            Text = postdto.Text,
            Visibility = postdto.Visibility
        };
        
        if (postdto.Images?.Length == 0 || postdto.Images is null) {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return new Result("Successfully uploaded post");
        }

        List<Image> filenames = []; 
        foreach (var image in postdto.Images) {
            if (!image.ContentType.StartsWith(@"image/", StringComparison.OrdinalIgnoreCase)) {
                _ = Cleanup();
                return new Result(new Error("Image", $"Invalid Image: {image.FileName}"));
            }

            var filename = $"{Guid.NewGuid().ToString()}.{image.FileName.Split('.').Last()}";
            filenames.Add(new Image {
                PostId = post.Id,
                FileName = filename
            });

            using var stream = new FileStream(Path.Combine(FilePath, filename), FileMode.Create);
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
    public async Task<Result> GetImage(string filename) {
        var image = await _context.Images.Include(i => i.Post).FirstOrDefaultAsync(i => i.FileName == filename);
        if (image is null) return new Result(new Error("Filename", "Image not found")) { ErrorCode = 404 };
        if (image.Post.Visibility != Visibility.Public) 
            return new Result(new Error("Forbidden", "Forbidden")) { ErrorCode = 403 };
        if (!File.Exists(Path.Combine(FilePath, filename))) 
            return new Result(new Error("Filename", "Image not found")) { ErrorCode = 404 };
        return new Result(File.ReadAllBytes(Path.Combine(FilePath, filename)));
    }
}
