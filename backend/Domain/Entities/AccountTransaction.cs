using System;

namespace Domain.Entities;

public class AccountTransaction
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public PrepaidAccount Account { get; set; } = null!;

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public string? ActivityId { get; set; }

    public Activity? Activity { get; set; }

    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}