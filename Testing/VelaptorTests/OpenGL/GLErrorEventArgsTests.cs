// <copyright file="GLErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using Velaptor.OpenGL;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="GLErrorEventArgs"/> class.
    /// </summary>
    public class GLErrorEventArgsTests
    {
        #region Constructor Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_WithNullErrorMessage_ThrowsException(string value)
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLErrorEventArgs(value);
            }, "The string parameter must not be null or empty. (Parameter 'errorMessage')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsErrorMessageProperty()
        {
            // Act
            var args = new GLErrorEventArgs("test-message");

            // Assert
            Assert.Equal("test-message", args.ErrorMessage);
        }
        #endregion
    }
}
