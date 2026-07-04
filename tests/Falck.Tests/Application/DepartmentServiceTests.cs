using Falck.Application.Interfaces;
using Falck.Application.Services;
using Falck.Domain.Entities;

namespace Falck.Tests.Application;

public class DepartmentServiceTests
{
    [Fact]
    public async Task GetAllAsync_MapsDepartmentsToDtos()
    {
        var service = new DepartmentService(new FakeDepartmentRepository());

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Engineering", result.Single(d => d.Id == 1).Name);
        Assert.Equal("Operations", result.Single(d => d.Id == 2).Name);
    }

    private sealed class FakeDepartmentRepository : IDepartmentRepository
    {
        public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(true);

        public Task<List<Department>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult(new List<Department>
            {
                new() { Id = 1, Name = "Engineering" },
                new() { Id = 2, Name = "Operations" }
            });
    }
}
