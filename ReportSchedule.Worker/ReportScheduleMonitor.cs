using Microsoft.EntityFrameworkCore;
using ReportSchedule.Data;
using ReportSchedule.Models;

namespace ReportSchedule.Worker;

public sealed class ReportScheduleMonitor : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan TriggerWindow = TimeSpan.FromMinutes(1);

    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly TimeZoneInfo _timeZone;
    private DateTime _currentMinuteLocal = DateTime.MinValue;
    private readonly HashSet<int> _notifiedThisMinute = new();

    public ReportScheduleMonitor(IDbContextFactory<AppDbContext> dbFactory, TimeZoneInfo timeZone)
    {
        _dbFactory = dbFactory;
        _timeZone = timeZone;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nowUtc = DateTime.UtcNow;
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _timeZone);

            var minuteLocal = new DateTime(
                nowLocal.Year, nowLocal.Month, nowLocal.Day,
                nowLocal.Hour, nowLocal.Minute, 0,
                DateTimeKind.Unspecified);

            if (minuteLocal != _currentMinuteLocal)
            {
                _currentMinuteLocal = minuteLocal;
                _notifiedThisMinute.Clear();
            }

            await CheckForDueSchedules(nowLocal, stoppingToken);
            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task CheckForDueSchedules(DateTime nowLocal, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        var active = await db.ReportSchedules
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var schedule in active)
        {
            if (!IsWithinDaysWindow(schedule, nowLocal, _timeZone))
            {
                continue;
            }

            if (!IsCorrectWeekday(schedule, nowLocal))
            {
                continue;
            }

            if (!IsWithinTimeWindow(schedule, nowLocal))
            {
                continue;
            }

            if (_notifiedThisMinute.Add(schedule.Id))
            {
                Console.WriteLine($"Nova tarefa encontrada. {schedule.Name} ({schedule.Id})");
            }
        }
    }

    private static bool IsWithinDaysWindow(EventReportSchedule schedule, DateTime nowLocal, TimeZoneInfo timeZone)
    {
        // Interpretação simples do requisito: considerar apenas registros criados nos últimos X dias.
        // cutoff = (hoje - Days)
        var days = schedule.Days;
        if (days <= 0)
        {
            return true;
        }

        var cutoffDateLocal = nowLocal.Date.AddDays(-days);

        var createdLocal = DateTime.SpecifyKind(schedule.CreatedAt, DateTimeKind.Unspecified);
        return createdLocal >= cutoffDateLocal;
    }

    private static bool IsCorrectWeekday(EventReportSchedule schedule, DateTime nowLocal)
    {
        // Usa as flags do modelo (Monday..Sunday) para filtrar pelo dia atual.
        return nowLocal.DayOfWeek switch
        {
            DayOfWeek.Monday => schedule.Monday,
            DayOfWeek.Tuesday => schedule.Tuesday,
            DayOfWeek.Wednesday => schedule.Wednesday,
            DayOfWeek.Thursday => schedule.Thursday,
            DayOfWeek.Friday => schedule.Friday,
            DayOfWeek.Saturday => schedule.Saturday,
            DayOfWeek.Sunday => schedule.Sunday,
            _ => false
        };
    }

    private static bool IsWithinTimeWindow(EventReportSchedule schedule, DateTime nowLocal)
    {
        var nowTime = nowLocal.TimeOfDay;
        var start = schedule.Time;
        var end = start + TriggerWindow;
        return nowTime >= start && nowTime < end;
    }
}
