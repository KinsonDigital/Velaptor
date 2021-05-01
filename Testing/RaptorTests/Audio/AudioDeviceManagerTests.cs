// <copyright file="AudioDeviceManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.Audio
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using System.Collections.ObjectModel;
    using Moq;
    using OpenTK.Audio.OpenAL;
    using Raptor.Audio;
    using Raptor.Audio.Exceptions;
    using Raptor.OpenAL;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
#pragma warning restore IDE0001 // Name can be simplified

    /// <summary>
    /// Tests the <see cref="AudioDeviceManager"/> class.
    /// </summary>
    public class AudioDeviceManagerTests
    {
        private static readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioDeviceManager)}' has not been initialized.\nInvoked the '{nameof(AudioDeviceManager.InitDevice)}()' to initialize the device manager.";
        private readonly string oggFilePath;
        private readonly Mock<IALInvoker> mockALInvoker;
        private readonly ALDevice device;
        private readonly ALContext context;
        private readonly int srcId = 4321;
        private readonly int bufferId = 9876;
        private IAudioDeviceManager? manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceManagerTests"/> class.
        /// </summary>
        public AudioDeviceManagerTests()
        {
            this.oggFilePath = @"C:\temp\Content\Sounds\sound.ogg";
            this.device = new ALDevice(new IntPtr(1234));
            this.context = new ALContext(new IntPtr(5678));

            this.mockALInvoker = new Mock<IALInvoker>();
            this.mockALInvoker.Setup(m => m.GenSource()).Returns(this.srcId);
            this.mockALInvoker.Setup(m => m.GenBuffer()).Returns(this.bufferId);
            this.mockALInvoker.Setup(m => m.OpenDevice(It.IsAny<string>())).Returns(this.device);
            this.mockALInvoker.Setup(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()))
                .Returns(this.context);
            this.mockALInvoker.Setup(m => m.MakeContextCurrent(this.context)).Returns(true);
        }

        #region Prop Tests
        [Fact]
        public void IsInitialized_WhenGettingValueAfterInitialization_ReturnsTrue()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.InitDevice();

            // Act
            var actual = this.manager.IsInitialized;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void DeviceNames_WhenGettingValueAfterBeingDisposed_ThrowsException()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.Dispose();

            // Act & Assert
            Assert.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                _ = this.manager.DeviceNames;
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void DeviceNames_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { "Device-1", "Device-2" };
            this.manager = CreateManager();
            this.manager.InitDevice();
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(() => new[] { "Device-1", "Device-2" });

            // Act
            var actual = this.manager.DeviceNames;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void InitDevice_WhenInvoked_InitializesDevice()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.Dispose();

            // Act
            this.manager = CreateManager();
            this.manager.InitDevice("test-device");

            // Assert
            this.mockALInvoker.Verify(m => m.OpenDevice("OpenAL Soft on test-device"), Times.Once());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public void InitDevice_WithIssueMakingContextCurrent_ThrowsException(bool? makeContextCurrentResult)
        {
            // Arrange
            // The MakeContextCurrent call does not take nullable bool.  This fixes that issue
            var contextResult = !(makeContextCurrentResult is null) && (bool)makeContextCurrentResult;

            this.mockALInvoker.Setup(m => m.MakeContextCurrent(this.context)).Returns(false);
            this.manager = CreateManager();

            // Act & Assert
            Assert.ThrowsWithMessage<SettingContextCurrentException>(() =>
            {
                this.manager.InitDevice("test-device");
            }, "There was an issue setting the audio context as the current context.");
        }

        [Fact]
        public void InitSound_WithSingleParamAndWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.Dispose();

            // Act & Assert
            Assert.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                this.manager.InitSound();
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void InitSound_WhenInvoked_SetsUpSoundAndReturnsCorrectResult()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.InitDevice();

            // Act
            var (actualSourceId, actualBufferId) = this.manager.InitSound();

            // Assert
            Assert.Equal(this.srcId, actualSourceId);
            Assert.Equal(this.bufferId, actualBufferId);
            this.mockALInvoker.Verify(m => m.GenSource(), Times.Once());
            this.mockALInvoker.Verify(m => m.GenBuffer(), Times.Once());
        }

        [Fact]
        public void UpdateSoundSource_WhenNotInitialized_ThrowsException()
        {
            // Arrange
            this.manager = CreateManager();

            // Act & Assert
            Assert.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                this.manager.UpdateSoundSource(It.IsAny<SoundSource>());
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void UpdateSoundSource_WhenSoundSourceDoesNotExist_ThrowsException()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.InitDevice();

            // Act & Assert
            Assert.ThrowsWithMessage<SoundSourceDoesNotExistException>(() =>
            {
                var soundSrc = new SoundSource()
                {
                    SourceId = 1234,
                };
                this.manager.UpdateSoundSource(soundSrc);
            }, $"The sound source with the source id '1234' does not exist.");
        }

        [Fact]
        public void UpdateSoundSource_WhenInvoked_UpdatesSoundSource()
        {
            // Arrange
            this.manager = CreateManager();
            this.manager.InitDevice();
            this.manager.InitSound();

            // Act & Assert
            Assert.DoesNotThrow<Exception>(() =>
            {
                var otherSoundSrc = new SoundSource()
                {
                    SourceId = 4321,
                };
                this.manager.UpdateSoundSource(otherSoundSrc);
            });
        }

        [Fact]
        public void ChangeDevice_WhenNotInitialized_ThrowsException()
        {
            // Arrange
            this.manager = CreateManager();

            // Act & Assert
            Assert.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                this.manager.ChangeDevice("test-device");
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void ChangeDevice_WhenUsingInvalidDeviceName_ThrowsException()
        {
            // Arrange
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(new[] { "device-1", "device-2" });

            this.manager = CreateManager();
            this.manager.InitDevice();

            // Act & Assert
            Assert.ThrowsWithMessage<AudioDeviceDoesNotExistException>(() =>
            {
                this.manager.ChangeDevice("test-device-1");
            }, "Device Name: test-device-1\nThe audio device does not exist.");
        }

        [Theory]
        [InlineData(10, 5, 5)]
        public void ChangeDevice_WhenCurrentTimePositionIsGreaterThenMaxTime_ChangesDevices(
            float timePosition,
            float totalSeconds,
            float expected)
        {
            /*NOTE:
             * To calculate the time position in seconds, take the sampleOffset and divide it by sample rate.
             * Example: 220,500 sample offset / 44100 sample rate = 5 seconds
             *          220,500 / 44100 = 5.0
             * When changing a device, the state and time position should be invoked once
             * for each sound source that exists.
             * sampleOffset is the amount of samples positionally the sound is currently at.
             * 44100 is the standard samples/Hz for 1 second worth of sound.
             * To get 10 seconds of sound, you would need 220,500 samples.
             */
            // Arrange
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(new[] { "device-1" });
            this.mockALInvoker.Setup(m => m.GetSourceState(this.srcId))
                .Returns(ALSourceState.Playing);
            this.mockALInvoker.Setup(m => m.GetSource(this.srcId, ALGetSourcei.SampleOffset))
                .Returns(500_000); // End result will be calculated to the time position that the sound is currently at
            this.mockALInvoker.Setup(m => m.GetSource(this.srcId, ALSourcef.SecOffset)).Returns(timePosition);

            var mockOggDecoder = new Mock<ISoundDecoder<float>>();
            mockOggDecoder.Setup(m => m.LoadData(It.IsAny<string>()))
                .Returns(() =>
                {
                    var oggData = new SoundData<float>
                    {
                        BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                        Format = AudioFormat.Stereo16,
                        Channels = 2,
                        SampleRate = 44100,
                        TotalSeconds = totalSeconds,
                    };

                    return oggData;
                });

            this.manager = CreateManager();

            var sound = new Sound(
                this.oggFilePath,
                this.mockALInvoker.Object,
                this.manager,
                mockOggDecoder.Object,
                new Mock<ISoundDecoder<byte>>().Object);

            // Act
            this.manager.ChangeDevice("device-1");

            // Assert
            this.mockALInvoker.Verify(m => m.GetSourceState(this.srcId), Times.Once());
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALSourcef.SecOffset), Times.Once());
            this.mockALInvoker.Verify(m => m.Source(this.srcId, ALSourcef.SecOffset, expected), Times.Once());
        }

        [Theory]
        [InlineData(ALSourceState.Stopped, 1, 0, 0)]
        [InlineData(ALSourceState.Playing, 1, 1, 220_500)] // 5 seconds of sound at a sample rate of 44100
        [InlineData(ALSourceState.Playing, 1, 1, 485_100)] // 11 seconds of sound at a sample rate of 44100
        [InlineData(ALSourceState.Playing, 1, 1, -100)] // negative second value result
        [InlineData(ALSourceState.Paused, 1, 1, 220_500)]
        public void ChangeDevice_WithOnlySingleSoundSource_MakesProperALCallsForCachingSources(
            ALSourceState srcState,
            int srcStateInvokeCount,
            int currentTimePositionInvokeCount,
            int sampleOffset)
        {
            /*NOTE:
             * When changing a device, the state and time position should be invoked once
             * for each sound source that exists.
             * sampleOffset is the amount of samples positionally the sound is currently at.
             * 44100 is the standard samples/Hz for 1 second worth of sound.
             * To get 10 seconds of sound, you would need 220,500 samples.
             */
            // Arrange
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(new[] { "device-1", "device-2" });
            this.mockALInvoker.Setup(m => m.GetSourceState(this.srcId))
                .Returns(srcState);
            this.mockALInvoker.Setup(m => m.GetSource(this.srcId, ALGetSourcei.SampleOffset))
                .Returns(sampleOffset); // End result will be calculated to the time position that the sound is currently at

            var mockOggDecoder = new Mock<ISoundDecoder<float>>();
            mockOggDecoder.Setup(m => m.LoadData(It.IsAny<string>()))
                .Returns(() =>
                {
                    var oggData = new SoundData<float>
                    {
                        BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                        Format = AudioFormat.Stereo16,
                        Channels = 2,
                        SampleRate = 44100,
                        TotalSeconds = 10,
                    };

                    return oggData;
                });

            this.manager = CreateManager();
            this.manager.InitDevice();

            var sound = new Sound(
                this.oggFilePath,
                this.mockALInvoker.Object,
                this.manager,
                mockOggDecoder.Object,
                new Mock<ISoundDecoder<byte>>().Object);

            // Act
            this.manager.ChangeDevice("device-1");

            // Assert
            this.mockALInvoker.Verify(m => m.GetSourceState(this.srcId), Times.Exactly(srcStateInvokeCount));
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALSourcef.SecOffset), Times.Exactly(currentTimePositionInvokeCount));
        }

        [Fact]
        public void ChangeDevice_WhenInvokedWithEventSubscription_InvokesDeviceChangedEvent()
        {
            // Arrange
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(new[] { "device-1", "device-2" });
            this.manager = CreateManager();
            this.manager.InitDevice();

            // Act & Assert
            Assert.Raises<EventArgs>((e) =>
            {
                this.manager.DeviceChanged += e;
            }, (e) =>
            {
                this.manager.DeviceChanged -= e;
            }, () =>
            {
                this.manager.ChangeDevice("device-1");
            });
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfManager()
        {
            // Arrange
            var manager = CreateManager();
            manager.InitDevice();

            // Act
            manager.Dispose();
            manager.Dispose();

            // Assert
            this.mockALInvoker.Verify(m => m.MakeContextCurrent(ALContext.Null), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AudioDeviceManager"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private AudioDeviceManager CreateManager() => new AudioDeviceManager(this.mockALInvoker.Object);
    }
}
