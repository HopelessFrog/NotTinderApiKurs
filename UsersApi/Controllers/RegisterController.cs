using Microsoft.AspNetCore.Mvc;
using UsersApi.Models.Entitys;
using UsersApi.Services.Interfaces;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;

    public RegisterController(IRegisterService registerService)
    {
        _registerService = registerService;
    }


    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _registerService.Register(request);
            return Ok(new { StatusMessage = "Acc created" });
        }
        catch (ArgumentException ex)
        {
            return Ok(new { StatusMessage = "User already exists" });
        }
        catch (Exception e)
        {
            return BadRequest(new { StatusMessage = "Happen something bad, try again later" });
        }
    }
}