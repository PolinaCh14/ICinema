using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; } = null;

    public string Password { get; set; } = null!;

    public string UserStatus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}
