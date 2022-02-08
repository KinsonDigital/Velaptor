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
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
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
        private const uint OggSoundId = 1234u;
        private const uint Mp3SoundId = 5678u;
        private readonly string oggSoundFilePath;
        private readonly string mp3SoundFilePath;
        private readonly Mock<IPathResolver> mockSoundPathResolver;
        private readonly Mock<ISoundFactory> soundFactory;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<IReactable<DisposeSoundData>> mockDisposeSoundReactable;
        private readonly Mock<ISound> mockOggSound;
        private readonly Mock<ISound> mockMp3Sound;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.oggSoundFilePath = $"{SoundDirPath}{SoundName}{OggFileExtension}";
            this.mp3SoundFilePath = $"{SoundDirPath}{SoundName}{Mp3FileExtension}";

            this.mockSoundPathResolver = new Mock<IPathResolver>();

            this.mockOggSound = new Mock<ISound>();
            this.mockOggSound.SetupGet(p => p.FilePath).Returns(this.oggSoundFilePath);
            this.mockOggSound.SetupGet(p => p.Id).Returns(OggSoundId);

            this.mockMp3Sound = new Mock<ISound>();
            this.mockMp3Sound.SetupGet(p => p.FilePath).Returns(this.mp3SoundFilePath);
            this.mockMp3Sound.SetupGet(p => p.Id).Returns(Mp3SoundId);

            this.soundFactory = new Mock<ISoundFactory>();
            this.soundFactory.Setup(m => m.Create(this.oggSoundFilePath)).Returns(this.mockOggSound.Object);
            this.soundFactory.Setup(m => m.Create(this.mp3SoundFilePath)).Returns(this.mockMp3Sound.Object);

            this.mockFile = new Mock<IFile>();
            this.mockPath = new Mock<IPath>();

            this.mockDisposeSoundReactable = new Mock<IReactable<DisposeSoundData>>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullSoundPathResolverParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    null,
                    this.soundFactory.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object);
            }, "The parameter must not be null. (Parameter 'soundPathResolver')");
        }

        [Fact]
        public void Ctor_WithNullSoundFactoryParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    null,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object);
            }, "The parameter must not be null. (Parameter 'soundFactory')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    this.soundFactory.Object,
                    null,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    this.soundFactory.Object,
                    this.mockFile.Object,
                    null,
                    this.mockDisposeSoundReactable.Object);
            }, "The parameter must not be null. (Parameter 'path')");
        }

        [Fact]
        public void Ctor_WithDisposeSoundReactorParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    this.soundFactory.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'disposeSoundReactable')");
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
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            this.soundFactory.Verify(m => m.Create(this.oggSoundFilePath), Times.Once());
            Assert.Equal(actual.FilePath, this.oggSoundFilePath);
            this.mockOggSound.VerifyGet(p => p.FilePath, Times.Once);
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
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            this.soundFactory.Verify(m => m.Create(this.mp3SoundFilePath), Times.Once());
            Assert.Equal(actual.FilePath, this.mp3SoundFilePath);
            this.mockMp3Sound.VerifyGet(p => p.FilePath, Times.Once);
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
            var disposeData = new DisposeSoundData(OggSoundId);
            this.mockDisposeSoundReactable.Verify(m =>
                m.PushNotification(disposeData, false), Times.Once);
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
            var disposeData = new DisposeSoundData(OggSoundId);
            this.mockDisposeSoundReactable.Verify(m =>
                m.PushNotification(disposeData, false), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The mockSound loader instance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new (
            this.mockSoundPathResolver.Object,
            this.soundFactory.Object,
            this.mockFile.Object,
            this.mockPath.Object,
            this.mockDisposeSoundReactable.Object);
    }
}
