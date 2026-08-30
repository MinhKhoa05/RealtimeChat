using Dapper;
using Dommel;
using DotNet.Testcontainers.Builders;
using RealtimeChat.DAL.Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlConnector;
using Respawn;
using Testcontainers.MariaDb;

namespace RealtimeChat.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static MariaDbContainer? Container;
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool Initialized;

    private Respawner? _respawner;

    static CustomWebApplicationFactory()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        DommelMapper.SetColumnNameResolver(new SnakeCaseResolver());
        DommelMapper.AddSqlBuilder(typeof(MySqlConnection), new MySqlSqlBuilder());
    }

    public async Task InitializeAsync()
    {
        try
        {
            var container = GetContainer();
            await container.StartAsync();
        }
        catch (DockerUnavailableException ex)
        {
            throw new InvalidOperationException(
                "Docker is required for integration tests. Please start Docker Desktop and run the tests again.", ex);
        }

        if (Initialized)
        {
            return;
        }

        await InitializationLock.WaitAsync();
        try
        {
            if (!Initialized)
            {
                await InitializeSchemaAsync();
                Initialized = true;
            }
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

    public async Task ResetDatabaseAsync()
    {
        var container = GetContainer();

        await using var connection = new MySqlConnection(container.GetConnectionString());
        await connection.OpenAsync();

        _respawner ??= await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.MySql,
            SchemasToInclude = ["fullstack_foundation_test"]
        });

        await _respawner.ResetAsync(connection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var container = GetContainer();

        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = container.GetConnectionString(),
                ["Jwt:Key"] = "integration-test-signing-key-that-is-long-enough-12345",
                ["Jwt:Issuer"] = "RealtimeChat.IntegrationTests",
                ["Jwt:Audience"] = "RealtimeChat.IntegrationTests",
                ["Jwt:ExpireMinutes"] = "60",
                ["App:BaseUrl"] = "https://api.test.local"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    private static async Task InitializeSchemaAsync()
    {
        var schemaPath = LocateSchemaPath();
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException("Could not find integration test schema.", schemaPath);
        }

        var statements = (await File.ReadAllTextAsync(schemaPath))
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(statement => !string.IsNullOrWhiteSpace(statement));

        await using var connection = new MySqlConnection(GetContainer().GetConnectionString());
        await connection.OpenAsync();

        foreach (var statement in statements)
        {
            await connection.ExecuteAsync(statement);
        }
    }

    private static string LocateSchemaPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var schemaPath = Path.Combine(directory.FullName, "database", "schema.sql");
            if (File.Exists(schemaPath))
            {
                return schemaPath;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "database", "schema.sql");
    }

    private static MariaDbContainer GetContainer()
    {
        return Container ??= new MariaDbBuilder("mariadb:11.4")
            .WithDatabase("fullstack_foundation_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();
    }
}
