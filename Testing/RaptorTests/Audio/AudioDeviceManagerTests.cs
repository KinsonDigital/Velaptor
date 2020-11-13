// <copyright file="AudioDeviceManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio
{
    using System;
    using System.Collections.ObjectModel;
    using Moq;
    using OpenTK.Audio.OpenAL;
    using Raptor.Audio;
    using Raptor.Audio.Exceptions;
    using Raptor.Content;
    using Raptor.OpenAL;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AudioDeviceManager"/> class.
    /// </summary>
    public class AudioDeviceManagerTests : IDisposable
    {
        private static readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioDeviceManager)}' has not been initialized.\nInvoked the '{nameof(AudioDeviceManager.InitDevice)}()' to initialize the device manager.";
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

        #region Method Tests
        [Fact]
        public void GetInstance_WithNullInvoker_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                this.manager = AudioDeviceManager.GetInstance(null);
            }, "Parameter must not be null. (Parameter 'alInvoker')");
        }

        [Fact]
        public void GetInstance_WhenInvoked_InitializesAudioDevice()
        {
            // Act
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();

            // Assert
            this.mockALInvoker.Verify(m => m.OpenDevice(It.IsAny<string>()), Times.Once());
            this.mockALInvoker.Verify(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()), Times.Once());
        }

        [Fact]
        public void GetInstance_WhenInvokedReInitialization_ReturnsReferenceToSameInstance()
        {
            // Act
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();
            object managerB = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Assert
            Assert.Same(this.manager, managerB);
        }

        [Fact]
        public void GetInstance_WhenInvokedAfterFirstInvoke_ReturnsReferenceToSameInstance()
        {
            // Act
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            object managerB = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Assert
            Assert.Same(this.manager, managerB);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void IsInitialized_WhenGettingValueAfterInitialization_ReturnsTrue()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                _ = this.manager.DeviceNames;
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void DeviceNames_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { "Device-1", "Device-2" };
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.Dispose();

            // Act
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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

            this.mockALInvoker.Setup(m => m.MakeContextCurrent(this.context)).Returns(contextResult);
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.Dispose();
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<SettingContextCurrentException>(() =>
            {
                this.manager.InitDevice("test-device");
            }, "There was an issue setting the audio context as the current context.");
        }

        [Fact]
        public void InitSound_WithSingleParamAndWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                this.manager.InitSound();
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void InitSound_WhenInvoked_SetsUpSoundAndReturnsCorrectResult()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
            {
                this.manager.UpdateSoundSource(It.IsAny<SoundSource>());
            }, IsDisposedExceptionMessage);
        }

        [Fact]
        public void UpdateSoundSource_WhenSoundSourceDoesNotExist_ThrowsException()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<SoundSourceDoesNotExistException>(() =>
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();
            this.manager.InitSound();

            // Act & Assert
            AssertHelpers.DoesNotThrow<Exception>(() =>
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<AudioDeviceManagerNotInitializedException>(() =>
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

            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<AudioDeviceDoesNotExistException>(() =>
            {
                this.manager.ChangeDevice("test-device-1");
            }, "Device Name: test-device-1\nThe audio device does not exist.");
        }

        [Fact]
        public void ChangeDevice_WhenCurrentTimePositionIsGreaterThenMaxTime_ChangesDevices()
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
            var fileName = @"C:\temp\Content\Sounds\sound.ogg";
            this.mockALInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(new[] { "device-1" });
            this.mockALInvoker.Setup(m => m.GetSourceState(this.srcId))
                .Returns(ALSourceState.Playing);
            this.mockALInvoker.Setup(m => m.GetSource(this.srcId, ALGetSourcei.SampleOffset))
                .Returns(500_000); // End result will be calculated to the time position that the sound is currently at

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
                        TotalSeconds = 1,
                    };

                    return oggData;
                });

            var mockContentSrc = new Mock<IContentSource>();
            mockContentSrc.Setup(m => m.GetContentPath(It.IsAny<string>())).Returns(fileName);

            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            var sound = new Sound(
                It.IsAny<string>(),
                this.mockALInvoker.Object,
                this.manager,
                mockOggDecoder.Object,
                new Mock<ISoundDecoder<byte>>().Object,
                mockContentSrc.Object);

            // Act
            this.manager.ChangeDevice("device-1");

            // Assert
            this.mockALInvoker.Verify(m => m.GetSourceState(this.srcId), Times.Once());
            this.mockALInvoker.Verify(m => m.GetSource(this.srcId, ALSourcef.SecOffset), Times.Once());
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
            var fileName = @"C:\temp\Content\Sounds\sound.ogg";
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

            var mockContentSrc = new Mock<IContentSource>();
            mockContentSrc.Setup(m => m.GetContentPath(It.IsAny<string>())).Returns(fileName);

            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.InitDevice();

            var sound = new Sound(
                "sound",
                this.mockALInvoker.Object,
                this.manager,
                mockOggDecoder.Object,
                new Mock<ISoundDecoder<byte>>().Object,
                mockContentSrc.Object);

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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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
        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            this.manager?.Dispose();
            this.manager = null;
            GC.SuppressFinalize(this);
        }
    }
}
