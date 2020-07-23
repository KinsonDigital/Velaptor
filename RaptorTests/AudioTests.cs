// <copyright file="AudioTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0017 // Simplify object initialization
namespace RaptorTests
{
    using System;
    using Moq;
    using OpenToolkit.Audio.OpenAL;
    using OpenToolkit.Graphics.OpenGL;
    using Raptor.Audio;
    using Raptor.OpenAL;
    using RaptorTests.Helpers;
    using Xunit;

    public class AudioTests
    {
        private readonly Mock<IALInvoker> mockALInvoker;
        private readonly Mock<ISoundDecoder<float>> mockOggDecoder;
        private readonly Mock<ISoundDecoder<byte>> mockMp3Decoder;
        private readonly int bufferId = 1234;
        private readonly int sourceId = 5678;
        private readonly string oggSoundFileName = "sound.ogg";
        private readonly string mp3SoundFileName = "sound.mp3";
        private readonly float[] oggBufferData = new float[] { 11f, 22f, 33f, 44f };
        private readonly byte[] mp3BufferData = new byte[] { 55, 66, 77, 88 };

        public AudioTests()
        {
            this.mockALInvoker = new Mock<IALInvoker>();
            this.mockALInvoker.Setup(m => m.GenBuffers(1)).Returns(new[] { this.bufferId });
            this.mockALInvoker.Setup(m => m.GenSources(1)).Returns(new[] { this.sourceId });

            this.mockOggDecoder = new Mock<ISoundDecoder<float>>();
            this.mockOggDecoder.Setup(m => m.LoadData(this.oggSoundFileName)).Returns(() =>
            {
                return new SoundStats<float>()
                {
                    BufferData = oggBufferData,
                    Channels = 2,
                    Format = AudioFormat.Stereo16,
                    SampleRate = 44100,
                    TotalSeconds = 10,
                };
            });

            this.mockMp3Decoder = new Mock<ISoundDecoder<byte>>();
            this.mockMp3Decoder.Setup(m => m.LoadData(this.mp3SoundFileName)).Returns(() =>
            {
                return new SoundStats<byte>()
                {
                    BufferData = mp3BufferData,
                    Channels = 1,
                    Format = AudioFormat.Mono16,
                    SampleRate = 22050,
                    TotalSeconds = 15,
                };
            });
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenUsingNullOggDecoder_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, null, this.mockMp3Decoder.Object);
            }, "The param must not be null. (Parameter 'oggDecoder')");
        }

        [Fact]
        public void Ctor_WhenUsingNullMp3Decoder_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, null);
            }, "The param must not be null. (Parameter 'mp3Decoder')");
        }

        [Fact]
        public void Ctor_WhenUsingOggFile_LoadsOggData()
        {
            // Act
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockOggDecoder.Verify(m => m.LoadData(this.oggSoundFileName), Times.Once());
        }

        [Fact]
        public void Ctor_WhenUsingMP3File_LoadsMP3Data()
        {
            // Act
            var sound = new Sound(this.mockALInvoker.Object, this.mp3SoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockMp3Decoder.Verify(m => m.LoadData(this.mp3SoundFileName), Times.Once());
        }

        [Theory]
        [InlineData(AudioFormat.Mono8, ALFormat.Mono8)]
        [InlineData(AudioFormat.Mono16, ALFormat.Mono16)]
        [InlineData(AudioFormat.Mono32Float, ALFormat.MonoFloat32Ext)]
        [InlineData(AudioFormat.Stereo8, ALFormat.Stereo8)]
        [InlineData(AudioFormat.Stereo16, ALFormat.Stereo16)]
        [InlineData(AudioFormat.StereoFloat32, ALFormat.StereoFloat32Ext)]
        [InlineData(default(AudioFormat), default(ALFormat))]
        public void Ctor_WhenUsingOggFile_AudioDataBuffered(AudioFormat incomingFormat, ALFormat alFormat)
        {
            // Act
            this.mockOggDecoder.Setup(m => m.LoadData(this.oggSoundFileName)).Returns(() =>
            {
                return new SoundStats<float>()
                {
                    BufferData = oggBufferData,
                    Channels = 2,
                    Format = incomingFormat,
                    SampleRate = 44100,
                    TotalSeconds = 10,
                };
            });
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(
                this.bufferId,
                alFormat,
                this.oggBufferData,
                this.oggBufferData.Length * sizeof(float),
                44100));
        }

        [Fact]
        public void Ctor_WhenUsingMp3File_AudioDataBuffered()
        {
            // Act
            var sound = new Sound(this.mockALInvoker.Object, this.mp3SoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(
                this.bufferId,
                ALFormat.Mono16,
                this.mp3BufferData,
                this.mp3BufferData.Length * sizeof(byte),
                22050));
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void IsLooping_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.IsLooping = true;
            _ = sound.IsLooping;

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourceb.Looping, true), Times.Once());
            this.mockALInvoker.Verify(m => m.GetSource(this.sourceId, ALSourceb.Looping), Times.Once());
        }

        [Theory]
        [InlineData(0.5f, 0.5f)]
        [InlineData(10f, 1f)]
        [InlineData(-10f, 0f)]
        public void Valume_WhenSettingValue_ReturnsCorrectResult(float gain, float expected)
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.Volume = gain;
            _ = sound.Volume;

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourcef.Gain, expected), Times.Once());
            this.mockALInvoker.Verify(m => m.GetSource(this.sourceId, ALSourcef.Gain), Times.Once());
        }

        [Fact]
        public void CurrentTimePosition_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            _ = sound.CurrentTimePosition;

            // Assert
            this.mockALInvoker.Verify(m => m.GetSource(this.sourceId, ALSourcef.SecOffset), Times.Once());
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Play_WhenInvoked_PlaysSound()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.Play();

            // Assert
            this.mockALInvoker.Verify(m => m.SourcePlay(this.sourceId), Times.Once());
        }

        [Fact]
        public void Pause_WhenInvoked_PausesSound()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.Pause();

            // Assert
            this.mockALInvoker.Verify(m => m.SourcePause(this.sourceId), Times.Once());
        }

        [Fact]
        public void Stop_WhenInvoked_StopsSound()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.Stop();

            // Assert
            this.mockALInvoker.Verify(m => m.SourceStop(this.sourceId), Times.Once());
        }

        [Fact]
        public void Reset_WhenInvoked_ResetsSound()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.Reset();

            // Assert
            this.mockALInvoker.Verify(m => m.SourceRewind(this.sourceId), Times.Once());
        }

        [Theory]
        [InlineData(5f, 5f)]
        [InlineData(20f, 10f)]
        [InlineData(0f, 0f)]
        public void SetTimePosition_WithOggFile_SetsTimePosition(float seconds, float expected)
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.oggSoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.SetTimePosition(seconds);

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourcef.SecOffset, expected), Times.Once());
        }

        [Theory]
        [InlineData(5f, 5f)]
        [InlineData(30f, 15f)]
        [InlineData(0f, 0f)]
        public void SetTimePosition_WithMp3File_SetsTimePosition(float seconds, float expected)
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, this.mp3SoundFileName, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            sound.SetTimePosition(seconds);

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourcef.SecOffset, expected), Times.Once());
        }

        [Fact]
        public void SetTimePosition_WhenUsingUnsupportedFileType_ThrowsException()
        {
            // Arrange
            var sound = new Sound(this.mockALInvoker.Object, "sound.wav", this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                sound.SetTimePosition(5);
            }, "Unsupported file type of '.wav'. Supported file types are 'ogg' and 'mp3'.");
        }
        #endregion
    }
}
