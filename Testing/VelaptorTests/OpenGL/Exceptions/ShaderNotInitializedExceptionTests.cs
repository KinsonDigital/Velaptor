// <copyright file="ShaderNotInitializedExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Exceptions;

using System;
using FluentAssertions;
using Velaptor.OpenGL.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="ShaderNotInitializedException"/> class.
/// </summary>
public class ShaderNotInitializedExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new ShaderNotInitializedException();

        // Assert
        exception.Message.Should().Be("The shader has not been initialized.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithOnlyMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new ShaderNotInitializedException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndShaderNameParams_CorrectlySetsMessage()
    {
        // Act
        const string shaderName = "test-shader";
        var exception = new ShaderNotInitializedException("test-message", shaderName);

        // Assert
        exception.Message.Should().Be("test-shader test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new ShaderNotInitializedException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
