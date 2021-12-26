// <copyright file="AtlasSubTextureDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Graphics;

namespace VelaptorTests.Graphics
{
    using System.Drawing;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasSubTextureData"/> class.
    /// </summary>
    public class AtlasSubTextureDataTests
    {
        #region Prop Tests
        [Fact]
        public void Bounds_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new Rectangle(11, 22, 33, 44);
            var data = new AtlasSubTextureData();

            // Act
            data.Bounds = new Rectangle(11, 22, 33, 44);
            var actual = data.Bounds;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Name_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var data = new AtlasSubTextureData();

            // Act
            data.Name = "test-name";
            var actual = data.Name;

            // Assert
            Assert.Equal("test-name", actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Equals_WhenComparingDifferentObjectTypes_ReturnsFalse()
        {
            // Arrange
            var dataA = new AtlasSubTextureData();
            var dataB = new object();

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenComparingObjectsWithDifferentNameProp_ReturnsFalse()
        {
            // Arrange
            var dataA = new AtlasSubTextureData();
            var dataB = new AtlasSubTextureData()
            {
                Name = "DataB",
            };

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenComparingObjectsWithDifferentBoundsProp_ReturnsFalse()
        {
            // Arrange
            var dataA = new AtlasSubTextureData();
            var dataB = new AtlasSubTextureData()
            {
                Bounds = new Rectangle(11, 22, 33, 44),
            };

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenComparingObjectsWithDifferentFrameIndexProp_ReturnsFalse()
        {
            // Arrange
            var dataA = new AtlasSubTextureData();
            var dataB = new AtlasSubTextureData()
            {
                FrameIndex = 111,
            };

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.False(actual);
        }
        #endregion
    }
}
