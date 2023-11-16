using Newtonsoft.Json;
using NsgArmMarkdownGenerator.Constants;
using NsgArmMarkdownGenerator.Models;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace NsgArmMarkdownGenerator.Helpers
{
    public class MarkdownGeneratorHelper
    {
        public string InputFile { get; private set; }
        public string PrJsonFile { get; private set; }
        public string OutputFile { get; private set; }
        public MarkdownGeneratorHelper(string inputFile, string prJsonFile, string outputFile)
        {
            InputFile = inputFile;
            PrJsonFile = prJsonFile;
            OutputFile = outputFile;
        }

        public void GenerateMarkdown()
        {
            Log.Information($"#{nameof(InputFile)}: {InputFile}");
            Log.Information($"#{nameof(PrJsonFile)}: {InputFile}");
            Log.Information($"#{nameof(OutputFile)}: {OutputFile}");
            RootTemplate rootTemplate = DeserializeFile<RootTemplate>(inputFile: InputFile);
            PrHistory prHistory = DeserializeFile<PrHistory>(inputFile: PrJsonFile);
            string markdownTempContent = MarkdownTemplate.MarkdownFileTemplate;
            ScanNsgAndGenerateMarkdown(
                rootTemplate: rootTemplate,
                prHistory: prHistory,
                markdownTempContent: markdownTempContent,
                inputFile: InputFile,
                outputFile: OutputFile);
        }

        private void ScanNsgAndGenerateMarkdown(
            RootTemplate rootTemplate,
            PrHistory prHistory,
            string markdownTempContent,
            string inputFile,
            string outputFile)
        {
            try
            {
                string nsgName = Path.GetFileNameWithoutExtension(inputFile);
                var resource = rootTemplate.Resources?.FirstOrDefault();
                int totalNsgRules = resource?.Properties?.SecurityRules.Count ?? 0;

                if (resource != null &&
                    resource.Properties != null &&
                    resource.Properties.SecurityRules != null)
                {
                    var nsgRules = resource.Properties.SecurityRules;
                    StringBuilder inboundNsgRuleSb = new StringBuilder();
                    StringBuilder outboundNsgRuleSb = new StringBuilder();
                    var inboundRules = nsgRules.Where(c => c.Properties.Direction == SecurityRuleDirection.Inbound);
                    var outboundRules = nsgRules.Where(c => c.Properties.Direction == SecurityRuleDirection.Outboud);

                    if (inboundRules != null && inboundRules.Any())
                    {
                        BuildSecurityRules(stringBuilder: inboundNsgRuleSb, securityRules: inboundRules);
                    }
                    else
                    {
                        BuildEmptySecurityRules(stringBuilder: inboundNsgRuleSb);
                    }

                    if (outboundRules != null && outboundRules.Any())
                    {
                        BuildSecurityRules(stringBuilder: outboundNsgRuleSb, securityRules: outboundRules);
                    }
                    else
                    {
                        BuildEmptySecurityRules(stringBuilder: outboundNsgRuleSb);
                    }

                    StringBuilder prHistorySb = new StringBuilder();
                    if (prHistory != null && prHistory.PrHistories != null && prHistory.PrHistories.Count > 0)
                    {
                        BuildPrHistories(stringBuilder: prHistorySb, prHistory.PrHistories);
                    }
                    else
                    {
                        BuildEmptyPrHistories(stringBuilder: prHistorySb);
                    }



                    string jsonNsgRules = JsonConvert.SerializeObject(nsgRules, Formatting.Indented);

                    //replace name
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgName, nsgName);

                    //replace total rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRuleTotalItems, totalNsgRules.ToString());
                    //replace total inboud rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRuleTotalInboundItems, (inboundRules?.Count() ?? 0).ToString());
                    //replace total outbound rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRuleTotalOutboundItems, (outboundRules?.Count() ?? 0).ToString());

                    //replace nsg rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRowInbound, inboundNsgRuleSb.ToString());
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRowOutbound, outboundNsgRuleSb.ToString());

                    //replace pr histories
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRowPullRequest, prHistorySb.ToString());

                    //replace json rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgJsonRules, jsonNsgRules);

                    File.WriteAllText(outputFile, markdownTempContent);
                    Log.Information($"Markdown '{outputFile}' created");
                }
                else
                {
                    Log.Error("Cannot proceed the process. Nsg rule list is empty");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while generating markdown file from deserialized nsg rules object");
                Log.Error(ex.Message);
                throw;
            }
        }

        private void BuildSecurityRules(StringBuilder stringBuilder, IEnumerable<SecurityRule> securityRules)
        {
            foreach (var rule in securityRules)
            {
                var ruleProp = rule.Properties;
                string priority = $"|{ruleProp.Priority}";
                string name = $"|{rule.Name}";
                string protocotol = $"|{ruleProp.Protocol}";
                string sourcePorts = "|";
                if (string.IsNullOrEmpty(ruleProp.SourcePortRange) &&
                   ruleProp.SourcePortRanges != null &&
                   ruleProp.SourcePortRanges.Count > 0)
                {
                    sourcePorts += string.Join(", ", ruleProp.SourcePortRanges);
                }
                else if (!string.IsNullOrEmpty(ruleProp.SourcePortRange))
                {
                    sourcePorts += ruleProp.SourcePortRange;
                }

                string sourceAddrs = "|";
                if (string.IsNullOrEmpty(ruleProp.SourceAddressPrefix) &&
                   ruleProp.SourceAddressPrefixes != null &&
                   ruleProp.SourceAddressPrefixes.Count > 0)
                {
                    sourceAddrs += string.Join(", ", ruleProp.SourceAddressPrefixes);
                }
                else if (!string.IsNullOrEmpty(ruleProp.SourceAddressPrefix))
                {
                    sourceAddrs += ruleProp.SourceAddressPrefix;
                }

                string destPorts = "|";
                if (string.IsNullOrEmpty(ruleProp.DestinationPortRange) &&
                   ruleProp.DestinationPortRanges != null &&
                   ruleProp.DestinationPortRanges.Count > 0)
                {
                    destPorts += string.Join(", ", ruleProp.DestinationPortRanges);
                }
                else if (!string.IsNullOrEmpty(ruleProp.DestinationPortRange))
                {
                    destPorts += ruleProp.DestinationPortRange;
                }

                string destAddrs = "|";
                if (string.IsNullOrEmpty(ruleProp.DestinationAddressPrefix) &&
                   ruleProp.DestinationAddressPrefixes != null &&
                   ruleProp.DestinationAddressPrefixes.Count > 0)
                {
                    destAddrs += string.Join(", ", ruleProp.DestinationAddressPrefixes);
                }
                else if (!string.IsNullOrEmpty(ruleProp.DestinationAddressPrefix))
                {
                    destAddrs += ruleProp.DestinationAddressPrefix;
                }

                string direction = $"|{ruleProp.Direction}";
                string action = $"|{ruleProp.Access}|";

                stringBuilder.AppendLine(
                        $"{priority}{name}{protocotol}{sourcePorts}{sourceAddrs}{destPorts}{destAddrs}{direction}{action}"
                    );
            }
        }

        private void BuildEmptySecurityRules(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("|n/a|n/a|n/a|n/a|n/a|n/a|n/a|n/a|n/a|");
        }

        private void BuildPrHistories(StringBuilder stringBuilder, IEnumerable<PrHistoryItem> prHistories)
        {
            prHistories = prHistories.OrderByDescending(c => c.CreatedDate);
            foreach (PrHistoryItem item in prHistories)
            {
                string pullRequestId = $"|{item.PullRequestId}";
                string title = $"|[{item.Title}]({item.PullRequestLink})";
                string createdBy = $"|{item.CreatedBy}";
                string createdDate = $"|{item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")}|";

                stringBuilder.AppendLine($"{pullRequestId}{title}{createdBy}{createdDate}");
            }
        }

        private void BuildEmptyPrHistories(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("|n/a|n/a|n/a|n/a|");

        }

        private T DeserializeFile<T>(string inputFile)
        {
            try
            {
                string content = File.ReadAllText(inputFile);
                T? instance = JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                }) ?? default(T);
                return instance;
            }
            catch (Exception ex)
            {
                Log.Error($"Error while deserializing file: {inputFile}");
                Log.Error(ex.Message);
                throw;
            }
        }

    }
}
