using UsersApi.Models.Entitys;
using UsersApi.Models.Entitys.Dtos;

namespace UsersApi.Services.Interfaces;

public interface IUserService
{
    Task<UserShortDto> GetUserShortById(Guid userId);

    Task<List<User>> GetAllUsers();
}