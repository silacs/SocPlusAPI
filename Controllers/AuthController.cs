using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocPlus.DTOs;
using SocPlus.Services;
using SocPlus.Utilities;
using System.Security.Claims;

namespace SocPlus.Controllers; 
[Route("api/auth")]
[ApiController]
public class AuthController(AuthService auth) : ControllerBase {
    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetSelf() {
        return (await auth.GetUser(User.FindFirstValue(ClaimTypes.NameIdentifier)!, true)).ToActionResult();
    }
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser() {
        return (await auth.DeleteUser(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).ToActionResult();
    }
    [HttpPut]
    [Authorize]
    public async Task<ActionResult> EditUser([FromBody] SignupDTO dto) {
        return await Task.Run(Ok);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser([FromRoute] string id) {
        return (await auth.GetUser(id)).ToActionResult();
    }
    [HttpPost("signup")]
    public async Task<ActionResult> Signup(SignupDTO signup) {
        return (await auth.Signup(signup)).ToActionResult();
    }
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO login) {
        return (await auth.Login(login)).ToActionResult();
    }
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshDTO refresh) {
        return (await auth.Refresh(refresh)).ToActionResult();
    }
    [HttpPost("verify")]
    public async Task<ActionResult> Verify(VerifyDTO verify) {
        return (await auth.Verify(verify)).ToActionResult();
    }
    [HttpPost("send-code")]
    public async Task<ActionResult> SendCode(SendCodeDTO code) {
        return (await auth.SendCode(code)).ToActionResult();
    }
}
