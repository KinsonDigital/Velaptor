// <copyright file="GLErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using FluentAssertions;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="GLErrorEventArgs"/> class.
/// </summary>
public class GLErrorEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GLErrorEventArgs(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithNullErrorMessage_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GLErrorEventArgs(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'errorMessage')");
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
