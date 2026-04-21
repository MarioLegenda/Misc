using System;
using System.Collections.Generic;

public partial class Salary
{
    public long EmployeeId { get; set; }

    public long Amount { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
