// <copyright file="LineGPUDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="LineGPUData"/> struct.
/// </summary>
public class LineGPUDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsProps()
    {
        // Arrange
        var expectedV1 = CreateVertexData(1);
        var expectedV2 = CreateVertexData(2);
        var expectedV3 = CreateVertexData(3);
        var expectedV4 = CreateVertexData(4);

        // Act
        var sut = new LineGPUData(expectedV1, expectedV2, expectedV3, expectedV4);

        // Assert
        sut.Vertex1.Should().Be(expectedV1);
        sut.Vertex2.Should().Be(expectedV2);
        sut.Vertex3.Should().Be(expectedV3);
        sut.Vertex4.Should().Be(expectedV4);
    }

    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = LineGPUData.GetTotalBytes();

        // Assert
        actual.Should().Be(96);
    }

    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const string testDataFileName = $"{nameof(LineGPUDataTests)}.{nameof(ToArray_WhenInvoked_ReturnsCorrectResult)}.json";
        var expected = TestDataLoader
            .LoadTestData<float[]>(string.Empty, testDataFileName);

        var allVertexData = CreateAllOrderedVertexData(0);

        var sut = new LineGPUData(allVertexData[0], allVertexData[1], allVertexData[2], allVertexData[3]);

        // Act
        var actual = sut.ToArray();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    /// <summary>
    /// Creates four <see cref="LineVertexData"/> items with all of its data components
    /// in a numerical order that is relative to the given <paramref name="start"/> value.
    /// </summary>
    /// <param name="start">The starting numerical value for the data components.</param>
    /// <returns>The data to use for testing.</returns>
    private LineVertexData[] CreateAllOrderedVertexData(int start)
    {
        var v1 = CreateOrderedVertexData(start);
        var v2 = CreateOrderedVertexData(v1.maxValue + 1);
        var v3 = CreateOrderedVertexData(v2.maxValue + 1);
        var v4 = CreateOrderedVertexData(v3.maxValue + 1);

        return new[] { v1.result, v2.result, v3.result, v4.result };
    }

    /// <summary>
    /// Creates a new vertex data object with all values set to the given <paramref name="allValues"/>.
    /// </summary>
    /// <param name="allValues">The value to set.</param>
    /// <returns>The data with all of vertex data components set to the same value.</returns>
    private LineVertexData CreateVertexData(int allValues) =>
        new (new Vector2(allValues, allValues),
            Color.FromArgb(allValues, allValues, allValues, allValues));

    /// <summary>
    /// Creates a single <see cref="LineVertexData"/> item with all of its data components
    /// in a numerical order that is relative to the given <paramref name="start"/> value.
    /// </summary>
    /// <param name="start">The starting numerical value for the data components.</param>
    /// <returns>The data to use for testing.</returns>
    private (LineVertexData result, int maxValue) CreateOrderedVertexData(int start)
    {
        var currentValue = start;

        var vectorResult = CreateOrderedVector(currentValue);
        currentValue = vectorResult.maxValue + 1;

        var colorResult = CreateOrderedColor(currentValue);
        currentValue = colorResult.maxValue;

        var newData = new LineVertexData(vectorResult.result, colorResult.result);

        return (newData, currentValue);
    }

    /// <summary>
    /// Creates a single <see cref="Vector2"/> item with all of its data components
    /// in a numerical order that is relative to the given <paramref name="start"/> value.
    /// </summary>
    /// <param name="start">The starting numerical value for the data components.</param>
    /// <returns>The data to use for testing.</returns>
    private (Vector2 result, int maxValue) CreateOrderedVector(int start)
        => (new Vector2(start, start + 1), start + 1);

    /// <summary>
    /// Creates a single <see cref="Color"/> item with all of its data components
    /// in a numerical order that is relative to the given <paramref name="start"/> value.
    /// </summary>
    /// <param name="start">The starting numerical value for the data components.</param>
    /// <returns>The data to use for testing.</returns>
    private (Color result, int maxValue) CreateOrderedColor(int start)
        => (Color.FromArgb(start, start + 1, start + 2, start + 3), start + 3);
}
