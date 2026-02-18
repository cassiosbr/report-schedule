namespace ReportSchedule.Dtos;

public sealed class EventReportScheduleDto
{
    public int Id { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<string> Emails { get; set; } = [];

    public bool Monday { get; set; }

    public bool Tuesday { get; set; }

    public bool Wednesday { get; set; }

    public bool Thursday { get; set; }

    public bool Friday { get; set; }

    public bool Saturday { get; set; }

    public bool Sunday { get; set; }

    public TimeSpan Time { get; set; }

    public int Days { get; set; }

    public bool IsActive { get; set; }
}
