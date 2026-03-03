using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;

namespace dotacp.unittest
{
    [TestClass]
    public class SchemaDownloaderTests
    {
        [TestMethod]
        public void ResolveRef_WithNull_ReturnsMain()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef(null);

            // Assert
            Assert.AreEqual("refs/heads/main", result);
        }

        [TestMethod]
        public void ResolveRef_WithEmptyString_ReturnsMain()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("");

            // Assert
            Assert.AreEqual("refs/heads/main", result);
        }

        [TestMethod]
        public void ResolveRef_WithRefsPrefix_ReturnsAsIs()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("refs/tags/v1.0.0");

            // Assert
            Assert.AreEqual("refs/tags/v1.0.0", result);
        }

        [TestMethod]
        public void ResolveRef_WithVersionWithV_ReturnsTag()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("v1.2.3");

            // Assert
            Assert.AreEqual("refs/tags/v1.2.3", result);
        }

        [TestMethod]
        public void ResolveRef_WithVersionWithoutV_ReturnsTagWithV()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("1.2.3");

            // Assert
            Assert.AreEqual("refs/tags/v1.2.3", result);
        }

        [TestMethod]
        public void ResolveRef_WithBranchName_ReturnsBranch()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("main");

            // Assert
            Assert.AreEqual("refs/heads/main", result);
        }

        [TestMethod]
        public void ResolveRef_WithDevelopBranch_ReturnsBranch()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("develop");

            // Assert
            Assert.AreEqual("refs/heads/develop", result);
        }

        [TestMethod]
        public void ResolveRef_WithFeatureBranch_ReturnsBranch()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result = downloader.ResolveRef("feature/new-thing");

            // Assert
            Assert.AreEqual("refs/heads/feature/new-thing", result);
        }

        [TestMethod]
        public void GetCachedRef_WithExistingFile_ReturnsContent()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var versionFile = Path.Combine(tempDir, "VERSION");
                File.WriteAllText(versionFile, "v0.10.8");

                var downloader = new SchemaDownloader();

                // Act
                var result = downloader.GetCachedRef(versionFile);

                // Assert
                Assert.AreEqual("v0.10.8", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetCachedRef_WithNonexistentFile_ReturnsNull()
        {
            // Arrange
            var downloader = new SchemaDownloader();
            var nonexistentFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "VERSION");

            // Act
            var result = downloader.GetCachedRef(nonexistentFile);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCachedRef_WithWhitespace_ReturnsTrimmed()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var versionFile = Path.Combine(tempDir, "VERSION");
                File.WriteAllText(versionFile, "  v1.0.0  \n");

                var downloader = new SchemaDownloader();

                // Act
                var result = downloader.GetCachedRef(versionFile);

                // Assert
                Assert.AreEqual("v1.0.0", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void DownloadSchema_CreatesOutputDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var downloader = new SchemaDownloader();

            // Note: This test will actually try to download from GitHub
            // Skip if no internet connection or to avoid hitting GitHub
            try
            {
                // Act
                downloader.DownloadSchema(
                    "agentclientprotocol/agent-client-protocol",
                    "refs/heads/main",
                    tempDir);

                // Assert
                Assert.IsTrue(Directory.Exists(tempDir));
                Assert.IsTrue(File.Exists(Path.Combine(tempDir, "schema.json")));
                Assert.IsTrue(File.Exists(Path.Combine(tempDir, "meta.json")));
                Assert.IsTrue(File.Exists(Path.Combine(tempDir, "VERSION")));

                var versionContent = File.ReadAllText(Path.Combine(tempDir, "VERSION"));
                Assert.AreEqual("refs/heads/main", versionContent);
            }
            catch (Exception ex) when (ex.InnerException is HttpRequestException || ex is HttpRequestException)
            {
                // Skip test if network is unavailable
                Assert.Inconclusive("Network unavailable or GitHub unreachable");
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void DownloadSchema_WithInvalidUrl_ThrowsException()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var downloader = new SchemaDownloader();

            try
            {
                // Act & Assert
                Exception? caughtException = null;
                try
                {
                    downloader.DownloadSchema(
                        "invalid/nonexistent-repo",
                        "refs/heads/nonexistent",
                        tempDir);
                    Assert.Fail("Expected exception was not thrown");
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }

                Assert.IsNotNull(caughtException);
                Assert.Contains("Failed to download schema", caughtException.Message);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void ResolveRef_WithComplexVersion_ReturnsCorrectTag()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result1 = downloader.ResolveRef("v10.20.30");
            var result2 = downloader.ResolveRef("0.1.0");

            // Assert
            Assert.AreEqual("refs/tags/v10.20.30", result1);
            Assert.AreEqual("refs/tags/v0.1.0", result2);
        }

        [TestMethod]
        public void ResolveRef_WithNonVersionString_ReturnsBranch()
        {
            // Arrange
            var downloader = new SchemaDownloader();

            // Act
            var result1 = downloader.ResolveRef("not-a-version");
            var result2 = downloader.ResolveRef("v1.2");  // Not x.y.z format
            var result3 = downloader.ResolveRef("1.2");   // Not x.y.z format

            // Assert
            Assert.AreEqual("refs/heads/not-a-version", result1);
            Assert.AreEqual("refs/heads/v1.2", result2);
            Assert.AreEqual("refs/heads/1.2", result3);
        }
    }
}
