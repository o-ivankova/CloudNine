using Azure.Storage.Queues;
using CloudTestingApp.Models;
using CloudTestingApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CloudTestingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoItemController : Controller
    {
        private readonly ToDoItemService _toDoItemService;
        private readonly QueueClient _queueClient;

        public ToDoItemController(ToDoItemService toDoItemService, QueueClient queueClient)
        {
            _toDoItemService = toDoItemService;
            _queueClient = queueClient;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task AddOrReplaceItem(ToDoItemModel item)
        {
            var message = JsonSerializer.Serialize(item);
            await _queueClient.SendMessageAsync(message);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task DeleteItemById(string id, string userId)
        {
            await _toDoItemService.DeleteItemById(id, userId);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task DeleteItemsByUserId(string userId)
        {
            await _toDoItemService.DeleteItemsByUserId(userId);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<ToDoItemModel>> GetAllItemsByUserId(string userId)
        {
            return await _toDoItemService.GetAllItemsByUserId(userId);
        }
    }
}
