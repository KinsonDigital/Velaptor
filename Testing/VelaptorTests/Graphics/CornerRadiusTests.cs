// <copyright file="CornerRadiusTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using Velaptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="CornerRadius"/> struct.
    /// </summary>
    public class CornerRadiusTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_ProperlySetsProps()
        {
            // Arrange & Act
            var corners = new CornerRadius(11, 22, 33, 44);

            // Assert
            Assert.Equal(11u, corners.TopLeft);
            Assert.Equal(22u, corners.BottomLeft);
            Assert.Equal(33u, corners.BottomRight);
            Assert.Equal(44u, corners.TopRight);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(0f, 0f, 0f, 0f, true)]
        [InlineData(2f, 0f, 0f, 0f, false)]
        [InlineData(0f, 2f, 0f, 0f, false)]
        [InlineData(0f, 0f, 2f, 0f, false)]
        [InlineData(0f, 0f, 0f, 2f, false)]
        public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
            float topLeft,
            float bottomLeft,
            float bottomRight,
            float topRight,
            bool expected)
        {
            // Arrange
            var radius = new CornerRadius(topLeft, bottomLeft, bottomRight, topRight);

            // Act
            var actual = radius.IsEmpty();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Empty_WhenInvoked_ReturnsEmptyCornerRadius()
        {
            // Arrange & Act
            var actual = CornerRadius.Empty();

            // Assert
            Assert.Equal(0f, actual.TopLeft);
            Assert.Equal(0f, actual.BottomLeft);
            Assert.Equal(0f, actual.BottomRight);
            Assert.Equal(0f, actual.TopRight);
        }

        [Fact]
        public void EqualsOperator_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var radiusA = new CornerRadius(1, 2, 3, 4);
            var radiusB = new CornerRadius(1, 2, 3, 4);

            // Act
            var actual = radiusA == radiusB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var radiusA = new CornerRadius(1, 2, 3, 4);
            var radiusB = new CornerRadius(11, 22, 33, 44);

            // Act
            var actual = radiusA != radiusB;

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, true)]
        [InlineData(88, 2, 3, 4, false)]
        [InlineData(1, 88, 3, 4, false)]
        [InlineData(1, 2, 88, 4, false)]
        [InlineData(1, 2, 3, 88, false)]
        public void Equals_WithSameTypeParam_ReturnsCorrectResult(
            float topLeft,
            float bottomLeft,
            float bottomRight,
            float topRight,
            bool expected)
        {
            // Arrange
            var radiusA = new CornerRadius(1, 2, 3, 4);
            var radiusB = new CornerRadius(topLeft, bottomLeft, bottomRight, topRight);

            // Act
            var actual = radiusA.Equals(radiusB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equals_WithObjectParamOfSameType_ReturnsTrue()
        {
            // Arrange
            var radiusA = new CornerRadius(1, 2, 3, 4);
            object radiusB = new CornerRadius(1, 2, 3, 4);

            // Act
            var actual = radiusA.Equals(radiusB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WithObjectParamOfDifferentType_ReturnsFalse()
        {
            // Arrange
            var radiusA = new CornerRadius(1, 2, 3, 4);
            var radiusB = new object();

            // Act
            var actual = radiusA.Equals(radiusB);

            // Assert
            Assert.False(actual);
        }
        #endregion
    }
}
