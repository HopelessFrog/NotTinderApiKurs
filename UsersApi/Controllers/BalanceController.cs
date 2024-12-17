using Microsoft.AspNetCore.Mvc;
using UsersApi.Services.Interfaces;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalanceController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBalance([FromServices] IUserBalanceService balanceService,
        [FromHeader] Guid userId
    )
    {
        var balance = await balanceService.GetBalance(userId);

        return Ok(balance);
    }

    [HttpPost]
    [Route("TopUp")]
    public async Task<IActionResult> TopUpBalance([FromServices] IUserBalanceService balanceService,
        [FromHeader] Guid userId, int value)
    {
        var balance = await balanceService.TopUpBalance(value, userId);

        return Ok(balance);
    }
}