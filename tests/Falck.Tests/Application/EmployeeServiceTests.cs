using Falck.Application.Common.Exceptions;
using Falck.Application.DTOs.Employees;
using Falck.Application.Interfaces;
using Falck.Application.Services;
using Falck.Domain.Entities;
using Falck.Domain.Enums;
using Falck.Tests.TestHelpers;

namespace Falck.Tests.Application;

public class EmployeeServiceTests
{
    private readonly FakeEmployeeRepository _employees = new();
    private readonly FakeDepartmentRepository _departments = new(
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Human Resources" });
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _service = new EmployeeService(_employees, _departments, TestMapper.Create());
    }

    private Employee SeedEmployee(
        int id = 1, PositionType position = PositionType.Developer, decimal salary = 5000m, int departmentId = 1)
    {
        var employee = new Employee
        {
            Id = id,
            Name = "Existing",
            CurrentPosition = position,
            Salary = salary,
            DepartmentId = departmentId,
            Department = new Department { Id = departmentId, Name = $"Dept {departmentId}" }
        };
        employee.PositionHistories.Add(PositionHistory.Open(id, position, new DateTime(2024, 1, 1)));
        _employees.Items.Add(employee);
        return employee;
    }

    // ---- GetAll ----

    [Fact]
    public async Task GetAllAsync_MapsEachEmployeeWithItsBonus()
    {
        SeedEmployee(id: 1, position: PositionType.Developer, salary: 5000m);
        SeedEmployee(id: 2, position: PositionType.Manager, salary: 9000m);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(500m, result.Single(e => e.Id == 1).YearlyBonus);   // 10%
        Assert.Equal(1800m, result.Single(e => e.Id == 2).YearlyBonus);  // 20%
    }

    // ---- GetById ----

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsDetailWithHistoryAndBonus()
    {
        SeedEmployee(id: 7, position: PositionType.Manager, salary: 10000m);

        var result = await _service.GetByIdAsync(7);

        Assert.Equal(7, result.Id);
        Assert.Equal(2000m, result.YearlyBonus);
        Assert.Single(result.PositionHistory);
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
    }

    // ---- Create ----

    [Fact]
    public async Task CreateAsync_ValidDepartment_PersistsAndOpensFirstHistoryRecord()
    {
        var request = new CreateEmployeeRequest
        {
            Name = "New Hire",
            CurrentPosition = PositionType.Analyst,
            Salary = 4000m,
            DepartmentId = 1
        };

        var result = await _service.CreateAsync(request);

        var stored = Assert.Single(_employees.Items);
        Assert.Equal("New Hire", stored.Name);
        var record = Assert.Single(stored.PositionHistories);
        Assert.Equal(nameof(PositionType.Analyst), record.Position);
        Assert.Null(record.EndDate);
        Assert.Equal(400m, result.YearlyBonus); // 10% de 4000
    }

    [Fact]
    public async Task CreateAsync_UnknownDepartment_ThrowsBadRequestAndPersistsNothing()
    {
        var request = new CreateEmployeeRequest
        {
            Name = "New Hire",
            CurrentPosition = PositionType.Analyst,
            Salary = 4000m,
            DepartmentId = 99
        };

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(request));
        Assert.Empty(_employees.Items);
    }

    // ---- Update ----

    [Fact]
    public async Task UpdateAsync_PositionChange_ClosesPreviousRecordAndOpensNew()
    {
        SeedEmployee(id: 3, position: PositionType.Developer, salary: 5000m);

        await _service.UpdateAsync(3, new UpdateEmployeeRequest
        {
            Name = "Promoted",
            CurrentPosition = PositionType.Manager,
            Salary = 8000m,
            DepartmentId = 1
        });

        var employee = _employees.Items.Single(e => e.Id == 3);
        Assert.Equal(PositionType.Manager, employee.CurrentPosition);
        Assert.Equal("Promoted", employee.Name);
        Assert.Equal(8000m, employee.Salary);
        Assert.Equal(2, employee.PositionHistories.Count);
        Assert.Single(employee.PositionHistories, h => h.EndDate is null);
        Assert.True(_employees.UpdateCalled);
    }

    [Fact]
    public async Task UpdateAsync_SamePosition_DoesNotAddHistoryRecord()
    {
        SeedEmployee(id: 4, position: PositionType.Developer, salary: 5000m);

        await _service.UpdateAsync(4, new UpdateEmployeeRequest
        {
            Name = "Raise Only",
            CurrentPosition = PositionType.Developer,
            Salary = 6000m,
            DepartmentId = 1
        });

        var employee = _employees.Items.Single(e => e.Id == 4);
        Assert.Single(employee.PositionHistories);
        Assert.Equal(6000m, employee.Salary);
    }

    [Fact]
    public async Task UpdateAsync_Missing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999,
            new UpdateEmployeeRequest { Name = "X", CurrentPosition = PositionType.Developer, Salary = 1m, DepartmentId = 1 }));
    }

    [Fact]
    public async Task UpdateAsync_UnknownDepartment_ThrowsBadRequest()
    {
        SeedEmployee(id: 5);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateAsync(5,
            new UpdateEmployeeRequest { Name = "X", CurrentPosition = PositionType.Manager, Salary = 1m, DepartmentId = 99 }));
    }

    // ---- Delete ----

    [Fact]
    public async Task DeleteAsync_Existing_RemovesEmployee()
    {
        SeedEmployee(id: 6);

        await _service.DeleteAsync(6);

        Assert.Empty(_employees.Items);
    }

    [Fact]
    public async Task DeleteAsync_Missing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    // ---- Sección 4.3 ----

    [Fact]
    public async Task GetByDepartmentWithProjectsAsync_UnknownDepartment_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByDepartmentWithProjectsAsync(99));
    }

    [Fact]
    public async Task GetByDepartmentWithProjectsAsync_KnownDepartment_ReturnsMappedDtos()
    {
        SeedEmployee(id: 1, position: PositionType.Manager, salary: 9000m);
        _employees.ByDepartmentWithProjects = [.. _employees.Items];

        var result = await _service.GetByDepartmentWithProjectsAsync(1);

        var dto = Assert.Single(result);
        Assert.Equal(1800m, dto.YearlyBonus);
    }

    // ---- Fakes ----

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> Items { get; } = [];
        public List<Employee> ByDepartmentWithProjects { get; set; } = [];
        public bool UpdateCalled { get; private set; }

        public Task<List<Employee>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult(Items.ToList());

        public Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

        public Task<Employee> AddAsync(Employee employee, CancellationToken ct = default)
        {
            if (employee.Id == 0)
                employee.Id = Items.Count == 0 ? 1 : Items.Max(e => e.Id) + 1;
            Items.Add(employee);
            return Task.FromResult(employee);
        }

        public Task UpdateAsync(Employee employee, CancellationToken ct = default)
        {
            UpdateCalled = true;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Employee employee, CancellationToken ct = default)
        {
            Items.Remove(employee);
            return Task.CompletedTask;
        }

        public Task<List<Employee>> GetByDepartmentWithProjectsAsync(int departmentId, CancellationToken ct = default) =>
            Task.FromResult(ByDepartmentWithProjects);
    }
}
