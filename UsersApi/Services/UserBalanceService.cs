using UsersApi.Context;
using UsersApi.Models.Entitys;
using UsersApi.Services.Interfaces;

namespace UsersApi.Services;

public class UserBalanceService : IUserBalanceService
{
    private readonly UserContext _context;

    public UserBalanceService(UserContext userContext)
    {
        _context = userContext;
    }

    public async Task<int> TopUpBalance(int value, Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) throw new Exception("Unable to find user");
        user.Balance += value;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Ups something wrong");
        }

        return user.Balance;
    }

    public async Task<int> GetBalance(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user is null ? 0 : user.Balance;
    }

    public async Task<int> TopDownBalance(int value, Guid userId)
    {
        User user;
        try
        {
            user = await _context.Users.FindAsync(userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        if (user is null) throw new Exception("Unable to find user");

        if (user.Balance < value) throw new ArgumentException("User has not enough balance");
        user.Balance -= value;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Ups something wrong");
        }

        return user.Balance;
    }
}