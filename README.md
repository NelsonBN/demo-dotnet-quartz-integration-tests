# Demo Integration Tests for Quartz Scheduler


## How to configure Tests for Quartz Scheduler
In integration tests, we need to override the Quartz Scheduler configuration by the test configuration.

```csharp
public sealed class IntegrationTestsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(QuartzExtensions.OverrideQuartzToTest<Program>);
    }
}
```


### For worker service
In integration tests for worker service, we need also configure `WebHostBuilder` using `builder.Configure((_) => { });`.

```csharp
public sealed class IntegrationTestsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.Configure((_) => { });
        builder.ConfigureTestServices(QuartzExtensions.OverrideQuartzToTest<Program>);
    }
}
```


## Quartz Configuration

The idea is to override the Quartz configuration by a configuration will execute the jobs immediately and only once.


```csharp
var jobKey = new JobKey(jobType.Name);

config.AddJob(jobType, jobKey, job =>
{
    job.WithIdentity(jobType.Name);
    job.StoreDurably();
    job.DisallowConcurrentExecution();
});

config.AddTrigger(trigger => trigger
    .ForJob(jobKey)
    .WithIdentity(jobType.Name)
    .WithSimpleSchedule(schedule => schedule
        .WithRepeatCount(0)
        .WithInterval(TimeSpan.Zero)));
```

### Full Example

```csharp
public static class QuartzExtensions
{
    private static readonly Type _iSchedulerFactoryAbstract = typeof(ISchedulerFactory);

    public static void OverrideQuartzToTest<TAssembly>(IServiceCollection services)
    {
        services.OverrideSchedulerFactory();

        var types = typeof(TAssembly).Assembly
            .GetTypes()
            .Where(w => !(w.IsInterface || w.IsAbstract || !typeof(IJob).IsAssignableFrom(w)))
            .ToList();

        services.AddQuartz(config =>
        {
            foreach (var jobType in types)
            {
                var jobKey = new JobKey(jobType.Name);

                config.AddJob(jobType, jobKey, job =>
                {
                    job.WithIdentity(jobType.Name);
                    job.StoreDurably();
                    job.DisallowConcurrentExecution();
                });

                config.AddTrigger(trigger => trigger
                    .ForJob(jobKey)
                    .WithIdentity(jobType.Name)
                    .WithSimpleSchedule(schedule => schedule
                        .WithRepeatCount(0)
                        .WithInterval(TimeSpan.Zero)));
            }
        });
    }


    private static IServiceCollection OverrideSchedulerFactory(this IServiceCollection services)
    {
        var serviceTypes = services.Where(f => f.ServiceType == _iSchedulerFactoryAbstract);
        if (!serviceTypes.Any())
        {
            throw new NotImplementedException($"The '{_iSchedulerFactoryAbstract.Name}' was not implemented");
        }

        services.RemoveAll(typeof(ISchedulerFactory));
        foreach (var serviceType in serviceTypes)
        {
            switch (serviceType.Lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();
                    break;

                case ServiceLifetime.Scoped:
                    services.AddScoped<ISchedulerFactory, StdSchedulerFactory>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    break;
            }
        }

        return services;
    }
}
```
