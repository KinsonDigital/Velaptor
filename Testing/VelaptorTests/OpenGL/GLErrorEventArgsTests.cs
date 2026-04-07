// <copyright file="GLErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using Shouldly;
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
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithNullErrorMessage_ThrowsException()
    {
        // Arrange & Act
        var act = () => new GLErrorEventArgs(string.Empty);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsErrorMessageProperty()
    {
        // Act
        var args = new GLErrorEventArgs("test-message");

        // Assert
        args.ErrorMessage.ShouldBe("test-message");
    }
    #endregion
}
