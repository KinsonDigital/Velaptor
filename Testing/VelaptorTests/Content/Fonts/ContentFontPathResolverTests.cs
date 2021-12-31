// <copyright file="ContentFontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using Moq;
    using Velaptor.Content.Fonts;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentFontPathResolver"/> class.
    /// </summary>
    public class ContentFontPathResolverTests
    {
        private const string ContentName = "test-content";
        private readonly string contentFilePath;
        private readonly string baseDir;
        private readonly string atlasContentDir;
        private readonly Mock<IDirectory> mockDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFontPathResolverTests"/> class.
        /// </summary>
        public ContentFontPathResolverTests()
        {
            this.mockDirectory = new Mock<IDirectory>();
            this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            var baseContentDir = $@"{this.baseDir}Content\";
            this.atlasContentDir = $@"{baseContentDir}Fonts\";
            this.contentFilePath = $"{this.atlasContentDir}{ContentName}.ttf";
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal("Fonts", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.ttf"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.baseDir}other-file-A.ttf",
                        $"{this.baseDir}other-file-B.txt",
                    };
                });

            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                resolver.ResolveFilePath(ContentName);
            }, $"The font file '{this.contentFilePath}' does not exist.");
        }

        [Theory]
        [InlineData("test-content")]
        [InlineData("test-content.ttf")]
        [InlineData("TEST-CONTENT.ttf")]
        public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
        {
            // Arrange
            this.mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.ttf"))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.txt",
                        this.contentFilePath,
                    };
                });

            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(this.contentFilePath, actual);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ContentFontPathResolver"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private ContentFontPathResolver CreateResolver() => new (this.mockDirectory.Object);
    }
}
