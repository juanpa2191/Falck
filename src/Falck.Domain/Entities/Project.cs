namespace Falck.Domain.Entities;

/// <summary>A project employees can be assigned to (many-to-many).</summary>
public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}
