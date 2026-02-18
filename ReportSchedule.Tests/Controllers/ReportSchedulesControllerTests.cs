using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportSchedule.Controllers;
using ReportSchedule.Data;
using ReportSchedule.Dtos;
using ReportSchedule.Models;
using Xunit;

namespace ReportSchedule.Tests.Controllers;

public class ReportSchedulesControllerTests
{
    private static AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    private static EventReportSchedule NewSchedule(string name)
    {
        return new EventReportSchedule
        {
            Name = name,
            Emails = new List<string> { "joao@teste.com" },
            Monday = true,
            Time = new TimeSpan(9, 0, 0),
            Days = 30,
            IsActive = true,
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithItemsOrderedById()
    {
        using var db = CreateDbContext(nameof(GetAll_ReturnsOk_WithItemsOrderedById));
        db.ReportSchedules.AddRange(NewSchedule("B"), NewSchedule("A"));
        await db.SaveChangesAsync();

        var controller = new ReportSchedulesController(db);

        var action = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<EventReportSchedule>>(ok.Value);
        var list = items.ToList();

        Assert.Equal(2, list.Count);
        Assert.True(list[0].Id < list[1].Id);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        using var db = CreateDbContext(nameof(GetById_WhenMissing_ReturnsNotFound));
        var controller = new ReportSchedulesController(db);

        var action = await controller.GetById(123, CancellationToken.None);

        Assert.IsType<NotFoundResult>(action.Result);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk_WithItem()
    {
        using var db = CreateDbContext(nameof(GetById_WhenExists_ReturnsOk_WithItem));
        var entity = NewSchedule("X");
        db.ReportSchedules.Add(entity);
        await db.SaveChangesAsync();

        var controller = new ReportSchedulesController(db);

        var action = await controller.GetById(entity.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var item = Assert.IsType<EventReportSchedule>(ok.Value);
        Assert.Equal(entity.Id, item.Id);
        Assert.Equal("X", item.Name);
    }

    [Fact]
    public async Task Create_SetsIdToZero_AndReturnsCreatedAtAction()
    {
        using var db = CreateDbContext(nameof(Create_SetsIdToZero_AndReturnsCreatedAtAction));
        var controller = new ReportSchedulesController(db);

        var input = NewSchedule("Created");
        input.Id = 999;

        var action = await controller.Create(input, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(action.Result);
        Assert.Equal(nameof(ReportSchedulesController.GetById), created.ActionName);

        var createdItem = Assert.IsType<EventReportSchedule>(created.Value);
        Assert.True(createdItem.Id > 0);

        Assert.NotNull(created.RouteValues);
        Assert.True(created.RouteValues!.TryGetValue("id", out var routeId));
        Assert.Equal(createdItem.Id, routeId);

        var inDb = await db.ReportSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == createdItem.Id);
        Assert.NotNull(inDb);
        Assert.Equal("Created", inDb!.Name);
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        using var db = CreateDbContext(nameof(Update_WhenMissing_ReturnsNotFound));
        var controller = new ReportSchedulesController(db);

        var input = NewSchedule("Updated");
        var action = await controller.Update(777, input, CancellationToken.None);

        Assert.IsType<NotFoundResult>(action.Result);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsOk_AndPersistsChanges()
    {
        using var db = CreateDbContext(nameof(Update_WhenExists_ReturnsOk_AndPersistsChanges));
        var existing = NewSchedule("Schedule v1");
        existing.Emails = new List<string> { "alberto@teste.com" };
        existing.Monday = true;
        existing.Tuesday = false;
        existing.Days = 10;
        existing.IsActive = true;

        db.ReportSchedules.Add(existing);
        await db.SaveChangesAsync();

        var controller = new ReportSchedulesController(db);

        var input = NewSchedule("New");
        input.Emails = new List<string> { "fernanda@teste.com", "raissa@teste.com" };
        input.Monday = false;
        input.Tuesday = true;
        input.Time = new TimeSpan(15, 30, 0);
        input.Days = 99;
        input.IsActive = false;

        var action = await controller.Update(existing.Id, input, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var returned = Assert.IsType<EventReportSchedule>(ok.Value);
        Assert.Equal(existing.Id, returned.Id);
        Assert.Equal("New", returned.Name);
        Assert.Equal(2, returned.Emails.Count);
        Assert.False(returned.Monday);
        Assert.True(returned.Tuesday);
        Assert.Equal(new TimeSpan(15, 30, 0), returned.Time);
        Assert.Equal(99, returned.Days);
        Assert.False(returned.IsActive);

        var inDb = await db.ReportSchedules.AsNoTracking().FirstAsync(x => x.Id == existing.Id);
        Assert.Equal("New", inDb.Name);
        Assert.Equal(2, inDb.Emails.Count);
        Assert.False(inDb.Monday);
        Assert.True(inDb.Tuesday);
        Assert.Equal(new TimeSpan(15, 30, 0), inDb.Time);
        Assert.Equal(99, inDb.Days);
        Assert.False(inDb.IsActive);
    }

    [Fact]
    public async Task Patch_WhenMissing_ReturnsNotFound()
    {
        using var db = CreateDbContext(nameof(Patch_WhenMissing_ReturnsNotFound));
        var controller = new ReportSchedulesController(db);

        var patch = new EventReportSchedulePatchDto { Name = "X" };

        var action = await controller.Patch(321, patch, CancellationToken.None);

        Assert.IsType<NotFoundResult>(action.Result);
    }

    [Fact]
    public async Task Patch_WhenExists_UpdatesOnlyProvidedFields()
    {
        using var db = CreateDbContext(nameof(Patch_WhenExists_UpdatesOnlyProvidedFields));
        var existing = NewSchedule("Schedule v1");
        existing.Emails = new List<string> { "alberto@teste.com" };
        existing.Monday = true;
        existing.Tuesday = false;
        existing.Time = new TimeSpan(8, 0, 0);
        existing.Days = 10;
        existing.IsActive = true;

        db.ReportSchedules.Add(existing);
        await db.SaveChangesAsync();

        var controller = new ReportSchedulesController(db);

        var patch = new EventReportSchedulePatchDto
        {
            Name = "Patched",
            IsActive = false,
            Days = 77,
        };

        var action = await controller.Patch(existing.Id, patch, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var returned = Assert.IsType<EventReportSchedule>(ok.Value);

        Assert.Equal(existing.Id, returned.Id);
        Assert.Equal("Patched", returned.Name);
        Assert.Equal(new List<string> { "alberto@teste.com" }, returned.Emails);
        Assert.True(returned.Monday);
        Assert.False(returned.Tuesday);
        Assert.Equal(new TimeSpan(8, 0, 0), returned.Time);
        Assert.Equal(77, returned.Days);
        Assert.False(returned.IsActive);

        var inDb = await db.ReportSchedules.AsNoTracking().FirstAsync(x => x.Id == existing.Id);
        Assert.Equal("Patched", inDb.Name);
        Assert.Equal(77, inDb.Days);
        Assert.False(inDb.IsActive);
        Assert.Equal(new List<string> { "alberto@teste.com" }, inDb.Emails);
        Assert.True(inDb.Monday);
        Assert.False(inDb.Tuesday);
        Assert.Equal(new TimeSpan(8, 0, 0), inDb.Time);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        using var db = CreateDbContext(nameof(Delete_WhenMissing_ReturnsNotFound));
        var controller = new ReportSchedulesController(db);

        var action = await controller.Delete(404, CancellationToken.None);

        Assert.IsType<NotFoundResult>(action);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesItem_AndReturnsNoContent()
    {
        using var db = CreateDbContext(nameof(Delete_WhenExists_RemovesItem_AndReturnsNoContent));
        var existing = NewSchedule("ToDelete");
        db.ReportSchedules.Add(existing);
        await db.SaveChangesAsync();

        var controller = new ReportSchedulesController(db);

        var action = await controller.Delete(existing.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(action);
        Assert.False(await db.ReportSchedules.AsNoTracking().AnyAsync(x => x.Id == existing.Id));
    }
}
