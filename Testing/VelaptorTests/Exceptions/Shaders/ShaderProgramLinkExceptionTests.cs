// <copyright file="ShaderProgramLinkExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions.Shaders
{
    using System;
    using Velaptor.Exceptions.Shaders;
    using Xunit;

    public sealed class ShaderProgramLinkExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new ShaderProgramLinkException();

            // Assert
            Assert.Equal("An error occurred while linking a shader program.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new ShaderProgramLinkException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
        {
            // Arrange
            var innerException = new Exception("inner-exception");

            // Act
            var deviceException = new ShaderProgramLinkException("test-exception", innerException);

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

            var exception = new ShaderProgramLinkException(0, expected);

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

            var exception = new ShaderProgramLinkException(expected, null);

            // Act
            var actual = exception.ShaderProgramId;

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
