using Falck.Application.DTOs;
using Falck.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Falck.Api.Controllers;

/// <summary>Departments read endpoints, including the section 4.3 query.</summary>
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(
    IDepartmentService departmentService,
    IEmployeeService employeeService) : ControllerBase
{
    /// <summary>Lists departments (useful to pick a DepartmentId when creating employees).</summary>
    [HttpGet]
    [ProducesResponseType<List<DepartmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DepartmentDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await departmentService.GetAllAsync(cancellationToken));

    /// <summary>
    /// Employees of the department that are assigned to at least one project
    /// (technical test, section 4.3).
    /// </summary>
    [HttpGet("{id:int}/employees-with-projects")]
    [ProducesResponseType<List<EmployeeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesWithProjects(
        int id, CancellationToken cancellationToken) =>
        Ok(await employeeService.GetByDepartmentWithProjectsAsync(id, cancellationToken));
}
