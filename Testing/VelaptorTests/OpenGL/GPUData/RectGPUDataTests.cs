// <copyright file="RectGPUDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData
{
    using Velaptor.OpenGL.GPUData;
    using VelaptorTests.Helpers;
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
            Assert.Equal(vertex1, data.Vertex1);
            Assert.Equal(vertex2, data.Vertex2);
            Assert.Equal(vertex3, data.Vertex3);
            Assert.Equal(vertex4, data.Vertex4);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void ToArray_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var testDataFileName = $"{nameof(RectGPUData)}TestData.json";
            var expected = TestDataLoader
                .LoadTestData<(string name, int index, float value)>(string.Empty, testDataFileName);

            var data = RectGPUDataGenerator.GenerateGPUData(1);

            // Act
            var actual = data.ToArray();

            // Assert
            Assert.Equal(64, actual.Length);
            Assert.All(expected, expectedValue =>
            {
                var (name, index, value) = expectedValue;
                Assert.True(index <= actual.Length, $"Missing data item at index {index}");
                AssertExtensions.EqualWithMessage(value, actual[index], name);
            });
        }
        #endregion
    }
}
