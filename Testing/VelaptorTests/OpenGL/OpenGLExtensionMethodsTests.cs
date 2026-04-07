// <copyright file="OpenGLExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System.Numerics;
using Shouldly;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the helper methods in the <see cref="OpenGLExtensionMethods"/> class.
/// </summary>
public class OpenGLExtensionMethodsTests
{
    #region Method Tests
    [Fact]
    public void ToNDC_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var vector = new Vector2(75, 150);

        // Act
        var actual = vector.ToNDC(100, 200);

        // Assert
        actual.X.ShouldBe(0.5f);
        actual.Y.ShouldBe(-0.5f);
    }

    [Fact]
    public void ToNDCTextureCoordX_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float value = 75f;

        // Act
        var actual = value.ToNDCTextureCoordX(100);

        // Assert
        actual.ShouldBe(0.75f);
    }

    [Fact]
    public void ToNDCTextureCoordY_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float value = 75f;

        // Act
        var actual = value.ToNDCTextureCoordY(100);

        // Assert
        actual.ShouldBe(0.25f);
    }

    [Fact]
    public void ToNDCTextureCoords_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var coord = new Vector2(75f, 75f);

        // Act
        var actual = coord.ToNDCTextureCoords(100, 100);

        // Assert
        actual.X.ShouldBe(0.75f);
        actual.Y.ShouldBe(0.25f);
    }
    #endregion
}
