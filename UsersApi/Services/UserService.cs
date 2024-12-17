using Microsoft.EntityFrameworkCore;
using UsersApi.Context;
using UsersApi.Models.Entitys;
using UsersApi.Models.Entitys.Dtos;
using UsersApi.Services.Interfaces;

namespace UsersApi.Services;

public class UserService : IUserService
{
    private readonly UserContext _context;

    public UserService(UserContext context)
    {
        _context = context;
    }

    public async Task<UserShortDto> GetUserShortById(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null) throw new KeyNotFoundException("User not found");

        return new UserShortDto
        {
            Name = user.Name,
            AvatarId = user.AvatarId
        };
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
}