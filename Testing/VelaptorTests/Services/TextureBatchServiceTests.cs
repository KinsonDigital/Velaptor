// <copyright file="TextureBatchServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    // ReSharper disable UseObjectOrCollectionInitializer
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using Velaptor.Graphics;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureBatchService"/> class.
    /// </summary>
    public class TextureBatchServiceTests
    {
        #region Prop Tests
        [Fact]
        public void BatchSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var service = CreateService();

            // Act
            service.BatchSize = 123u;
            var actual = service.BatchSize;

            // Assert
            Assert.Equal(123u, actual);
        }

        [Fact]
        public void BatchItems_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batchItem1 = (true, new SpriteBatchItem
            {
                Angle = 1,
                Effects = RenderEffects.None,
                Size = 2,
                DestRect = new RectangleF(3, 4, 5, 6),
                SrcRect = new RectangleF(7, 8, 9, 10),
                TextureId = 11,
                TintColor = Color.FromArgb(12, 13, 14, 15),
                ViewPortSize = new SizeF(16, 17),
            });
            var batchItem2 = (true, new SpriteBatchItem
            {
                Angle = 18,
                Effects = RenderEffects.FlipHorizontally,
                Size = 19,
                DestRect = new RectangleF(20, 21, 22, 23),
                SrcRect = new RectangleF(24, 25, 26, 27),
                TextureId = 28,
                TintColor = Color.FromArgb(29, 30, 31, 32),
                ViewPortSize = new SizeF(33, 34),
            });

            var batchItems = new List<(bool, SpriteBatchItem)> { batchItem1, batchItem2 };
            var expected = new ReadOnlyDictionary<uint, (bool, SpriteBatchItem)>(batchItems.ToDictionary());
            var service = CreateService();

            // Act
            service.BatchItems = batchItems.ToReadOnlyDictionary();
            var actual = service.BatchItems;

            // Assert
            AssertExtensions.ItemsEqual(expected.Keys.ToArray(), actual.Keys.ToArray());
            AssertExtensions.ItemsEqual(expected.Values.ToArray(), actual.Values.ToArray());
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Add_WhenSwitchingTextures_InvokesBatchFilledEvent()
        {
            // Arrange
            var batchItem1 = default(SpriteBatchItem);
            batchItem1.TextureId = 10;
            var batchItem2 = default(SpriteBatchItem);
            batchItem2.TextureId = 20;

            var service = CreateService();
            service.BatchSize = 100;
            service.Add(batchItem1);

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                service.BatchFilled += e;
            }, e =>
            {
                service.BatchFilled -= e;
            }, () =>
            {
                service.Add(batchItem2);
            });
        }

        [Fact]
        public void Add_WhenBatchIsFull_InvokesBatchFilledEvent()
        {
            // Arrange
            var batchItem1 = default(SpriteBatchItem);
            batchItem1.TextureId = 10;
            var batchItem2 = default(SpriteBatchItem);
            batchItem2.TextureId = 10;

            var service = CreateService();
            service.BatchSize = 1;
            service.Add(batchItem1);

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                service.BatchFilled += e;
            }, e =>
            {
                service.BatchFilled -= e;
            }, () =>
            {
                service.Add(batchItem2);
            });

            Assert.Equal(2, service.BatchItems.Count);
        }

        [Fact]
        public void AddRange_WhenInvoked_InvokesBatchFilledEvent()
        {
            // Arrange
            var batchItem1 = default(SpriteBatchItem);
            batchItem1.TextureId = 10;
            var batchItem2 = default(SpriteBatchItem);
            batchItem2.TextureId = 10;

            var service = CreateService();
            service.BatchSize = 1;

            // Act & Assert
            Assert.Raises<EventArgs>(e =>
            {
                service.BatchFilled += e;
            }, e =>
            {
                service.BatchFilled -= e;
            }, () =>
            {
                service.AddRange(new[] { batchItem1, batchItem2 });
            });

            Assert.Equal(2, service.BatchItems.Count);
        }

        [Fact]
        public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
        {
            // Arrange
            var batchItem1 = default(SpriteBatchItem);
            batchItem1.TextureId = 10;
            var batchItem2 = default(SpriteBatchItem);
            batchItem2.TextureId = 10;

            var service = CreateService();
            service.BatchSize = 2;
            service.AddRange(new[] { batchItem1, batchItem2 });

            // Act
            service.EmptyBatch();

            // Assert
            Assert.NotEqual(batchItem1, service.BatchItems[0].item);
        }

        [Fact]
        public void EmptyBatch_WithNoItemsReadyToRender_DoesNotEmptyItems()
        {
            // Arrange
            var batchItem1 = default(SpriteBatchItem);
            batchItem1.TextureId = 10;
            var batchItem2 = default(SpriteBatchItem);
            batchItem2.TextureId = 10;

            var service = CreateService();
            service.BatchSize = 2;
            service.BatchItems = new List<(bool, SpriteBatchItem)> { (false, batchItem1), (false, batchItem2) }.ToReadOnlyDictionary();

            // Act
            service.EmptyBatch();

            // Assert
            Assert.Equal(batchItem1, service.BatchItems[0].item);
            Assert.Equal(batchItem2, service.BatchItems[1].item);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureBatchService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private static TextureBatchService CreateService() => new ();
    }
}
