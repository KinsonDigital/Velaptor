// <copyright file="AtlasJSONDataPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using Moq;
    using Raptor.Content;
    using Xunit;
    using Assert = Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="AtlasJSONDataPathResolver"/> class.
    /// </summary>
    public class AtlasJSONDataPathResolverTests
    {
        private const string ContentName = "test-content";
        private readonly string contentFilePath;
        private readonly string baseDir;
        private readonly string baseContentDir;
        private readonly string atlasContentDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasJSONDataPathResolverTests"/> class.
        /// </summary>
        public AtlasJSONDataPathResolverTests()
        {
            this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            this.baseContentDir = $@"{this.baseDir}Content\";
            this.atlasContentDir = $@"{this.baseContentDir}Atlas\";
            this.contentFilePath = $"{this.atlasContentDir}{ContentName}.json";
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var resolver = new AtlasJSONDataPathResolver(mockDirectory.Object);
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal("Atlas", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.json"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.baseDir}other-file-A.json",
                        $"{this.baseDir}other-file-B.txt",
                    };
                });

            var resolver = new AtlasJSONDataPathResolver(mockDirectory.Object);

            // Act & Assert
            Assert.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                resolver.ResolveFilePath(ContentName);
            }, $"The texture atlas data file '{this.contentFilePath}' does not exist.");
        }

        [Theory]
        [InlineData("test-content")]
        [InlineData("test-content.json")]
        [InlineData("TEST-CONTENT.json")]
        public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.json"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.json",
                        this.contentFilePath,
                    };
                });

            var resolver = new AtlasJSONDataPathResolver(mockDirectory.Object);

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(this.contentFilePath, actual);
        }
        #endregion
    }
}
