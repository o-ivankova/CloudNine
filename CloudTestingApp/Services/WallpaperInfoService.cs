using CloudTestingApp.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CloudTestingApp.Services
{
    public class WallpaperInfoService
    {
        private const string _connectionString = "AccountEndpoint=https://ivankova1.documents.azure.com:443/;AccountKey=JlsmlWhhHrI2C6TBjkngJ5aWqKMFRTNcVulFBZM8nFgHYwoxdtXjYPxvoyYBnZCAiicq92WwKiwwACDbhUxd7w==;";
        private readonly CosmosClient _client;

        public WallpaperInfoService()
        {
            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            _client = new(connectionString: _connectionString, options);
        }

        public async Task<WallpaperInfoModel> GetItemById(string userId)
        {
            var container = await GetContainer();

            var queryable = container.GetItemLinqQueryable<WallpaperInfoModel>();
            var iterator = queryable
                .Where(p => p.Id == userId);

            using FeedIterator<WallpaperInfoModel> linqFeed = iterator.ToFeedIterator();

            var list = new List<WallpaperInfoModel>();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<WallpaperInfoModel> page = await linqFeed.ReadNextAsync();
                foreach (var el in page)
                {
                    list.Add(el);
                }
            }

            return list.FirstOrDefault();
        }

        public async Task<string> AddOrReplaceItem(string userId, string fileName)
        {
            var wallpaperInfo = new WallpaperInfoModel
            {
                Id = userId,
                FileName = fileName,
                Partition = "image",
            };

            var container = await GetContainer();

            var upsertedItem = await container.UpsertItemAsync<WallpaperInfoModel>(wallpaperInfo, new PartitionKey(wallpaperInfo.Partition));

            return upsertedItem.StatusCode.ToString();
        }

        public async Task DeleteItemById(string userId)
        {
            var container = await GetContainer();

            await container.DeleteItemAsync<WallpaperInfoModel>(userId, new PartitionKey("image"));

            return;
        }

        private async Task<Container> GetContainer()
        {
            Database database = await _client.CreateDatabaseIfNotExistsAsync(id: "ToDoList");

            return await database.CreateContainerIfNotExistsAsync(id: "wallpapers", partitionKeyPath: "/partition", throughput: 400);
        }
    }
}
