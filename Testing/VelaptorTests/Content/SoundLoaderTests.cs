// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using Moq;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundLoader"/> class.
    /// </summary>
    public class SoundLoaderTests
    {
        private const string OggSoundDirPath = @"C:\temp\Content\Sounds\";
        private readonly string oggSoundFilepath;
        private readonly Mock<IPathResolver> mockSoundPathResolver;
        private readonly Mock<ISound> sound;
        private readonly Mock<ISoundFactory> soundFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.oggSoundFilepath = $"{OggSoundDirPath}sound.ogg";

            this.mockSoundPathResolver = new Mock<IPathResolver>();
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath("sound")).Returns(this.oggSoundFilepath);

            this.sound = new Mock<ISound>();
            this.sound.SetupGet(p => p.Path).Returns(this.oggSoundFilepath);

            this.soundFactory = new Mock<ISoundFactory>();
            this.soundFactory.Setup(m => m.CreateSound(this.oggSoundFilepath)).Returns(this.sound.Object);
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_SoundNotNull()
        {
            // Arrange
            var loader = CreateSoundLoader();

            // Act
            var actual = loader.Load("sound");

            // Assert
            this.soundFactory.Verify(m => m.CreateSound($"{OggSoundDirPath}sound.ogg"), Times.Once());
            Assert.Equal(actual.Path, this.oggSoundFilepath);
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsSound()
        {
            // Arrange
            var loader = CreateSoundLoader();
            loader.Load("sound");

            // Act
            loader.Unload("sound");
            loader.Unload("sound");

            // Assert
            this.sound.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_ProperlyDisposesOfSounds()
        {
            // Arrange
            var loader = CreateSoundLoader();
            loader.Load("sound");

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.sound.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The sound loader isntance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new (this.mockSoundPathResolver.Object, this.soundFactory.Object);
    }
}
