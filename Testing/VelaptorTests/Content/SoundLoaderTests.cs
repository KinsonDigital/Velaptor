// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Factories;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundLoader"/> class.
    /// </summary>
    public class SoundLoaderTests
    {
        private const string SoundName = "test-sound";
        private const string OggSoundDirPath = @"C:\temp\Content\Sounds\";
        private readonly string oggSoundFilepath;
        private readonly Mock<IPathResolver> mockSoundPathResolver;
        private readonly Mock<ISound> mockSound;
        private readonly Mock<ISoundFactory> soundFactory;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<IObservable<bool>> mockShutDownObservable;
        private readonly Mock<IDisposable> mockShutDownUnsubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.oggSoundFilepath = $"{OggSoundDirPath}{SoundName}.ogg";

            this.mockSoundPathResolver = new Mock<IPathResolver>();
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(SoundName)).Returns(this.oggSoundFilepath);

            this.mockSound = new Mock<ISound>();
            this.mockSound.SetupGet(p => p.FilePath).Returns(this.oggSoundFilepath);

            this.soundFactory = new Mock<ISoundFactory>();
            this.soundFactory.Setup(m => m.Create(this.oggSoundFilepath)).Returns(this.mockSound.Object);

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.HasExtension(SoundName)).Returns(false);

            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownObservable = new Mock<IObservable<bool>>();
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
                    this.mockPath.Object,
                    this.mockShutDownObservable.Object);
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
                    this.mockPath.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'soundFactory')");
        }

        [Fact]
        public void Ctor_WithNullNullParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    this.soundFactory.Object,
                    null,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'path')");
        }

        [Fact]
        public void Ctor_WithNullShutDownObservableParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SoundLoader(
                    this.mockSoundPathResolver.Object,
                    this.soundFactory.Object,
                    this.mockPath.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownObservable')");
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(SoundName, "")]
        [InlineData(SoundName, ".txt")]
        public void Load_WhenInvoked_LoadsSound(string contentName, string extension)
        {
            // Arrange
            this.mockPath.Setup(m => m.HasExtension($"{contentName}.txt")).Returns(true);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}")).Returns(contentName);

            var loader = CreateSoundLoader();

            // Act
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            this.soundFactory.Verify(m => m.Create($"{OggSoundDirPath}{SoundName}.ogg"), Times.Once());
            Assert.Equal(actual.FilePath, this.oggSoundFilepath);
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsSound()
        {
            // Arrange
            var loader = CreateSoundLoader();
            loader.Load(SoundName);

            // Act
            loader.Unload(SoundName);
            loader.Unload(SoundName);

            // Assert
            this.mockSound.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfLoader()
        {
            // Arrange
            IObserver<bool>? shutDownObserver = null;
            this.mockShutDownObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IObserver<bool>>(observer =>
                {
                    if (observer is null)
                    {
                        Assert.True(false, "Shutdown observable subscription failed.  Observer is null.");
                    }

                    shutDownObserver = observer;
                });

            var loader = CreateSoundLoader();
            loader.Load(SoundName);

            // Act
            shutDownObserver?.OnNext(true);
            shutDownObserver?.OnNext(true);

            // Assert
            this.mockSound.Verify(m => m.Dispose(), Times.Once());
            this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The mockSound loader instance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new (
            this.mockSoundPathResolver.Object,
            this.soundFactory.Object,
            this.mockPath.Object,
            this.mockShutDownObservable.Object);
    }
}
