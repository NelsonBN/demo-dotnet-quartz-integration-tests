using Demo.Worker.DefaultBuilder;
using Demo.Worker.DefaultBuilder.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddQuartzHostedService(o => o.WaitForJobsToComplete = true)
            .AddQuartz(config => config
                // Every second
                .RunCronScheduleJob<Job1>("* * * * * ? *")
                // Every 10 seconds
                .RunCronScheduleJob<Job2>("*/10 * * * * ? *")
                // At 01:01:01am, on the 1st day, in January
                .RunCronScheduleJob<Job3>("1 1 1 1 1 ? *"));

        services.AddSingleton<IDataService, DataService>();
    });


var app = builder.Build();
await app.RunAsync();


public partial class Program;
