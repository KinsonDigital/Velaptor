// <copyright file="SystemDisplayExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using Shouldly;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="SystemDisplayException"/> class.
/// </summary>
public class SystemDisplayExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new SystemDisplayException();

        // Assert
        exception.Message.ShouldBe("There was an issue with one of the system displays.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new SystemDisplayException("test-message");

        // Assert
        exception.Message.ShouldBe("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new SystemDisplayException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.ShouldBe("inner-exception");
        deviceException.Message.ShouldBe("test-exception");
    }
    #endregion
}
