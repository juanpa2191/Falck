namespace Falck.Domain.Entities;

/// <summary>Un departamento de la empresa; agrupa empleados (uno a muchos).</summary>
public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}
