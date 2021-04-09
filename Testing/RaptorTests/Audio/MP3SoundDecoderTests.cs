// <copyright file="MP3SoundDecoderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio
{
    using System;
    using System.Collections.ObjectModel;
    using Moq;
    using Raptor.Audio;
    using RaptorTests.Helpers;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="MP3SoundDecoder"/> class.
    /// </summary>
    public class MP3SoundDecoderTests
    {
        private readonly Mock<IAudioDataStream<byte>> mockDataStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="MP3SoundDecoderTests"/> class.
        /// </summary>
        public MP3SoundDecoderTests() => this.mockDataStream = new Mock<IAudioDataStream<byte>>();

        #region Method Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LoadData_WhenFileNameIsEmptyOrNull_ThrowsException(string fileName)
        {
            // Arrange
            var decoder = new MP3SoundDecoder(this.mockDataStream.Object);

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                decoder.LoadData(fileName);
            }, "The param must not be null or empty. (Parameter 'fileName')");
        }

        [Fact]
        public void LoadData_WhenUsingFileNameWithWrongFileExtension_ThrowsException()
        {
            // Arrange
            var decoder = new MP3SoundDecoder(this.mockDataStream.Object);

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                decoder.LoadData("sound.wav");
            }, "The file name must have an mp3 file extension. (Parameter 'fileName')");
        }

        [Fact]
        public unsafe void LoadData_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var bufferData = new byte[2];

            this.mockDataStream.SetupGet(p => p.SampleRate).Returns(1);
            this.mockDataStream.SetupGet(p => p.Channels).Returns(1);
            this.mockDataStream.SetupGet(p => p.Format).Returns(AudioFormat.Stereo16);
            this.mockDataStream.Setup(m => m.ReadSamples(bufferData, 0, It.IsAny<int>()))
                .Returns<byte[], int, int>((buffer, offset, count) =>
                {
                    fixed (byte* pBuffer = buffer)
                    {
                        pBuffer[0] = 10;
                        pBuffer[1] = 20;
                    }

                    return 2;
                });

            var decoder = new MP3SoundDecoder(this.mockDataStream.Object);
            var expected = new SoundData<byte>
            {
                BufferData = new ReadOnlyCollection<byte>(new byte[] { 10, 20 }),
                Channels = 1,
                Format = AudioFormat.Stereo16,
                SampleRate = 1,
                TotalSeconds = 0.5f,
            };

            // Act
            var actual = decoder.LoadData("sound.mp3");

            // Assert
            Assert.Equal(expected, actual);
            this.mockDataStream.Verify(m => m.ReadSamples(new byte[] { 10, 20 }, 0, 2), Times.Exactly(2));
        }

        [Fact]
        public void Dispose_WhenInvoked_ProperlyDisposesDecoder()
        {
            // Arrange
            var decoder = new MP3SoundDecoder(this.mockDataStream.Object);

            // Act
            decoder.Dispose();
            decoder.Dispose();

            // Assert
            this.mockDataStream.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion
    }
}
