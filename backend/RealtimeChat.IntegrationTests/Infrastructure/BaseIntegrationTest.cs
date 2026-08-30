using RealtimeChat.DAL.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RealtimeChat.IntegrationTests.Infrastructure;

[Collection("Integration")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public virtual Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected HttpClient CreateHeaderAuthenticatedClient(User user)
    {
        var client = CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-UserId", user.Id.ToString());

        return client;
    }

    protected HttpClient CreateClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
