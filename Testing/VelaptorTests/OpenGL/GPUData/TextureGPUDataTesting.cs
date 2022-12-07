// <copyright file="TextureGPUDataTesting.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Numerics;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureGPUData"/> struct.
/// </summary>
public class TextureGPUDataTesting
{
    #region Overloaded Operator Tests
    [Fact]
    public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGPUData);
        var sutB = default(TextureGPUData);

        // Act
        var actual = sutA == sutB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
    {
        // Arrange
        var sutA = default(TextureGPUData);
        var sutB = new TextureGPUData(
            new TextureVertexData(
                new Vector2(11, 22),
                default,
                default),
            default,
            default,
            default);

        // Act
        var actual = sutA != sutB;

        // Assert
        Assert.True(actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureGPUData.GetTotalBytes();

        // Assert
        Assert.Equal(128u, actual);
    }

    [Fact]
    public void Equals_WithEqualParam_ReturnsTrue()
    {
        // Arrange
        var sut = default(TextureGPUData);
        var dataB = default(TextureGPUData);

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var sut = default(TextureGPUData);
        var dataB = new object();

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenInvokedWithEqualParamOfSameType_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGPUData);
        object sutB = default(TextureGPUData);

        // Act
        var actual = sutA.Equals(sutB);

        // Assert
        Assert.True(actual);
    }
    #endregion
}
