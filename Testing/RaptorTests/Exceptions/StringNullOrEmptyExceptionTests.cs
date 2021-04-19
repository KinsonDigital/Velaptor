// <copyright file="StringNullOrEmptyExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Exceptions
{
    using System;
    using Raptor.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="StringNullOrEmptyException"/> class.
    /// </summary>
    public class StringNullOrEmptyExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new StringNullOrEmptyException();

            // Assert
            Assert.Equal("The string must not be null or empty.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new StringNullOrEmptyException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new StringNullOrEmptyException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
