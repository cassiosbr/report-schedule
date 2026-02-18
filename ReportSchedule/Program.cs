using Microsoft.EntityFrameworkCore;
using ReportSchedule.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

var timeZoneId = builder.Configuration["Scheduling:TimeZoneId"] ?? "America/Sao_Paulo";
builder.Services.AddSingleton(_ => ResolveTimeZone(timeZoneId));

builder.Services.AddDbContext<ReportSchedule.Data.AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static TimeZoneInfo ResolveTimeZone(string timeZoneId)
{
    try
    {
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
    catch (TimeZoneNotFoundException)
    {
        if (string.Equals(timeZoneId, "America/Sao_Paulo", StringComparison.OrdinalIgnoreCase))
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }

        throw;
    }
}
