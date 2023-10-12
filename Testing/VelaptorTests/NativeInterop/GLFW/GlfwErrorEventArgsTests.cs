// <copyright file="GlfwErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.GLFW;

using System;
using FluentAssertions;
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
        // Act
        var act = () => new GlfwErrorEventArgs(GlfwErrorCode.NoError, message);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("The string parameter must not be null or empty. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange & Act
        var eventArgs = new GlfwErrorEventArgs(GlfwErrorCode.ApiUnavailable, "test-message");

        // Assert
        eventArgs.ErrorCode.Should().Be(GlfwErrorCode.ApiUnavailable);
        eventArgs.ErrorMessage.Should().Be("test-message");
    }
    #endregion
}
