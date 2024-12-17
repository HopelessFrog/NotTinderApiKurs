using Microsoft.AspNetCore.Mvc;
using Redis.Cache;
using StartupsApi.Interfaces;
using StartupsApi.Models.DTOs;
using StartupsApi.Models.Requests;

namespace StartupsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StartupsController : ControllerBase
{
    private readonly IStartupsService _startupsService;

    public StartupsController(IStartupsService startupsService, IRedisCacheService redisCacheService)
    {
        _startupsService = startupsService;
    }

    [HttpGet]
    [Route("Donaters")]
    public async Task<IActionResult> GetDonaters(int id)
    {
        return Ok(await _startupsService.GetDonaters(id));
    }

    [HttpGet]
    [Route("Donated")]
    public async Task<IActionResult> GetDonatedSmount(int id)
    {
        return Ok(await _startupsService.GetDonatedAmount(id));
    }

    [HttpGet]
    [Route("TopStartups")]
    public async Task<IActionResult> GetTopStartups()
    {
        var startups = await _startupsService.GetTopStartups();
        if (startups.Count != 0)
            return Ok(startups);
        return NoContent();
    }

    [HttpGet]
    [Route("TopDonaters")]
    public async Task<IActionResult> GetTopDonaters()
    {
        var donaters = await _startupsService.GetTopDonaters();
        if (donaters.Count != 0)
            return Ok(donaters);
        return NoContent();
    }


    [HttpGet]
    [Route("MyStartups")]
    public async Task<IActionResult> GetUserStartups([FromServices] IRedisCacheService redisCacheService,
        [FromHeader] Guid userId)
    {
        var startupIds = await _startupsService.GetUserStartupIds(userId);

        var startups = new List<StartupDTO>();

        foreach (var startupId in startupIds)
        {
            var startup = await redisCacheService.GetAsync<StartupDTO>(startupId.ToString());
            if (startup != null)
            {
                startups.Add(startup);
            }
            else
            {
                startup = await _startupsService.GetStartup(startupId);

                if (startup != null)
                {
                    await redisCacheService.SaveAsync(startupId.ToString(), startup);
                    startups.Add(startup);
                }
            }
        }

        if (startups.Count > 0)
            return Ok(startups);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateStartup([FromHeader] Guid userId,
        [FromForm] CreateOrUpdateStartupRequest request)
    {
        await _startupsService.CreateStartup(request, userId);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStartup([FromHeader] Guid userId, [FromQuery] int id,
        [FromServices] IRedisCacheService redisCacheService)
    {
        await _startupsService.DeleteStartup(userId, id);
        await redisCacheService.DeleteAsync(id.ToString());
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStartup([FromHeader] Guid userId, [FromQuery] int id,
        [FromForm] CreateOrUpdateStartupRequest request,
        [FromServices] IRedisCacheService redisCacheService)
    {
        await _startupsService.UpdateStartup(userId, id, request);
        await redisCacheService.DeleteAsync(id.ToString());

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetStartup([FromServices] IRedisCacheService redisCacheService, int startupId)
    {
        var startup = await redisCacheService.GetAsync<StartupDTO>(startupId.ToString());
        if (startup != null) return Ok(startup);

        startup = await _startupsService.GetStartup(startupId);
        if (startup != null)
            await redisCacheService.SaveAsync(startupId.ToString(), startup);
        else
            return NotFound();

        return Ok(startup);
    }

    [HttpPost]
    [Route("StartupsIds")]
    public async Task<IActionResult> GetStartupIds([FromHeader] Guid userId, [FromBody] List<int> existing)
    {
        return Ok(await _startupsService.GetStartupIds(userId, existing));
    }
}