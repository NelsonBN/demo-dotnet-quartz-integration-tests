using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Extensions.Tests;

namespace Demo.Api.Tests;

public sealed class IntegrationTestsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(QuartzExtensions.OverrideQuartzToTest<Program>);
    }

    public TService GetService<TService>() where TService : notnull
    {
        return Services.GetRequiredService<TService>();
    }
}

[CollectionDefinition(nameof(CollectionIntegrationTests))]
public sealed class CollectionIntegrationTests : ICollectionFixture<IntegrationTestsFactory> { }
