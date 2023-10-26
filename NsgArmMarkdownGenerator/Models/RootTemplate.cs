

using Newtonsoft.Json;

namespace NsgArmMarkdownGenerator.Models
{
    public partial class RootTemplate
    {
        [JsonProperty("$schema")]
        [JsonIgnore]

        public Uri? Schema { get; set; }

        [JsonProperty("contentVersion")]
        [JsonIgnore]

        public string? ContentVersion { get; set; }

        [JsonProperty("parameters")]
        [JsonIgnore]

        public object? Parameters { get; set; }

        [JsonProperty("variables")]
        [JsonIgnore]
        public object? Variables { get; set; }

        [JsonProperty("resources")]
        public List<Resource> Resources { get; set; } = new List<Resource>();
    }
}
