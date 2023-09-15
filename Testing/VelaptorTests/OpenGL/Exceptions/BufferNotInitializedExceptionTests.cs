// <copyright file="BufferNotInitializedExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Exceptions;

using System;
using FluentAssertions;
using Velaptor.OpenGL.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="BufferNotInitializedException"/> class.
/// </summary>
public class BufferNotInitializedExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new BufferNotInitializedException();

        // Assert
        exception.Message.Should().Be("The buffer has not been initialized.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithOnlyMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new BufferNotInitializedException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndBufferNameParams_CorrectlySetsMessage()
    {
        // Act
        const string bufferName = "test-buffer";
        var exception = new BufferNotInitializedException("test-message", bufferName);

        // Assert
        exception.Message.Should().Be("test-buffer test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new BufferNotInitializedException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
