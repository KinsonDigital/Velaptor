// <copyright file="FreeTypeErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.FreeType;

using System;
using FluentAssertions;
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
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WithEmptyParam_ThrowsException()
    {
        // Assert
        var act = () => new FreeTypeErrorEventArgs(string.Empty);

        // Act
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'errorMessage')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsErrorMessageProperty()
    {
        // Act
        var eventArgs = new FreeTypeErrorEventArgs("test-message");

        // Assert
        eventArgs.ErrorMessage.Should().Be("test-message");
    }
    #endregion
}
