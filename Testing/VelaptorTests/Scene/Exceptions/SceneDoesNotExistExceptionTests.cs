// <copyright file="SceneDoesNotExistExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene.Exceptions;

using System;
using FluentAssertions;
using Velaptor.Scene.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="SceneDoesNotExistException"/> class.
/// </summary>
public class SceneDoesNotExistExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new SceneDoesNotExistException();

        // Assert
        exception.Message.Should().Be("The scene does not exist.");
    }

    [Fact]
    public void Ctor_WithSceneID_CorrectlySetsExceptionMessage()
    {
        // Arrange
        var id = new Guid("c0b7410a-356d-488c-8b62-cb48a30b76e5");
        var expected = $"The scene with the ID '{id.ToString()}' does not exist.";
        var sut = new SceneDoesNotExistException(id);

        // Act
        var actual = sut.Message;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new SceneDoesNotExistException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new SceneDoesNotExistException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
