using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Infrastructure.BackgroundServices;
using CatalogService.Infrastructure.Behaviours;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.DependencyInjection;
using CatalogService.Infrastructure.DomainEvents;
using CatalogService.Infrastructure.Jobs;
using CatalogService.Infrastructure.Messaging;
using CatalogService.Infrastructure.Messaging.Consumers;
using CatalogService.Infrastructure.Messaging.IntegrationEvents;
using CatalogService.Infrastructure.Observability;
using Hangfire;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using migApp.Shared.Behaviours;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Grpc;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace CatalogService.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddHangfire(configuration)
            .AddGrpcServices()
            .AddHealth(configuration)
            .AddMessaging()
            .AddIntegrationEventHandlers()
            .AddObservability(configuration)
            .AddBehaviours();

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services
            .AddOptions<SoftDeletedProductsCleanupOptions>()
            .Bind(configuration.GetSection(
                SoftDeletedProductsCleanupOptions.SectionName))
            .Validate(
                options => options.Interval > TimeSpan.Zero,
                $"{nameof(SoftDeletedProductsCleanupOptions.Interval)} must be greater than 0.")
            .Validate(
                options => options.RetentionPeriod > TimeSpan.Zero,
                $"{nameof(SoftDeletedProductsCleanupOptions.RetentionPeriod)} must be greater than 0.")
            .Validate(
                options => options.BatchSize > 0,
                $"{nameof(SoftDeletedProductsCleanupOptions.BatchSize)} must be greater than 0.")
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<SoftDeletedProductsCleanupOptions>>().Value);

        services.AddHostedService<SoftDeletedProductsCleanupService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.CatalogWrite);
            }));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        services.AddHangfireServer();
        services.AddScoped<ISuspendVendorProductsJob, SuspendVendorProductsJob>();
        services.AddScoped<IDeleteVendorProductsJob, DeleteVendorProductsJob>();
        services.AddScoped<ISuspendBrandProductsJob, SuspendBrandProductsJob>();
        services.AddScoped<ISuspendCategoryProductsJob, SuspendCategoryProductsJob>();

        return services;
    }

    private static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        return services;
    }

    private static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        var rabbitMq = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()
                ?? throw new InvalidOperationException("RabbitMQ configuration is missing.");

        services.AddGrpcHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
            .AddSqlServer(
                connectionString: databaseConnectionString,
                name: "mssql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRabbitMQ(
                factory: async sp =>
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rabbitMq.Host,
                        Port = rabbitMq.Port,
                        VirtualHost = rabbitMq.VirtualHost,
                        UserName = rabbitMq.Username,
                        Password = rabbitMq.Password
                    };
                    return await factory.CreateConnectionAsync();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services
           .AddOptions<RabbitMqOptions>()
           .BindConfiguration(RabbitMqOptions.SectionName)
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Host),
               "RabbitMQ:Host is required.")
           .Validate(
               options => options.Port > 0,
               "RabbitMQ:Port must be greater than zero.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Username),
               "RabbitMQ:Username is required.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Password),
               "RabbitMQ:Password is required.")
           .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("catalog-service", false));

            x.AddConsumersFromNamespaceContaining<ConsumersAssemblyMarker>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                cfg.Host(options.Host,
                    options.Port,
                    options.VirtualHost,
                    host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
                    });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(IntegrationEventsAssemblyMarker).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IPreCommitDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    private static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var otlpEndpoint = configuration["Otlp:Endpoint"]
            ?? throw new InvalidOperationException("OtlpEndpoint is not configured");

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("CatalogService"))
            .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otlpEndpoint))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(opts =>
                    opts.Filter = ctx =>
                        !ctx.Request.Path.StartsWithSegments("/health"))
                .AddHttpClientInstrumentation()
                .AddSource("MassTransit")
                .AddSource("CatalogService"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("MassTransit")
                .AddMeter(CatalogServiceMetrics.MeterName));

        services.AddSingleton<ICatalogMetrics, CatalogServiceMetrics>();
        services.AddHostedService<ActiveProductsMetricCollector>();

        return services;
    }

    private static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}