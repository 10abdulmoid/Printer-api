using Microsoft.EntityFrameworkCore;
using PrintQueueService.Application.Data;

namespace PrintQueueService.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
