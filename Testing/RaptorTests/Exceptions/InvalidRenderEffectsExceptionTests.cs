// <copyright file="InvalidRenderEffectsExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions
{
    using System;
    using Velaptor.Exceptions;
    using Velaptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="UnknownContentException"/> class.
    /// </summary>
    public class InvalidRenderEffectsExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
        {
            // Act
            var exception = new InvalidRenderEffectsException();

            // Assert
            Assert.Equal($"{nameof(RenderEffects)} value invalid.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new InvalidRenderEffectsException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new InvalidRenderEffectsException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion
    }
}
