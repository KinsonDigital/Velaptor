// <copyright file="RectShapeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129
namespace VelaptorTests.Graphics
{
    // ReSharper disable UseObjectOrCollectionInitializer
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;
    using Velaptor.Graphics;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="RectShape"/> struct.
    /// </summary>
    public class RectShapeTests
    {
        /// <summary>
        /// Provides test data for the <see cref="RectShape.IsEmpty"/> method unit test.
        /// </summary>
        /// <returns>The data to use during the test.</returns>
        public static IEnumerable<object[]> IsEmptyTestData()
        {
            yield return new object[]
            {
                Vector2.Zero, // Position
                0f, // Width
                0f, // Height
                Color.Empty, // Color
                false, // IsFilled
                1f, // Border Thickness
                new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
                ColorGradient.None, // Gradient Type
                Color.Empty, // Gradient Start
                Color.Empty, // Gradient Stop
                true, // EXPECTED
            };
        }

        #region Constructor Tests
        [Fact]
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1129:Do not use default value type constructor",
            Justification = "Unit test requires use of constructor.")]
        public void Ctor_WhenInvoked_SetsDefaultValues()
        {
            // Arrange & Act
            var rect = new RectShape();

            // Assert
            Assert.Equal(Vector2.Zero, rect.Position);
            Assert.Equal(0f, rect.Width);
            Assert.Equal(0f, rect.Height);
            Assert.Equal(Color.White, rect.Color);
            Assert.True(rect.IsFilled);
            Assert.Equal(1f, rect.BorderThickness);
            Assert.Equal(new CornerRadius(1f, 1f, 1f, 1f), rect.CornerRadius);
            Assert.Equal(ColorGradient.None, rect.GradientType);
            Assert.Equal(Color.White, rect.GradientStart);
            Assert.Equal(Color.White, rect.GradientStop);
        }
        #endregion

        #region Prop Tests
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-10f, 0)]
        [InlineData(123, 123)]
        public void Width_WhenSettingValue_ReturnsCorrectResult(float value, float expected)
        {
            // Arrange
            var rect = new RectShape();

            // Act
            rect.Width = value;
            var actual = rect.Width;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-10f, 0)]
        [InlineData(123, 123)]
        public void Height_WhenSettingValue_ReturnsCorrectResult(float value, float expected)
        {
            // Arrange
            var rect = new RectShape();

            // Act
            rect.Height = value;
            var actual = rect.Height;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(10, 20, 30, 5)] // Width < Height & Value > Smallest Dimension
        [InlineData(20, 10, 30, 5)] // Width < Height & Value < Smallest Dimension
        [InlineData(200, 200, 50, 50)] // Width == Height & Value < Smallest Dimension
        [InlineData(200, 400, -50, 1)] // Width < Height & Value Is Negative
        public void BorderThickness_WhenSettingValue_ReturnsCorrectResult(
            float width,
            float height,
            float value,
            float expected)
        {
            // Arrange
            var rect = new RectShape();
            rect.Width = width;
            rect.Height = height;

            // Act
            rect.BorderThickness = value;
            var actual = rect.BorderThickness;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(10, 20, 30, 5)] // Width < Height & Value > Smallest Dimension
        [InlineData(20, 10, 30, 5)] // Width < Height & Value < Smallest Dimension
        [InlineData(200, 200, 50, 50)] // Width == Height & Value < Smallest Dimension
        [InlineData(200, 400, -50, 0)] // Width < Height & Value Is Negative
        public void CornerRadius_WhenInvoked_ReturnsCorrectResult(
            float width,
            float height,
            float radius,
            float expected)
        {
            // Arrange
            var rect = new RectShape();
            rect.Width = width;
            rect.Height = height;

            // Act
            rect.CornerRadius = new CornerRadius(radius, radius, radius, radius);
            var actual = rect.CornerRadius;

            // Assert
            AssertExtensions.EqualWithMessage(expected, actual.TopLeft, "The top left value is incorrect.");
            AssertExtensions.EqualWithMessage(expected, actual.BottomLeft, "The bottom left value is incorrect.");
            AssertExtensions.EqualWithMessage(expected, actual.BottomRight, "The bottom right value is incorrect.");
            AssertExtensions.EqualWithMessage(expected, actual.TopRight, "The top right value is incorrect.");
        }
        #endregion

        #region Method Tests
        [Theory]
        [MemberData(nameof(IsEmptyTestData))]
        public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
            Vector2 position,
            float width,
            float height,
            Color color,
            bool isFilled,
            float borderThickness,
            CornerRadius cornerRadius,
            ColorGradient gradientType,
            Color gradientStart,
            Color gradientStop,
            bool expected)
        {
            // Arrange
            var rect = new RectShape();
            rect.Position = position;
            rect.Width = width;
            rect.Height = height;
            rect.Color = color;
            rect.IsFilled = isFilled;
            rect.BorderThickness = borderThickness;
            rect.CornerRadius = cornerRadius;
            rect.GradientType = gradientType;
            rect.GradientStart = gradientStart;
            rect.GradientStop = gradientStop;

            // Act
            var actual = rect.IsEmpty();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Empty_WhenInvoked_EmptiesStruct()
        {
            // Arrange
            var rect = new RectShape();
            rect.Position = new Vector2(1, 2);
            rect.Width = 3f;
            rect.Height = 4f;
            rect.Color = Color.FromArgb(5, 6, 7, 8);
            rect.IsFilled = true;
            rect.BorderThickness = 9f;
            rect.CornerRadius = new CornerRadius(10, 11, 12, 13);
            rect.GradientType = ColorGradient.Horizontal;
            rect.GradientStart = Color.FromArgb(14, 15, 16, 17);
            rect.GradientStop = Color.FromArgb(18, 19, 20, 21);

            // Act
            rect.Empty();

            // Assert
            Assert.Equal(Vector2.Zero, rect.Position);
            Assert.Equal(0f, rect.Width);
            Assert.Equal(0f, rect.Height);
            Assert.Equal(Color.Empty, rect.Color);
            Assert.False(rect.IsFilled);
            Assert.Equal(1f, rect.BorderThickness);
            Assert.Equal(new CornerRadius(0f, 0f, 0f, 0f), rect.CornerRadius);
            Assert.Equal(ColorGradient.None, rect.GradientType);
            Assert.Equal(Color.Empty, rect.GradientStart);
            Assert.Equal(Color.Empty, rect.GradientStop);
        }
        #endregion
    }
}
