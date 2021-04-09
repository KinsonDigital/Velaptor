// <copyright file="SoundPathResolverTests.cs" company="KinsonDigital">
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
    /// Tests the <see cref="SoundPathResolver"/> class.
    /// </summary>
    public class SoundPathResolverTests
    {
        private const string ContentName = "test-content";
        private readonly string contentFilePath;
        private readonly string baseDir;
        private readonly string baseContentDir;
        private readonly string atlasContentDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundPathResolverTests"/> class.
        /// </summary>
        public SoundPathResolverTests()
        {
            this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            this.baseContentDir = $@"{this.baseDir}Content\";
            this.atlasContentDir = $@"{this.baseContentDir}Sounds\";
            this.contentFilePath = $"{this.atlasContentDir}{ContentName}";
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var source = new SoundPathResolver(mockDirectory.Object);
            var actual = source.ContentDirectoryName;

            // Assert
            Assert.Equal("Sounds", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.baseDir}other-file-A.png",
                        $"{this.baseDir}other-file-B.txt",
                    };
                });

            var resolver = new SoundPathResolver(mockDirectory.Object);

            // Act & Assert
            Assert.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                resolver.ResolveFilePath(ContentName);
            }, $"The sound file '{this.contentFilePath}' does not exist.");
        }

        [Theory]
        [InlineData(".ogg", ".OGG")]
        [InlineData(".ogg", ".ogg")]
        [InlineData(".OGG", ".ogg")]
        [InlineData(".mp3", ".ogg")]
        [InlineData(".mp3", ".OGG")]
        [InlineData(".MP3", ".ogg")]
        [InlineData(".MP3", ".OGG")]
        public void ResolveFilePath_WhenBothOggAndMp3FilesExist_ResolvesToOggFile(string resolvePathExtension, string actualFileExtension)
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.txt",
                        $"{this.contentFilePath}{actualFileExtension}",
                        $"{this.contentFilePath}.mp3",
                    };
                });

            var resolver = new SoundPathResolver(mockDirectory.Object);

            // Act
            var actual = resolver.ResolveFilePath($"{ContentName}{resolvePathExtension}");

            // Assert
            Assert.Equal($"{this.contentFilePath}{actualFileExtension}", actual);
        }

        [Fact]
        public void ResolveFilePath_WhenOnlyMp3FilesExist_ResolvesToMp3File()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();
            mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
                .Returns(() =>
                {
                    return new[]
                    {
                        $"{this.atlasContentDir}other-file.txt",
                        $"{this.contentFilePath}.mp3",
                    };
                });

            var resolver = new SoundPathResolver(mockDirectory.Object);

            // Act
            var actual = resolver.ResolveFilePath($"{ContentName}.mp3");

            // Assert
            Assert.Equal($"{this.contentFilePath}.mp3", actual);
        }
        #endregion
    }
}
