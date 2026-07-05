using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>
/// Patrón Repository: abstrae la persistencia de empleados para que la capa de
/// aplicación dependa de este contrato, nunca de EF Core (Inversión de Dependencias).
/// </summary>
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtiene un empleado con su departamento, historial de cargos y proyectos.</summary>
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Empleados del departamento dado que están asignados a al menos un
    /// proyecto (prueba técnica, sección 4.3).
    /// </summary>
    Task<List<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default);
}
