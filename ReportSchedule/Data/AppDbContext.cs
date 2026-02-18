using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReportSchedule.Models;

namespace ReportSchedule.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EventReportSchedule> ReportSchedules => Set<EventReportSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var emailsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        modelBuilder.Entity<EventReportSchedule>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(300);

            entity.Property(x => x.Emails)
                .HasConversion(emailsConverter)
                .HasColumnType("json");
        });

        base.OnModelCreating(modelBuilder);
    }
}
