// <copyright file="SoundCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundCache"/> class.
    /// </summary>
    public class SoundCacheTests
    {
        private const string OggFileExtension = ".ogg";
        private const string Mp3FileExtension = ".mp3";
        private const string SoundDirPath = @"C:\sounds\";
        private const string SoundName = "test-sound";
        private readonly string oggSoundFilePath = $"{SoundDirPath}{SoundName}{OggFileExtension}";
        private readonly string mp3SoundFilePath = $"{SoundDirPath}{SoundName}{Mp3FileExtension}";
        private readonly Mock<IDisposable> mockShutDownUnsubscriber;
        private readonly Mock<ISoundFactory> mockSoundFactory;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<IReactable<DisposeSoundData>> mockDisposeSoundReactable;
        private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCacheTests"/> class.
        /// </summary>
        public SoundCacheTests()
        {
            this.mockSoundFactory = new Mock<ISoundFactory>();
            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
            this.mockFile.Setup(m => m.Exists(this.mp3SoundFilePath)).Returns(true);

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
            this.mockPath.Setup(m => m.GetExtension(this.mp3SoundFilePath)).Returns(Mp3FileExtension);

            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>> { Name = nameof(this.mockDisposeSoundReactable) };

            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownUnsubscriber.Name = nameof(this.mockShutDownUnsubscriber);

            this.mockDisposeSoundReactable = new Mock<IReactable<DisposeSoundData>>();
            this.mockDisposeSoundReactable.Name = nameof(this.mockDisposeSoundReactable);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullSoundFactoryParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundCache(
                    null,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'soundFactory')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundCache(
                    this.mockSoundFactory.Object,
                    null,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundCache(
                    this.mockSoundFactory.Object,
                    this.mockFile.Object,
                    null,
                    this.mockDisposeSoundReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'path')");
        }

        [Fact]
        public void Ctor_WithNullDisposeSoundsReactorParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundCache(
                    this.mockSoundFactory.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    null,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'disposeSoundsReactable')");
        }

        [Fact]
        public void Ctor_WithNullShutDownReactableParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundCache(
                    this.mockSoundFactory.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockDisposeSoundReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var cache = CreateCache();
            cache.GetItem(this.oggSoundFilePath);

            // Act
            var actual = cache.TotalCachedItems;

            // Assert
            Assert.Equal(1, actual);
        }

        [Fact]
        public void CacheKeys_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var cache = CreateCache();
            cache.GetItem(this.oggSoundFilePath);

            // Act
            var actual = cache.CacheKeys;

            // Assert
            Assert.Single(actual);
            Assert.Equal(this.oggSoundFilePath, actual[0]);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetItem_WithNullOrEmptySoundFilePath_ThrowsException(string filePath)
        {
            // Arrange
            var cache = CreateCache();

            // Act, & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                cache.GetItem(filePath);
            }, "The parameter must not be null or empty. (Parameter 'soundFilePath')");
        }

        [Fact]
        public void GetItem_WithUnsupportedFileType_ThrowsException()
        {
            // Arrange
            const string dirPath = @"C:\my-sounds\";
            const string soundName = "test-sound";
            const string invalidExtension = ".txt";
            var soundFilePath = $"{dirPath}{soundName}{invalidExtension}";
            var exceptionMsg = $"Sound file type '{invalidExtension}' is not supported.";
            exceptionMsg += $"\nSupported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

            this.mockPath.Setup(m => m.GetExtension(soundFilePath)).Returns(invalidExtension);

            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadSoundException>(() =>
            {
                cache.GetItem(soundFilePath);
            }, exceptionMsg);
        }

        [Fact]
        public void GetItem_WhenOggFileDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
            this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(false);
            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                cache.GetItem(this.oggSoundFilePath);
            }, $"The '{OggFileExtension}' sound file does not exist.");
        }

        [Fact]
        public void GetItem_WhenMp3FileDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockPath.Setup(m => m.GetExtension(this.mp3SoundFilePath)).Returns(Mp3FileExtension);
            this.mockFile.Setup(m => m.Exists(this.mp3SoundFilePath)).Returns(false);
            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                cache.GetItem(this.mp3SoundFilePath);
            }, $"The '{Mp3FileExtension}' sound file does not exist.");
        }

        [Fact]
        public void GetItem_WhenGettingSound_ReturnsSound()
        {
            // Arrange
            var mockMp3Sound = new Mock<ISound>();
            mockMp3Sound.Name = nameof(mockMp3Sound);
            mockMp3Sound.SetupGet(p => p.FilePath).Returns(this.mp3SoundFilePath);
            mockMp3Sound.SetupGet(p => p.Id).Returns(123u);

            var mockOggSound = new Mock<ISound>();
            mockOggSound.Name = nameof(mockOggSound);
            mockOggSound.SetupGet(p => p.FilePath).Returns(this.oggSoundFilePath);
            mockOggSound.SetupGet(p => p.Id).Returns(456u);

            this.mockSoundFactory.Setup(m => m.Create(this.mp3SoundFilePath))
                .Returns(mockMp3Sound.Object);
            this.mockSoundFactory.Setup(m => m.Create(this.oggSoundFilePath))
                .Returns(mockOggSound.Object);

            var cache = CreateCache();

            // Act
            var mp3Sound = cache.GetItem(this.mp3SoundFilePath);
            var oggSound = cache.GetItem(this.oggSoundFilePath);

            // Assert
            Assert.NotNull(mp3Sound);
            Assert.NotNull(oggSound);

            Assert.NotSame(mp3Sound, oggSound);

            Assert.Equal(123u, mp3Sound.Id);
            Assert.Equal(456u, oggSound.Id);

            Assert.Equal(this.mp3SoundFilePath, mp3Sound.FilePath);
            Assert.Equal(this.oggSoundFilePath, oggSound.FilePath);
        }

        [Fact]
        public void Unload_WhenSoundToUnloadExists_RemovesAndDisposesOfSound()
        {
            // Arrange
            var mockSound = new Mock<ISound>();
            mockSound.SetupGet(p => p.Id).Returns(123u);
            this.mockSoundFactory.Setup(m => m.Create(this.oggSoundFilePath))
                .Returns(mockSound.Object);

            var cache = CreateCache();
            var unused = cache.GetItem(this.oggSoundFilePath);

            // Act
            cache.Unload(this.oggSoundFilePath);

            // Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                cache.Unload(this.oggSoundFilePath);
            });

            Assert.Equal(0, cache.TotalCachedItems);
            this.mockDisposeSoundReactable
                .Verify(m
                    => m.PushNotification(new DisposeSoundData(123u), false), Times.Once);
        }

        [Fact]
        public void Unload_WhenSoundToUnloadDoesNotExist_DoesNotAttemptToDispose()
        {
            // Arrange
            var mockSound = new Mock<ISound>();
            mockSound.SetupGet(p => p.Id).Returns(123u);

            var cache = CreateCache();
            cache.GetItem(this.oggSoundFilePath);

            // Act
            cache.Unload("non-existing-texture");

            // Assert
            this.mockDisposeSoundReactable
                .Verify(m
                    => m.PushNotification(new DisposeSoundData(123u), false), Times.Never);
        }

        [Fact]
        public void ShutDownNotification_WhenReceived_DisposesOfSounds()
        {
            // Arrange
            IReactor<ShutDownData>? shutDownReactor = null;

            var mockSoundA = new Mock<ISound>();
            mockSoundA.SetupGet(p => p.Id).Returns(11u);
            mockSoundA.Name = nameof(mockSoundA);

            var mockSoundB = new Mock<ISound>();
            mockSoundB.SetupGet(p => p.Id).Returns(22u);
            mockSoundB.Name = nameof(mockSoundB);

            var soundPathA = $"{SoundDirPath}soundA{OggFileExtension}";
            var soundPathB = $"{SoundDirPath}soundB{OggFileExtension}";

            this.mockSoundFactory.Setup(m => m.Create(soundPathA))
                .Returns(mockSoundA.Object);

            this.mockSoundFactory.Setup(m => m.Create(soundPathB))
                .Returns(mockSoundB.Object);

            this.mockPath.Setup(m => m.GetExtension(soundPathA)).Returns(OggFileExtension);
            this.mockPath.Setup(m => m.GetExtension(soundPathB)).Returns(OggFileExtension);

            this.mockFile.Setup(m => m.Exists(soundPathA)).Returns(true);
            this.mockFile.Setup(m => m.Exists(soundPathB)).Returns(true);

            this.mockShutDownReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<ShutDownData>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IReactor<ShutDownData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "Shutdown reactable subscription failed.  Reactor is null.");
                    }

                    shutDownReactor = reactor;
                });

            var cache = CreateCache();

            cache.GetItem(soundPathA);
            cache.GetItem(soundPathB);

            // Act
            shutDownReactor?.OnNext(default);
            shutDownReactor?.OnNext(default);

            // Assert
            this.mockDisposeSoundReactable
                .Verify(m =>
                    m.PushNotification(new DisposeSoundData(11u), false), Times.Once);
            this.mockDisposeSoundReactable
                .Verify(m =>
                    m.PushNotification(new DisposeSoundData(22u), false), Times.Once);
            this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="SoundCache"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private SoundCache CreateCache() =>
            new (this.mockSoundFactory.Object,
                this.mockFile.Object,
                this.mockPath.Object,
                this.mockDisposeSoundReactable.Object,
                this.mockShutDownReactable.Object);
    }
}
