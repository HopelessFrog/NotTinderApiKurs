using Microsoft.AspNetCore.Mvc;
using Redis.Cache;
using UsersApi.Models.Entitys.Dtos;
using UsersApi.Services.Interfaces;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser([FromServices] IRedisCacheService redisCache, Guid id)
    {
        var user = await redisCache.GetAsync<UserShortDto>(id.ToString());

        if (user != null) return Ok(user);

        user = await _userService.GetUserShortById(id);

        if (user != null)
            await redisCache.SaveAsync(id.ToString(), user);
        else
            return NotFound();

        return Ok(user);
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetUser([FromServices] IRedisCacheService redisCache)
    {
        return Ok(await _userService.GetAllUsers());
    }
}