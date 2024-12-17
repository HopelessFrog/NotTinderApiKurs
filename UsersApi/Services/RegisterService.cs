using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using UsersApi.Context;
using UsersApi.Models.Entitys;
using UsersApi.Services.Interfaces;

namespace AuthApi.Services;

public class RegisterService : IRegisterService
{
    private readonly UserContext _context;

    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterService(UserContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Register(RegisterRequest request)
    {
        if (await _context.Users.SingleOrDefaultAsync(u => u.Name == request.Login) != null)
            throw new ArgumentException();

        var user = new User
        {
            Name = request.Login,
            Email = request.Email
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new CreateUserMessage
        {
            Id = user.Id.ToString(),
            Login = request.Login,
            Password = request.Password
        });
    }
}