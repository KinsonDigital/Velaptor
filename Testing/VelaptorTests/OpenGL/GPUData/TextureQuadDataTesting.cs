// <copyright file="TextureQuadDataTesting.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Numerics;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureQuadData"/> struct.
/// </summary>
public class TextureQuadDataTesting
{
    #region Overloaded Operator Tests
    [Fact]
    public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
    {
        // Arrange
        var quadA = default(TextureQuadData);
        var quadB = default(TextureQuadData);

        // Act
        var actual = quadA == quadB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
    {
        // Arrange
        var quadA = default(TextureQuadData);
        var quadB = new TextureQuadData(
            new TextureVertexData(
                new Vector2(11, 22),
                default,
                default),
            default,
            default,
            default);

        // Act
        var actual = quadA != quadB;

        // Assert
        Assert.True(actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureQuadData.GetTotalBytes();

        // Assert
        Assert.Equal(128u, actual);
    }

    [Fact]
    public void Equals_WithEqualParam_ReturnsTrue()
    {
        // Arrange
        var quadA = default(TextureQuadData);
        var quadB = default(TextureQuadData);

        // Act
        var actual = quadA.Equals(quadB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var quadA = default(TextureQuadData);
        var quadB = new object();

        // Act
        var actual = quadA.Equals(quadB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenInvokedWithEqualParamOfSameType_ReturnsTrue()
    {
        // Arrange
        var quadA = default(TextureQuadData);
        object quadB = default(TextureQuadData);

        // Act
        var actual = quadA.Equals(quadB);

        // Assert
        Assert.True(actual);
    }
    #endregion
}
