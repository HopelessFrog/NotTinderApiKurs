using StartupsApi.Models.DTOs;
using StartupsApi.Models.Entity;
using StartupsApi.Models.Requests;

namespace StartupsApi.Interfaces;

public interface IStartupsService
{
    Task CreateStartup(CreateOrUpdateStartupRequest createOrUpdateStartupRequest, Guid ownerId);
    Task<StartupDTO> GetStartup(int startupId);
    Task DeleteStartup(Guid userId, int startupId);
    Task UpdateStartup(Guid userId, int startupId, CreateOrUpdateStartupRequest createOrUpdateStartupRequest);
    Task<List<int>> GetStartupIds(Guid userId, List<int> excludedIds);

    Task CreditStartup(int startupId, Guid userId, int amount);
    Task<List<Donater>> GetDonaters(int startupId);
    Task<int> GetDonatedAmount(int startupId);
    Task<List<int>> GetUserStartupIds(Guid userId);
    Task<List<DonaterDto>> GetTopDonaters();

    Task<List<TopStartupDto>> GetTopStartups();
}