using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

[AllowAnonymous, ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("authenticate"), AllowAnonymous]
    public async Task<IActionResult> Authenticate(AuthViewModel auth)
    {
        var result = await _userService.Auth(auth);

        return Ok(result);
    }

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Create(CreateUserViewModel user)
    {
        var result = await _userService.Create(user);

        return Ok(result);
    }

    [HttpGet("{nickName}")]
    public async Task<IActionResult> GetUser(string nickName){
        return Ok(await _userService.GetUser(nickName));
    }

    [HttpPost("{id}/cards")]
    public async Task<IActionResult> AddCard(string id, AddCardDto card){
        return Ok(await _userService.AddCard(id, card));
    }

    // [HttpPost("validate-code")]
    // public async Task<IActionResult> ValidateCode(int code)
    // {
    //     var userId = Guid.Parse(User.FindFirst(wh => wh.Type == JwtRegisteredClaimNames.Jti).Value);

    //     var result = await _userService.ValidateCode(userId, code);

    //     return Ok(result);
    // }
}