namespace TransactionsApi.Models.Entitys;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int StartupId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}