using Azure;
using Azure.Storage.Queues;

namespace StorageQueueReader;

class Program
{
    static string ConnectionString = "";
    static string QueueName = "test";
    static async Task Main(string[] args)
    {
        await ReadFromQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadFromQueueAsync()
    {
        var token = new AzureSasCredential("?");
        var client = new QueueClient(new Uri("https://psstoor.queue.core.windows.net/myqueue"), token);
        
        //var client = new QueueClient(ConnectionString, QueueName);
        int i = 0;
        do
        {
            // 10 seconds "lease" time
            i++;
            var response = await client.ReceiveMessageAsync(TimeSpan.FromSeconds(30));
            if (response.Value == null)
            {
                await Task.Delay(100);
                continue;
            }
            if (i % 10 == 0)
            {
                Console.WriteLine("Oooops");
                continue;
            }
            var msg = response.Value;
            Console.WriteLine(msg.Body.ToString());

            // We need more time to process
            //await client.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.Body, TimeSpan.FromSeconds(30));
            // Don't forget to remove
            await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
        }
        while (true);
    }
}
