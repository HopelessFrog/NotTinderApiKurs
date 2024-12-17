using GateWayApi.CustomAggregators.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GateWayApi.Agregators;

[ApiController]
[Authorize]
[Route("api")]
public class DonatersAgregator : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;

    public DonatersAgregator(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
    }

    [HttpGet("Donaters")]
    public async Task<IActionResult> GetDonaters(int id)
    {
        var client = _httpClientFactory.CreateClient();

        var donatersResponse = await client.GetAsync($"https://localhost:7185/api/Startups/Donaters?id={id}");
        if (donatersResponse.IsSuccessStatusCode)
        {
            var donaters = await donatersResponse.Content.ReadFromJsonAsync<List<DonaterDto>>();

            foreach (var donater in donaters)
                if (!_memoryCache.TryGetValue(donater.UserId.ToString(), out UserDto value))
                {
                    var userResponse = await client.GetAsync($"https://localhost:7226/api/User?id={donater.UserId}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();

                        _memoryCache.Set(donater.UserId.ToString(), user, TimeSpan.FromMinutes(1));
                        donater.Name = user.Name;
                    }
                    else
                    {
                        donater.Name = value.Name;
                    }
                }

            return Ok(donaters);
        }

        return BadRequest("Something went wrong");
    }
}