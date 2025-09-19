using AODB.Infrastructure.Data;
using AODB.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Kafka;

// Clean Architecture - API Application Setup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Infrastructure Layer - Logging with Kafka Support
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "AODB")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    // Development console logging
    if (context.HostingEnvironment.IsDevelopment())
    {
        loggerConfig.WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
    }

    // Kafka logging - her zaman aktif
    var kafkaEnabled = context.Configuration.GetValue<bool>("Logging:Kafka:Enabled", true);
    if (kafkaEnabled)
    {
        var kafkaBootstrapServers = context.Configuration.GetValue<string>("Logging:Kafka:BootstrapServers", "localhost:29092");
        var kafkaTopic = context.Configuration.GetValue<string>("Logging:Kafka:Topic", "aodb-logs");
        var batchSizeLimit = context.Configuration.GetValue<int>("Logging:Kafka:BatchSizeLimit", 50);
        var period = context.Configuration.GetValue<int>("Logging:Kafka:Period", 5);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Kafka()
            .CreateLogger();

        loggerConfig.WriteTo.Kafka(
            batchSizeLimit: batchSizeLimit,
            period: period,
            bootstrapServers: kafkaBootstrapServers,
            topic: kafkaTopic);
    }

    // File logging backup
    if (!context.HostingEnvironment.IsDevelopment())
    {
        loggerConfig.WriteTo.File(
            new CompactJsonFormatter(),
            path: "logs/aodb-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            shared: true);
    }
});

// Clean Architecture Layers
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();        // Application Layer
builder.Services.AddInfrastructureServices(builder.Configuration);  // Infrastructure Layer (includes Kafka)
builder.Services.AddWebServices();                // Web Layer

var app = builder.Build();

// API Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

// API Middleware Pipeline
app.UseHealthChecks("/health");
app.UseHttpsRedirection();

// Infrastructure Layer - Request/Response Logging Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Security
app.UseAuthentication();
app.UseAuthorization();

// API Documentation
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

// Exception Handling
app.UseExceptionHandler(options => { });

// API Routing
app.Map("/", () => Results.Redirect("/swagger"));
app.MapEndpoints();  // Clean Architecture - Endpoint mapping

app.Run();

public partial class Program
{
    // NSwag için gerekli method - .NET 8 API için
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((context, services) =>
                {
                    var configuration = new ConfigurationManager();
                    ((IConfigurationBuilder)configuration).AddConfiguration(context.Configuration);
                    ConfigureServices(services, configuration);
                });

                webBuilder.Configure(app =>
                {
                    if (app is IApplicationBuilder builder)
                    {
                        ConfigurePipeline((WebApplication)builder);
                    }
                });
            });
    }
    
    private static WebApplication CreateApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure services
        ConfigureServices(builder.Services, builder.Configuration);
        
        // Configure logging - minimal setup for NSwag
        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        var app = builder.Build();
        
        // Configure pipeline
        ConfigurePipeline(app);
        
        return app;
    }
    
    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddKeyVaultIfConfigured(configuration);
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);
        services.AddWebServices();

        // API Explorer ve Swagger/OpenAPI yapılandırması
        services.AddEndpointsApiExplorer();
        
        // NSwag için özel yapılandırma
        services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "v1_nswag";
            config.Title = "AODB API";
            config.Version = "v1";
            config.Description = "AODB API Documentation";
            
            // NSwag.AspNetCore ile uyumluluk için
            config.PostProcess = document =>
            {
                document.Info.Version = "v1";
                document.Info.Title = "AODB API";
                document.Info.Description = "AODB API Documentation";
            };
        });
    }
    
    private static void ConfigurePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // NSwag generation için development middleware'ler
        }
        else
        {
            app.UseHsts();
        }

        // API-only middleware pipeline
        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // API Documentation
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/api";
            settings.DocumentPath = "/api/specification.json";
        });

        // Exception Handling
        app.UseExceptionHandler(options => { });

        // API Routes
        app.Map("/", () => Results.Redirect("/api"));
        app.MapEndpoints();
    }
}
