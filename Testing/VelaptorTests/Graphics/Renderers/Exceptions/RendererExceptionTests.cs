// <copyright file="RendererExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers.Exceptions;

using System;
using Shouldly;
using Velaptor.Graphics.Renderers.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="RendererException"/> class.
/// </summary>
public class RendererExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new RendererException();

        // Assert
        exception.Message.ShouldBe("There was an issue with the renderer.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new RendererException("test-message");

        // Assert
        exception.Message.ShouldBe("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new RendererException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.ShouldBe("inner-exception");
        deviceException.Message.ShouldBe("test-exception");
    }
    #endregion
}
