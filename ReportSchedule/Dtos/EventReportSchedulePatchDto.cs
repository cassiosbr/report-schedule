using System.ComponentModel.DataAnnotations;

namespace ReportSchedule.Dtos;

public class EventReportSchedulePatchDto
{
    [MaxLength(300)]
    public string? Name { get; set; }

    public List<string>? Emails { get; set; }

    public bool? Monday { get; set; }
    public bool? Tuesday { get; set; }
    public bool? Wednesday { get; set; }
    public bool? Thursday { get; set; }
    public bool? Friday { get; set; }
    public bool? Saturday { get; set; }
    public bool? Sunday { get; set; }

    public TimeSpan? Time { get; set; }

    public int? Days { get; set; }

    public bool? IsActive { get; set; }
}
