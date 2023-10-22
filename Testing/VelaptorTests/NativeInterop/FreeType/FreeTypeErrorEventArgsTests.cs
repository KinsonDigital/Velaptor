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
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullErrorMessageParam_ThrowsException(string message)
    {
        // Assert
        var act = () => new FreeTypeErrorEventArgs(message);

        // Act
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'errorMessage')");
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
