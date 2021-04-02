// <copyright file="FontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using Moq;
    using Raptor.Content;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontPathResolver"/> class.
    /// </summary>
    public class FontPathResolverTests
    {
        private const string ContentName = "test-content";
        private readonly string contentFilePath;
        private readonly string baseDir;
        private readonly string baseContentDir;
        private readonly string atlasContentDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontPathResolverTests"/> class.
        /// </summary>
        public FontPathResolverTests()
        {
            this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            this.baseContentDir = $@"{this.baseDir}Content\";
            this.atlasContentDir = $@"{this.baseContentDir}Fonts\";
            this.contentFilePath = $"{this.atlasContentDir}{ContentName}.ttf";
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var resolver = new FontPathResolver(mockDirectory.Object);
            var actual = resolver.FileDirectoryName;

            // Assert
            Assert.Equal("Fonts", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.ttf"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.baseDir}other-file-A.ttf",
                        $"{this.baseDir}other-file-B.txt",
                    };
                });

            var resolver = new FontPathResolver(mockDirectory.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                resolver.ResolveFilePath(ContentName);
            }, $"The font file '{this.contentFilePath}' does not exist.");
        }

        [Theory]
        [InlineData("test-content")]
        [InlineData("test-content.ttf")]
        public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.ttf"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.txt",
                        this.contentFilePath,
                    };
                });

            var resolver = new FontPathResolver(mockDirectory.Object);

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(this.contentFilePath, actual);
        }
        #endregion
    }
}
