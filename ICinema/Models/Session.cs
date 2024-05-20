using System;
using System.Collections.Generic;

namespace ICinema.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int SessionTypeId { get; set; }

    public int MovieId { get; set; }

    public int HallId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Format { get; set; } = null!;

    public virtual Hall Hall { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual SessionType SessionType { get; set; } = null!;
}

public enum SessionScheduleDates { Today, Tomorrow, Week = 7};