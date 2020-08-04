// <copyright file="AudioDeviceDoesNotExistExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace RaptorTests.Audio.Exceptions
{
    using System;
    using Raptor.Audio.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AudioDeviceDoesNotExistException"/> class.
    /// </summary>
    public class AudioDeviceDoesNotExistExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new AudioDeviceDoesNotExistException();

            // Assert
            Assert.Equal("The audio device does not exist.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new AudioDeviceDoesNotExistException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndDeviceNameParams_CorrectlySetsMessage()
        {
            // Act
            var exception = new AudioDeviceDoesNotExistException("test-message", "device");

            // Assert
            Assert.Equal("Device Name: device\ntest-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new AudioDeviceDoesNotExistException("device-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("device-exception", deviceException.Message);
        }
        #endregion
    }
}
