// <copyright file="EnumOutOfRangeExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
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
        Assert.Equal($"The value of the enum '{nameof(TestEnum)}' is invalid and out of range.", exception.Message);
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new EnumOutOfRangeException<TestEnum>("test-message");

        // Assert
        Assert.Equal("test-message", exception.Message);
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new EnumOutOfRangeException<TestEnum>("test-exception", innerException);

        // Assert
        Assert.Equal("inner-exception", deviceException.InnerException.Message);
        Assert.Equal("test-exception", deviceException.Message);
    }
    #endregion
}
