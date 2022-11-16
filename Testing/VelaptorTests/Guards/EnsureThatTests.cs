// <copyright file="EnsureThatTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Guards;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.Guards;

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
    #endregion
}
