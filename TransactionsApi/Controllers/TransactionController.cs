using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.TransactionMessages;
using TransactionsApi.Context;
using TransactionsApi.Saga.Dtos;

namespace TransactionsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> DonateToStartup([FromHeader] Guid userId, [FromBody] TransactionDto transactionDto,
        [FromServices] IBus bus)
    {
        var response = await bus.Request<DonateToStartupRequest, DonateToStartupResponse>(new DonateToStartupRequest
        {
            UserId = userId, TransactionId = Guid.NewGuid(), StartupId = transactionDto.StartupId,
            Amount = transactionDto.Amount
        });
        if (response.Message.Result != null) return BadRequest(response.Message.Result);

        return Ok();
    }

    [HttpGet("UserTransactions")]
    public async Task<IActionResult> GetUserTransactions(Guid id, [FromServices] TransactionDbContext context)
    {
        var transactions = await context.TransactionsStats
            .Where(t => t.UserId == id)
            .OrderByDescending(t => t.Date) // Сортировка по времени
            .ToListAsync();

        if (transactions.Count == 0) return NoContent();
        return Ok(transactions);
    }
}