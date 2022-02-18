// <copyright file="FreeTypeErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.FreeType
{
    using System;
    using Velaptor.NativeInterop.FreeType;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FreeTypeErrorEventArgs"/> class.
    /// </summary>
    public class FreeTypeErrorEventArgsTests
    {
        #region Constructor Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_WithNullErrorMessageParam_ThrowsException(string message)
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new FreeTypeErrorEventArgs(message);
            }, "The parameter must not be null or empty. (Parameter 'errorMessage')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsErrorMessageProperty()
        {
            // Act
            var eventArgs = new FreeTypeErrorEventArgs("test-message");

            // Assert
            Assert.Equal("test-message", eventArgs.ErrorMessage);
        }
        #endregion
    }
}
