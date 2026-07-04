using Falck.Application.DTOs;
using Falck.Domain.Entities;

namespace Falck.Application.Mappings;

/// <summary>
/// Manual entity-to-DTO mapping. Deliberately explicit instead of AutoMapper:
/// the surface is small and the mapping stays compile-time safe.
/// </summary>
public static class EmployeeMapper
{
    public static EmployeeDto ToDto(this Employee employee, decimal yearlyBonus) =>
        new(employee.Id,
            employee.Name,
            employee.CurrentPosition,
            employee.Salary,
            yearlyBonus,
            employee.DepartmentId,
            employee.Department?.Name);

    public static EmployeeDetailDto ToDetailDto(this Employee employee, decimal yearlyBonus) =>
        new(employee.Id,
            employee.Name,
            employee.CurrentPosition,
            employee.Salary,
            yearlyBonus,
            employee.DepartmentId,
            employee.Department?.Name,
            employee.PositionHistories
                .OrderBy(h => h.StartDate)
                .Select(h => new PositionHistoryDto(h.Position, h.StartDate, h.EndDate))
                .ToList(),
            employee.Projects
                .Select(p => new ProjectDto(p.Id, p.Name))
                .ToList());

    public static DepartmentDto ToDto(this Department department) =>
        new(department.Id, department.Name);
}
