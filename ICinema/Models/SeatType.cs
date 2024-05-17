using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class SeatType
{
    public int SeatTypeId { get; set; }

    public string SeatTypeName { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
