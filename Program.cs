using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Lisää Serilog
builder.Host.UseSerilog((context, config) =>
{
	config.WriteTo.Console();
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
