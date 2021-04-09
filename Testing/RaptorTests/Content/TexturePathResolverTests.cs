// <copyright file="TexturePathResolverTests.cs" company="KinsonDigital">
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
    using Assert = RaptorTests.Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="TexturePathResolver"/> class.
    /// </summary>
    public class TexturePathResolverTests
    {
        private const string ContentName = "test-content";
        private readonly string contentFilePath;
        private readonly string baseDir;
        private readonly string baseContentDir;
        private readonly string atlasContentDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturePathResolverTests"/> class.
        /// </summary>
        public TexturePathResolverTests()
        {
            this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            this.baseContentDir = $@"{this.baseDir}Content\";
            this.atlasContentDir = $@"{this.baseContentDir}Graphics\";
            this.contentFilePath = $"{this.atlasContentDir}{ContentName}.png";
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var resolver = new TexturePathResolver(mockDirectory.Object);
            var actual = resolver.FileDirectoryName;

            // Assert
            Assert.Equal("Graphics", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.png"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.baseDir}other-file-A.png",
                        $"{this.baseDir}other-file-B.txt",
                    };
                });

            var resolver = new TexturePathResolver(mockDirectory.Object);

            // Act & Assert
            Assert.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                resolver.ResolveFilePath(ContentName);
            }, $"The texture image file '{this.contentFilePath}' does not exist.");
        }

        [Theory]
        [InlineData("test-content")]
        [InlineData("test-content.png")]
        public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.png"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.png",
                        this.contentFilePath,
                    };
                });

            var resolver = new TexturePathResolver(mockDirectory.Object);

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(this.contentFilePath, actual);
        }
        #endregion
    }
}
