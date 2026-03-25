using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Consumer;
using System.Text;
using Azure.Storage.Blobs;
using System.Collections.Concurrent;

namespace EvtHubConsumer;

class Program
{
    private const string conStr = "Endpoint=sb://ps-hup.servicebus.windows.net/;SharedAccessKeyName=lezert;SharedAccessKey=ffF6c/Y0PeaS8hHDqNDVDUKgHPDxqG8R2+AEhH0WnL0=;EntityPath=hupholland";
    private const string hubName = "hupholland";

    private const string checkpointStorage = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=7qnH81G0v5gBkTtFLFk4ZInxsfkxMCVywp0CWmLM6a66jZLJdnDehC4ANOZO1f7Vj09hsLnXWlL1+ASt5tVGGw==;EndpointSuffix=core.windows.net";

    static async Task Main(string[] args)
    {
        //await LeanAndMean();
       await UsingProcessors(args.Length >= 1 ? args[0]: EventHubConsumerClient.DefaultConsumerGroupName);
        Console.ReadLine();
        Console.WriteLine("Started...");
    }

    private static async Task UsingProcessors(string naam)
    {
        // For checkpoints
        // Checkpoints keep track of what you read (stored in blob)
        // Otherwise you'll see the same events over and over again
        var partitionEventCount = new ConcurrentDictionary<string, int>();
        BlobContainerClient blobContainerClient = new BlobContainerClient(checkpointStorage, naam);
        blobContainerClient.CreateIfNotExists();

        var processor = new EventProcessorClient(blobContainerClient,
            EventHubConsumerClient.DefaultConsumerGroupName,
            conStr, hubName);

        processor.ProcessEventAsync += async partitionEvent =>
        {
            var partID = partitionEvent.Partition.PartitionId;
            Console.WriteLine($"Event Read ({partID}): {Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray())}");

            // Set new checkpoint
            int eventsSince = partitionEventCount.AddOrUpdate(partID, 1, (str, cnt) => cnt + 1);

            if (eventsSince >= 25)
            {
                await partitionEvent.UpdateCheckpointAsync();
                partitionEventCount[partID] = 0;
            }
        };
        processor.ProcessErrorAsync += errorEvent =>
        {
            Console.WriteLine($"Ooops (Partition: {errorEvent.PartitionId}): {errorEvent.Exception.Message}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("Processor is running. Press Enter to stop");
        Console.ReadLine();
        await processor.StopProcessingAsync();

    }

    private static async Task LeanAndMean()
    {
        await using (var consumerClient = new EventHubConsumerClient(
            EventHubConsumerClient.DefaultConsumerGroupName,
            //"memyselfandi",
            conStr,
            hubName))
        {
            int eventsRead = 0;

            //ReadEventOptions opts = new ReadEventOptions();
            await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync())
            {
                Console.WriteLine($"Event Read ({partitionEvent.Partition.PartitionId}): {Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray())}");
                eventsRead++;
            }
            Console.WriteLine($"Events read: {eventsRead}");
        }

    }

}
