// <copyright file="LoadEmbeddedResourceExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Exceptions;
using Xunit;

namespace VelaptorTests.Exceptions;

/// <summary>
/// Tests the <see cref="LoadEmbeddedResourceException"/> class.
/// </summary>
public class LoadEmbeddedResourceExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new LoadEmbeddedResourceException();

        // Assert
        Assert.Equal("Issue loading the embedded resource.", exception.Message);
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new LoadEmbeddedResourceException("test-message");

        // Assert
        Assert.Equal("test-message", exception.Message);
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new LoadEmbeddedResourceException("test-exception", innerException);

        // Assert
        Assert.Equal("inner-exception", deviceException.InnerException.Message);
        Assert.Equal("test-exception", deviceException.Message);
    }
    #endregion
}
