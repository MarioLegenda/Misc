using System;
using System.Collections.Generic;

public partial class Title
{
    public long EmployeeId { get; set; }

    public string Title1 { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
