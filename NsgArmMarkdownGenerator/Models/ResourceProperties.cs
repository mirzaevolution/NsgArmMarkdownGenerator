using Newtonsoft.Json;

namespace NsgArmMarkdownGenerator.Models
{
    public partial class ResourceProperties
    {
        [JsonProperty("securityRules")]
        public List<SecurityRule> SecurityRules { get; set; } = new List<SecurityRule>();
    }
}
