// <copyright file="AtlasRegionRectangleTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasRegionRectangle"/> class.
    /// </summary>
    public class AtlasRegionRectangleTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsNameProp()
        {
            // Act
            var rect = new AtlasRegionRectangle("test-name", It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal("test-name", rect.Name);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsXProp()
        {
            // Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), 1234, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(1234, rect.X);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsYProp()
        {
            // Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), 1234, It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(1234, rect.Y);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsWidthProp()
        {
            // Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 1234, It.IsAny<int>());

            // Assert
            Assert.Equal(1234, rect.Width);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsHeightProp()
        {
            // Act
            var rect = new AtlasRegionRectangle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 1234);

            // Assert
            Assert.Equal(1234, rect.Height);
        }
        #endregion
    }
}
