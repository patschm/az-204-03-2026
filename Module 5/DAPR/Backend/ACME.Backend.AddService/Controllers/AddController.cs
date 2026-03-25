using ACME.Business.Interfaces;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Backend.AddService.Controllers;

[ApiController]
[Route("[controller]")]
public class AddController(ILogger<AddController> logger, ICalculator calculator, DaprClient dapr) : ControllerBase
{
    [HttpGet(Name = "Add")]
    public async Task<int> Get(int a, int b)
    {
        var result = calculator.Add(a, b);
        var model = new OperationModel
        {
            DateAdded = DateTime.UtcNow,
            Operation = '+',
            A = a,
            B = b,
            Result = result
        };
        try
        {
            await dapr.PublishEventAsync("regops-queue", "regops", model);
            logger.LogInformation("Published operation to pubsub: {Model}", model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish operation to pubsub");
        }
        return result;
    }
}
