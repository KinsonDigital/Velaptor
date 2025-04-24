// <copyright file="FreeTypeErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.FreeType;

using System;
using Shouldly;
using Velaptor.NativeInterop.FreeType;
using Xunit;

/// <summary>
/// Tests the <see cref="FreeTypeErrorEventArgs"/> class.
/// </summary>
public class FreeTypeErrorEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullParam_ThrowsException()
    {
        // Assert
        var act = () => new FreeTypeErrorEventArgs(null);

        // Act
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithEmptyParam_ThrowsException()
    {
        // Assert
        var act = () => new FreeTypeErrorEventArgs(string.Empty);

        // Act
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsErrorMessageProperty()
    {
        // Act
        var eventArgs = new FreeTypeErrorEventArgs("test-message");

        // Assert
        eventArgs.ErrorMessage.ShouldBe("test-message");
    }
    #endregion
}
