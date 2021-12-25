// <copyright file="BufferNotInitializedExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Exceptions
{
    using System;
    using Velaptor.OpenGL.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="BufferNotInitializedException"/> class.
    /// </summary>
    public class BufferNotInitializedExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
        {
            // Act
            var exception = new BufferNotInitializedException();

            // Assert
            Assert.Equal("The buffer has not been initialized.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithOnlyMessageParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new BufferNotInitializedException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndBufferNameParams_CorrectlySetsMessage()
        {
            // Act
            const string bufferName = "test-buffer";
            var exception = new BufferNotInitializedException("test-message", bufferName);

            // Assert
            Assert.Equal("test-buffer test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new BufferNotInitializedException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
