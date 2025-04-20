// <copyright file="GlfwErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using Shouldly;
using Velaptor.NativeInterop.GLFW;
using Xunit;

public class GlfwErrorEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullErrorMessageParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwErrorEventArgs(GlfwErrorCode.NoError, null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithEmptyErrorMessageParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwErrorEventArgs(GlfwErrorCode.NoError, string.Empty);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange & Act
        var eventArgs = new GlfwErrorEventArgs(GlfwErrorCode.ApiUnavailable, "test-message");

        // Assert
        eventArgs.ErrorCode.ShouldBe(GlfwErrorCode.ApiUnavailable);
        eventArgs.ErrorMessage.ShouldBe("test-message");
    }
    #endregion
}
