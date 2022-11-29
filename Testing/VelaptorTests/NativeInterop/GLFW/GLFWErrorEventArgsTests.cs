// <copyright file="GLFWErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using Velaptor.NativeInterop.GLFW;
using Helpers;
using Xunit;

public class GLFWErrorEventArgsTests
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
            _ = new GLFWErrorEventArgs(GLFWErrorCode.NoError, message);
        }, "The string parameter must not be null or empty. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange & Act
        var eventArgs = new GLFWErrorEventArgs(GLFWErrorCode.ApiUnavailable, "test-message");

        // Assert
        Assert.Equal(GLFWErrorCode.ApiUnavailable, eventArgs.ErrorCode);
        Assert.Equal("test-message", eventArgs.ErrorMessage);
    }
    #endregion
}
