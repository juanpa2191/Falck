using AutoMapper;
using Falck.Application.DTOs;
using Falck.Domain.Entities;
using Falck.Domain.Enums;
using Falck.Tests.TestHelpers;

namespace Falck.Tests.Application;

public class MappingProfilesTests
{
    private readonly IMapper _mapper = TestMapper.Create();

    [Fact]
    public void Configuration_IsValid()
    {
        // Falla si algún miembro de destino queda sin mapear en cualquier perfil.
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Employee_MapsToDto_WithBonusAndDepartmentName()
    {
        var employee = new Employee
        {
            Id = 1,
            Name = "Laura",
            CurrentPosition = PositionType.Manager,
            Salary = 9000m,
            DepartmentId = 3,
            Department = new Department { Id = 3, Name = "Engineering" }
        };

        var dto = _mapper.Map<EmployeeDto>(employee);

        Assert.Equal(1, dto.Id);
        Assert.Equal("Laura", dto.Name);
        Assert.Equal(1800m, dto.YearlyBonus); // 20% gerencial
        Assert.Equal("Engineering", dto.DepartmentName);
    }

    [Fact]
    public void Employee_MapsToDetailDto_WithHistoryOrderedByStartDate()
    {
        var employee = new Employee
        {
            Id = 2,
            Name = "Carlos",
            CurrentPosition = PositionType.Developer,
            Salary = 5000m,
            DepartmentId = 1
        };
        employee.PositionHistories.Add(
            new PositionHistory { Position = "Manager", StartDate = new DateTime(2023, 1, 1) });
        employee.PositionHistories.Add(
            new PositionHistory { Position = "Developer", StartDate = new DateTime(2020, 1, 1) });

        var dto = _mapper.Map<EmployeeDetailDto>(employee);

        Assert.Equal(500m, dto.YearlyBonus); // 10% regular
        Assert.Equal(2, dto.PositionHistory.Count);
        Assert.Equal("Developer", dto.PositionHistory[0].Position); // el más antiguo primero
    }

    [Fact]
    public void Employee_WithoutDepartment_MapsNullDepartmentName()
    {
        var employee = new Employee
        {
            Id = 3,
            Name = "Sin Depto",
            CurrentPosition = PositionType.Analyst,
            Salary = 4000m,
            DepartmentId = 9
        };

        var dto = _mapper.Map<EmployeeDto>(employee);

        Assert.Null(dto.DepartmentName);
    }

    [Fact]
    public void Department_MapsToDto()
    {
        var department = new Department { Id = 5, Name = "Operations" };

        var dto = _mapper.Map<DepartmentDto>(department);

        Assert.Equal(5, dto.Id);
        Assert.Equal("Operations", dto.Name);
    }
}
