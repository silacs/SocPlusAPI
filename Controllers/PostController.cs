using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocPlus.DTOs;
using SocPlus.Services;
using SocPlus.Utilities;
using System.Security.Claims;

namespace SocPlus.Controllers {
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostController(PostService postService) : ControllerBase {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetPosts([FromQuery] PaginationDTO dto) {
            return (await postService.GetPosts(null, dto)).ToActionResult();
        }
        [HttpGet("{id}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult> GetComments([FromRoute] string id, [FromQuery] PaginationDTO dto) {
            return (await postService.GetComments(id, null, dto)).ToActionResult();
        }
        [HttpGet("{id}/votes")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVotes([FromRoute] string id) {
            var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;
            return (await postService.GetVotes(id, userId)).ToActionResult();
        }

        [HttpGet("/followers")]
        public async Task<ActionResult> GetFollowers() {
            return (await postService.GetFollowers(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).ToActionResult();
        }
        [HttpGet("/following")]
        public async Task<ActionResult> GetFollowings() {
            return (await postService.GetFollowings(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).ToActionResult();
        }
        [HttpPost("upload")]
        public async Task<ActionResult> UploadPost([FromForm] PostUploadDTO dto) {
            return (await postService.UploadPost(User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto)).ToActionResult();
        }
        [HttpPost("add-comment")]
        public async Task<ActionResult> AddComment(AddCommentDTO dto) {
            dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!; 
            return (await postService.AddComment(dto)).ToActionResult();
        }
        [HttpPost("add-vote")]
        public async Task<ActionResult> AddVote(VoteDTO dto) {
            dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return (await postService.AddVote(dto)).ToActionResult();
        }

        [HttpPost("add-follow")]
        public async Task<ActionResult> AddFollow(FollowDTO dto) {
            return (await postService.AddFollow(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    dto.UserId)
                ).ToActionResult();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost([FromRoute] string id) {
            return (await postService.DeletePost(User.FindFirstValue(ClaimTypes.NameIdentifier)!, id)).ToActionResult();
        }

        [HttpDelete("/comments/{id}")]
        public async Task<ActionResult> DeleteComment([FromRoute] string id) {
            return (await postService.DeleteComment(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    id)
                ).ToActionResult();
        }
        [AllowAnonymous]
        [HttpGet("image/{filename}")]
        public async Task<ActionResult> GetImage([FromRoute] string filename) {
            var result = await postService.GetImage(filename);
            return !result.Success 
                ? result.ToActionResult() 
                : File((byte[])result.Value!, $"image/{ExtensionToMimeType(filename)}");

            string ExtensionToMimeType(string fileName) {
                var extension = fileName.Split('.').Last();
                return extension switch {
                    "jpg" => "jpeg",
                    "svg" => "svg+xml",
                    "tif" => "tiff",
                    "heif" => "heic",
                    _ => extension
                };
            }
        }
        
    }
}
