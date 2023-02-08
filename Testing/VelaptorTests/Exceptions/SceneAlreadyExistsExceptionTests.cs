// <copyright file="SceneAlreadyExistsExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using FluentAssertions;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="SceneAlreadyExistsException"/> class.
/// </summary>
public class SceneAlreadyExistsExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new SceneAlreadyExistsException();

        // Assert
        exception.Message.Should().Be("The scene already exists.");
    }

    [Theory]
    [InlineData(
        "test-scene",
        "2ab9a4ff-03cb-46e6-8e5f-99989567d968",
        "The scene 'test-scene' with the ID '2ab9a4ff-03cb-46e6-8e5f-99989567d968' already exists.")]
    [InlineData(
        null,
        "b2c7123b-6044-4c6d-b79b-4086f3f89939",
        "The scene with the ID 'b2c7123b-6044-4c6d-b79b-4086f3f89939' already exists.")]
    [InlineData(
        "",
        "c0b7410a-356d-488c-8b62-cb48a30b76e5",
        "The scene with the ID 'c0b7410a-356d-488c-8b62-cb48a30b76e5' already exists.")]
    public void Ctor_WithSceneNameAndID_CorrectlySetsExceptionMessage(
        string name,
        Guid id,
        string expected)
    {
        // Arrange
        var sut = new SceneAlreadyExistsException(name, id);

        // Act
        var actual = sut.Message;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new SceneAlreadyExistsException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new SceneAlreadyExistsException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
    }
    #endregion
}
