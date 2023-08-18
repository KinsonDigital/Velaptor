// <copyright file="ShapeGpuDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Linq;
using FluentAssertions;
using Helpers;
using Velaptor.OpenGL.GpuData;
using Xunit;

/// <summary>
/// Tests the <see cref="ShapeGpuData"/> struct.
/// </summary>
public class ShapeGpuDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperties()
    {
        // Arrange
        var vertex1 = ShapeGpuDataGenerator.GenerateVertexData(1, out var next);
        var vertex2 = ShapeGpuDataGenerator.GenerateVertexData(next, out next);
        var vertex3 = ShapeGpuDataGenerator.GenerateVertexData(next, out next);
        var vertex4 = ShapeGpuDataGenerator.GenerateVertexData(next, out next);

        // Act
        var data = new ShapeGpuData(vertex1, vertex2, vertex3, vertex4);

        // Assert
        data.Vertex1.Should().Be(vertex1);
        data.Vertex2.Should().Be(vertex2);
        data.Vertex3.Should().Be(vertex3);
        data.Vertex4.Should().Be(vertex4);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const string testDataFileName = $"{nameof(ShapeGpuData)}TestData.json";
        var expected = TestDataLoader
            .LoadTestData<(string name, int index, float value)[]>(string.Empty, testDataFileName)
            .Select(i => i.value).ToArray();

        var data = ShapeGpuDataGenerator.GenerateGpuData(1);

        // Act
        var actual = data.ToArray();

        // Assert
        actual.Length.Should().Be(64);
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion
}
