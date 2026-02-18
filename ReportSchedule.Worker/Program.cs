
using Microsoft.EntityFrameworkCore;
using ReportSchedule.Data;
using ReportSchedule.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
	throw new InvalidOperationException(
		"Connection string 'DefaultConnection' not found. " +
		"Set it via environment variable ConnectionStrings__DefaultConnection or appsettings.json."
	);
}

builder.Services.AddDbContextFactory<AppDbContext>(options =>
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var timeZoneId = builder.Configuration["Scheduling:TimeZoneId"];
if (string.IsNullOrWhiteSpace(timeZoneId))
{
	throw new InvalidOperationException(
		"Scheduling: ID do fuso horário não encontrado. Configure-o (por exemplo, 'America/Sao_Paulo') nas configurações do aplicativo ou nas variáveis de ambiente."
	);
}

builder.Services.AddSingleton(sp => ResolveTimeZone(timeZoneId));

builder.Services.AddHostedService<ReportScheduleMonitor>();

var host = builder.Build();
host.Run();

static TimeZoneInfo ResolveTimeZone(string timeZoneId)
{
	// Linux/macOS typically support IANA IDs (e.g., America/Sao_Paulo).
	// Windows typically uses Windows IDs (e.g., Horário Padrão da América do Sul).
	try
	{
		return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
	}
	catch (TimeZoneNotFoundException)
	{
		if (string.Equals(timeZoneId, "America/Sao_Paulo", StringComparison.OrdinalIgnoreCase))
		{
			return TimeZoneInfo.FindSystemTimeZoneById("Horário Padrão da América do Sul");
		}

		throw;
	}
}
