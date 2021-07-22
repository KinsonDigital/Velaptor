// <copyright file="FontAtlasMetricsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using Raptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontAtlasMetrics"/> struct.
    /// </summary>
    public class FontAtlasMetricsTests
    {
        #region Method Tests
        [Theory]
        [InlineData(11u, 22u, 33u, 44u, true)]
        [InlineData(1111u, 22u, 33u, 44u, false)]
        [InlineData(11u, 2222u, 33u, 44u, false)]
        [InlineData(11u, 22u, 3333u, 44u, false)]
        [InlineData(11u, 22u, 33u, 4444u, false)]
        public void Equals_WhenObjectsAreEqual_ReturnsTrue(uint rows, uint cols, uint width, uint height, bool expected)
        {
            // Arrange
            var metricsA = default(FontAtlasMetrics);
            metricsA.Rows = 11;
            metricsA.Columns = 22;
            metricsA.Width = 33;
            metricsA.Height = 44;

            var metricsB = default(FontAtlasMetrics);
            metricsB.Rows = rows;
            metricsB.Columns = cols;
            metricsB.Width = width;
            metricsB.Height = height;

            // Act
            var actual = metricsA.Equals(metricsB);

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
