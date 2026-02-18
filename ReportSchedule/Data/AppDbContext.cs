using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var emailsComparer = new ValueComparer<List<string>>(
            (a, b) => ReferenceEquals(a, b) || (a != null && b != null && a.SequenceEqual(b)),
            v => v == null ? 0 : string.Join("\u001F", v).GetHashCode(),
            v => v == null ? new List<string>() : v.ToList()
        );

        modelBuilder.Entity<EventReportSchedule>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(300);

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(x => x.Emails)
                .HasConversion(emailsConverter)
                .HasColumnType("json")
                .Metadata.SetValueComparer(emailsComparer);
        });

        base.OnModelCreating(modelBuilder);
    }
}
