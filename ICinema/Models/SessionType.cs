using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class SessionType
{
    public int SessionTypeId { get; set; }

    public string SessionTypeName { get; set; } = null!;

    public decimal Coefficient { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
