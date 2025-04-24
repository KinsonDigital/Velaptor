// <copyright file="LineVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Shouldly;
using Velaptor.OpenGL.GpuData;
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
        sut.VertexPos.ShouldBeEquivalentTo(expectedPos);
        sut.Color.ShouldBeEquivalentTo(expectedClr);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Empty_WhenInvoked_ReturnsEmptyItem()
    {
        // Arrange & Act
        var sut = LineVertexData.Empty();

        // Assert
        sut.VertexPos.ShouldBeEquivalentTo(Vector2.Zero);
        sut.Color.ShouldBeEquivalentTo(Color.Empty);
    }

    [Fact]
    public void GetStride_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = LineVertexData.GetStride();

        // Assert
        actual.ShouldBe(24u);
    }

    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        IEnumerable<float> expected = [1, 2, 4, 5, 6, 3];

        var sut = new LineVertexData(new Vector2(1, 2), Color.FromArgb(3, 4, 5, 6));

        // Act
        var actual = sut.ToArray();

        // Assert
        actual.ShouldBe(expected);
    }
    #endregion
}
