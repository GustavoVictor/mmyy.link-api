using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
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

    [HttpGet("{nickName}"), AllowAnonymous]
    public async Task<IActionResult> GetUser(string nickName){
        if (nickName.Length == 0)
            throw new ErrorException(ErrorCode.UserInvalidUserNickIsEmpty);

        return Ok(await _userService.GetUser(nickName));
    }

    [HttpPost("cards")]
    [Authorize(Roles = "USER")]
    public async Task<IActionResult> AddCard(AddCardDto card){
        string userId = User.Claims.ElementAt(0).Value;

        return Ok(await _userService.AddCard(userId, card));
    }

    // [HttpPost("validate-code")]
    // public async Task<IActionResult> ValidateCode(int code)
    // {
    //     var userId = Guid.Parse(User.FindFirst(wh => wh.Type == JwtRegisteredClaimNames.Jti).Value);

    //     var result = await _userService.ValidateCode(userId, code);

    //     return Ok(result);
    // }
}