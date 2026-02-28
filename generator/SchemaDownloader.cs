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
        private static readonly HttpClient httpClient = new HttpClient();

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
            if (Regex.IsMatch(version, @"^v?\d+\.\d+\.\d+$"))
            {
                if (!version.StartsWith("v"))
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
        /// Download schema files from GitHub
        /// </summary>
        public void DownloadSchema(string repository, string gitRef, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            var refDisplay = gitRef.Replace("refs/tags/", "").Replace("refs/heads/", "");
            Console.WriteLine($"  Fetching from: {repository}@{refDisplay}");

            var baseUrl = $"https://raw.githubusercontent.com/{repository}/{gitRef}/schema";
            var schemaUrl = $"{baseUrl}/schema.json";
            var metaUrl = $"{baseUrl}/meta.json";

            try
            {
                // Download schema.json
                DownloadFile(schemaUrl, Path.Combine(outputDir, "schema.json"));

                // Download meta.json
                DownloadFile(metaUrl, Path.Combine(outputDir, "meta.json"));

                // Write VERSION file
                File.WriteAllText(Path.Combine(outputDir, "VERSION"), gitRef, System.Text.Encoding.UTF8);

                Console.WriteLine("  [OK] Schema and meta files downloaded");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download schema: {ex.Message}", ex);
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
