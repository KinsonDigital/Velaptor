// <copyright file="LoadAtlasExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Exceptions;

using System;
using FluentAssertions;
using Velaptor.Content.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="LoadAtlasException"/> class.
/// </summary>
public class LoadAtlasExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var sut = new LoadAtlasException();

        // Assert
        sut.Message.Should().Be("There was an issue loading the atlas data.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var sut = new LoadAtlasException("test-message");

        // Assert
        sut.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var sut = new LoadAtlasException("test-exception", innerException);

        // Assert
        sut.InnerException.Message.Should().Be("inner-exception");
        sut.Message.Should().Be("test-exception");
    }
    #endregion
}
