using CoinRateApi.Context;
using CoinRateApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoinRateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoinRateController : ControllerBase
{
    private readonly CoinRateContext _context;

    public CoinRateController(CoinRateContext context)
    {
        _context = context;
    }

    [HttpGet("CoinsRates")]
    public async Task<ActionResult<IEnumerable<CoinRate>>> GetCoinRates()
    {
        var coinRates = await _context.CoinRates.OrderBy(r => r.DateTime).ToListAsync();
        return Ok(coinRates.Select(c => new CoinRateDTO
        {
            Value = c.Value,
            DateTime = c.DateTime
        }).ToList());
    }

    [HttpGet("ActualCoinRate")]
    public async Task<ActionResult<CoinRate>> GetLastCoinRate()
    {
        var coinRate = _context.CoinRates.OrderBy(r => r.DateTime).Last();
        return Ok(new CoinRateDTO { DateTime = coinRate.DateTime, Value = coinRate.Value });
    }
}