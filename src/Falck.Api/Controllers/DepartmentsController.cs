using Falck.Application.DTOs;
using Falck.Application.Services;
using Falck.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falck.Api.Controllers;

/// <summary>
/// Endpoints de lectura de departamentos, incluida la consulta de la sección
/// 4.3. De solo lectura, así que ambos roles pueden invocarlos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
public class DepartmentsController(
    IDepartmentService departmentService,
    IEmployeeService employeeService) : ControllerBase
{
    /// <summary>Lista los departamentos (útil para elegir un DepartmentId al crear empleados).</summary>
    [HttpGet]
    [ProducesResponseType<List<DepartmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DepartmentDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await departmentService.GetAllAsync(cancellationToken));

    /// <summary>
    /// Empleados del departamento que están asignados a al menos un proyecto
    /// (prueba técnica, sección 4.3).
    /// </summary>
    [HttpGet("{id:int}/employees-with-projects")]
    [ProducesResponseType<List<EmployeeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesWithProjects(
        int id, CancellationToken cancellationToken) =>
        Ok(await employeeService.GetByDepartmentWithProjectsAsync(id, cancellationToken));
}
