using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocPlus.DTOs;
using SocPlus.Services;
using SocPlus.Utilities;
using System.Security.Claims;

namespace SocPlus.Controllers; 
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase {
    private readonly AuthService _auth;
    public AuthController(AuthService auth) {
        _auth = auth;
    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetSelf() {
        return (await _auth.GetUser(User.FindFirstValue(ClaimTypes.NameIdentifier)!, true)).ToActionResult();
    }
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser() {
        return (await _auth.DeleteUser(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).ToActionResult();
    }
    [HttpPut]
    public async Task<ActionResult> EditUser() {
        return Ok();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser([FromRoute] string id) {
        return (await _auth.GetUser(id)).ToActionResult();
    }
    [HttpPost("signup")]
    public async Task<ActionResult> Signup(SignupDTO signup) {
        return (await _auth.Signup(signup)).ToActionResult();
    }
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO login) {
        return (await _auth.Login(login)).ToActionResult();
    }
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshDTO refresh) {
        return (await _auth.Refresh(refresh)).ToActionResult();
    }
    [HttpPost("verify")]
    public async Task<ActionResult> Verify(VerifyDTO verify) {
        return (await _auth.Verify(verify)).ToActionResult();
    }
    [HttpPost("send-code")]
    public async Task<ActionResult> SendCode(SendCodeDTO code) {
        return (await _auth.SendCode(code)).ToActionResult();
    }
}
