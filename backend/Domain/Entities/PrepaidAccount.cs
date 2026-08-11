using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class PrepaidAccount
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public decimal Balance { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public ICollection<AccountTransaction> Transactions { get; set; }
        = new List<AccountTransaction>();
}