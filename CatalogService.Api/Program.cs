using CatalogService.Api.Grpc.V1;
using CatalogService.Application.DependencyInjection;
using CatalogService.Infrastructure.DependencyInjection;
using migApp.Shared.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseMigAppSerilog("CatalogService");

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

//await app.MigrateDatabaseAsync();
//await app.SeedDatabaseAsync();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();