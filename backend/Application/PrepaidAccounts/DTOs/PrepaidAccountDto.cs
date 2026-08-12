namespace Application.PrepaidAccounts.DTOs;

public class PrepaidAccountDto
{
    public decimal Balance { get; set; }
    public List<TransactionDto> Transactions { get; set; } = [];
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ActivityId { get; set; }
    public string? ActivityTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}
