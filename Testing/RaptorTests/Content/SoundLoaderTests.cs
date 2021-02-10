// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.Collections.ObjectModel;
    using Moq;
    using OpenTK.Audio.OpenAL;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.OpenAL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundLoader"/> class.
    /// </summary>
    public class SoundLoaderTests
    {
        private const string OggSoundDirPath = @"C:\temp\Content\Sounds\";
        private const int OpenALSoundID = 1234;
        private const int OpenALBufferID = 5678;
        private readonly string oggSoundFilepath;
        private readonly Mock<IALInvoker> mockAlInvoker;
        private readonly Mock<IAudioDeviceManager> mockAudioManager;
        private readonly Mock<IPathResolver> mockSoundPathResolver;
        private readonly Mock<ISoundDecoder<float>> mockOggDecoder;
        private readonly Mock<ISoundDecoder<byte>> mockMp3Decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.oggSoundFilepath = $"{OggSoundDirPath}sound.ogg";
            this.mockAlInvoker = new Mock<IALInvoker>();
            this.mockAlInvoker.Setup(m => m.MakeContextCurrent(It.IsAny<ALContext>())).Returns(true);

            this.mockAudioManager = new Mock<IAudioDeviceManager>();
            this.mockAudioManager.Setup(m => m.InitSound()).Returns((srcId: OpenALSoundID, bufferId: OpenALBufferID));

            this.mockSoundPathResolver = new Mock<IPathResolver>();
            this.mockSoundPathResolver.Setup(m => m.ResolveFilePath("sound")).Returns(this.oggSoundFilepath);

            this.mockOggDecoder = new Mock<ISoundDecoder<float>>();
            this.mockOggDecoder.Setup(m => m.LoadData(this.oggSoundFilepath))
                .Returns(() =>
                {
                    var result = default(SoundData<float>);
                    result.Format = AudioFormat.Stereo16;
                    result.BufferData = new ReadOnlyCollection<float>(new[] { 1f });
                    return result;
                });

            this.mockMp3Decoder = new Mock<ISoundDecoder<byte>>();
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
            Assert.NotNull(actual);
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsSound()
        {
            // Arrange
            var loader = CreateSoundLoader();
            loader.Load("sound");

            // Act
            loader.Unload("sound");

            // Assert
            this.mockOggDecoder.Verify(m => m.Dispose(), Times.Once());
            this.mockMp3Decoder.Verify(m => m.Dispose(), Times.Once());
            this.mockAlInvoker.Verify(m => m.DeleteSource(OpenALSoundID), Times.Once());
            this.mockAlInvoker.Verify(m => m.DeleteBuffer(OpenALBufferID), Times.Once());
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
            this.mockOggDecoder.Verify(m => m.Dispose(), Times.Once());
            this.mockMp3Decoder.Verify(m => m.Dispose(), Times.Once());
            this.mockAlInvoker.Verify(m => m.DeleteSource(OpenALSoundID), Times.Once());
            this.mockAlInvoker.Verify(m => m.DeleteBuffer(OpenALBufferID), Times.Once());
        }

        [Fact]
        public void Dispose_WhenSourceIDIsNotGreaterThenZero_DoesNotDeleteSource()
        {
            // Arrange
            this.mockAudioManager.Setup(m => m.InitSound()).Returns((srcId: 0, bufferId: OpenALBufferID));

            var loader = CreateSoundLoader();
            loader.Load("sound");

            // Act
            loader.Dispose();

            // Assert
            this.mockAlInvoker.Verify(m => m.DeleteSource(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Dispose_WhenBufferIDIsZero_DoesNotDeleteBuffer()
        {
            // Arrange
            this.mockAudioManager.Setup(m => m.InitSound()).Returns((srcId: OpenALSoundID, bufferId: 0));

            var loader = CreateSoundLoader();
            loader.Load("sound");

            // Act
            loader.Dispose();

            // Assert
            this.mockAlInvoker.Verify(m => m.DeleteBuffer(It.IsAny<int>()), Times.Never());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The sound loader isntance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new SoundLoader(
            this.mockAlInvoker.Object,
            this.mockAudioManager.Object,
            this.mockSoundPathResolver.Object,
            this.mockOggDecoder.Object,
            this.mockMp3Decoder.Object);
    }
}
