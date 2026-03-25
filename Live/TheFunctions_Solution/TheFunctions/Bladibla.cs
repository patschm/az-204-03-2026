using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TheFunctions;

public class Bladibla
{
    private readonly ILogger<Bladibla> _logger;

    public Bladibla(ILogger<Bladibla> logger)
    {
        _logger = logger;
    }

    [Function("Pinger")]
    public IActionResult Bliep([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="ping/{wie}")] HttpRequest req, string wie)
    {
        _logger.LogInformation($"C# HTTP trigger function processed a request. from {wie}");
        return new OkObjectResult($"Welcome to Azure Functions {wie}!");
    }

    [Function("EverySecond")]
    public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}