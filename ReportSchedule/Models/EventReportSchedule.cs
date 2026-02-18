using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace ReportSchedule.Models;

public class EventReportSchedule
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<string> Emails { get; set; } = [];

    public bool Monday { get; set; } = false;

    public bool Tuesday { get; set; } = false;

    public bool Wednesday { get; set; } = false;

    public bool Thursday { get; set; } = false;

    public bool Friday { get; set; } = false;

    public bool Saturday { get; set; } = false;

    public bool Sunday { get; set; } = false;

    [Required]
    public TimeSpan Time { get; set; }

    public int Days { get; set; } = 30;

    public bool IsActive { get; set; } = true;
}
