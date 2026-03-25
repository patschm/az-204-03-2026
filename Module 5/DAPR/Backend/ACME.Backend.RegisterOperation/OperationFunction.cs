using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Dapr;
using Newtonsoft.Json.Linq;
using Dapr.Client;

namespace ACME.Backend.RegisterOperation;

// Important Note:
// Port 3001 is only exposed and listened to if a Dapr trigger is defined in the function app.
// When using Dapr, the sidecar waits to receive a response from the defined port before completing instantiation.
// Do not define the dapr.io/port annotation or --app-port unless you have a trigger.
// Doing so may lock your application from the Dapr sidecar.
// If you're only using input and output bindings, port 3001 doesn't need to be exposed or defined.

// Even more important!
// If and Azure Container Apps schales to zero instances, it will not be able to respond to triggers.

public static class OperationFunction
{
    const string STATE_STORE_NAME = "statestore";

    [FunctionName("RegisterOperation")]
    public static async Task<IActionResult> Run(
        [DaprTopicTrigger("regops-queue", Topic = "regops")] JObject msg,
        ILogger log)
    {
        log.LogInformation("RegisterOperation executing...");
        log.LogInformation($"Received message: {msg.ToString()}");
        var model = msg["data"].ToObject<OperationModel>();
        model.DateAdded = DateTime.UtcNow;

        var clientBuilder = new DaprClientBuilder().Build();
        try
        {
            await clientBuilder.SaveStateAsync(STATE_STORE_NAME, $"{model.A}+{model.B}", model);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to save state");
            return new StatusCodeResult(400);
        }

        return new OkResult();
    }
}
