using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public decimal Price { get; set; }

    public int? OrderId { get; set; }

    public int SessionId { get; set; }

    public int SeatId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Session Session { get; set; } = null!;
}
