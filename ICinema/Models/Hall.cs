using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Hall
{
    public int HallId { get; set; }

    public string HallName { get; set; } = null!;

    public int TechnologyId { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual Technology Technology { get; set; } = null!;
}
