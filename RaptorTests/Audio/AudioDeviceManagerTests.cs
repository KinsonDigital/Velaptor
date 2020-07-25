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
    public class AudioDeviceManagerTests : IDisposable
    {
        private readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioDeviceManager)}' has already been destroyed.\nInvoked the '{nameof(AudioDeviceManager.InitDevice)}()' to re-setup the device manager.";
        private readonly Mock<IALInvoker> mockALInvoker;
        private readonly ALDevice device;
        private readonly ALContext context;
        private readonly string soundFileName = "sound.ogg";
        private Guid soundId = new Guid("1ad5904c-b55a-4638-8fd3-76bbd960f074");
        private readonly int sourceId = 4321;
        private readonly int bufferId = 9876;
        private IAudioDeviceManager? manager;

        public AudioDeviceManagerTests()
        {
            this.device = new ALDevice(new IntPtr(1234));
            this.context = new ALContext(new IntPtr(5678));

            this.mockALInvoker = new Mock<IALInvoker>();
            this.mockALInvoker.Setup(m => m.GenSource()).Returns(this.sourceId);
            this.mockALInvoker.Setup(m => m.GenBuffer()).Returns(this.bufferId);
            this.mockALInvoker.Setup(m => m.OpenDevice(It.IsAny<string>())).Returns(this.device);
            this.mockALInvoker.Setup(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()))
                .Returns(this.context);
        }

        #region Method Tests
        [Fact]
        public void GetInstance_WhenInvoked_InitializesAudioDevice()
        {
            // Act
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Assert
            this.mockALInvoker.Verify(m => m.OpenDevice(It.IsAny<string>()), Times.Once());
            this.mockALInvoker.Verify(m => m.CreateContext(this.device, It.IsAny<ALContextAttributes>()), Times.Once());
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
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
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
        public void InitSound_WithSingleParamAndWhileDisposed_ThrowsException()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);
            this.manager.Dispose();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.manager.InitSound();
            }, this.IsDisposedExceptionMessage);
        }

        [Fact]
        public void InitSound_WhenInvoked_SetsUpSoundAndReturnsCorrectResult()
        {
            // Arrange
            this.manager = AudioDeviceManager.GetInstance(this.mockALInvoker.Object);

            // Act
            var (actualSourceId, actualBufferId) = this.manager.InitSound();

            // Assert
            Assert.Equal(this.sourceId, actualSourceId);
            Assert.Equal(this.bufferId, actualBufferId);
            this.mockALInvoker.Verify(m => m.GenSource(), Times.Once());
            this.mockALInvoker.Verify(m => m.GenBuffer(), Times.Once());
        }
        #endregion

        public void Dispose()
        {
            this.manager?.Dispose();
            this.manager = null;
        }
    }
}
