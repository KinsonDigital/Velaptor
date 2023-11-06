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
    [Fact]
    public void Ctor_WithNullErrorMessageParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwErrorEventArgs(GlfwErrorCode.NoError, null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithEmptyErrorMessageParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GlfwErrorEventArgs(GlfwErrorCode.NoError, string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'errorMessage')");
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
