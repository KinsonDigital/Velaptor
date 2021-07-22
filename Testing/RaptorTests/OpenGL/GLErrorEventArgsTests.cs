// <copyright file="GLErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.OpenGL
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using Raptor.OpenGL;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
#pragma warning restore IDE0001 // Name can be simplified

    /// <summary>
    /// Tests the <see cref="GLErrorEventArgs"/> class.
    /// </summary>
    public class GLErrorEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullErrorMessage_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GLErrorEventArgs(null);
            }, "The parameter must not be null or empty. (Parameter 'errorMessage')");
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
