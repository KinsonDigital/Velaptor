// <copyright file="UnsupportedSoundTypeExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio.Exceptions
{
    using System;
    using Raptor.Audio.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="UnsupportedSoundTypeException"/> class.
    /// </summary>
    public class UnsupportedSoundTypeExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new UnsupportedSoundTypeException();

            // Assert
            Assert.Equal("Unsupported sound type.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new UnsupportedSoundTypeException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndDeviceNameParams_CorrectlySetsMessage()
        {
            // Act
            var exception = new UnsupportedSoundTypeException("test-message", ".wav");

            // Assert
            Assert.Equal("Unsupported sound type: '.wav'\ntest-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new UnsupportedSoundTypeException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
