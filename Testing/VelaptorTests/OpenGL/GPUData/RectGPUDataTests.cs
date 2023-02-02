// <copyright file="RectGPUDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Linq;
using FluentAssertions;
using Helpers;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="RectGPUData"/> struct.
/// </summary>
public class RectGPUDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperties()
    {
        // Arrange
        var vertex1 = RectGPUDataGenerator.GenerateVertexData(1, out var next);
        var vertex2 = RectGPUDataGenerator.GenerateVertexData(next, out next);
        var vertex3 = RectGPUDataGenerator.GenerateVertexData(next, out next);
        var vertex4 = RectGPUDataGenerator.GenerateVertexData(next, out next);

        // Act
        var data = new RectGPUData(vertex1, vertex2, vertex3, vertex4);

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
        const string testDataFileName = $"{nameof(RectGPUData)}TestData.json";
        var expected = TestDataLoader
            .LoadTestData<(string name, int index, float value)[]>(string.Empty, testDataFileName)
            .Select(i => i.value).ToArray();

        var data = RectGPUDataGenerator.GenerateGPUData(1);

        // Act
        var actual = data.ToArray();

        // Assert
        actual.Length.Should().Be(64);
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion
}
