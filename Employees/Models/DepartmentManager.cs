using System;
using System.Collections.Generic;

public partial class DepartmentManager
{
    public long EmployeeId { get; set; }

    public string DepartmentId { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
