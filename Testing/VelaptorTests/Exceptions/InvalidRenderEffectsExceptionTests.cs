// <copyright file="InvalidRenderEffectsExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using FluentAssertions;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="InvalidRenderEffectsException"/> class.
/// </summary>
public class InvalidRenderEffectsExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new InvalidRenderEffectsException();

        // Assert
        exception.Message.Should().Be($"{nameof(RenderEffects)} value invalid.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new InvalidRenderEffectsException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new InvalidRenderEffectsException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
