// <copyright file="BatchManagerServiceAssertHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.Services;

    /// <summary>
    /// Provides helpers for invoke verifications of the <see cref="IBatchManagerService"/> type.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class BatchManagerServiceAssertHelpers
    {
        /// <summary>
        /// Verifies if the <see cref="IBatchManagerService.BuildTransformationMatrix(Vector2, float, float, int, int, float, float)"/>
        /// method has been invoked with the any param values for the given number of <paramref name="times"/>.
        /// </summary>
        /// <param name="mockService">The service mock to verify against.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyAnyBuildTransformationMatrix(this Mock<IBatchManagerService> mockService, Times times)
        {
            mockService.Verify(m => m.BuildTransformationMatrix(
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>()), times);
        }

        /// <summary>
        /// Verifies if the <see cref="IBatchManagerService.BuildTransformationMatrix(Vector2, float, float, int, int, float, float)"/>
        /// method has been invoked with the correct width value for given the number of <paramref name="times"/>.
        /// </summary>
        /// <param name="mockService">The service mock to verify against.</param>
        /// <param name="width">The width to check for.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyBuildTransformationMatrixSrcRectWidth(this Mock<IBatchManagerService> mockService, int width, Times times)
        {
            mockService.Verify(m => m.BuildTransformationMatrix(
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                width,
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>()), times);
        }

        /// <summary>
        /// Verifies if the <see cref="IBatchManagerService.BuildTransformationMatrix(Vector2, float, float, int, int, float, float)"/>
        /// method has been invoked with the correct height value for given the number of <paramref name="times"/>.
        /// </summary>
        /// <param name="mockService">The service mock to verify against.</param>
        /// <param name="height">The height to check for.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyBuildTransformationMatrixSrcRectHeight(this Mock<IBatchManagerService> mockService, int height, Times times)
        {
            mockService.Verify(m => m.BuildTransformationMatrix(
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<int>(),
                height,
                It.IsAny<float>(),
                It.IsAny<float>()), times);
        }

        /// <summary>
        /// Sets up the mock to raise the <see cref="IBatchManagerService.BatchReady"/> event
        /// when the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// is invoked.
        /// </summary>
        /// <param name="mockService">The mock to setup.</param>
        public static void SetupRaiseEventWithUpdateBatch(this Mock<IBatchManagerService> mockService)
        {
            mockService.Setup(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>()))
                    .Raises(s => s.BatchReady += null, EventArgs.Empty);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given <paramref name="srcRect"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="srcRect">The source rectangle param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifySrcRectForUpdateBatch(this Mock<IBatchManagerService> mockService, Rectangle srcRect, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                srcRect,
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>()), times);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given <paramref name="destRect"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="destRect">The destination rectangle param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyDestRectForUpdateBatch(this Mock<IBatchManagerService> mockService, Rectangle destRect, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                destRect,
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>()), times);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given <paramref name="size"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="size">The size param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifySizeForUpdateBatch(this Mock<IBatchManagerService> mockService, float size, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                size,
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>()), times);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given <paramref name="angle"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="angle">The angle param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyAngleForUpdateBatch(this Mock<IBatchManagerService> mockService, float angle, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                angle,
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>()), times);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="color">The color param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyColorForUpdateBatch(this Mock<IBatchManagerService> mockService, Color color, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                color,
                It.IsAny<RenderEffects>()), times);
        }

        /// <summary>
        /// Verifies that the <see cref="IBatchManagerService.UpdateBatch(ITexture, Rectangle, Rectangle, float, float, Color, RenderEffects)"/>
        /// method is invoked the given number of <paramref name="times"/> with the given render <paramref name="effects"/>.
        /// </summary>
        /// <param name="mockService">The mock to verify against.</param>
        /// <param name="effects">The effects param to check.</param>
        /// <param name="times">The total number of times to expect the invocation to occur.</param>
        public static void VerifyRenderEffectForUpdateBatch(this Mock<IBatchManagerService> mockService, RenderEffects effects, Times times)
        {
            mockService.Verify(m => m.UpdateBatch(
                It.IsAny<ITexture>(),
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                effects), times);
        }
    }
}
