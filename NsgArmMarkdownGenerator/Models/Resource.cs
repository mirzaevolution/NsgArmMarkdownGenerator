using Newtonsoft.Json;

namespace NsgArmMarkdownGenerator.Models
{
    public partial class Resource
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("apiVersion")]
        public string? ApiVersion { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("location")]
        public string? Location { get; set; }

        [JsonProperty("properties")]
        public ResourceProperties Properties { get; set; } = new ResourceProperties();
    }
}
