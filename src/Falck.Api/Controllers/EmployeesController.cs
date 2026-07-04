using Falck.Application.DTOs;
using Falck.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Falck.Api.Controllers;

/// <summary>
/// CRUD endpoints for the employees entity (technical test, section 2.1).
/// Controllers stay thin: validation via model binding, logic in the service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>Returns a list of all employees.</summary>
    [HttpGet]
    [ProducesResponseType<List<EmployeeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await employeeService.GetAllAsync(cancellationToken));

    /// <summary>Returns details of a specific employee, including position history and projects.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<EmployeeDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await employeeService.GetByIdAsync(id, cancellationToken));

    /// <summary>Adds a new employee (opens its first position history record).</summary>
    [HttpPost]
    [ProducesResponseType<EmployeeDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDetailDto>> Create(
        CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var created = await employeeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing employee. A position change is recorded in the
    /// position history automatically.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        await employeeService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes an employee (its position history goes with it).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
