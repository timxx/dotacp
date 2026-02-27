using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotacp.unittest
{
    /// <summary>
    /// Integration test that runs Pester tests for the PowerShell schema generation script.
    /// This ensures PowerShell script logic is tested as part of the regular test suite.
    /// </summary>
    [TestClass]
    public class PesterTestRunner
    {
        [TestMethod]
        public void RunPesterTests_SchemaGeneration()
        {
            // Get the path to the Pester test file
            string testFilePath = GetPesterTestPath();

            // Run Pester tests and capture output
            var result = RunPesterTestsWithPowerShell(testFilePath);

            // Assert that all tests passed
            Assert.IsTrue(result.AllTestsPassed,
                $"Pester tests failed.{Environment.NewLine}{result.Output}");
        }

        private string GetPesterTestPath()
        {
            // Get the solution root directory
            string solutionRoot = GetSolutionRoot();
            string testPath = System.IO.Path.Combine(solutionRoot, "protocol", "scripts", "gen_schema.Tests.ps1");

            if (!System.IO.File.Exists(testPath))
            {
                throw new System.IO.FileNotFoundException($"Pester test file not found at {testPath}");
            }

            return testPath;
        }

        private string GetSolutionRoot()
        {
            // Start from current directory and walk up to find .sln file
            string currentDir = System.IO.Directory.GetCurrentDirectory();

            while (currentDir != null)
            {
                string[] slnFiles = System.IO.Directory.GetFiles(currentDir, "*.sln");
                if (slnFiles.Length > 0)
                {
                    return currentDir;
                }

                currentDir = System.IO.Directory.GetParent(currentDir)?.FullName;
            }

            throw new InvalidOperationException("Could not find solution root (.sln file)");
        }

        private PesterResult RunPesterTestsWithPowerShell(string testFilePath)
        {
            string psScript = $@"
$ErrorActionPreference = 'Stop'
try {{
    $result = Invoke-Pester -Path '{testFilePath}' -PassThru -Quiet
    
    if ($result.FailedCount -eq 0) {{
        Write-Host 'PESTER_SUCCESS'
        exit 0
    }} else {{
        Write-Host 'PESTER_FAILURE'
        Write-Host ""Passed: $($result.PassedCount) Failed: $($result.FailedCount)""
        exit 1
    }}
}} catch {{
    Write-Host 'PESTER_ERROR'
    Write-Host ""Error: $_""
    exit 1
}}
";

            var processInfo = new ProcessStartInfo
            {
                FileName = "pwsh",
                Arguments = $"-NoProfile -NonInteractive -Command \"{psScript.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                string fullOutput = $"{output}{error}";
                bool success = output.Contains("PESTER_SUCCESS") || process.ExitCode == 0;

                return new PesterResult
                {
                    AllTestsPassed = success,
                    Output = fullOutput
                };
            }
        }

        private class PesterResult
        {
            public bool AllTestsPassed { get; set; }
            public string Output { get; set; }
        }
    }
}
