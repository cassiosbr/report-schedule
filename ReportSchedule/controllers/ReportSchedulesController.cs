using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using ReportSchedule.Models;

namespace ReportSchedule.controllers;

[ApiController]
[Route("api/report-schedules")]
public class ReportSchedulesController : ControllerBase
{
    private static readonly ConcurrentDictionary<int, EventReportSchedule> Items = new();
    private static int _nextId = 0;

    [HttpGet]
    public ActionResult<IEnumerable<EventReportSchedule>> GetAll()
    {
        return Ok(Items.Values.OrderBy(x => x.Id));
    }

    [HttpGet("{id:int}")]
    public ActionResult<EventReportSchedule> GetById(int id)
    {
        if (!Items.TryGetValue(id, out var item))
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public ActionResult<EventReportSchedule> Create([FromBody] EventReportSchedule input)
    {
        var id = Interlocked.Increment(ref _nextId);

        var created = new EventReportSchedule
        {
            Id = id,
            Name = input.Name,
            Emails = input.Emails,
            Monday = input.Monday,
            Tuesday = input.Tuesday,
            Wednesday = input.Wednesday,
            Thursday = input.Thursday,
            Friday = input.Friday,
            Saturday = input.Saturday,
            Sunday = input.Sunday,
            Time = input.Time,
            Days = input.Days,
            IsActive = input.IsActive,
        };

        Items[id] = created;
        return CreatedAtAction(nameof(GetById), new { id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<EventReportSchedule> Update(int id, [FromBody] EventReportSchedule input)
    {
        if (!Items.ContainsKey(id))
        {
            return NotFound();
        }

        var updated = new EventReportSchedule
        {
            Id = id,
            Name = input.Name,
            Emails = input.Emails,
            Monday = input.Monday,
            Tuesday = input.Tuesday,
            Wednesday = input.Wednesday,
            Thursday = input.Thursday,
            Friday = input.Friday,
            Saturday = input.Saturday,
            Sunday = input.Sunday,
            Time = input.Time,
            Days = input.Days,
            IsActive = input.IsActive,
        };

        Items[id] = updated;
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!Items.TryRemove(id, out _))
        {
            return NotFound();
        }

        return NoContent();
    }
}
