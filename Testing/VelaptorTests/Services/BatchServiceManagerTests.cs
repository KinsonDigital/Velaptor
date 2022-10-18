// <copyright file="BatchServiceManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace VelaptorTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Moq;
    using Velaptor.Graphics;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="BatchServiceManager"/> class.
    /// </summary>
    public class BatchServiceManagerTests
    {
        private readonly Mock<IBatchingService<TextureBatchItem>> mockTextureBatchingService;
        private readonly Mock<IBatchingService<FontGlyphBatchItem>> mockFontGlyphBatchingService;
        private readonly Mock<IBatchingService<RectShape>> mockRectBatchingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchServiceManagerTests"/> class.
        /// </summary>
        public BatchServiceManagerTests()
        {
            this.mockTextureBatchingService = new Mock<IBatchingService<TextureBatchItem>>();
            this.mockFontGlyphBatchingService = new Mock<IBatchingService<FontGlyphBatchItem>>();
            this.mockRectBatchingService = new Mock<IBatchingService<RectShape>>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullTextureBatchingServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new BatchServiceManager(
                    null,
                    new Mock<IBatchingService<FontGlyphBatchItem>>().Object,
                    new Mock<IBatchingService<RectShape>>().Object);
            }, "The parameter must not be null. (Parameter 'textureBatchingService')");
        }

        [Fact]
        public void Ctor_WithNullFontGlyphBatchingServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new BatchServiceManager(
                    new Mock<IBatchingService<TextureBatchItem>>().Object,
                    null,
                    new Mock<IBatchingService<RectShape>>().Object);
            }, "The parameter must not be null. (Parameter 'fontGlyphBatchingService')");
        }

        [Fact]
        public void Ctor_WithNullRectBatchingServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new BatchServiceManager(
                    new Mock<IBatchingService<TextureBatchItem>>().Object,
                    new Mock<IBatchingService<FontGlyphBatchItem>>().Object,
                    null);
            }, "The parameter must not be null. (Parameter 'rectBatchingService')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SubscribesToBatchFilledEvents()
        {
            // Arrange & Act
            CreateManager();

            // Assert
            this.mockTextureBatchingService.VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockFontGlyphBatchingService.VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockRectBatchingService.VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void TextureBatchItems_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batchItem = default(TextureBatchItem);
            var batchItems = new Dictionary<uint, (bool shouldRender, TextureBatchItem item)>()
            {
                { 123u, (true, batchItem) },
            };
            var expected = new ReadOnlyDictionary<uint, (bool shouldRender, TextureBatchItem item)>(batchItems);

            this.mockTextureBatchingService.SetupProperty(p => p.BatchItems);

            var manager = CreateManager();

            // Act
            manager.TextureBatchItems = new ReadOnlyDictionary<uint, (bool shouldRender, TextureBatchItem item)>(batchItems);
            var actual = manager.TextureBatchItems;

            // Assert
            this.mockTextureBatchingService.VerifySet(p => p.BatchItems = expected,
                Times.Once,
                $"The setter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)}' was not invoked.");
            this.mockTextureBatchingService.VerifyGet(p => p.BatchItems,
                Times.Once,
                $"The getter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)} was not invoked.'");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FontGlyphBatchItems_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batchItem = default(FontGlyphBatchItem);
            var batchItems = new Dictionary<uint, (bool shouldRender, FontGlyphBatchItem item)>()
            {
                { 123u, (true, batchItem) },
            };
            var expected = new ReadOnlyDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)>(batchItems);

            this.mockFontGlyphBatchingService.SetupProperty(p => p.BatchItems);

            var manager = CreateManager();

            // Act
            manager.FontGlyphBatchItems = new ReadOnlyDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)>(batchItems);
            var actual = manager.FontGlyphBatchItems;

            // Assert
            this.mockFontGlyphBatchingService.VerifySet(p => p.BatchItems = expected,
                Times.Once,
                $"The setter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)}' was not invoked.");
            this.mockFontGlyphBatchingService.VerifyGet(p => p.BatchItems,
                Times.Once,
                $"The getter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)} was not invoked.'");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RectBatchItems_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batchItem = default(RectShape);
            var batchItems = new Dictionary<uint, (bool shouldRender, RectShape item)>()
            {
                { 123u, (true, batchItem) },
            };
            var expected = new ReadOnlyDictionary<uint, (bool shouldRender, RectShape item)>(batchItems);

            this.mockRectBatchingService.SetupProperty(p => p.BatchItems);

            var manager = CreateManager();

            // Act
            manager.RectBatchItems = new ReadOnlyDictionary<uint, (bool shouldRender, RectShape item)>(batchItems);
            var actual = manager.RectBatchItems;

            // Assert
            this.mockRectBatchingService.VerifySet(p => p.BatchItems = expected,
                Times.Once,
                $"The setter for property '{nameof(IBatchingService<RectShape>)}.{nameof(IBatchingService<RectShape>.BatchItems)}' was not invoked.");
            this.mockRectBatchingService.VerifyGet(p => p.BatchItems,
                Times.Once,
                $"The getter for property '{nameof(IBatchingService<RectShape>)}.{nameof(IBatchingService<RectShape>.BatchItems)} was not invoked.'");
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(1, 123u)]
        [InlineData(2, 456u)]
        [InlineData(3, 789u)]
        public void GetBatchSize_WhenInvoked_ReturnsCorrectResult(int serviceTypeValue, uint expected)
        {
            // Arrange
            var serviceType = (BatchServiceType)serviceTypeValue;
            this.mockTextureBatchingService.SetupGet(p => p.BatchSize)
                .Returns(123u);
            this.mockFontGlyphBatchingService.SetupGet(p => p.BatchSize)
                .Returns(456u);
            this.mockRectBatchingService.SetupGet(p => p.BatchSize)
                .Returns(789u);
            var manager = CreateManager();

            // Act
            var actual = manager.GetBatchSize(serviceType);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBatchSize_WithInvalidServiceType_ThrowsException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                manager.GetBatchSize((BatchServiceType)1234);
            }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");
        }

        [Theory]
        [InlineData(1, 123u, 123u)]
        [InlineData(2, 456u, 456u)]
        [InlineData(3, 789u, 789u)]
        public void SetBatchSize_WhenInvoked_ReturnsCorrectValue(int serviceTypeValue, uint batchSize, uint expected)
        {
            // Arrange
            var serviceType = (BatchServiceType)serviceTypeValue;
            this.mockTextureBatchingService.SetupProperty(p => p.BatchSize);
            this.mockFontGlyphBatchingService.SetupProperty(p => p.BatchSize);
            this.mockRectBatchingService.SetupProperty(p => p.BatchSize);
            var manager = CreateManager();

            // Act
            manager.SetBatchSize(serviceType, batchSize);
            var actual = manager.GetBatchSize(serviceType);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetBatchSize_WithInvalidBatchServiceType_ThrowsException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                manager.SetBatchSize((BatchServiceType)1234, It.IsAny<uint>());
            }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");

            // Assert
            this.mockTextureBatchingService.VerifySet(p => p.BatchSize = It.IsAny<uint>(), Times.Never);
            this.mockFontGlyphBatchingService.VerifySet(p => p.BatchSize = It.IsAny<uint>(), Times.Never);
            this.mockRectBatchingService.VerifySet(p => p.BatchSize = It.IsAny<uint>(), Times.Never);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void EmptyBatch_WhenInvoked_EmptiesCorrectBatch(int serviceTypeValue)
        {
            // Arrange
            var serviceType = (BatchServiceType)serviceTypeValue;
            var manager = CreateManager();

            // Act
            manager.EmptyBatch(serviceType);

            // Assert
            switch (serviceType)
            {
                case BatchServiceType.Texture:
                    this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                    this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    break;
                case BatchServiceType.FontGlyph:
                    this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                    this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    break;
                case BatchServiceType.Rectangle:
                    this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                    this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                    break;
            }
        }

        [Fact]
        public void EmptyBatch_WithInvalidServiceType_ThrowsException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                manager.EmptyBatch((BatchServiceType)1234);
            }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");
        }

        [Fact]
        public void AddTextureBatchItem_WhenInvoked_AddsBatchItem()
        {
            // Arrange
            var manager = CreateManager();
            var item = default(TextureBatchItem);

            // Act
            manager.AddTextureBatchItem(item);

            // Assert
            this.mockTextureBatchingService.Verify(m => m.Add(item), Times.Once);

            // Assert that the font glyph batching service is not used
            this.mockFontGlyphBatchingService.Verify(m =>
                m.Add(It.IsAny<FontGlyphBatchItem>()), Times.Never);

            // Assert that the rectangle batching service is not used
            this.mockRectBatchingService.Verify(m =>
                m.Add(It.IsAny<RectShape>()), Times.Never);
        }

        [Fact]
        public void AddTextureBatchItem_WithFullBatch_RaisesTextureBatchFilledEvent()
        {
            // Arrange
            var batchItem = default(TextureBatchItem);
            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>((_) =>
                {
                    this.mockTextureBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);
                });

            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.TextureBatchFilled += e;
            }, e =>
            {
                manager.TextureBatchFilled -= e;
            }, () =>
            {
                manager.AddTextureBatchItem(batchItem);
            });

            this.mockTextureBatchingService.VerifyOnce(m => m.Add(batchItem));
        }

        [Fact]
        public void AddFontGlyphBatchItem_WithFullBatch_RaisesFontGlyphBatchFilledEvent()
        {
            // Arrange
            var batchItem = default(FontGlyphBatchItem);
            this.mockFontGlyphBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>((_) =>
                {
                    this.mockFontGlyphBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);
                });

            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.FontGlyphBatchFilled += e;
            }, e =>
            {
                manager.FontGlyphBatchFilled -= e;
            }, () =>
            {
                manager.AddFontGlyphBatchItem(batchItem);
            });

            this.mockFontGlyphBatchingService.VerifyOnce(m => m.Add(batchItem));
        }

        [Fact]
        public void AddRectBatchItem_WithFullBatch_RaisesFontGlyphBatchFilledEvent()
        {
            // Arrange
            var batchItem = default(RectShape);
            this.mockRectBatchingService.Setup(m => m.Add(It.IsAny<RectShape>()))
                .Callback<RectShape>((_) =>
                {
                    this.mockRectBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);
                });

            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.RectBatchFilled += e;
            }, e =>
            {
                manager.RectBatchFilled -= e;
            }, () =>
            {
                manager.AddRectBatchItem(batchItem);
            });

            this.mockRectBatchingService.VerifyOnce(m => m.Add(batchItem));
        }

        [Fact]
        public void EndBatch_WhenTextureBatch_RaisesTextureBatchFilledEvent()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.TextureBatchFilled += e;
            }, e =>
            {
                manager.TextureBatchFilled -= e;
            }, () =>
            {
                manager.EndBatch(BatchServiceType.Texture);
            });
        }

        [Fact]
        public void EndBatch_WhenFontGlyphBatch_RaisesFontGlyphBatchFilledEvent()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.FontGlyphBatchFilled += e;
            }, e =>
            {
                manager.FontGlyphBatchFilled -= e;
            }, () =>
            {
                manager.EndBatch(BatchServiceType.FontGlyph);
            });
        }

        [Fact]
        public void EndBatch_WhenRectBatch_RaisesRectBatchFilledEvent()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                manager.RectBatchFilled += e;
            }, e =>
            {
                manager.RectBatchFilled -= e;
            }, () =>
            {
                manager.EndBatch(BatchServiceType.Rectangle);
            });
        }

        [Fact]
        public void EndBatch_WithInvalidServiceTypeValue_ThrowsException()
        {
            // Arrange
            var manager = CreateManager();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
            {
                manager.EndBatch((BatchServiceType)1234);
            }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfManager()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            manager.Dispose();
            manager.Dispose();

            // Assert
            this.mockTextureBatchingService.VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockFontGlyphBatchingService.VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockRectBatchingService.VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="BatchServiceManager"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private BatchServiceManager CreateManager() =>
            new (this.mockTextureBatchingService.Object,
                this.mockFontGlyphBatchingService.Object,
                this.mockRectBatchingService.Object);
    }
}
