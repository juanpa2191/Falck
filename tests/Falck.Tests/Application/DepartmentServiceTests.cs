using Falck.Application.Services;
using Falck.Domain.Entities;
using Falck.Tests.TestHelpers;

namespace Falck.Tests.Application;

public class DepartmentServiceTests
{
    [Fact]
    public async Task GetAllAsync_MapsDepartmentsToDtos()
    {
        var service = new DepartmentService(
            new FakeDepartmentRepository(
                new Department { Id = 1, Name = "Engineering" },
                new Department { Id = 2, Name = "Operations" }),
            TestMapper.Create());

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Engineering", result.Single(d => d.Id == 1).Name);
        Assert.Equal("Operations", result.Single(d => d.Id == 2).Name);
    }
}
