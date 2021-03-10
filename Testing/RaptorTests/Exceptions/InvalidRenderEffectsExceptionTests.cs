using System;
using System.Collections.Generic;
using System.Text;

namespace RaptorTests.Exceptions
{
    using System;
    using Raptor.Exceptions;
    using Raptor.Graphics;
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
            var deviceException = new InvalidRenderEffectsException("device-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("device-exception", deviceException.Message);
        }
        #endregion
    }
}
