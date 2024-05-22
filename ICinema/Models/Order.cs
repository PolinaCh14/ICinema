using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public decimal Price { get; set; }

    public DateTime CreateDate { get; set; }

    public string UserFirstName { get; set; } = null!;

    public string UserLastName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public int? UserId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentCredentials { get; set; }

    public string PaymentType { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User? User { get; set; }
}
