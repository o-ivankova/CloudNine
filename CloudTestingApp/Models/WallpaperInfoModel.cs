using Newtonsoft.Json;

namespace CloudTestingApp.Models
{
    public class WallpaperInfoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("partition")]
        public string Partition { get; set; }
    }
}
