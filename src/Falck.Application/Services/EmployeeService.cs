using AutoMapper;
using Falck.Application.Common.Exceptions;
using Falck.Application.DTOs;
using Falck.Application.Interfaces;
using Falck.Domain.Entities;

namespace Falck.Application.Services;

/// <inheritdoc cref="IEmployeeService"/>
public class EmployeeService(
    IEmployeeRepository employees,
    IDepartmentRepository departments,
    IMapper mapper) : IEmployeeService
{
    public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await employees.GetAllAsync(cancellationToken);
        return mapper.Map<List<EmployeeDto>>(all);
    }

    public async Task<EmployeeDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        return mapper.Map<EmployeeDetailDto>(employee);
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

        // Abre el primer registro del historial de cargos al momento de la
        // contratación. EF Core completa el EmployeeId una vez guardado el empleado.
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

        // Enruta los cambios de cargo a través de la regla de dominio para que
        // el historial se mantenga consistente (cierra el registro abierto, abre uno nuevo).
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
        return mapper.Map<List<EmployeeDto>>(result);
    }

    private async Task EnsureDepartmentExists(int departmentId, CancellationToken cancellationToken)
    {
        if (!await departments.ExistsAsync(departmentId, cancellationToken))
            throw new BadRequestException($"Department '{departmentId}' does not exist.");
    }
}
