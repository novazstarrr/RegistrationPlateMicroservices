using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Catalog.API.Data;
using Catalog.API;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://seq:80")
    .CreateLogger();

builder.Host.UseSerilog();


var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();


var pathBase = app.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    Log.Information("Using PATH BASE '{PathBase}'", pathBase);
    app.UsePathBase(pathBase);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options => {
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.Use(async (context, next) =>
{
    Log.Information(
        "Request: {Method} {Path}{QueryString} {ContentType}",
        context.Request.Method,
        context.Request.Path,
        context.Request.QueryString,
        context.Request.ContentType);

    await next();

    Log.Information(
        "Response: {StatusCode} for {Method} {Path}",
        context.Response.StatusCode,
        context.Request.Method,
        context.Request.Path);
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }


        var seeder = services.GetRequiredService<ApplicationDbContextSeed>();
        var logger = services.GetRequiredService<ILogger<ApplicationDbContextSeed>>();
        await seeder.SeedAsync(context, app.Environment, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

var startupLogger = app.Services.GetRequiredService<ILogger<Startup>>();
startup.Configure(app, app.Environment, startupLogger);

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Catalog API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}