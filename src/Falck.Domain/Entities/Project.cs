namespace Falck.Domain.Entities;

/// <summary>Un proyecto al que se pueden asignar empleados (muchos a muchos).</summary>
public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}
