// <copyright file="TextureBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System.Collections.Generic;
    using System.Drawing;
    using Velaptor.Graphics;
    using Velaptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Test the <see cref="TextureBatchItem"/> struct.
    /// </summary>
    public class TextureBatchItemTests
    {
        /// <summary>
        /// Gets all of the test data related to testing the end result
        /// values for the batch items to be rendered.
        /// </summary>
        /// <returns>The test data.</returns>
        public static IEnumerable<object[]> GetBatchItemData()
        {
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                true, // Expected Equal Result
            };
            yield return new object[]
            {
                45, // Angle <-- THIS ONE IS DIFFERENT
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.FlipHorizontally, //Render Effects <-- THIS ONE IS DIFFERENT
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                20, // Size <-- THIS ONE IS DIFFERENT
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(33, 44, 55, 66), // Dest Rect <-- THIS ONE IS DIFFERENT
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(77, 88, 99, 100), // Src Rect <-- THIS ONE IS DIFFERENT
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                111, // TextureId <-- THIS ONE IS DIFFERENT
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(120, 130, 140, 150), // Tint Color <-- THIS ONE IS DIFFERENT
                new SizeF(16, 17), // Viewport Size
                false, // Expected Equal Result
            };
            yield return new object[]
            {
                1, // Angle
                RenderEffects.None, //Render Effects
                2, // Size
                new RectangleF(3, 4, 5, 6), // Dest Rect
                new RectangleF(7, 8, 9, 10), // Src Rect
                11, // TextureId
                Color.FromArgb(12, 13, 14, 15), // Tint Color
                new SizeF(160, 170), // Viewport Size <-- THIS ONE IS DIFFERENT
                false, // Expected Equal Result
            };
        }

        #region Method Tests
        [Fact]
        public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue()
        {
            // Arrange
            var item = default(TextureBatchItem);

            // Act
            var actual = item.IsEmpty();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Empty_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batchItem = default(TextureBatchItem);
            batchItem.Angle = 1;
            batchItem.Effects = RenderEffects.None;
            batchItem.Size = 2;
            batchItem.DestRect = new RectangleF(3, 4, 5, 6);
            batchItem.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItem.TextureId = 11;
            batchItem.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItem.ViewPortSize = new SizeF(16, 17);

            // Act
            batchItem.Empty();

            // Assert
            Assert.Equal(0u, batchItem.TextureId);
            Assert.Equal(Rectangle.Empty, batchItem.SrcRect);
            Assert.Equal(Rectangle.Empty, batchItem.DestRect);
            Assert.Equal(0f, batchItem.Size);
            Assert.Equal(0f, batchItem.Angle);
            Assert.Equal(Color.Empty, batchItem.TintColor);
        }

        [Theory]
        [MemberData(nameof(GetBatchItemData))]
        public void Equals_WhenUsingBatchItemParamOverload_ReturnsCorrectResult(
            float angle,
            RenderEffects effects,
            float size,
            RectangleF destRect,
            RectangleF srcRect,
            uint textureId,
            Color tintColor,
            SizeF viewPortSize,
            bool expected)
        {
            // Arrange
            var batchItemA = default(TextureBatchItem);
            batchItemA.Angle = 1;
            batchItemA.Effects = RenderEffects.None;
            batchItemA.Size = 2;
            batchItemA.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemA.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemA.TextureId = 11;
            batchItemA.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemA.ViewPortSize = new SizeF(16, 17);

            var batchItemB = default(TextureBatchItem);
            batchItemB.Angle = angle;
            batchItemB.Effects = effects;
            batchItemB.Size = size;
            batchItemB.DestRect = destRect;
            batchItemB.SrcRect = srcRect;
            batchItemB.TextureId = textureId;
            batchItemB.TintColor = tintColor;
            batchItemB.ViewPortSize = viewPortSize;

            // Act
            var actual = batchItemA.Equals(batchItemB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EqualsOperator_WithEqualOperands_ReturnsTrue()
        {
            // Arrange
            var batchItemA = default(TextureBatchItem);
            batchItemA.Angle = 1;
            batchItemA.Effects = RenderEffects.None;
            batchItemA.Size = 2;
            batchItemA.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemA.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemA.TextureId = 11;
            batchItemA.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemA.ViewPortSize = new SizeF(16, 17);

            var batchItemB = default(TextureBatchItem);
            batchItemB.Angle = 1;
            batchItemB.Effects = RenderEffects.None;
            batchItemB.Size = 2;
            batchItemB.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemB.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemB.TextureId = 11;
            batchItemB.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemB.ViewPortSize = new SizeF(16, 17);

            // Act
            var actual = batchItemA == batchItemB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WithUnequalOperands_ReturnsTrue()
        {
            // Arrange
            var batchItemA = default(TextureBatchItem);
            batchItemA.Angle = 1;
            batchItemA.Effects = RenderEffects.None;
            batchItemA.Size = 2;
            batchItemA.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemA.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemA.TextureId = 11;
            batchItemA.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemA.ViewPortSize = new SizeF(16, 17);

            var batchItemB = default(TextureBatchItem);
            batchItemB.Angle = 11;
            batchItemB.Effects = RenderEffects.None;
            batchItemB.Size = 22;
            batchItemB.DestRect = new RectangleF(33, 44, 55, 66);
            batchItemB.SrcRect = new RectangleF(77, 88, 99, 100);
            batchItemB.TextureId = 110;
            batchItemB.TintColor = Color.FromArgb(120, 130, 140, 150);
            batchItemB.ViewPortSize = new SizeF(160, 170);

            // Act
            var actual = batchItemA != batchItemB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenUsingObjectParamOverloadWithMatchingType_ReturnsTrue()
        {
            // Arrange
            var batchItemA = default(TextureBatchItem);
            batchItemA.Angle = 1;
            batchItemA.Effects = RenderEffects.None;
            batchItemA.Size = 2;
            batchItemA.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemA.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemA.TextureId = 11;
            batchItemA.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemA.ViewPortSize = new SizeF(16, 17);

            object batchItemB = new TextureBatchItem()
            {
                Angle = 1,
                Effects = RenderEffects.None,
                Size = 2,
                DestRect = new RectangleF(3, 4, 5, 6),
                SrcRect = new RectangleF(7, 8, 9, 10),
                TextureId = 11,
                TintColor = Color.FromArgb(12, 13, 14, 15),
                ViewPortSize = new SizeF(16, 17),
            };

            // Act
            var actual = batchItemA.Equals(batchItemB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenUsingObjectParamOverloadWithDifferentType_ReturnsFalse()
        {
            // Arrange
            var batchItemA = default(TextureBatchItem);
            batchItemA.Angle = 1;
            batchItemA.Effects = RenderEffects.None;
            batchItemA.Size = 2;
            batchItemA.DestRect = new RectangleF(3, 4, 5, 6);
            batchItemA.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItemA.TextureId = 11;
            batchItemA.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItemA.ViewPortSize = new SizeF(16, 17);

            var batchItemB = new object();

            // Act
            var actual = batchItemA.Equals(batchItemB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ToString_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var expected = "Texture Batch Item Values:";
            expected += "\r\nSrc Rect: {X=7,Y=8,Width=9,Height=10}";
            expected += "\r\nDest Rect: {X=3,Y=4,Width=5,Height=6}";
            expected += "\r\nSize: 2";
            expected += "\r\nAngle: 1";
            expected += "\r\nTint Clr: {A=12,R=13,G=14,B=15}";
            expected += "\r\nEffects: None";
            expected += "\r\nView Port Size: {W=16,H=17}";
            expected += "\r\nTexture ID: 11";

            var batchItem = default(TextureBatchItem);
            batchItem.Angle = 1;
            batchItem.Effects = RenderEffects.None;
            batchItem.Size = 2;
            batchItem.DestRect = new RectangleF(3, 4, 5, 6);
            batchItem.SrcRect = new RectangleF(7, 8, 9, 10);
            batchItem.TextureId = 11;
            batchItem.TintColor = Color.FromArgb(12, 13, 14, 15);
            batchItem.ViewPortSize = new SizeF(16, 17);

            // Act
            var actual = batchItem.ToString();

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
