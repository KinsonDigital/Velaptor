// <copyright file="EnsureThatTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable ExplicitCallerInfoArgument
namespace VelaptorTests.Guards;

using System;
using FluentAssertions;
using Velaptor.Guards;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="EnsureThat"/> class.
/// </summary>
public class EnsureThatTests
{
    #region Method Tests
    [Fact]
    public void ParamIsNotNull_WithNullValue_ThrowsException()
    {
        // Arrange
        object? nullObj = null;

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            EnsureThat.ParamIsNotNull(nullObj);
        }, "The parameter must not be null. (Parameter 'nullObj')");
    }

    [Fact]
    public void ParamIsNotNull_WithNonNullValue_DoesNotThrowException()
    {
        // Arrange
        object nonNullObj = "non-null-obj";

        // Act & Assert
        AssertExtensions.DoesNotThrow<Exception>(() =>
        {
            EnsureThat.ParamIsNotNull(nonNullObj);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void StringParamIsNotNullOrEmpty_WhenInvoked_ThrowsException(string value)
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            EnsureThat.StringParamIsNotNullOrEmpty(value);
        }, $"The string parameter must not be null or empty. (Parameter '{nameof(value)}')");
    }

    [Fact]
    public void PointerIsNotNull_WithNonZeroIntPointer_DoesNotThrowException()
    {
        // Arrange
        nint pointer = 123;

        // Act
        var act = () => EnsureThat.PointerIsNotNull(pointer);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0, "pointer", "The pointer parameter 'pointer' cannot be a value of zero.")]
    [InlineData(0, "", "The pointer cannot be a value of zero.")]
    public void PointerIsNotNull_WithZeroIntPointer_ThrowsException(
        nint pointer,
        string paramName,
        string expected)
    {
        // Arrange & Act
        var act = () => EnsureThat.PointerIsNotNull(pointer, paramName);

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(123u)]
    public void PointerIsNotNull_WithNonZeroUIntPointer_DoesNotThrowException(nuint pointer)
    {
        // Arrange & Act
        var act = () => EnsureThat.PointerIsNotNull(pointer);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0u, "pointer", "The pointer parameter 'pointer' cannot be a value of zero.")]
    [InlineData(0u, "", "The pointer cannot be a value of zero.")]
    public void PointerIsNotNull_WithZeroUIntPointer_ThrowsException(
        nuint pointer,
        string paramName,
        string expected)
    {
        // Arrange & Act
        var act = () => EnsureThat.PointerIsNotNull(pointer, paramName);

        // Assert
        act.Should().Throw<NullReferenceException>()
            .WithMessage(expected);
    }
    #endregion
}
