// <copyright file="QuadDataTesting.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using OpenTK.Mathematics;
    using Raptor.OpenGL;
    using Xunit;

    public class QuadDataTesting
    {
        #region Overloaded Operator Tests
        [Fact]
        public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
        {
            // Arrange
            var quadA = default(QuadData);
            var quadB = default(QuadData);

            // Act
            var actual = quadA == quadB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
        {
            // Arrange
            var quadA = default(QuadData);
            var quadB = new QuadData()
            {
                Vertex1 = new VertexData()
                {
                    Vertex = new Vector3(11, 22, 33),
                },
            };

            // Act
            var actual = quadA != quadB;

            // Assert
            Assert.True(actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Equals_WithEqualParam_ReturnsTrue()
        {
            // Arrange
            var quadA = default(QuadData);
            var quadB = default(QuadData);

            // Act
            var actual = quadA.Equals(quadB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
        {
            // Arrange
            var quadA = default(QuadData);
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
            var quadA = default(QuadData);
            object quadB = default(QuadData);

            // Act
            var actual = quadA.Equals(quadB);

            // Assert
            Assert.True(actual);
        }
        #endregion
    }
}
