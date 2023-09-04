// <copyright file="EnumOutOfRangeExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using FluentAssertions;
using Helpers;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="EnumOutOfRangeException{T}"/> class.
/// </summary>
public class EnumOutOfRangeExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new EnumOutOfRangeException<TestEnum>();

        // Assert
        exception.Message.Should().Be($"The value of the enum '{nameof(TestEnum)}' is invalid and out of range.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new EnumOutOfRangeException<TestEnum>("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new EnumOutOfRangeException<TestEnum>("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
