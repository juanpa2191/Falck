using Falck.Application.DTOs;
using Falck.Application.Services;
using Falck.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falck.Api.Controllers;

/// <summary>
/// Endpoints CRUD para la entidad de empleados (prueba técnica, sección 2.1).
/// Los controladores se mantienen delgados: validación por model binding, lógica
/// en el servicio. Protección por roles de la sección 3.3: Admin tiene acceso
/// total; User solo puede hacer GET.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>Devuelve la lista de todos los empleados. Roles: Admin, User.</summary>
    [HttpGet]
    [ProducesResponseType<List<EmployeeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await employeeService.GetAllAsync(cancellationToken));

    /// <summary>Devuelve el detalle de un empleado específico, incluyendo historial de cargos y proyectos. Roles: Admin, User.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<EmployeeDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await employeeService.GetByIdAsync(id, cancellationToken));

    /// <summary>Agrega un nuevo empleado (abre su primer registro de historial de cargos). Roles: Admin.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType<EmployeeDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDetailDto>> Create(
        CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var created = await employeeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un empleado existente. Un cambio de cargo se registra
    /// automáticamente en el historial de cargos. Roles: Admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        await employeeService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>Elimina un empleado (su historial de cargos se elimina con él). Roles: Admin.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
