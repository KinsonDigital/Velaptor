// <copyright file="InvalidInputExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Exceptions
{
    using System;
    using Raptor.Exceptions;
    using Xunit;

    public class InvalidInputExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
        {
            // Act
            var exception = new InvalidInputException();

            // Assert
            Assert.Equal($"Invalid Input", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new InvalidInputException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new InvalidInputException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
