using Newtonsoft.Json;

namespace NsgArmMarkdownGenerator.Models
{
    public class PrHistory
    {
        [JsonProperty("prHistories")]
        public List<PrHistoryItem> PrHistories { get; set; } = new List<PrHistoryItem>();
    }

    public class PrHistoryItem
    {
        [JsonProperty("pullRequestId", NullValueHandling = NullValueHandling.Ignore)]
        public string? PullRequestId { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty("pullRequestLink", NullValueHandling = NullValueHandling.Ignore)]
        public string? PullRequestLink { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
