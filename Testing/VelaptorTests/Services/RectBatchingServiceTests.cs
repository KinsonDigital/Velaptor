// <copyright file="RectBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="RectBatchService"/> class.
    /// </summary>
    public class RectBatchingServiceTests
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
            var batchItem1 = (true, new RectShape
            {
                Position = new Vector2(1f, 2f),
                Width = 3f,
                Height = 4f,
                Color = Color.FromArgb(5, 6, 7, 8),
                IsFilled = true,
                BorderThickness = 9,
                CornerRadius = new CornerRadius(10, 11, 12, 13),
                GradientType = ColorGradient.None,
                GradientStart = Color.FromArgb(14, 15, 16, 17),
                GradientStop = Color.FromArgb(18, 19, 20, 21),
            });
            var batchItem2 = (true, new RectShape()
            {
                Position = new Vector2(22f, 23f),
                Width = 24f,
                Height = 25f,
                Color = Color.FromArgb(26, 27, 28, 29),
                IsFilled = true,
                BorderThickness = 30,
                CornerRadius = new CornerRadius(31, 32, 33, 34),
                GradientType = ColorGradient.None,
                GradientStart = Color.FromArgb(35, 36, 37, 38),
                GradientStop = Color.FromArgb(39, 40, 41, 42),
            });

            var batchItems = new List<(bool, RectShape)> { batchItem1, batchItem2 };
            var expected = new ReadOnlyDictionary<uint, (bool, RectShape)>(batchItems.ToDictionary());
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
        public void Add_WhenBatchIsFull_InvokesBatchFilledEvent()
        {
            // Arrange
            var batchItem1 = default(RectShape);
            var batchItem2 = default(RectShape);

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
            var batchItem1 = default(RectShape);
            var batchItem2 = default(RectShape);

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
            var batchItem1 = default(RectShape);
            var batchItem2 = default(RectShape);

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
            var batchItem1 = default(RectShape);
            var batchItem2 = default(RectShape);

            var service = CreateService();
            service.BatchSize = 2;
            service.BatchItems = new List<(bool, RectShape)> { (false, batchItem1), (false, batchItem2) }.ToReadOnlyDictionary();

            // Act
            service.EmptyBatch();

            // Assert
            Assert.Equal(batchItem1, service.BatchItems[0].item);
            Assert.Equal(batchItem2, service.BatchItems[1].item);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="RectBatchService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private static RectBatchService CreateService() => new ();

    }
}
