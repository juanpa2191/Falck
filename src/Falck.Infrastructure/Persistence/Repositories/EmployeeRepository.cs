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
        context.SaveChangesAsync(cancellationToken);

    public Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        context.Employees.Remove(employee);
        return context.SaveChangesAsync(cancellationToken);
    }

    // Section 4.3: employees of a department assigned to at least one project.
    public Task<List<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default) =>
        context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
            .Include(e => e.Department)
            .Include(e => e.Projects)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
}
