// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.Collections.ObjectModel;
    using Moq;
    using OpenToolkit.Audio.OpenAL;
    using Raptor.Audio;
    using Raptor.Audio.Exceptions;
    using Raptor.Content;
    using Raptor.OpenAL;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundLoader"/> class.
    /// </summary>
    public class SoundLoaderTests
    {
        private readonly Mock<IALInvoker> mockAlInvoker;
        private readonly Mock<IAudioDeviceManager> mockAudioManager;
        private readonly Mock<IContentSource> mockContentSource;
        private readonly Mock<ISoundDecoder<float>> mockOggDecoder;
        private readonly Mock<ISoundDecoder<byte>> mockMp3Decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
        /// </summary>
        public SoundLoaderTests()
        {
            this.mockAlInvoker = new Mock<IALInvoker>();
            this.mockAlInvoker.Setup(m => m.MakeContextCurrent(It.IsAny<ALContext>())).Returns(true);

            this.mockAudioManager = new Mock<IAudioDeviceManager>();

            this.mockContentSource = new Mock<IContentSource>();
            this.mockContentSource.Setup(m => m.GetContentPath(ContentType.Sounds, "sound.ogg")).Returns(@"C:\temp\Content\Sounds\sound.ogg");

            this.mockOggDecoder = new Mock<ISoundDecoder<float>>();
            this.mockOggDecoder.Setup(m => m.LoadData(@"C:\temp\Content\Sounds\sound.ogg"))
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
        public void Load_WhenUsingUnsupportedExtension_ThrowsException()
        {
            // Arrange
            var loader = CreateSoundLoader();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<UnsupportedSoundTypeException>(() =>
            {
                loader.Load("sound.wav");
            }, "The extension '.wav' is not supported.  Supported audio files are '.ogg' and '.mp3'.");
        }

        [Fact]
        public void Load_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var loader = CreateSoundLoader();

            // Act
            var actual = loader.Load("sound.ogg");

            // Assert
            Assert.Equal("sound", actual.ContentName);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
        /// </summary>
        /// <returns>The sound loader isntance used for testing.</returns>
        private SoundLoader CreateSoundLoader() => new SoundLoader(
            this.mockAlInvoker.Object,
            this.mockAudioManager.Object,
            this.mockContentSource.Object,
            this.mockOggDecoder.Object,
            this.mockMp3Decoder.Object);
    }
}
