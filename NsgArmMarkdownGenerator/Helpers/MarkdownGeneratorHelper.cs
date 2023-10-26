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
        public string OutputFile { get; private set; }
        public MarkdownGeneratorHelper(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
        }

        public void GenerateMarkdown()
        {
            Log.Information($"#{nameof(InputFile)}: {InputFile}");
            Log.Information($"#{nameof(OutputFile)}: {OutputFile}");
            RootTemplate rootTemplate = DeserializeFile(inputFile: InputFile);
            string markdownTempContent = MarkdownTemplate.MarkdownFileTemplate;
            ScanNsgAndGenerateMarkdown(rootTemplate, markdownTempContent, InputFile, OutputFile);
        }

        private void ScanNsgAndGenerateMarkdown(
            RootTemplate rootTemplate,
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
                    resource.Properties.SecurityRules != null &&
                    totalNsgRules > 0)
                {
                    var nsgRules = resource.Properties.SecurityRules;
                    StringBuilder nsgRulesSb = new StringBuilder();
                    foreach (var rule in nsgRules)
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

                        nsgRulesSb.AppendLine(
                                $"{priority}{name}{protocotol}{sourcePorts}{sourceAddrs}{destPorts}{destAddrs}{direction}{action}"
                            );
                    }
                    string jsonNsgRules = JsonConvert.SerializeObject(nsgRules, Formatting.Indented);

                    //replace name
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgName, nsgName);

                    //replace total rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRuleTotalItems, totalNsgRules.ToString());

                    //replace nsg rules
                    markdownTempContent = Regex.Replace(markdownTempContent, MarkdownParameterConstants.NsgRow, nsgRulesSb.ToString());

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

        private RootTemplate DeserializeFile(string inputFile)
        {
            try
            {
                string content = File.ReadAllText(inputFile);
                RootTemplate template = JsonConvert.DeserializeObject<RootTemplate>(content, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                }) ?? new RootTemplate();
                return template;
            }
            catch (Exception ex)
            {
                Log.Error("Error while deserializing the ARM template file");
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
