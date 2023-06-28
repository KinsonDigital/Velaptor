// <copyright file="ShaderCompileExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions.Shaders
{
    using System;
    using Velaptor.Exceptions.Shaders;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ShaderCompileException"/>.
    /// </summary>
    public sealed class ShaderCompileExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new ShaderCompileException();

            // Assert
            Assert.Equal("An error occurred while compiling a shader.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new ShaderCompileException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new ShaderCompileException("test-exception", innerException);

            // Assert
            Assert.Equal("inner-exception", deviceException.InnerException.Message);
            Assert.Equal("test-exception", deviceException.Message);
        }
        #endregion

        #region Properties

        [Fact]
        public void ErrorInformation_WhenSetWithConstructor_CorrectlySetsProperty()
        {
            // Arrange
            const string expected = "test-error-information";

            var exception = new ShaderCompileException(0, expected);

            // Act
            var actual = exception.ErrorInformation;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShaderProgramId_WhenSetWithConstructor_CorrectlySetsProperty()
        {
            // Arrange
            const uint expected = 40;

            var exception = new ShaderCompileException(expected, null);

            // Act
            var actual = exception.ShaderId;

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
