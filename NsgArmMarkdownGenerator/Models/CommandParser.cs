using CommandLine;

namespace NsgArmMarkdownGenerator.Models
{
    public class CommandParser
    {
        [Option("input-file", HelpText = "Input file location", Required = true)]
        public string? InputFile { get; set; }

        [Option("pr-json-file", HelpText = "PR json file location", Required = true)]
        public string? PrJsonFile { get; set; }

        [Option("output-file", HelpText = "Output file location", Required = true)]
        public string? OutputFile { get; set; }
    }
}
