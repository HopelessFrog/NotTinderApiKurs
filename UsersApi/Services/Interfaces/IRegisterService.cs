using UsersApi.Models.Entitys;

namespace UsersApi.Services.Interfaces;

public interface IRegisterService
{
    Task Register(RegisterRequest request);
}