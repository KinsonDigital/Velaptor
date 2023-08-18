// <copyright file="TextureGpuDataTesting.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Numerics;
using FluentAssertions;
using Velaptor.OpenGL.GpuData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureGpuData"/> struct.
/// </summary>
public class TextureGpuDataTesting
{
    #region Overloaded Operator Tests
    [Fact]
    public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        var sutB = default(TextureGpuData);

        // Act
        var actual = sutA == sutB;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        var sutB = new TextureGpuData(
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
        actual.Should().BeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureGpuData.GetTotalBytes();

        // Assert
        actual.Should().Be(128u);
    }

    [Fact]
    public void Equals_WithEqualParam_ReturnsTrue()
    {
        // Arrange
        var sut = default(TextureGpuData);
        var dataB = default(TextureGpuData);

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var sut = default(TextureGpuData);
        var dataB = new object();

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenInvokedWithEqualParamOfSameType_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        object sutB = default(TextureGpuData);

        // Act
        var actual = sutA.Equals(sutB);

        // Assert
        actual.Should().BeTrue();
    }
    #endregion
}
