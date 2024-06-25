using Demo.Worker.ApplicationBuilder.Jobs;

namespace Demo.Worker.ApplicationBuilder.Tests.Jobs;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class Job3Test(IntegrationTestsFactory factory)
{
    private readonly IntegrationTestsFactory _factory = factory;
    private const string JobName = nameof(Job3);


    [Fact]
    public async Task Test()
    {
        // Arrange
        var service = _factory.GetService<IDataService>();


        // Act
        await Task.Delay(TimeSpan.FromSeconds(2));
        var act = service.Get(JobName);


        // Assert
        Assert.NotNull(act);

    }
}