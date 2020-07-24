using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using OpenToolkit.Audio.OpenAL;
using Raptor.Audio;
using Raptor.Factories;
using Raptor.OpenAL;
using RaptorTests.Helpers;
using Xunit;

namespace RaptorTests.Audio
{
    public class AudioManagerTests : IDisposable
    {
        private readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioManager)}' has already been destroyed.\nInvoked the '{nameof(AudioManager.Init)}()' to re-setup the device manager.";
        private readonly Mock<IALInvoker> mockALInvoker;
        private readonly Mock<IALCInvoker> mockALCInvoker;
        private readonly ALDevice device;
        private readonly ALContext context;
        private readonly string soundFileName = "sound.ogg";
        private Guid soundId = new Guid("1ad5904c-b55a-4638-8fd3-76bbd960f074");
        private readonly int sourceId = 4321;
        private readonly int bufferId = 9876;
        private IAudioManager? manager;

        public AudioManagerTests()
        {
            this.device = new ALDevice(new IntPtr(1234));
            this.context = new ALContext(new IntPtr(5678));

            this.mockALInvoker = new Mock<IALInvoker>();
            this.mockALInvoker.Setup(m => m.GenSource()).Returns(this.sourceId);
            this.mockALInvoker.Setup(m => m.GenBuffer()).Returns(this.bufferId);

            this.mockALCInvoker = new Mock<IALCInvoker>();
            this.mockALCInvoker.Setup(m => m.OpenDevice(It.IsAny<string>())).Returns(this.device);
            this.mockALCInvoker.Setup(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()))
                .Returns(this.context);
        }

        #region Method Tests
        [Fact]
        public void GetInstance_WhenInvoked_InitializesAudioDevice()
        {
            // Act
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            // Assert
            this.mockALCInvoker.Verify(m => m.OpenDevice(It.IsAny<string>()), Times.Once());
            this.mockALCInvoker.Verify(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()), Times.Once());
        }

        [Fact]
        public void GetInstance_WhenInvokedAfterFirstInvoke_ReturnsReferenceToSameInstance()
        {
            // Act
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            object managerB = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            // Assert
            Assert.Same(this.manager, managerB);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void IsInitialized_WhenGettingValueAfterInitialization_ReturnsTrue()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            // Act
            var actual = this.manager.IsInitialized;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void DeviceNames_WhenGettingValueAfterBeingDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                _ = this.manager.DeviceNames;
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void DeviceNames_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { "Device-1", "Device-2" };
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.mockALCInvoker.Setup(m => m.GetString(this.device, AlcGetStringList.AllDevicesSpecifier))
                .Returns(() => new[] { "Device-1", "Device-2" });

            // Act
            var actual = this.manager.DeviceNames;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void CreateSoundID_WithSingleParamAndWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.CreateSoundID(It.IsAny<string>());
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void CreateSoundID_WithSingleParam_SetsUpSound()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            // Act
            var actual = this.manager.CreateSoundID(this.soundFileName);

            // Assert
            Assert.NotEqual(Guid.Empty, actual);
            this.mockALInvoker.Verify(m => m.GenSource(), Times.Once());
            this.mockALInvoker.Verify(m => m.GenBuffer(), Times.Once());
        }

        [Fact]
        public void CreateSoundID_WithBothParamsWHileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.CreateSoundID(It.IsAny<string>(), It.IsAny<Guid>());
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void CreateSoundID_WithBothParams_ReturnsCorrectResult()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            // Act
            var actual = this.manager.CreateSoundID(this.soundFileName, this.soundId);

            // Assert
            Assert.Equal(this.soundId, actual);
        }

        [Fact]
        public void UploadOggData_WhenInvokedWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.UploadOggData(It.IsAny<SoundStats<float>>(), It.IsAny<Guid>());
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void UploadOggData_WhenSoundIDDoesNotExist_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.CreateSoundID(It.IsAny<string>(), this.soundId);
            var invalidSoundId = new Guid("0605d390-9605-4d08-9b2e-c7f8cd0c20e2");

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.UploadOggData(It.IsAny<SoundStats<float>>(), invalidSoundId);
            }, $"The sound id '{invalidSoundId}' does not exist.");
        }

        [Fact]
        public void UploadOggData_WhenInvoked_ProperlyUploadsBufferData()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            SoundStats<float> oggData;
            oggData.BufferData = new[] { 1f, 2f };
            oggData.Format = AudioFormat.Stereo16;
            oggData.SampleRate = 44100;
            oggData.Channels = 2;
            oggData.TotalSeconds = 3f;

            this.manager.CreateSoundID(this.soundFileName, this.soundId);

            // Act
            this.manager.UploadOggData(oggData, this.soundId);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(this.bufferId, ALFormat.Stereo16, new[] { 1f, 2f }, 8, 44100), Times.Once());
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourcei.Buffer, this.bufferId), Times.Once());
        }

        [Fact]
        public void UploadMp3Data_WhenInvokedWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.UploadMp3Data(It.IsAny<SoundStats<byte>>(), It.IsAny<Guid>());
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void UploadMp3Data_WhenSoundIDDoesNotExist_ThrowsException()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);
            this.manager.CreateSoundID(It.IsAny<string>(), this.soundId);
            var invalidSoundId = new Guid("0605d390-9605-4d08-9b2e-c7f8cd0c20e2");

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.UploadMp3Data(It.IsAny<SoundStats<byte>>(), invalidSoundId);
            }, $"The sound id '{invalidSoundId}' does not exist.");
        }

        [Fact]
        public void UploadMp3Data_WhenInvoked_ProperlyUploadsBufferData()
        {
            // Arrange
            this.manager = AudioManager.GetInstance(this.mockALInvoker.Object, this.mockALCInvoker.Object);

            SoundStats<byte> mp3Data;
            mp3Data.BufferData = new byte[] { 1, 2 };
            mp3Data.Format = AudioFormat.Stereo16;
            mp3Data.SampleRate = 44100;
            mp3Data.Channels = 2;
            mp3Data.TotalSeconds = 3f;

            this.manager.CreateSoundID(this.soundFileName, this.soundId);

            // Act
            this.manager.UploadMp3Data(mp3Data, this.soundId);

            // Assert
            this.mockALInvoker.Verify(m => m.BufferData(this.bufferId, ALFormat.Stereo16, new byte[] { 1, 2 }, 2, 44100), Times.Once());
            this.mockALInvoker.Verify(m => m.Source(this.sourceId, ALSourcei.Buffer, this.bufferId), Times.Once());
        }
        #endregion

        public void Dispose()
        {
            this.manager?.Dispose();
            this.manager = null;
        }
    }
}
