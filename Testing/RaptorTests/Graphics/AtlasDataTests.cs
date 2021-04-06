// <copyright file="AtlasDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System;
    using System.Drawing;
    using Moq;
    using Raptor.Graphics;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasData"/> class.
    /// </summary>
    public class AtlasDataTests
    {
        private readonly AtlasSubTextureData[] spriteData;
        private readonly Mock<ITexture> mockTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataTests"/> class.
        /// </summary>
        public AtlasDataTests()
        {
            this.spriteData = new AtlasSubTextureData[]
            {
                new AtlasSubTextureData() // First frame of Animating sub texture
                {
                    Name = "sub-texture",
                    FrameIndex = 0,
                    Bounds = new Rectangle(11, 22, 33, 44),
                },
                new AtlasSubTextureData() // Second frame of Animating sub texture
                {
                    Name = "sub-texture",
                    FrameIndex = 1,
                    Bounds = new Rectangle(55, 66, 77, 88),
                },
                new AtlasSubTextureData() // Non animating sub texture
                {
                    Name = "other-sub-texture",
                    FrameIndex = -1,
                    Bounds = new Rectangle(111, 222, 333, 444),
                },
            };

            this.mockTexture = new Mock<ITexture>();
            this.mockTexture.SetupGet(p => p.Width).Returns(100);
            this.mockTexture.SetupGet(p => p.Height).Returns(200);
        }

        #region Prop Tests
        [Fact]
        public void Ctor_WhenInvoked_ReturnsCorrectFrameNameList()
        {
            // Arrange
            /* NOTE: In the constructor of this test, the items are added by frame index order 0, 1 then -1.
             * In the AtlasData ctor, it auto sorts the data by frame index from lowest to highest.  This is
             * why the order of names comes in the order shown in the expected array
            */
            var expected = new[]
            {
                "other-sub-texture",
                "sub-texture",
            };

            var data = CreateAtlasData();

            // Act
            var actual = data.SubTextureNames;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Name_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var data = CreateAtlasData();

            // Act
            var actual = data.Name;

            // Assert
            Assert.Equal("test-atlas", actual);
        }

        [Fact]
        public void Width_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var data = CreateAtlasData();

            // Act
            var actual = data.Width;

            // Assert
            Assert.Equal(100, actual);
        }

        [Fact]
        public void Height_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var data = CreateAtlasData();

            // Act
            var actual = data.Height;

            // Assert
            Assert.Equal(200, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Iterator_WhenGettingValueAtIndex_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new AtlasSubTextureData()
            {
                Name = "sub-texture",
                Bounds = new Rectangle(55, 66, 77, 88),
                FrameIndex = 1,
            };

            var data = CreateAtlasData();

            // Act
            /* NOTE: In the constructor of this test, the items are added by frame index order 0, 1 then -1.
             * In the AtlasData ctor, it auto sorts the data by frame index from lowest to highest.  This is
             * why the last item in the list of data is the correct item
            */
            var actual = data[2];

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFrames_WhenInvokedWithExistingSubTextureID_ReturnsCorrectFrameRectangle()
        {
            // Arrange
            var expected = new AtlasSubTextureData[]
            {
                new AtlasSubTextureData() // First frame of Animating sub texture
                {
                    Name = "sub-texture",
                    FrameIndex = 0,
                    Bounds = new Rectangle(11, 22, 33, 44),
                },
                new AtlasSubTextureData() // Second frame of Animating sub texture
                {
                    Name = "sub-texture",
                    FrameIndex = 1,
                    Bounds = new Rectangle(55, 66, 77, 88),
                },
            };

            var data = CreateAtlasData();

            // Act
            var actual = data.GetFrames("sub-texture");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFrame_WhenSubTextureIDDoesNotExist_ThrowsException()
        {
            // Arrange
            var expected = new AtlasSubTextureData[]
            {
                new AtlasSubTextureData() // First frame of Animating sub texture
                {
                    Name = "sub-texture",
                    FrameIndex = 0,
                    Bounds = new Rectangle(11, 22, 33, 44),
                },
            };

            var data = CreateAtlasData();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                data.GetFrame("missing-texture");
            }, "The frame 'missing-texture' was not found in the atlas 'test-atlas'.");
        }

        [Fact]
        public void GetFrame_WithExistingSubTexture_ReturnsSubTextureData()
        {
            // Arrange
            var expected = new AtlasSubTextureData()
            {
                Name = "sub-texture",
                FrameIndex = 0,
                Bounds = new Rectangle(11, 22, 33, 44),
            };

            var data = CreateAtlasData();

            // Act
            var actual = data.GetFrame("sub-texture");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTexture()
        {
            // Arrange
            var data = CreateAtlasData();

            // Act
            data.Dispose();
            data.Dispose();

            // Assert
            this.mockTexture.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AtlasData"/> for testing purposes.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private AtlasData CreateAtlasData() => new AtlasData(this.mockTexture.Object, this.spriteData, "test-atlas", $@"C:\temp\test-atlas.png");
    }
}
