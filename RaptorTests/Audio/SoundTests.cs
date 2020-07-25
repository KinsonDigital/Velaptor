// <copyright file="SoundTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0017 // Simplify object initialization
namespace RaptorTests.Audio
{
    using System;
    using Castle.DynamicProxy.Tokens;
    using Moq;
    using OpenToolkit.Audio.OpenAL;
    using Raptor.Audio;
    using Raptor.OpenAL;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Sound"/> class.
    /// </summary>
    public class SoundTests : IDisposable
    {
        private readonly Mock<IAudioDeviceManager> mockAudioManager;
        private readonly Mock<ISoundDecoder<float>> mockOggDecoder;
        private readonly Mock<ISoundDecoder<byte>> mockMp3Decoder;
        private readonly Mock<IALInvoker> mockALInvoker;
        private readonly string oggSoundFileName = "sound.ogg";
        private readonly string mp3SoundFileName = "sound.mp3";
        private readonly float[] oggBufferData = new float[] { 11f, 22f, 33f, 44f };
        private readonly byte[] mp3BufferData = new byte[] { 55, 66, 77, 88 };
        private Sound? sound;
        private int srcId = 1234;
        private int bufferId = 5678;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundTests"/> class.
        /// </summary>
        public SoundTests()
        {
            this.mockAudioManager = new Mock<IAudioDeviceManager>();
            this.mockAudioManager.Setup(m => m.InitSound()).Returns((this.srcId, this.bufferId));

            this.mockOggDecoder = new Mock<ISoundDecoder<float>>();
            this.mockOggDecoder.Setup(m => m.LoadData(this.oggSoundFileName)).Returns(() =>
            {
                return new SoundData<float>()
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
                return new SoundData<byte>()
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
        public void Ctor_WhenUsingOggFile_LoadsOggData()
        {
            // Act
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockOggDecoder.Verify(m => m.LoadData(this.oggSoundFileName), Times.Once());
        }

        [Fact]
        public void Ctor_WhenUsingMP3File_LoadsMP3Data()
        {
            // Act
            this.sound = new Sound(this.mp3SoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockMp3Decoder.Verify(m => m.LoadData(this.mp3SoundFileName), Times.Once());
        }

        [Fact]
        public void Ctor_WhenUsingUnsupportedFileType_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound = new Sound("sound.wav", this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);
            }, "The file extension '.wav' is not supported file type.");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void ContentName_WhenGettingValue_ReturnsCorrectValue()
        {
            // Act
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            Assert.Equal("sound", this.sound.ContentName);
        }

        [Fact]
        public void IsLooping_WhenGettingValueWhileDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                _ = this.sound.IsLooping;
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void IsLooping_WhenGettingValue_GetsSoundLoopingValue()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            _ = this.sound.IsLooping;

            // Assert
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALSourceb.Looping), Times.Once());
        }

        [Fact]
        public void IsLooping_WhenSettingValueWhileDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.IsLooping = true;
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void IsLooping_WhenSettingValue_SetsSoundLoopingSetting()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.IsLooping = true;

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.srcId, ALSourceb.Looping, true), Times.Once());
        }

        [Fact]
        public void Volume_WhenGettingValueWhileDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                _ = this.sound.Volume;
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Volume_WhenGettingValue_GetsSoundVolume()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            _ = this.sound.Volume;

            // Assert
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALSourcef.Gain), Times.Once());
        }

        [Fact]
        public void Volume_WhenSettingValueWhileDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.Volume = 0.5f;
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Volume_WhenSettingValue_SetsSoundVolume()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.Volume = 0.5f;

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.srcId, ALSourcef.Gain, 0.5f), Times.Once());
        }

        [Fact]
        public void CurrentTimePosition_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                _ = this.sound.TimePosition;
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void CurrentTimePosition_WhenGettingValue_GetsSoundTimePosition()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            _ = this.sound.TimePosition;

            // Assert
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALGetSourcei.SampleOffset), Times.Once());
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Play_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.PlaySound();
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Play_WhenInvoked_PlaysSound()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.PlaySound();

            // Assert
            this.mockALInvoker.Verify(m => m.SourcePlay(this.srcId), Times.Once());
        }

        [Fact]
        public void Pause_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.PauseSound();
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Pause_WhenInvoked_PausesSound()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.PauseSound();

            // Assert
            this.mockALInvoker.Verify(m => m.SourcePause(this.srcId), Times.Once());
        }

        [Fact]
        public void Stop_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.StopSound();
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Stop_WhenInvoked_StopsSound()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.StopSound();

            // Assert
            this.mockALInvoker.Verify(m => m.SourceStop(this.srcId), Times.Once());
        }

        [Fact]
        public void Reset_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.Reset();
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void Reset_WhenInvoked_ResetsSound()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.Reset();

            // Assert
            this.mockALInvoker.Verify(m => m.SourceRewind(this.srcId), Times.Once());
        }

        [Fact]
        public void SetTimePosition_WhenDisposed_ThrowsException()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act & Assert
            this.sound.Dispose();

            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.sound.SetTimePosition(5);
            }, "The sound is disposed.  You must create another sound instance.");
        }

        [Fact]
        public void SetTimePosition_WithOggFile_SetsTimePosition()
        {
            // Arrange
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Act
            this.sound.SetTimePosition(123f);

            // Assert
            this.mockALInvoker.Verify(m => m.Source(this.srcId, ALSourcef.SecOffset, 123f), Times.Once());
        }

        [Fact]
        public void UploadOggData_WhenInvoked_ProperlyUploadsBufferData()
        {
            // Act
            this.sound = new Sound(this.oggSoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(this.bufferId, ALFormat.Stereo16, new[] { 1f, 2f }, 8, 44100), Times.Once());
        }

        [Fact]
        public void UploadMp3Data_WhenInvoked_ProperlyUploadsBufferData()
        {
            // Act
            this.sound = new Sound(this.mp3SoundFileName, this.mockALInvoker.Object, this.mockAudioManager.Object, this.mockOggDecoder.Object, this.mockMp3Decoder.Object);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(this.bufferId, ALFormat.Stereo16, new[] { 1f, 2f }, 2, 44100), Times.Once());
        }
        #endregion

        public void Dispose()
        {
            this.sound?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
