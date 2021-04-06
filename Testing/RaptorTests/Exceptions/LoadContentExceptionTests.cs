// <copyright file="LoadContentExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Exceptions
{
    using System;
    using Raptor.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="UnknownContentException"/> class.
    /// </summary>
    public class LoadContentExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
        {
            // Act
            var exception = new LoadContentException();

            // Assert
            Assert.Equal($"There was an issue loading the content.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new LoadContentException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new LoadContentException("device-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("device-exception", deviceException.Message);
        }
        #endregion
    }
}
