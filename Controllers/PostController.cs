using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocPlus.DTOs;
using SocPlus.Services;
using SocPlus.Utilities;
using System.Security.Claims;

namespace SocPlus.Controllers {
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase {
        public readonly PostService _postService;
        public PostController(PostService postService) {
            _postService = postService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetPosts() {
            return (await _postService.GetPosts(null)).ToActionResult();
        }
        [HttpGet("{id}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult> GetComments([FromRoute] string id) {
            return (await _postService.GetComments(id)).ToActionResult();
        }
        [HttpGet("{id}/votes")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVotes([FromRoute] string id) {
            var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;
            return (await _postService.GetVotes(id, userId)).ToActionResult();
        }
        [HttpPost("upload")]
        public async Task<ActionResult> UploadPost([FromForm] PostUploadDTO dto) {
            return (await _postService.UploadPost(User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto)).ToActionResult();
        }
        [HttpPost("add-comment")]
        public async Task<ActionResult> AddComment(AddCommentDTO dto) {
            dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!; 
            return (await _postService.AddComment(dto)).ToActionResult();
        }
        [HttpPost("add-vote")]
        public async Task<ActionResult> AddVote(VoteDTO dto) {
            dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return (await _postService.AddVote(dto)).ToActionResult();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost([FromRoute] string id) {
            return (await _postService.DeletePost(User.FindFirstValue(ClaimTypes.NameIdentifier)!, id)).ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("image/{filename}")]
        public async Task<ActionResult> GetImage([FromRoute] string filename) {
            var result = await _postService.GetImage(filename);
            if (!result.Success) return result.ToActionResult();
            return File((byte[])result.Value, $"image/{ExtensionToMimeType(filename)}");

            string ExtensionToMimeType(string filename) {
                var extension = filename.Split('.').Last();
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
