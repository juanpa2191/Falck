namespace Falck.Domain.Entities;

/// <summary>A company department; groups employees (one-to-many).</summary>
public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}
