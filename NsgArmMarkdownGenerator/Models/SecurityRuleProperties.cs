using Newtonsoft.Json;

namespace NsgArmMarkdownGenerator.Models
{
    public partial class SecurityRuleProperties
    {
        [JsonProperty("protocol")]
        public string? Protocol { get; set; }

        [JsonProperty("sourcePortRange")]
        public string? SourcePortRange { get; set; }

        [JsonProperty("destinationPortRange")]
        public string? DestinationPortRange { get; set; }

        [JsonProperty("sourceAddressPrefix")]
        public string? SourceAddressPrefix { get; set; }

        [JsonProperty("destinationAddressPrefix")]
        public string? DestinationAddressPrefix { get; set; }

        [JsonProperty("access")]
        public string? Access { get; set; }

        [JsonProperty("priority")]
        public long? Priority { get; set; }

        [JsonProperty("direction")]
        public string? Direction { get; set; }

        [JsonProperty("sourcePortRanges", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SourcePortRanges { get; set; } = new List<string>();

        [JsonProperty("destinationPortRanges", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DestinationPortRanges { get; set; } = new List<string>();

        [JsonProperty("sourceAddressPrefixes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SourceAddressPrefixes { get; set; } = new List<string>();

        [JsonProperty("destinationAddressPrefixes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DestinationAddressPrefixes { get; set; } = new List<string>();

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }
    }
}
