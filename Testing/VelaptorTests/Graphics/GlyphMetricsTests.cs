// <copyright file="GlyphMetricsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="GlyphMetrics"/> struct.
    /// </summary>
    public class GlyphMetricsTests
    {
        #region Method Tests
        [Fact]
        public void ToString_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var metrics = default(GlyphMetrics);
            metrics.AtlasBounds = new Rectangle(11, 22, 33, 44);
            metrics.Glyph = 'Z';

            // Act
            var actual = metrics.ToString();

            // Assert
            Assert.Equal("Name: Z | Bounds: {X=11,Y=22,Width=33,Height=44}", actual);
        }
        #endregion
    }
}
