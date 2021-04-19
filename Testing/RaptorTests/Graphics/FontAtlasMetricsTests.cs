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
        [InlineData(11, 22, 33, 44, true)]
        [InlineData(1111, 22, 33, 44, false)]
        [InlineData(11, 2222, 33, 44, false)]
        [InlineData(11, 22, 3333, 44, false)]
        [InlineData(11, 22, 33, 4444, false)]
        public void Equals_WhenObjectsAreEqual_ReturnsTrue(int rows, int cols, int width, int height, bool expected)
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
