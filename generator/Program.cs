using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace dotacp.generator
{
    class Program
    {
        private static Func<SchemaDownloader> _schemaDownloaderFactory = () => new SchemaDownloader();

        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("ACP Protocol Code Generator");

            // Common options
            var schemaDirOption = new Option<string>("--schema-dir")
            {
                Description = "Path to schema directory",
                DefaultValueFactory = _ => GetDefaultSchemaDir()
            };
            var outputDirOption = new Option<string>("--output-dir")
            {
                Description = "Path to output directory",
                DefaultValueFactory = _ => GetDefaultOutputDir()
            };

            var schemaCommand = new Command("schema", "Generate C# models from schema.json");
            schemaCommand.Options.Add(schemaDirOption);
            schemaCommand.Options.Add(outputDirOption);
            schemaCommand.SetAction(parseResult =>
            {
                var schemaDir = parseResult.GetValue(schemaDirOption);
                var outputDir = parseResult.GetValue(outputDirOption);
                return GenerateSchema(schemaDir, outputDir);
            });

            var metaCommand = new Command("meta", "Generate Meta.cs from meta.json");
            metaCommand.Options.Add(schemaDirOption);
            metaCommand.Options.Add(outputDirOption);
            metaCommand.SetAction(parseResult =>
            {
                var schemaDir = parseResult.GetValue(schemaDirOption);
                var outputDir = parseResult.GetValue(outputDirOption);
                return GenerateMeta(schemaDir, outputDir);
            });

            var interfacesCommand = new Command("interfaces", "Generate agent/client interfaces and connections");
            var protocolNamespaceOption = new Option<string>("--protocol-namespace")
            {
                Description = "Protocol namespace used by generated agent/client files",
                DefaultValueFactory = _ => "dotacp.protocol"
            };
            var agentNamespaceOption = new Option<string>("--agent-namespace")
            {
                Description = "Namespace used by generated agent interface/connection files",
                DefaultValueFactory = _ => "dotacp.agent"
            };
            var clientNamespaceOption = new Option<string>("--client-namespace")
            {
                Description = "Namespace used by generated client interface/connection files",
                DefaultValueFactory = _ => "dotacp.client"
            };
            var outputSubdirOption = new Option<string>("--output-subdir")
            {
                Description = "Optional subdirectory under agent/ and client/ for generated files",
                DefaultValueFactory = _ => string.Empty
            };
            interfacesCommand.Options.Add(schemaDirOption);
            interfacesCommand.Options.Add(outputDirOption);
            interfacesCommand.Options.Add(protocolNamespaceOption);
            interfacesCommand.Options.Add(agentNamespaceOption);
            interfacesCommand.Options.Add(clientNamespaceOption);
            interfacesCommand.Options.Add(outputSubdirOption);
            interfacesCommand.SetAction(parseResult =>
            {
                var schemaDir = parseResult.GetValue(schemaDirOption);
                var outputDir = parseResult.GetValue(outputDirOption);
                var protocolNamespace = parseResult.GetValue(protocolNamespaceOption);
                var agentNamespace = parseResult.GetValue(agentNamespaceOption);
                var clientNamespace = parseResult.GetValue(clientNamespaceOption);
                var outputSubdir = parseResult.GetValue(outputSubdirOption);
                return GenerateInterfaces(schemaDir, outputDir, protocolNamespace, agentNamespace, clientNamespace, outputSubdir);
            });

            var allCommand = new Command("all", "Generate all code (schema + meta + interfaces)");
            var versionOption = new Option<string>("--version")
            {
                Description = "Git ref (tag/branch) to fetch schema from"
            };
            var repoOption = new Option<string>("--repo")
            {
                Description = "Source repository",
                DefaultValueFactory = _ => "agentclientprotocol/agent-client-protocol"
            };
            var noDownloadOption = new Option<bool>("--no-download")
            {
                Description = "Skip downloading schema files"
            };
            var forceOption = new Option<bool>("--force")
            {
                Description = "Force schema download"
            };

            allCommand.Options.Add(versionOption);
            allCommand.Options.Add(repoOption);
            allCommand.Options.Add(noDownloadOption);
            allCommand.Options.Add(forceOption);
            allCommand.Options.Add(schemaDirOption);
            allCommand.Options.Add(outputDirOption);
            allCommand.SetAction(parseResult =>
            {
                var version = parseResult.GetValue(versionOption);
                var repo = parseResult.GetValue(repoOption);
                var noDownload = parseResult.GetValue(noDownloadOption);
                var force = parseResult.GetValue(forceOption);
                var schemaDir = parseResult.GetValue(schemaDirOption);
                var outputDir = parseResult.GetValue(outputDirOption);
                return GenerateAll(version, repo, noDownload, force, schemaDir, outputDir);
            });

            rootCommand.Subcommands.Add(schemaCommand);
            rootCommand.Subcommands.Add(metaCommand);
            rootCommand.Subcommands.Add(interfacesCommand);
            rootCommand.Subcommands.Add(allCommand);

            return await rootCommand.Parse(args).InvokeAsync();
        }

        static string GetDefaultRepoRoot()
        {
            var generatorDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            var repoRoot = Path.GetFullPath(Path.Combine(generatorDir, "..", "..", "..", ".."));
            return repoRoot;
        }

        static string GetDefaultSchemaDir()
        {
            return Path.Combine(GetDefaultRepoRoot(), "protocol", "schema");
        }

        static string GetDefaultOutputDir()
        {
            return Path.Combine(GetDefaultRepoRoot(), "protocol");
        }

        static int GenerateSchema(string schemaDir, string outputDir, string targetNamespace = "dotacp.protocol")
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
                var output = generator.Generate(schemaPath, versionPath, targetNamespace);

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

        static int GenerateMeta(string schemaDir, string outputDir, string targetNamespace = "dotacp.protocol")
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
                var output = generator.Generate(metaPath, versionPath, targetNamespace);

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

        static int GenerateInterfaces(string schemaDir, string outputDir, string protocolNamespace = "dotacp.protocol",
            string agentNamespace = "dotacp.agent", string clientNamespace = "dotacp.client",
            string outputSubdir = "")
        {
            try
            {
                var metaPath = Path.Combine(schemaDir, "meta.json");
                var schemaPath = Path.Combine(schemaDir, "schema.json");
                var versionPath = Path.Combine(schemaDir, "VERSION");

                if (!File.Exists(metaPath))
                {
                    Console.Error.WriteLine($"Error: meta.json not found at {metaPath}");
                    return 1;
                }

                if (!File.Exists(schemaPath))
                {
                    Console.Error.WriteLine($"Error: schema.json not found at {schemaPath}");
                    return 1;
                }

                Console.WriteLine("  Parsing meta.json and schema.json...");
                var repoRoot = ResolveRepoRoot(outputDir);
                var agentDir = Path.Combine(repoRoot, "agent");
                var clientDir = Path.Combine(repoRoot, "client");

                if (!string.IsNullOrWhiteSpace(outputSubdir))
                {
                    agentDir = Path.Combine(agentDir, outputSubdir);
                    clientDir = Path.Combine(clientDir, outputSubdir);
                }

                Directory.CreateDirectory(agentDir);
                Directory.CreateDirectory(clientDir);

                var generator = new InterfaceGenerator();
                generator.Generate(metaPath, schemaPath, versionPath, agentDir, clientDir, protocolNamespace,
                    agentNamespace, clientNamespace);

                Console.WriteLine($"  [OK] Generated interface files in agent/ and client/");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating interfaces: {ex.Message}");
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
                    var downloader = _schemaDownloaderFactory();
                    var gitRef = downloader.ResolveRef(version);
                    var unstableSchemaCacheDir = Path.Combine(schemaDir, "unstable");
                    var cachedStableRef = downloader.GetCachedRef(Path.Combine(schemaDir, "VERSION"));
                    var cachedUnstableRef = downloader.GetCachedRef(Path.Combine(unstableSchemaCacheDir, "VERSION"));

                    if (force || cachedStableRef != gitRef || cachedUnstableRef != gitRef)
                    {
                        Console.WriteLine($"Downloading ACP schema from {repo}@{gitRef.Replace("refs/tags/", "").Replace("refs/heads/", "")}...");
                        downloader.DownloadSchemaSet(repo, gitRef, schemaDir, unstableSchemaCacheDir);
                    }
                    else
                    {
                        Console.WriteLine($"Schema set {gitRef} already cached");
                    }
                }

                // Generate stable schema
                Console.WriteLine("Generating Schema.cs...");
                var schemaResult = GenerateSchema(schemaDir, outputDir);
                if (schemaResult != 0) return schemaResult;

                // Generate stable meta
                Console.WriteLine("Generating Meta.cs...");
                var metaResult = GenerateMeta(schemaDir, outputDir);
                if (metaResult != 0) return metaResult;

                // Generate unstable schema and meta
                var unstableSchemaDir = Path.Combine(schemaDir, "unstable");
                var unstableOutputDir = Path.Combine(outputDir, "unstable");
                Directory.CreateDirectory(unstableOutputDir);

                Console.WriteLine("Generating unstable/Schema.cs...");
                var unstableSchemaResult = GenerateSchema(unstableSchemaDir, unstableOutputDir, "dotacp.protocol.unstable");
                if (unstableSchemaResult != 0) return unstableSchemaResult;

                Console.WriteLine("Generating unstable/Meta.cs...");
                var unstableMetaResult = GenerateMeta(unstableSchemaDir, unstableOutputDir, "dotacp.protocol.unstable");
                if (unstableMetaResult != 0) return unstableMetaResult;

                // Generate stable interfaces
                Console.WriteLine("Generating interfaces...");
                var interfacesResult = GenerateInterfaces(schemaDir, outputDir);
                if (interfacesResult != 0) return interfacesResult;

                // Generate unstable interfaces from unstable protocol for local testing
                Console.WriteLine("Generating unstable interfaces...");
                var unstableInterfacesResult = GenerateInterfaces(
                    unstableSchemaDir,
                    outputDir,
                    "dotacp.protocol.unstable",
                    "dotacp.agent.unstable",
                    "dotacp.client.unstable",
                    "unstable");
                if (unstableInterfacesResult != 0) return unstableInterfacesResult;

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

        private static string ResolveRepoRoot(string outputDir)
        {
            if (string.IsNullOrWhiteSpace(outputDir))
                return GetDefaultRepoRoot();

            var candidate = outputDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var agentPath = Path.Combine(candidate, "agent");
            var clientPath = Path.Combine(candidate, "client");

            if (Directory.Exists(agentPath) && Directory.Exists(clientPath))
                return candidate;

            if (string.Equals(Path.GetFileName(candidate), "protocol", StringComparison.OrdinalIgnoreCase))
                return Path.GetFullPath(Path.Combine(candidate, ".."));

            return candidate;
        }

        private static void WriteText(string text, string outputPath)
        {
            var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            File.WriteAllText(outputPath, text, utf8NoBom);
        }
    }
}
