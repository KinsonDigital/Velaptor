// <copyright file="PublicExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests
{
    using System.Drawing;
    using System.Numerics;
    using Velaptor;
    using Xunit;

    public class PublicExtensionMethodsTests
    {
        #region Method Tests
        [Fact]
        public void ForcePositive_WhenUsingNegativeValue_ReturnsPositiveResult()
        {
            // Act & Assert
            Assert.Equal(123f, (-123f).ForcePositive());
        }

        [Fact]
        public void ForcePositive_WhenUsingPositiveValue_ReturnsPositiveResult()
        {
            // Act & Assert
            Assert.Equal(123f, 123f.ForcePositive());
        }

        [Fact]
        public void ForceNegative_WhenUsingPositiveValue_ReturnsNegativeResult()
        {
            // Act & Assert
            Assert.Equal(-123f, 123f.ForceNegative());
        }

        [Fact]
        public void ForceNegative_WhenUsingNegativeValue_ReturnsNegativeResult()
        {
            // Act & Assert
            Assert.Equal(-123f, (-123f).ForceNegative());
        }

        [Fact]
        public void ToRadians_WhenInvoking_ReturnsCorrectResult()
        {
            // Act & Assert
            Assert.Equal(70710.06f, 1234.1234f.ToDegrees());
        }

        [Fact]
        public void RotateAround_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            const float angle = 13f;
            var expected = new Vector2(1.25290489f, -0.996605873f);

            // Act
            var actual = vectorToRotate.RotateAround(origin, angle);

            // Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }

        [Fact]
        public void RotateAround_WhenInvokedWithClockwiseFalse_ReturnsCorrectResult()
        {
            // Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            var angle = 45f;
            var expected = new Vector2(-2.07106781f, 5f);

            // Act
            var actual = vectorToRotate.RotateAround(origin, angle, false);

            // Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }

        [Fact]
        public void ToVector4_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var color = Color.FromArgb(11, 22, 33, 44);
            var expected = new Vector4(22, 33, 44, 11);

            // Act
            var actual = color.ToVector4();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(126.5f, 0f, 255f, 0f, 1f, 0.49607843f)]
        [InlineData(500f, 0f, 1000f, 200f, 400f, 300f)]
        public void MapValue_WhenUsingFloatType_ReturnsCorrectResult(
            float testValue,
            float fromStart,
            float fromStop,
            float toStart,
            float toStop,
            float expected)
        {
            // Act
            var actual = testValue.MapValue(fromStart, fromStop, toStart, toStop);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(5, 0, 10, 0, 100, 50)]
        [InlineData(126, 0, 255, 0, 1, 0)]
        public void MapValue_WhenUsingByteValues_ReturnsCorrectResult(
            byte testValue,
            byte fromStart,
            byte fromStop,
            byte toStart,
            byte toStop,
            byte expected)
        {
            // Act
            var actual = testValue.MapValue(fromStart, fromStop, toStart, toStop);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MapValue_WhenInvokedWithIntegerType_ReturnsCorrectResult()
        {
            // Arrange
            const int testValue = 500;

            // Act
            var actual = testValue.MapValue(0, 1_000, 0, 100_000);

            // Assert
            Assert.Equal(50_000, actual);
        }

        [Theory]
        [InlineData(10u, 30f)]
        [InlineData(0u, 0f)]
        public void ApplySize_WithUnsignedInt_ReturnsCorrectResult(uint value, float expected)
        {
            // Act
            var actual = value.ApplySize(2f);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(10f, 30f)]
        [InlineData(0f, 0f)]
        public void ApplySize_WithFloat_ReturnsCorrectResult(float value, float expected)
        {
            // Act
            var actual = value.ApplySize(2f);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ApplySize_WithSizeF_ReturnsCorrectResult()
        {
            // Arrange
            var rect = new SizeF(10, 20);

            // Act
            var actual = rect.ApplySize(2f);

            // Assert
            Assert.Equal(30f, actual.Width);
            Assert.Equal(60f, actual.Height);
        }

        [Fact]
        public void ApplySize_WithRectangleF_ReturnsCorrectResult()
        {
            // Arrange
            var rect = new RectangleF(1, 2, 10, 20);

            // Act
            var actual = rect.ApplySize(2f);

            // Assert
            Assert.Equal(3f, actual.X);
            Assert.Equal(6f, actual.Y);
            Assert.Equal(30f, actual.Width);
            Assert.Equal(60f, actual.Height);
        }
        #endregion
    }
}
