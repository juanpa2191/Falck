using Falck.Application.Common.Exceptions;
using Falck.Application.DTOs;
using Falck.Application.Interfaces;
using Falck.Application.Mappings;
using Falck.Domain.Entities;
using Falck.Domain.Strategies;

namespace Falck.Application.Services;

/// <inheritdoc cref="IEmployeeService"/>
public class EmployeeService(
    IEmployeeRepository employees,
    IDepartmentRepository departments,
    IBonusStrategyFactory bonusStrategyFactory) : IEmployeeService
{
    public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await employees.GetAllAsync(cancellationToken);
        return all.Select(e => e.ToDto(e.CalculateYearlyBonus(bonusStrategyFactory))).ToList();
    }

    public async Task<EmployeeDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        return employee.ToDetailDto(employee.CalculateYearlyBonus(bonusStrategyFactory));
    }

    public async Task<EmployeeDetailDto> CreateAsync(
        CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);

        var employee = new Employee
        {
            Name = request.Name,
            CurrentPosition = request.CurrentPosition,
            Salary = request.Salary,
            DepartmentId = request.DepartmentId
        };

        // Open the first record of the position history at hire time. EF Core
        // fills in the EmployeeId once the employee is saved.
        employee.PositionHistories.Add(
            PositionHistory.Open(employee.Id, request.CurrentPosition, DateTime.UtcNow.Date));

        await employees.AddAsync(employee, cancellationToken);

        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task UpdateAsync(
        int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);

        employee.Name = request.Name;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;

        // Route position changes through the domain rule so the history stays
        // consistent (closes the open record, opens a new one).
        if (employee.CurrentPosition != request.CurrentPosition)
            employee.ChangePosition(request.CurrentPosition, DateTime.UtcNow.Date);

        await employees.UpdateAsync(employee, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        await employees.DeleteAsync(employee, cancellationToken);
    }

    public async Task<List<EmployeeDto>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default)
    {
        if (!await departments.ExistsAsync(departmentId, cancellationToken))
            throw new NotFoundException(nameof(Department), departmentId);

        var result = await employees.GetByDepartmentWithProjectsAsync(departmentId, cancellationToken);
        return result.Select(e => e.ToDto(e.CalculateYearlyBonus(bonusStrategyFactory))).ToList();
    }

    private async Task EnsureDepartmentExists(int departmentId, CancellationToken cancellationToken)
    {
        if (!await departments.ExistsAsync(departmentId, cancellationToken))
            throw new BadRequestException($"Department '{departmentId}' does not exist.");
    }
}
