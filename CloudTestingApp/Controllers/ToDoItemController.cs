using CloudTestingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Container = Microsoft.Azure.Cosmos.Container;

namespace CloudTestingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoItemController : Controller
    {
        private readonly CosmosClient _client;

        public ToDoItemController()
        {
            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            _client = new(connectionString: "AccountEndpoint=https://ivankova1.documents.azure.com:443/;AccountKey=JlsmlWhhHrI2C6TBjkngJ5aWqKMFRTNcVulFBZM8nFgHYwoxdtXjYPxvoyYBnZCAiicq92WwKiwwACDbhUxd7w==;", options);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<string> AddOrReplaceItem(ToDoItemModel item)
        {
            var container = await GetContainer();

            item.Id = Guid.NewGuid().ToString();

            var upsertedItem = await container.UpsertItemAsync<ToDoItemModel>(item, new PartitionKey(item.UserId));

            return upsertedItem.StatusCode.ToString();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task DeleteItemById(string id, string userId)
        {
            var container = await GetContainer();

            await container.DeleteItemAsync<ToDoItemModel>(id, new PartitionKey(userId));

            return;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task DeleteItemsByUserId(string userId)
        {
            var container = await GetContainer();

            var queryable = container.GetItemLinqQueryable<ToDoItemModel>();
            var matches = queryable
                .Where(p => p.UserId == userId);

            using FeedIterator<ToDoItemModel> linqFeed = matches.ToFeedIterator();

            while (linqFeed.HasMoreResults)
            {
                FeedResponse<ToDoItemModel> page = await linqFeed.ReadNextAsync();
                foreach (var el in page)
                {
                    await container.DeleteItemAsync<ToDoItemModel>(el.Id, new PartitionKey(el.UserId));
                }
            }

            return;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<ToDoItemModel>> GetAllItemsByUserId(string userId)
        {
            var container = await GetContainer();

            var queryable = container.GetItemLinqQueryable<ToDoItemModel>();
            var matches = queryable
                .Where(p => p.UserId == userId);

            using FeedIterator<ToDoItemModel> linqFeed = matches.ToFeedIterator();

            var list = new List<ToDoItemModel>();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<ToDoItemModel> page = await linqFeed.ReadNextAsync();
                foreach (var el in page)
                {
                    list.Add(el);
                }
            }

            return list;
        }

        private async Task<Container> GetContainer()
        {
            Database database = await _client.CreateDatabaseIfNotExistsAsync(id: "ToDoList");

            return await database.CreateContainerIfNotExistsAsync(id: "toDoItems", partitionKeyPath: "/userId", throughput: 400);
        }
    }
}
