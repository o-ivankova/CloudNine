using Azure.Storage.Queues;
using CloudTestingApp.Models;
using System.Text.Json;

namespace CloudTestingApp.Services
{
    public class ToDoItemQueueService : BackgroundService
    {
        private readonly QueueClient _queueClient;
        private readonly ToDoItemService _toDoItemService;

        public ToDoItemQueueService(QueueClient queueClient, ToDoItemService toDoItemService)
        {
            _queueClient = queueClient;
            _toDoItemService = toDoItemService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var queueMessage = await _queueClient.ReceiveMessageAsync();

                if (queueMessage.Value == null)
                {
                    return;
                }

                var toDoItem = JsonSerializer.Deserialize<ToDoItemModel>(queueMessage.Value.MessageText);
                await _toDoItemService.AddOrReplaceItem(toDoItem);

                await _queueClient.DeleteMessageAsync(queueMessage.Value.MessageId, queueMessage.Value.PopReceipt);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
