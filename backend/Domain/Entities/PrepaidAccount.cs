using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class PrepaidAccount
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public decimal Balance { get; set; }

    public ICollection<AccountTransaction> Transactions { get; set; }
        = new List<AccountTransaction>();

}