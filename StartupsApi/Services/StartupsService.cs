using Microsoft.EntityFrameworkCore;
using StartupsApi.Context;
using StartupsApi.Interfaces;
using StartupsApi.Models.DTOs;
using StartupsApi.Models.Entity;
using StartupsApi.Models.Requests;

namespace StartupsApi;

public class StartupsService : IStartupsService
{
    private const string IconBucketName = "icons";
    private const string StartupBucketPrefix = "startup-";
    private readonly StartupsDbContext _context;

    private readonly IMinioService _minioService;

    public StartupsService(IMinioService minioClient, StartupsDbContext context)
    {
        _minioService = minioClient;
        _context = context;
    }


    public async Task CreateStartup(CreateOrUpdateStartupRequest createOrUpdateStartupRequest, Guid ownerId)
    {
        try
        {
            var startup = _context.Startups.Add(new Startup
            {
                Name = createOrUpdateStartupRequest.Data.Name,
                Description = createOrUpdateStartupRequest.Data.Description,
                DonationTarget = createOrUpdateStartupRequest.Data.DonationTarget,
                OwnerId = ownerId
            });
            await _context.SaveChangesAsync();
            await _minioService.AddImage(createOrUpdateStartupRequest.Icon, IconBucketName,
                startup.Entity.Id.ToString());
            await _minioService.AddImages(createOrUpdateStartupRequest.Images,
                StartupBucketPrefix + startup.Entity.Id);
        }
        catch (Exception e)
        {
            throw new Exception("Unable to create Startup");
        }
    }

    public async Task<StartupDTO> GetStartup(int startupId)
    {
        try
        {
            var startupEntity = await _context.Startups.FindAsync(startupId);
            var images = _minioService.GetImageUrls(StartupBucketPrefix + startupId).ToBlockingEnumerable();
            var icon = await _minioService.GetImageUrl(IconBucketName, startupId.ToString());

            return new StartupDTO
            {
                Id = startupEntity.Id,
                Name = startupEntity.Name,
                Description = startupEntity.Description,
                Icon = icon,
                OwnerId = startupEntity.OwnerId,
                Images = images.ToList(),
                DonationTarget = startupEntity.DonationTarget
            };
        }
        catch (Exception e)
        {
            throw new Exception("Unable to get Startup");
        }
    }

    public async Task DeleteStartup(Guid userId, int startupId)
    {
        try
        {
            var startupEntity = await _context.Startups.FindAsync(startupId);

            if (userId != startupEntity?.OwnerId)
                throw new Exception();

            if (startupEntity == null) throw new Exception("Unable to get startup");

            _context.Startups.Remove(startupEntity);
            await _context.SaveChangesAsync();
            await _minioService.RemoveBucket(StartupBucketPrefix + startupEntity.Id);
            await _minioService.RemoveObject(IconBucketName, startupEntity.Id.ToString());
        }
        catch (Exception e)
        {
            throw new Exception("Unable to delete startup");
        }
    }

    public async Task UpdateStartup(Guid userId, int startupId,
        CreateOrUpdateStartupRequest createOrUpdateStartupRequest)
    {
        try
        {
            var startupEntity = await _context.Startups.FindAsync(startupId);

            if (userId != startupEntity?.OwnerId)
                throw new Exception();

            if (startupEntity == null) throw new Exception("Unable to get startup");

            startupEntity.Name = createOrUpdateStartupRequest.Data.Name;
            startupEntity.Description = createOrUpdateStartupRequest.Data.Description;
            startupEntity.DonationTarget = createOrUpdateStartupRequest.Data.DonationTarget;


            await _context.SaveChangesAsync();
            await _minioService.AddImage(createOrUpdateStartupRequest.Icon, IconBucketName,
                startupEntity.Id.ToString());
            await _minioService.RemoveBucket(StartupBucketPrefix + startupEntity.Id);
            await _minioService.AddImages(createOrUpdateStartupRequest.Images,
                StartupBucketPrefix + startupEntity.Id);
        }
        catch (Exception e)
        {
            throw new Exception("Unable to update startup");
        }
    }

    public async Task<List<int>> GetStartupIds(Guid userId, List<int> excludedIds)
    {
        return await _context.Startups
            .Where(s => !excludedIds.Contains(s.Id) && s.OwnerId != userId)
            .Select(s => s.Id)
            .Take(3)
            .ToListAsync();
    }

    public async Task<int> GetDonatedAmount(int startupId)
    {
        var startup = await _context.Startups.FindAsync(startupId);

        if (startup == null) throw new Exception("Unable to get startup");

        return startup.Donaters?.Sum(donater => donater.Donated) ?? 0;
    }

    public async Task<List<int>> GetUserStartupIds(Guid userId)
    {
        return await _context.Startups.Where(s => s.OwnerId == userId).Select(s => s.Id).ToListAsync();
    }

    public async Task<List<DonaterDto>> GetTopDonaters()
    {
        return await _context.Donaters
            .GroupBy(d => d.UserId)
            .Select(group => new DonaterDto
            {
                UserId = group.Key,
                Donated = group.Sum(d => d.Donated)
            })
            .Take(100)
            .ToListAsync();
    }

    public async Task<List<TopStartupDto>> GetTopStartups()
    {
        var startups = await _context.Startups
            .Select(s => new TopStartupDto
            {
                Id = s.Id,
                Name = s.Name,
                Donated = s.Donaters.Sum(d => d.Donated)
            })
            .OrderByDescending(s => s.Donated)
            .Take(100)
            .ToListAsync();
        foreach (var startup in startups)
            startup.Icon = await _minioService.GetImageUrl(IconBucketName, startup.Id.ToString());

        return startups;
    }

    public async Task CreditStartup(int startupId, Guid userId, int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        var startupEntity = await _context.Startups.FindAsync(startupId);

        if (startupEntity is null) throw new Exception("Unable to find Startup");

        var donater = startupEntity.Donaters.Find(d => d.UserId == userId);
        if (donater is null)
            startupEntity.Donaters.Add(new Donater { UserId = userId, Donated = amount });
        else
            donater.Donated += amount;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Ups something wrong");
        }
    }

    public async Task<List<Donater>> GetDonaters(int startupId)
    {
        var startup = await _context.Startups.FindAsync(startupId);

        if (startup is null) throw new Exception("Unable to find Startup");

        return startup.Donaters;
    }
}