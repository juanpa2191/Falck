using Falck.Application.Common.Exceptions;
using Falck.Application.Interfaces;
using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IEmployeeRepository"/>
public class EmployeeRepository(FalckDbContext context) : IEmployeeRepository
{
    public Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees
            .Include(e => e.Department)
            .Include(e => e.PositionHistories.OrderBy(h => h.StartDate))
            .Include(e => e.Projects)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        context.Employees.Add(employee);
        await context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default) =>
        SaveGuardingConcurrencyAsync(cancellationToken);

    public Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        context.Employees.Remove(employee);
        return SaveGuardingConcurrencyAsync(cancellationToken);
    }

    // Sección 4.3: empleados de un departamento asignados a al menos un proyecto.
    // Sin Include(Projects): el DTO no los expone y el filtro ya se ejecuta como
    // una subconsulta EXISTS, así que cargar las filas del join sería I/O desperdiciado.
    public Task<List<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default) =>
        context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
            .Include(e => e.Department)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

    // Guarda de concurrencia optimista: RowVersion permite que una
    // actualización/eliminación concurrente del mismo empleado falle limpiamente
    // (409) en lugar de corromper el historial de cargos con dos registros
    // abiertos o arrojar un 500 crudo.
    private async Task SaveGuardingConcurrencyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "El empleado fue modificado o eliminado por otra petición. Por favor reintenta.");
        }
    }
}
