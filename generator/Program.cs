using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace dotacp.generator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("ACP Protocol Code Generator");

            // Common options
            var schemaDirOption = new Option<string>("--schema-dir", GetDefaultSchemaDir, "Path to schema directory");
            var outputDirOption = new Option<string>("--output-dir", GetDefaultOutputDir, "Path to output directory");

            var schemaCommand = new Command("schema", "Generate C# models from schema.json");
            schemaCommand.AddOption(schemaDirOption);
            schemaCommand.AddOption(outputDirOption);
            schemaCommand.SetHandler((string schemaDir, string outputDir) => GenerateSchema(schemaDir, outputDir), 
                schemaDirOption, outputDirOption);

            var metaCommand = new Command("meta", "Generate Meta.cs from meta.json");
            metaCommand.AddOption(schemaDirOption);
            metaCommand.AddOption(outputDirOption);
            metaCommand.SetHandler((string schemaDir, string outputDir) => GenerateMeta(schemaDir, outputDir),
                schemaDirOption, outputDirOption);

            var allCommand = new Command("all", "Generate all code (schema + meta)");
            var versionOption = new Option<string>("--version", "Git ref (tag/branch) to fetch schema from");
            var repoOption = new Option<string>("--repo", () => "agentclientprotocol/agent-client-protocol", "Source repository");
            var noDownloadOption = new Option<bool>("--no-download", "Skip downloading schema files");
            var forceOption = new Option<bool>("--force", "Force schema download");
            
            allCommand.AddOption(versionOption);
            allCommand.AddOption(repoOption);
            allCommand.AddOption(noDownloadOption);
            allCommand.AddOption(forceOption);
            allCommand.AddOption(schemaDirOption);
            allCommand.AddOption(outputDirOption);
            allCommand.SetHandler((string version, string repo, bool noDownload, bool force, string schemaDir, string outputDir) => 
                GenerateAll(version, repo, noDownload, force, schemaDir, outputDir), 
                versionOption, repoOption, noDownloadOption, forceOption, schemaDirOption, outputDirOption);

            rootCommand.AddCommand(schemaCommand);
            rootCommand.AddCommand(metaCommand);
            rootCommand.AddCommand(allCommand);

            return await rootCommand.InvokeAsync(args);
        }

        static string GetDefaultSchemaDir()
        {
            var generatorDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            var repoRoot = Path.GetFullPath(Path.Combine(generatorDir, "..", ".."));
            return Path.Combine(repoRoot, "protocol", "schema");
        }

        static string GetDefaultOutputDir()
        {
            var generatorDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            var repoRoot = Path.GetFullPath(Path.Combine(generatorDir, "..", ".."));
            return Path.Combine(repoRoot, "protocol");
        }

        static int GenerateSchema(string schemaDir, string outputDir)
        {
            try
            {
                var schemaPath = Path.Combine(schemaDir, "schema.json");
                var versionPath = Path.Combine(schemaDir, "VERSION");
                var outputPath = Path.Combine(outputDir, "Schema.cs");

                if (!File.Exists(schemaPath))
                {
                    Console.Error.WriteLine($"Error: schema.json not found at {schemaPath}");
                    return 1;
                }

                Console.WriteLine("  Parsing schema.json...");
                var generator = new SchemaGenerator();
                var output = generator.Generate(schemaPath, versionPath);

                WriteText(output, outputPath);
                Console.WriteLine($"  [OK] Generated C# models at {outputPath}");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating schema: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        static int GenerateMeta(string schemaDir, string outputDir)
        {
            try
            {
                var metaPath = Path.Combine(schemaDir, "meta.json");
                var versionPath = Path.Combine(schemaDir, "VERSION");
                var outputPath = Path.Combine(outputDir, "Meta.cs");

                if (!File.Exists(metaPath))
                {
                    Console.Error.WriteLine($"Error: meta.json not found at {metaPath}");
                    return 1;
                }

                Console.WriteLine("  Parsing meta.json...");
                var generator = new MetaGenerator();
                var output = generator.Generate(metaPath, versionPath);

                WriteText(output, outputPath);
                Console.WriteLine($"  [OK] Generated meta definitions at {outputPath}");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating meta: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        static int GenerateAll(string version, string repo, bool noDownload, bool force, string schemaDir, string outputDir)
        {
            try
            {
                // Handle schema download if needed
                if (!string.IsNullOrEmpty(version) && !noDownload)
                {
                    var downloader = new SchemaDownloader();
                    var gitRef = downloader.ResolveRef(version);
                    var cachedRef = downloader.GetCachedRef(Path.Combine(schemaDir, "VERSION"));

                    if (force || cachedRef != gitRef)
                    {
                        Console.WriteLine($"Downloading ACP schema from {repo}@{gitRef.Replace("refs/tags/", "").Replace("refs/heads/", "")}...");
                        downloader.DownloadSchema(repo, gitRef, schemaDir);
                    }
                    else
                    {
                        Console.WriteLine($"Schema {gitRef} already cached");
                    }
                }

                // Generate schema
                Console.WriteLine("Generating Schema.cs...");
                var schemaResult = GenerateSchema(schemaDir, outputDir);
                if (schemaResult != 0) return schemaResult;

                // Generate meta
                Console.WriteLine("Generating Meta.cs...");
                var metaResult = GenerateMeta(schemaDir, outputDir);
                if (metaResult != 0) return metaResult;

                Console.WriteLine("Code generation complete!");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating code: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        private static void WriteText(string text, string outputPath)
        {
            var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            File.WriteAllText(outputPath, text, utf8NoBom);
        }
    }
}
