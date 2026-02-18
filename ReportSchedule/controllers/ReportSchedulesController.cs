using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ReportSchedule.Models;
using ReportSchedule.Data;

namespace ReportSchedule.controllers;

[ApiController]
[Route("api/report-schedules")]
public class ReportSchedulesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportSchedulesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventReportSchedule>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _db.ReportSchedules
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventReportSchedule>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _db.ReportSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EventReportSchedule>> Create([FromBody] EventReportSchedule input, CancellationToken cancellationToken)
    {
        input.Id = 0;
        _db.ReportSchedules.Add(input);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventReportSchedule>> Update(int id, [FromBody] EventReportSchedule input, CancellationToken cancellationToken)
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
        return Ok(existing);
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
}
