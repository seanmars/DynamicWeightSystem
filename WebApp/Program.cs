using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using WebApp;

Log.Logger = new LoggerConfiguration()
    .Destructure.ToMaximumDepth(3)
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlite(connectionString);
    });

    builder.Services.AddFastEndpoints();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseStaticFiles();
    app.UseHttpsRedirection();

    app.UseFastEndpoints(config =>
    {
        config.Endpoints.RoutePrefix = "api";
    });

    app.Run();
}
catch (HostAbortedException)
{
    // Ignore
    // Throw HostAbortedException when using EF CLI.
    // see more details: https://github.com/dotnet/efcore/issues/28478
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}