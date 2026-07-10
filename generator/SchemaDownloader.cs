using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace dotacp.generator
{
    /// <summary>
    /// Helper for downloading ACP schema files from GitHub
    /// </summary>
    public class SchemaDownloader
    {
        private static HttpClient defaultHttpClient = new HttpClient();
        private readonly HttpClient httpClient;

        public SchemaDownloader()
            : this(defaultHttpClient)
        {
        }

        public SchemaDownloader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Resolve a version string to a git ref
        /// </summary>
        public string ResolveRef(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return "refs/heads/main";
            }

            if (version.StartsWith("refs/"))
            {
                return version;
            }

            // Check if it's a version number (with or without 'v' prefix)
            if (Regex.IsMatch(version, @"^(schema-)?v?\d+\.\d+\.\d+$"))
            {
                if (!version.StartsWith("schema-") && !version.StartsWith("v"))
                {
                    version = "v" + version;
                }
                return "refs/tags/" + version;
            }

            // Otherwise treat as branch name
            return "refs/heads/" + version;
        }

        /// <summary>
        /// Get the cached git ref from VERSION file
        /// </summary>
        public string GetCachedRef(string versionFilePath)
        {
            if (File.Exists(versionFilePath))
            {
                return File.ReadAllText(versionFilePath).Trim();
            }
            return null;
        }

        /// <summary>
        /// Download unstable schema files from GitHub.
        /// </summary>
        public void DownloadSchema(string repository, string gitRef, string outputDir)
        {
            DownloadSchemaVariant(repository, gitRef, outputDir, unstable: true);
        }

        /// <summary>
        /// Download both stable and unstable schema files from GitHub.
        /// </summary>
        public void DownloadSchemaSet(string repository, string gitRef, string stableOutputDir, string unstableOutputDir)
        {
            DownloadSchemaVariant(repository, gitRef, stableOutputDir, unstable: false);
            DownloadSchemaVariant(repository, gitRef, unstableOutputDir, unstable: true);
        }

        private void DownloadSchemaVariant(string repository, string gitRef, string outputDir, bool unstable)
        {
            Directory.CreateDirectory(outputDir);

            var refDisplay = gitRef.Replace("refs/tags/", "").Replace("refs/heads/", "");
            var variant = unstable ? "unstable" : "stable";
            Console.WriteLine($"  Fetching {variant} schema from: {repository}@{refDisplay}");

            var baseUrl = $"https://raw.githubusercontent.com/{repository}/{gitRef}/schema/v1";
            var schemaUrl = unstable ? $"{baseUrl}/schema.unstable.json" : $"{baseUrl}/schema.json";
            var metaUrl = unstable ? $"{baseUrl}/meta.unstable.json" : $"{baseUrl}/meta.json";

            try
            {
                // Download schema.json
                DownloadFile(schemaUrl, Path.Combine(outputDir, "schema.json"));

                // Download meta.json
                DownloadFile(metaUrl, Path.Combine(outputDir, "meta.json"));

                // Write VERSION file
                File.WriteAllText(Path.Combine(outputDir, "VERSION"), gitRef, System.Text.Encoding.UTF8);

                Console.WriteLine($"  [OK] {variant} schema and meta files downloaded");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download {variant} schema: {ex.Message}", ex);
            }
        }

        private void DownloadFile(string url, string outputFile)
        {
            try
            {
                var task = httpClient.GetStreamAsync(url);
                task.Wait();
                using var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None);
                task.Result.CopyTo(fileStream);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}
