using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using System;
using System.Linq;

namespace Quartz.Extensions.Tests;

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
