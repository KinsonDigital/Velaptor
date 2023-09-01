// <copyright file="GlfwErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using Helpers;
using Velaptor.NativeInterop.GLFW;
using Xunit;

public class GlfwErrorEventArgsTests
{
    #region Constructor Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullOrEmptyErrorMessageParam_ThrowsException(string message)
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GlfwErrorEventArgs(GlfwErrorCode.NoError, message);
        }, "The string parameter must not be null or empty. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange & Act
        var eventArgs = new GlfwErrorEventArgs(GlfwErrorCode.ApiUnavailable, "test-message");

        // Assert
        Assert.Equal(GlfwErrorCode.ApiUnavailable, eventArgs.ErrorCode);
        Assert.Equal("test-message", eventArgs.ErrorMessage);
    }
    #endregion
}
