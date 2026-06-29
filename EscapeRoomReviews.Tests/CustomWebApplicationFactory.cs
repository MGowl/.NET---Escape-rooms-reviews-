using EscapeRoomReviews.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EscapeRoomReviews.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Unique per instance so every new CustomWebApplicationFactory gets its own
    // isolated in-memory database regardless of how many factories are alive at once.
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";

    // Zamjenjuje bootstrap Serilog logger s fiksnim no-op loggerom da izbjegnemo
    // "logger is already frozen" grešku kad se više factory instanci kreira paralelno.
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseSerilog(new LoggerConfiguration().MinimumLevel.Fatal().CreateLogger(), dispose: true);
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Google:ClientId"] = "test-client-id",
                ["Authentication:Google:ClientSecret"] = "test-client-secret"
            });
        });

        builder.ConfigureServices(services =>
        {
            // EF Core 9 AddDbContext registers one singleton IDbContextOptionsConfiguration<T>
            // per AddDbContext call. The DbContextOptions<T> factory then resolves ALL of them.
            // We must remove both the options factory AND the configuration singletons so that
            // only the InMemory provider ends up in the rebuilt options.
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration") &&
                     d.ServiceType.GenericTypeArguments.Length == 1 &&
                     d.ServiceType.GenericTypeArguments[0] == typeof(ApplicationDbContext)))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Register a test authentication handler that auto-authenticates every request
            // as an Admin user, allowing [Authorize(Roles = "Admin,Editor")] to pass.
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);

            // PostConfigure runs after AddDefaultIdentity has set its own defaults, so this
            // override reliably takes effect for all [Authorize] checks in tests.
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = "Test";
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultSignInScheme = "Test";
                options.DefaultSignOutScheme = "Test";
                options.DefaultForbidScheme = "Test";
            });
        });
    }
}
