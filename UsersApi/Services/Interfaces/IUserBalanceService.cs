namespace UsersApi.Services.Interfaces;

public interface IUserBalanceService
{
    Task<int> TopUpBalance(int value, Guid userId);
    Task<int> GetBalance(Guid userId);
    Task<int> TopDownBalance(int value, Guid userId);
}