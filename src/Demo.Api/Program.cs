using Demo.Api;
using Demo.Api.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddQuartzHostedService(o => o.WaitForJobsToComplete = true)
    .AddQuartz(config => config
        // Every second
        .RunCronScheduleJob<Job1>("* * * * * ? *")
        // Every 10 seconds
        .RunCronScheduleJob<Job2>("*/10 * * * * ? *")
        // At 01:01:01am, on the 1st day, in January
        .RunCronScheduleJob<Job3>("1 1 1 1 1 ? *"));

builder.Services.AddSingleton<IDataService, DataService>();


var app = builder.Build();

app.MapGet("", ()
    => Results.Ok("Hello, World!"));

await app.RunAsync();

public partial class Program;
