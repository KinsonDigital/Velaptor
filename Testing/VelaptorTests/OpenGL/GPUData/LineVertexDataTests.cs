// <copyright file="LineVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="LineVertexData"/> struct.
/// </summary>
public class LineVertexDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange
        var expectedPos = new Vector2(1, 2);
        var expectedClr = Color.FromArgb(3, 4, 5, 6);

        // Act
        var sut = new LineVertexData(expectedPos, expectedClr);

        // Assert
        sut.VertexPos.Should().BeEquivalentTo(expectedPos);
        sut.Color.Should().BeEquivalentTo(expectedClr);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Empty_WhenInvoked_ReturnsEmptyItem()
    {
        // Arrange & Act
        var sut = LineVertexData.Empty();

        // Assert
        sut.VertexPos.Should().BeEquivalentTo(Vector2.Zero);
        sut.Color.Should().BeEquivalentTo(Color.Empty);
    }

    [Fact]
    public void GetStride_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = LineVertexData.GetStride();

        // Assert
        actual.Should().Be(24);
    }

    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 1, 2, 4, 5, 6, 3 };

        var sut = new LineVertexData(new Vector2(1, 2), Color.FromArgb(3, 4, 5, 6));

        // Act
        var actual = sut.ToArray();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion
}
