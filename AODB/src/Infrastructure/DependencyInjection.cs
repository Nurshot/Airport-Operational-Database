using AODB.Application.Common.Interfaces;
using AODB.Domain.Constants;
using AODB.Infrastructure.Data;
using AODB.Infrastructure.Data.Interceptors;
using AODB.Infrastructure.Identity;
using AODB.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Keycloak authentication yapılandırması
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var authority = configuration["Keycloak:Authority"];
                var realm = configuration["Keycloak:Realm"];
                
                options.Authority = $"{authority}/realms/{realm}";
                options.Audience = configuration["Keycloak:ClientId"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{authority}/realms/{realm}",
                    ValidateAudience = true,
                    ValidAudiences = new[] { "broker", "account", "login" },
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role,
                    AuthenticationType = "Bearer"
                };
                
                // Keycloak role claim transformation
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Transform Keycloak realm_access roles to standard role claims
                        var identity = context.Principal?.Identity as ClaimsIdentity;
                        if (identity != null)
                        {
                            var realmAccessClaim = identity.FindFirst("realm_access");
                            if (realmAccessClaim?.Value != null)
                            {
                                try
                                {
                                    var realmAccess = JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccessClaim.Value);
                                    if (realmAccess?.ContainsKey("roles") == true)
                                    {
                                        foreach (var role in realmAccess["roles"])
                                        {
                                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                                        }
                                    }
                                }
                                catch (JsonException)
                                {
                                    // Ignore realm_access parsing errors
                                }
                            }
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("admin", policy =>
                policy.RequireRole("admin"));
        });

        services.AddHttpClient<KeycloakIdentityService>();
        services.AddHttpClient<KeycloakAuthenticationService>();
        services.AddHttpClient();
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddSingleton(TimeProvider.System);

        // Yeni bir IIdentityService implementasyonu ekleyelim
        services.AddScoped<IIdentityService, KeycloakIdentityService>();
        services.AddScoped<IAuthenticationService, KeycloakAuthenticationService>();

        // Logging services - Kafka logging is now handled by Serilog.Sinks.ConfluentKafka
        services.AddScoped<ILoggingService, AODB.Infrastructure.Logging.LoggingService>();

        return services;
    }
}
