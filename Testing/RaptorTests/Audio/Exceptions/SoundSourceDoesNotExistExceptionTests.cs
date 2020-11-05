// <copyright file="SoundSourceDoesNotExistExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio.Exceptions
{
    using System;
    using Raptor.Audio.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundSourceDoesNotExistException"/> class.
    /// </summary>
    public class SoundSourceDoesNotExistExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new SoundSourceDoesNotExistException();

            // Assert
            Assert.Equal("Sound source does not exist.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new SoundSourceDoesNotExistException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new SoundSourceDoesNotExistException("device-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("device-exception", deviceException.Message);
        }
        #endregion
    }
}
