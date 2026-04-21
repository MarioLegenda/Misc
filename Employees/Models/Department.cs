using System;
using System.Collections.Generic;

public partial class Department
{
    public string Id { get; set; } = null!;

    public string DeptName { get; set; } = null!;

    public virtual ICollection<DepartmentEmployee> DepartmentEmployees { get; set; } = new List<DepartmentEmployee>();

    public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();
}
