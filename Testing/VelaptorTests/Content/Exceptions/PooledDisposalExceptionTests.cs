// <copyright file="PooledDisposalExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Exceptions
{
    using System;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="PooledDisposalException"/> class.
    /// </summary>
    public class PooledDisposalExceptionTests
    {
        private static readonly string Message =
            @$"Cannot manually dispose of '{nameof(IContent)}' objects.
            \nTo override manual disposal of pooled objects, set the '{nameof(IContent.IsPooled)}' to a value of 'false'.
            \n!!WARNING!! It is not recommended to do this due to the object possibly being used somewhere else in the application.
            \nThe benefit of object pooling is to improve performance and reusability of objects.";

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
        {
            // Act
            var exception = new PooledDisposalException();

            // Assert
            Assert.Equal(Message, exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new PooledDisposalException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new PooledDisposalException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
