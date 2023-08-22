using Newtonsoft.Json;

namespace CloudTestingApp.Models
{
    public class ToDoItemModel
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        [JsonProperty("task")]
        public string Task { get; set; }
        [JsonProperty("completed")]
        public bool Completed { get; set; }
    }
}
