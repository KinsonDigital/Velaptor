// <copyright file="BufferNotInitializedExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Exceptions;

using System;
using Shouldly;
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
        exception.Message.ShouldBe("The buffer has not been initialized.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithOnlyMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new BufferNotInitializedException("test-message");

        // Assert
        exception.Message.ShouldBe("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndBufferNameParams_CorrectlySetsMessage()
    {
        // Act
        const string bufferName = "test-buffer";
        var exception = new BufferNotInitializedException("test-message", bufferName);

        // Assert
        exception.Message.ShouldBe("test-buffer test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new BufferNotInitializedException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.ShouldBe("inner-exception");
        deviceException.Message.ShouldBe("test-exception");
    }
    #endregion
}
