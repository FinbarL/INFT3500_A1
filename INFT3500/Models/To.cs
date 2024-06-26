﻿using System;
using System.Collections.Generic;

namespace INFT3500.Models;

public partial class To
{
    public int CustomerId { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? StreetAddress { get; set; }

    public int? PostCode { get; set; }

    public string? Suburb { get; set; }

    public string? State { get; set; }

    public string? CardNumber { get; set; }

    public string? CardOwner { get; set; }

    public string? Expiry { get; set; }

    public int? Cvv { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
