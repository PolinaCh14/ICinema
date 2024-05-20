using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Technology
{
    public int TechnologyId { get; set; }

    public string TechnologyName { get; set; } = null!;

    public decimal Coefficient { get; set; }

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}

enum Technologies { Cinetech = 1, IMAX, _4DX}
