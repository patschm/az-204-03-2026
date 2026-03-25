using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serveless
{
    public class Function
    {
        [FunctionName("Function")]
        public void Run([RabbitMQTrigger("myqueue", ConnectionStringSetting = "ConSt")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

        }
    }
}
