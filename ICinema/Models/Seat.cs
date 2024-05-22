using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int RowNumber { get; set; }

    public int SeatNumber { get; set; }

    public int SeatTypeId { get; set; }

    public int HallId { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual SeatType SeatType { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
