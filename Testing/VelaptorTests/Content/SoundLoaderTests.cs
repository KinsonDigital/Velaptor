// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundLoader"/> class.
    /// </summary>
    public class SoundLoaderTests
    {
        private const string OggFileExtension = ".ogg";
        private const string Mp3FileExtension = ".mp3";
        private const string SoundDirPath = @"C:\temp\Content\Sounds\";
        private const string SoundName = "test-sound";
        private readonly string oggSoundFilePath;
        private readonly string mp3SoundFilePath;
        private readonly Mock<IItemCache<string, ISound>> mockSoundCache;
        private readonly Mock<IPathResolver> mockSoundPathResolver;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPath> mockPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.oggSoundFilePath = $"{SoundDirPath}{SoundName}{OggFileExtension}";
            this.mp3SoundFilePath = $"{SoundDirPath}{SoundName}{Mp3FileExtension}";

            this.mockSoundCache = new Mock<IItemCache<string, ISound>>();
            this.mockSoundPathResolver = new Mock<IPathResolver>();

            this.mockFile = new Mock<IFile>();
            this.mockPath = new Mock<IPath>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullSoundCacheParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    null,
                    this.mockSoundPathResolver.Object,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'soundCache')");
        }

        [Fact]
        public void Ctor_WithNullSoundPathResolverParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundCache.Object,
                    null,
                    this.mockFile.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'soundPathResolver')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundCache.Object,
                    this.mockSoundPathResolver.Object,
                    null,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundCache.Object,
                    this.mockSoundPathResolver.Object,
                    this.mockFile.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'path')");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Load_WithInvalidExtensionForFullFilePath_ThrowsException()
        {
            // Arrange
            const string invalidExtension = ".txt";
            var invalidSoundFilePath = $"{SoundDirPath}{SoundName}{invalidExtension}";
            this.mockFile.Setup(m => m.Exists(invalidSoundFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(invalidSoundFilePath)).Returns(invalidExtension);

            var expectedMsg = $"The file '{invalidSoundFilePath}' must be a sound file with";
            expectedMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

            var loader = CreateSoundLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadSoundException>(() =>
            {
                loader.Load(invalidSoundFilePath);
            }, expectedMsg);
        }

        [Fact]
        public void Load_WhenSoundFileDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(false);

            var expectedMsg = $"The sound file does not exist.";

            var loader = CreateSoundLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                loader.Load(this.oggSoundFilePath);
            }, expectedMsg);
        }

        [Theory]
        [InlineData(SoundName, "")]
        [InlineData(SoundName, ".txt")]
        public void Load_WhenLoadingOggSoundByName_LoadsOggSound(string contentName, string extension)
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}"))
                .Returns(contentName);
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(SoundName)).Returns(this.oggSoundFilePath);

            var loader = CreateSoundLoader();

            // Act
            loader.Load($"{contentName}{extension}");

            // Assert
            this.mockSoundCache.Verify(m => m.GetItem(this.oggSoundFilePath), Times.Once);
        }

        [Theory]
        [InlineData(SoundName, "")]
        [InlineData(SoundName, ".txt")]
        public void Load_WhenLoadingMp3SoundByName_LoadsMp3Sound(string contentName, string extension)
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.mp3SoundFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(this.mp3SoundFilePath)).Returns(Mp3FileExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}"))
                .Returns(contentName);
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(SoundName)).Returns(this.mp3SoundFilePath);

            var loader = CreateSoundLoader();

            // Act
            loader.Load($"{contentName}{extension}");

            // Assert
            this.mockSoundCache.Verify(m => m.GetItem(this.mp3SoundFilePath), Times.Once);
        }

        [Fact]
        public void Unload_WhenUnloadingUsingContentName_UnloadsSound()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(SoundName)).Returns(SoundName);
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(SoundName)).Returns(this.oggSoundFilePath);

            var loader = CreateSoundLoader();
            loader.Load(SoundName);

            // Act
            loader.Unload(SoundName);

            // Assert
            this.mockSoundCache.Verify(m => m.Unload(this.oggSoundFilePath), Times.Once);
        }

        [Fact]
        public void Unload_WhenUnloadingUsingFullDirectPath_UnloadsSound()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(this.oggSoundFilePath)).Returns(this.oggSoundFilePath);

            var loader = CreateSoundLoader();
            loader.Load(this.oggSoundFilePath);

            // Act
            loader.Unload(this.oggSoundFilePath);

            // Assert
            this.mockSoundCache.Verify(m => m.Unload(this.oggSoundFilePath), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The mockSound loader instance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new (
            this.mockSoundCache.Object,
            this.mockSoundPathResolver.Object,
            this.mockFile.Object,
            this.mockPath.Object);
    }
}
