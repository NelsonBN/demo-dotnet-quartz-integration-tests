using Quartz;
using System;

namespace Demo.Worker.ApplicationBuilder.Jobs;

public static class JobsExtensions
{
    // https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/aspnet-core-integration.html
    // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#building-crontriggers

    public static IServiceCollectionQuartzConfigurator RunScheduledJob<TJob>(this IServiceCollectionQuartzConfigurator config, int hour, int minute)
        where TJob : IJob
    {
        var jobKey = new JobKey(typeof(TJob).Name);

        config.AddJob<TJob>(jobKey, job => job.DisallowConcurrentExecution());

        config.AddTrigger(trigger => trigger
              .ForJob(jobKey)
              .WithIdentity(typeof(TJob).Name)
              .StartNow()
              .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)
                                                            .InTimeZone(TimeZoneInfo.Utc)));

        return config;
    }

    public static IServiceCollectionQuartzConfigurator RunCronScheduleJob<TJob>(this IServiceCollectionQuartzConfigurator config, string cronExpression)
        where TJob : IJob
    {
        var jobKey = new JobKey(typeof(TJob).Name);

        config.AddJob<TJob>(jobKey, job => job.DisallowConcurrentExecution());

        config.AddTrigger(trigger => trigger
              .ForJob(jobKey)
              .WithIdentity(typeof(TJob).Name)
              .StartNow()
              .WithCronSchedule(cronExpression));

        return config;
    }

    public static IServiceCollectionQuartzConfigurator RunOnceJob<TJob>(this IServiceCollectionQuartzConfigurator config)
        where TJob : IJob
    {
        config.ScheduleJob<TJob>(trigger => trigger
              .WithIdentity(typeof(TJob).Name)
              .WithSimpleSchedule(schedule =>
                schedule.WithRepeatCount(0).WithInterval(TimeSpan.Zero)));

        return config;
    }
}