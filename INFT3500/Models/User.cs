using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INFT3500.Models;

public partial class User
{
    public int UserId { get; set; }
    [Required]
    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Name { get; set; }

    public bool? IsAdmin { get; set; }

    public string? Salt { get; set; }

    public string? HashPw { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
