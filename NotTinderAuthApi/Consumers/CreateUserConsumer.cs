using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using SharedModels;

namespace AuthApi.Consumers;

public class CreateUserConsumer : IConsumer<CreateUserMessage>
{
    private readonly UserManager<User> _userManager;

    public CreateUserConsumer(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task Consume(ConsumeContext<CreateUserMessage> context)
    {
        await _userManager.CreateAsync(new User
        {
            Id = context.Message.Id,
            UserName = context.Message.Login
        }, context.Message.Password);
    }
}