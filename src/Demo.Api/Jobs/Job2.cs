using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Demo.Api.Jobs;

public sealed class Job2(
    ILogger<Job2> logger,
    IDataService data) : IJob
{
    private readonly ILogger<Job2> _logger = logger;
    private readonly IDataService _data = data;

    public async Task Execute(IJobExecutionContext context)
    {
        const string jobName = nameof(Job2);

        var lastId = _data.Get(jobName);

        _logger.LogInformation("[{Job}][BEFORE][{Id}] LastId: '{LastId}' {Time}",
            jobName,
            context.FireInstanceId,
            lastId,
            DateTimeOffset.UtcNow);

        _data.Set(jobName, context.FireInstanceId);

        await Task.Delay(
            TimeSpan.FromSeconds(1),
            context.CancellationToken);

        _logger.LogInformation("[{Job}][AFTER][{Id}] {Time}",
            jobName,
            context.FireInstanceId,
            DateTimeOffset.UtcNow);
    }
}
