using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EvtHubClient;

class Program
{
    private static string conStr = "Endpoint=sb://ps-platform.servicebus.windows.net/;SharedAccessKeyName=Schrijvert;SharedAccessKey=HqyFkSC9iSmxc0P08C3GZQ5z8EWON0IBQ+AEhL70DL8=;EntityPath=klanta";
    private static string hubName = "klanta";

    static async Task Main(string[] args)
    {
       // var options = new CreateBatchOptions { PartitionId = "1" };
       // var opts = new SendEventOptions { PartitionId = "2" };
        
        await using (var producerClient = new EventHubProducerClient(conStr, hubName))
        {
            int i = 1;
            ConsoleKeyInfo key;
            do
            {
                var eventBatch = await producerClient.CreateBatchAsync(/*options*/);
                for (int j = 0; j < 200; j++, i++)
                {
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Hello World {i}")));
                }
     
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("Sent");
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape);
        }

        Console.WriteLine("Done!");
        Console.ReadLine();
    }
}
