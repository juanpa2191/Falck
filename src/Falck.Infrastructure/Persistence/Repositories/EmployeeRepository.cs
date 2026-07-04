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

    // Section 4.3: employees of a department assigned to at least one project.
    // No Include(Projects): the DTO doesn't expose them and the filter already
    // runs as an EXISTS subquery, so loading the join rows would be wasted I/O.
    public Task<List<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default) =>
        context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == departmentId && e.Projects.Any())
            .Include(e => e.Department)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

    // Optimistic-concurrency guard: RowVersion lets a concurrent update/delete
    // of the same employee fail cleanly (409) instead of corrupting the
    // position history with two open records or throwing a raw 500.
    private async Task SaveGuardingConcurrencyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The employee was modified or removed by another request. Please retry.");
        }
    }
}
