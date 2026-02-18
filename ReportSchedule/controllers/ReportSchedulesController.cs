using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ReportSchedule.Dtos;
using ReportSchedule.Models;
using ReportSchedule.Data;
using System.Globalization;

namespace ReportSchedule.Controllers;

[ApiController]
[Route("api/report-schedules")]
public class ReportSchedulesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TimeZoneInfo _timeZone;

    public ReportSchedulesController(AppDbContext db, TimeZoneInfo timeZone)
    {
        _db = db;
        _timeZone = timeZone;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventReportScheduleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _db.ReportSchedules
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventReportScheduleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _db.ReportSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<EventReportScheduleDto>> Create([FromBody] EventReportSchedule input, CancellationToken cancellationToken)
    {
        input.Id = 0;
        input.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
        _db.ReportSchedules.Add(input);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, ToDto(input));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventReportScheduleDto>> Update(int id, [FromBody] EventReportSchedule input, CancellationToken cancellationToken)
    {
        var existing = await _db.ReportSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = input.Name;
        existing.Emails = input.Emails;
        existing.Monday = input.Monday;
        existing.Tuesday = input.Tuesday;
        existing.Wednesday = input.Wednesday;
        existing.Thursday = input.Thursday;
        existing.Friday = input.Friday;
        existing.Saturday = input.Saturday;
        existing.Sunday = input.Sunday;
        existing.Time = input.Time;
        existing.Days = input.Days;
        existing.IsActive = input.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(existing));
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<EventReportScheduleDto>> Patch(int id, [FromBody] EventReportSchedulePatchDto patch, CancellationToken cancellationToken)
    {
        var existing = await _db.ReportSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (patch.Name is not null)
        {
            existing.Name = patch.Name;
        }

        if (patch.Emails is not null)
        {
            existing.Emails = patch.Emails;
        }

        if (patch.Monday is not null) existing.Monday = patch.Monday.Value;
        if (patch.Tuesday is not null) existing.Tuesday = patch.Tuesday.Value;
        if (patch.Wednesday is not null) existing.Wednesday = patch.Wednesday.Value;
        if (patch.Thursday is not null) existing.Thursday = patch.Thursday.Value;
        if (patch.Friday is not null) existing.Friday = patch.Friday.Value;
        if (patch.Saturday is not null) existing.Saturday = patch.Saturday.Value;
        if (patch.Sunday is not null) existing.Sunday = patch.Sunday.Value;

        if (patch.Time is not null)
        {
            existing.Time = patch.Time.Value;
        }

        if (patch.Days is not null)
        {
            existing.Days = patch.Days.Value;
        }

        if (patch.IsActive is not null)
        {
            existing.IsActive = patch.IsActive.Value;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(existing));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _db.ReportSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _db.ReportSchedules.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private EventReportScheduleDto ToDto(EventReportSchedule entity)
    {
        var createdLocal = DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Unspecified);

        return new EventReportScheduleDto
        {
            Id = entity.Id,
            CreatedAt = createdLocal.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            Name = entity.Name,
            Emails = entity.Emails,
            Monday = entity.Monday,
            Tuesday = entity.Tuesday,
            Wednesday = entity.Wednesday,
            Thursday = entity.Thursday,
            Friday = entity.Friday,
            Saturday = entity.Saturday,
            Sunday = entity.Sunday,
            Time = entity.Time,
            Days = entity.Days,
            IsActive = entity.IsActive,
        };
    }
}
