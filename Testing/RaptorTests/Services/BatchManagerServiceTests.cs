// <copyright file="BatchManagerServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0002 // Name can be simplified
namespace RaptorTests.Services
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using System.Drawing;
    using System.Linq;
    using Moq;
    using OpenTK.Mathematics;
    using Raptor.Graphics;
    using Raptor.Services;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
    using SysVector2 = System.Numerics.Vector2;
    using TKVector4 = OpenTK.Mathematics.Vector4;
#pragma warning restore IDE0001 // Name can be simplified

    public class BatchManagerServiceTests
    {
        #region Prop Tests
        [Fact]
        public void BatchSize_WhenGettingValue_ReturnsDefaultResult()
        {
            // Arrange
            var service = new BatchManagerService();

            // Act
            var actual = service.BatchSize;

            // Assert
            Assert.Equal(10u, actual);
        }

        [Fact]
        public void EntireBatchEmpty_WithEntireBatchAsEmpty_ReturnsTrue()
        {
            // Arrange
            var service = new BatchManagerService();

            // Act
            var actual = service.EntireBatchEmpty;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void TotalItemsToRender_WithSingleNonEmptyItem_ReturnsCorrectResult()
        {
            // Arrange
            var mockTextureA = CreateTextureMock(1, 10, 20);
            var mockTextureB = CreateTextureMock(2, 10, 20);

            var service = new BatchManagerService();
            service.BatchSize = 2;

            service.UpdateBatch(
                mockTextureA.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            service.UpdateBatch(
                mockTextureB.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act
            var actual = service.TotalItemsToRender;

            // Assert
            Assert.Equal(1u, actual);
        }

        [Fact]
        public void EntireBatchEmpty_WithNonEmptyBatch_ReturnsFalse()
        {
            // Arrange
            var mockTextureA = CreateTextureMock(1, 10, 20);

            var service = new BatchManagerService();
            service.BatchSize = 2;

            service.UpdateBatch(
                mockTextureA.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act
            var actual = service.EntireBatchEmpty;

            // Assert
            Assert.False(actual);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(0f)]
        [InlineData(-1f)]
        public void BuildTransformationMatrix_WithInvalidPortSizeX_ThrowsException(float vectorX)
        {
            // Arrange
            var service = new BatchManagerService();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                service.BuildTransformationMatrix(
                    new SysVector2(vectorX, 20),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<float>(),
                    It.IsAny<float>());
            });
        }

        [Theory]
        [InlineData(0f)]
        [InlineData(-1f)]
        public void BuildTransformationMatrix_WithInvalidPortSizeY_ThrowsException(float vectorY)
        {
            // Arrange
            var service = new BatchManagerService();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                service.BuildTransformationMatrix(
                    new SysVector2(10, vectorY),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<float>(),
                    It.IsAny<float>());
            });
        }

        [Fact]
        public void BuildTransformationMatrix_WhenInvoked_ReturnsCorrectMatrix()
        {
            // Arrange
            var expected = default(Matrix4);
            expected.Row0 = new TKVector4(1.4917829f, -0.104528464f, 0f, 0f);
            expected.Row1 = new TKVector4(0.1567927f, 0.9945219f, 0f, 0f);
            expected.Row2 = new TKVector4(0f, 0f, 1f, 0f);
            expected.Row3 = new TKVector4(-0.8f, 0.8f, 0f, 1f);

            var service = new BatchManagerService();

            // Act
            var actual = service.BuildTransformationMatrix(
                            new SysVector2(10, 20),
                            x: 1,
                            y: 2,
                            width: 3,
                            height: 4,
                            size: 5,
                            angle: 6);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateBatch_WithNullTexture_ThrowsException()
        {
            // Arrange
            var service = new BatchManagerService();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                service.UpdateBatch(
                    null,
                    It.IsAny<Rectangle>(),
                    It.IsAny<Rectangle>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>(),
                    It.IsAny<RenderEffects>());
            }, "The parameter must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void UpdateBatch_WhenInvoked_CreatesBatchItemsToMatchSize()
        {
            // Arrange
            var service = new BatchManagerService();
            service.BatchSize = 2;

            // Act
            service.UpdateBatch(
                new Mock<ITexture>().Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Assert
            Assert.Equal(2, service.BatchItems.Count);
        }

        [Fact]
        public void UpdateBatch_WithSwitchedTexture_InvokesBatchReadyEvent()
        {
            // Arrange
            var mockTextureA = CreateTextureMock(1, 10, 20);
            var mockTextureB = CreateTextureMock(2, 10, 20);

            var service = new BatchManagerService();
            service.UpdateBatch(
                mockTextureA.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act & Assert
            Assert.Raises<EventArgs>(
                e => service.BatchReady += e,
                e => service.BatchReady -= e,
                () =>
                {
                    service.UpdateBatch(
                        mockTextureB.Object,
                        It.IsAny<Rectangle>(),
                        It.IsAny<Rectangle>(),
                        It.IsAny<float>(),
                        It.IsAny<float>(),
                        It.IsAny<Color>(),
                        It.IsAny<RenderEffects>());
                });
        }

        [Fact]
        public void UpdateBatch_WhenBatchIsFull_InvokesBatchReadyEvent()
        {
            // Arrange
            var mockTextureA = CreateTextureMock(1, 10, 20);

            var service = new BatchManagerService();
            service.BatchSize = 1;

            service.UpdateBatch(
                mockTextureA.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act & Assert
            Assert.Raises<EventArgs>(
                e => service.BatchReady += e,
                e => service.BatchReady -= e,
                () =>
                {
                    service.UpdateBatch(
                        mockTextureA.Object,
                        It.IsAny<Rectangle>(),
                        It.IsAny<Rectangle>(),
                        It.IsAny<float>(),
                        It.IsAny<float>(),
                        It.IsAny<Color>(),
                        It.IsAny<RenderEffects>());
                });
        }

        [Fact]
        public void EmptyBatch_WhenInvoked_EmptiesEntireBatch()
        {
            // Arrange
            var mockTextureA = CreateTextureMock(1, 10, 20);

            var service = new BatchManagerService();
            service.BatchSize = 4;

            service.UpdateBatch(
                mockTextureA.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act
            service.EmptyBatch();
            var actual = service.BatchItems.All(i => i.Value.IsEmpty);

            // Assert
            Assert.True(actual);
        }
        #endregion

        /// <summary>
        /// Creates a texture mock for the purpose of testing.
        /// </summary>
        /// <param name="textureId">The ID of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>The texture mock to use for testing.</returns>
        private static Mock<ITexture> CreateTextureMock(uint textureId, int width, int height)
        {
            var result = new Mock<ITexture>();

            result.SetupGet(p => p.ID).Returns(textureId);
            result.SetupGet(p => p.Width).Returns(width);
            result.SetupGet(p => p.Height).Returns(height);

            return result;
        }
    }
}
