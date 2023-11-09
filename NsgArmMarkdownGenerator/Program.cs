using CommandLine;
using NsgArmMarkdownGenerator.Helpers;
using NsgArmMarkdownGenerator.Models;
using Serilog;
using System.Text.Json;

namespace NsgArmMarkdownGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();
            Log.Information("--------------------------------------------------");
            Log.Information("Markdown NSG ARM Template Generator");
            Log.Information("Developed by uptec.io (Mirza Ghulam Rasyid)");
            Log.Information("--------------------------------------------------\n");


            string inputFileLocation = string.Empty;
            string prJsonFileLocation = string.Empty;
            string outputFileLocation = string.Empty;
            List<string> parserErrors = new List<string>();
            Parser.Default.ParseArguments<CommandParser>(args)
                .WithParsed<CommandParser>(o =>
                {
                    if (!string.IsNullOrEmpty(o.InputFile))
                    {
                        inputFileLocation = o.InputFile.Trim();
                        if (!File.Exists(inputFileLocation))
                        {
                            parserErrors.Add($"File input: '{inputFileLocation}' not found");
                            Log.Error($"File input: '{inputFileLocation}' not found");
                        }
                    }
                    else
                    {
                        parserErrors.Add("'--input-file' parameter is required!");
                        Log.Error("'--input-file' parameter is required!");
                    }

                    if (!string.IsNullOrEmpty(o.PrJsonFile))
                    {
                        prJsonFileLocation = o.PrJsonFile.Trim();
                        if (!File.Exists(prJsonFileLocation))
                        {
                            parserErrors.Add($"Pr Json File: '{prJsonFileLocation}' not found");
                            Log.Error($"Pr Json File: '{prJsonFileLocation}' not found");
                        }
                    }
                    else
                    {
                        parserErrors.Add("'--pr-json-file' parameter is required!");
                        Log.Error("'--pr-json-file' parameter is required!");
                    }

                    if (!string.IsNullOrEmpty(o.OutputFile))
                    {
                        outputFileLocation = o.OutputFile.Trim();
                    }
                    else
                    {
                        parserErrors.Add("'--output-file' parameter is required!");
                        Log.Error("'--output-file' parameter is required!");
                    }
                });
            if (parserErrors.Count > 0)
            {
                throw new Exception(JsonSerializer.Serialize(parserErrors));
            }
            else
            {
                new MarkdownGeneratorHelper(inputFileLocation, prJsonFileLocation, outputFileLocation)
                    .GenerateMarkdown();
            }
        }
    }
}