// <copyright file="SystemDisplayExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using FluentAssertions;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="SystemMonitorException"/> class.
/// </summary>
public class SystemDisplayExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new SystemMonitorException();

        // Assert
        exception.Message.Should().Be("There was an issue with one of the system monitors.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new SystemMonitorException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new SystemMonitorException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
