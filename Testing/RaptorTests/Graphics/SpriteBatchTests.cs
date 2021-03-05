// <copyright file="SpriteBatchTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System;
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SpriteBatch"/> class.
    /// </summary>
    public class SpriteBatchTests : IDisposable
    {
        private readonly Mock<ITexture> mockTextureOne;
        private readonly Mock<ITexture> mockTextureTwo;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly Mock<IShaderProgram> mockShader;
        private readonly Mock<IGPUBuffer> mockBuffer;
        private readonly Mock<IFile> mockFile;
        private readonly SpriteBatch batch;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchTests"/> class.
        /// </summary>
        public SpriteBatchTests()
        {
            this.mockTextureOne = new Mock<ITexture>();
            this.mockTextureOne.SetupGet(p => p.Width).Returns(10);
            this.mockTextureOne.SetupGet(p => p.Height).Returns(20);

            this.mockTextureTwo = new Mock<ITexture>();
            this.mockTextureTwo.SetupGet(p => p.ID).Returns(1);
            this.mockTextureTwo.SetupGet(p => p.Width).Returns(10);
            this.mockTextureTwo.SetupGet(p => p.Height).Returns(20);

            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockGLInvoker.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvoker.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);

            this.mockShader = new Mock<IShaderProgram>();

            this.mockBuffer = new Mock<IGPUBuffer>();

            this.mockFile = new Mock<IFile>();

            this.batch = CreateSpriteBatch();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(null, this.mockShader.Object, this.mockBuffer.Object);
            }, $"The '{nameof(IGLInvoker)}' must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullShader_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(this.mockGLInvoker.Object, null, this.mockBuffer.Object);
            }, $"The '{nameof(IShaderProgram)}' must not be null. (Parameter 'shader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGPUBuffer_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(this.mockGLInvoker.Object, this.mockShader.Object, null);
            }, $"The '{nameof(IGPUBuffer)}' must not be null. (Parameter 'gpuBuffer')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public unsafe void Width_WhenSettingValueWithOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(11, 22));
            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            this.batch.RenderSurfaceWidth = 100;
            _ = this.batch.RenderSurfaceWidth;

            // Assert
            this.mockGLInvoker.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLInvoker.Verify(m => m.SetViewPortSize(new Vector2(100, 22)), Times.Once());
        }

        [Fact]
        public unsafe void Height_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(11, 22));
            IGLInvoker.SetOpenGLAsInitialized();

            // Act
            this.batch.RenderSurfaceHeight = 100;
            _ = this.batch.RenderSurfaceHeight;

            // Assert
            this.mockGLInvoker.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLInvoker.Verify(m => m.SetViewPortSize(new Vector2(11, 100)), Times.Once());
        }

        [Fact]
        public void BatchSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.batch.BatchSize = 3;

            // Act
            var actual = this.batch.BatchSize;

            // Assert
            Assert.Equal(3u, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Clear_WhenInvoked_ClearsBuffer()
        {
            // Act
            this.batch.Clear();

            // Assert
            this.mockGLInvoker.Verify(m => m.Clear(ClearBufferMask.ColorBufferBit), Times.Once());
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                this.batch.Render(this.mockTextureOne.Object, 10, 20);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParamsAndNullTexture_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                this.batch.BeginBatch();
                this.batch.Render(null, 10, 20);
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParams_RendersBatch()
        {
            // Arrange
            this.batch.BatchSize = 1;

            this.batch.BeginBatch();

            // Act
            this.batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            this.batch.EndBatch();
            this.batch.EndBatch();

            // Assert
            AssertBatchRendered(1, 1, 1, 1);
        }

        [Fact]
        public void Render_WhenExcedingBatchSize_RendersBatchTwoTimes()
        {
            // Arrange
            this.batch.BatchSize = 1;

            this.batch.BeginBatch();

            // Act
            this.batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            this.batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            // Assert
            AssertBatchRendered(1, 1, 1, 1);
        }

        [Fact]
        public void Render_WhenUsingOverloadWithSixParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var srcRect = new Rectangle(1, 2, 3, 4);
                var destRect = new Rectangle(5, 6, 7, 8);
                var tintClr = Color.FromArgb(11, 22, 33, 44);

                this.batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithSixParamsAndWithNullTexture_ThrowsException()
        {
            // Arrange

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 10, 20);
                this.batch.BeginBatch();
                this.batch.Render(null, srcRect, It.IsAny<Rectangle>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<Color>());
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WithNoSourceRectWidth_ThrowsExceptoin()
        {
            // Arrange
            this.batch.BeginBatch();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 0, 20);
                var destRect = new Rectangle(10, 20, 100, 200);

                this.batch.Render(this.mockTextureOne.Object, srcRect, destRect, 1, 1, Color.White);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void Render_WithNoSourceRectHeight_ThrowsExceptoin()
        {
            // Arrange
            this.batch.BeginBatch();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 10, 0);
                var destRect = new Rectangle(10, 20, 100, 200);

                this.batch.Render(this.mockTextureOne.Object, srcRect, destRect, 1, 1, Color.White);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void Render_WhenSwitchingTextures_RendersBatchOnce()
        {
            // Arrange
            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(10, 20));
            this.batch.RenderSurfaceWidth = 10;
            this.batch.RenderSurfaceHeight = 20;
            this.batch.BatchSize = 3;

            // Act
            // WARNING - Changing these values will change the trans matrix
            var srcRect = new Rectangle(1, 2, 3, 4);
            var destRect = new Rectangle(5, 6, 7, 8);
            var tintClr = Color.FromArgb(11, 22, 33, 44);

            this.batch.BeginBatch();
            this.batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            this.batch.Render(this.mockTextureTwo.Object, srcRect, destRect, 0.5f, 90, tintClr);
            var transMatrix = new Matrix4()
            {
                Column0 = new Vector4(-6.5567085E-09f, 0.15f, 0f, 0f),
                Column1 = new Vector4(-0.1f, -4.371139E-09F, 0f, 0.39999998f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f),
            };

            // Assert
            AssertBatchRendered(1, 1, 1, 1, transMatrix);
        }

        [Fact]
        public void Render_WhenBatchIsFull_RendersBatch()
        {
            // Arrange
            this.batch.RenderSurfaceWidth = 10;
            this.batch.RenderSurfaceHeight = 20;
            this.batch.BatchSize = 2u;

            // Act
            this.batch.BeginBatch();
            this.batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());
            this.batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());
            this.batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());

            // Assert
            AssertBatchRendered(2, 2u, 1, 1);
        }

        [Fact]
        public void Render_WhenBatchIsNotFull_OnlyRendersRequiredItems()
        {
            // Arrange
            this.batch.BatchSize = 2;

            // Act
            this.batch.BeginBatch();

            this.batch.Render(
                this.mockTextureOne.Object,
                new Rectangle(1, 2, 3, 4),
                new Rectangle(5, 6, 7, 8),
                9f,
                10f,
                Color.FromArgb(11, 22, 33, 44));

            this.batch.EndBatch();

            // Assert
            AssertBatchRendered(1, 1, 1, 1);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Act
            this.batch.Dispose();
            this.batch.Dispose();

            // Assert
            this.mockShader.Verify(m => m.Dispose(), Times.Once());
            this.mockBuffer.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose() => this.batch.Dispose();

        /// <summary>
        /// Creates a new instance of <see cref="SpriteBatch"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private SpriteBatch CreateSpriteBatch() => new SpriteBatch(this.mockGLInvoker.Object, this.mockShader.Object, this.mockBuffer.Object);

        /// <summary>
        /// Assert that a single batch was rendered the given amount of <paramref name="totalBatchUpdates"/>.
        /// </summary>
        /// <param name="totalItemsInBatch">The total amount of textures to be expected in the batch.</param>
        /// <param name="totalBatchUpdates">The total amount of batch data updates.</param>
        private void AssertBatchRendered(uint totalItemsInBatch, uint totalBatchUpdates, uint totalTextureBinds, uint totalDrawCalls)
            => AssertBatchRendered(totalItemsInBatch, totalBatchUpdates, totalTextureBinds, totalDrawCalls, default);

        /// <summary>
        /// Assert that a single batch was rendered the given amount of <paramref name="totalBatchUpdates"/>.
        /// </summary>
        /// <param name="totalItemsInBatch">The total amount of textures to be expected in the batch.</param>
        /// <param name="totalBatchUpdates">The total amount of batch data updates.</param>
        /// <param name="transform">The transform that was sent to the GPU.  An empty transform means any transform data would assert true.</param>
        private void AssertBatchRendered(uint totalItemsInBatch, uint totalBatchUpdates, uint totalTextureBinds, uint totalDrawCalls, Matrix4 transform)
        {
            this.mockGLInvoker.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<uint>()),
                Times.Exactly((int)totalTextureBinds),
                "Did not bind texture");

            if (transform.IsEmpty())
            {
                // Verify with any transform
                AssertTransformUpdate(totalBatchUpdates);
            }
            else
            {
                // Verify with given transform
                AssertTransformUpdate(totalBatchUpdates, transform);
            }

            // Invoked in the GPUBuffer.UpdateQuad() method
            this.mockBuffer.Verify(m => m.UpdateQuad(It.IsAny<uint>(),
                                                     It.IsAny<Rectangle>(),
                                                     It.IsAny<int>(),
                                                     It.IsAny<int>(),
                                                     It.IsAny<Color>()),
                Times.Exactly((int)totalBatchUpdates),
                "Quad was not updated on GPU.");

            this.mockGLInvoker.Verify(m => m.DrawElements(PrimitiveType.Triangles,
                    6 * totalItemsInBatch,
                    DrawElementsType.UnsignedInt,
                    IntPtr.Zero),
                Times.Exactly((int)totalDrawCalls),
                $"Expected total draw calls of {totalDrawCalls} not reached.");
        }

        /// <summary>
        /// Asserts that the OpenGL.<see cref="UniformMatrix4(uint, bool, ref Matrix4)"/>
        /// method is invoked the given amount of times.
        /// </summary>
        /// <param name="times">The amount of times that it is invoked.</param>
        private void AssertTransformUpdate(uint times)
        {
            // Verify with any transform
            this.mockGLInvoker.Verify(m => m.UniformMatrix4(It.IsAny<uint>(), true, ref It.Ref<Matrix4>.IsAny),
                Times.Exactly((int)times),
                "Transformation matrix not updated on GPU");
        }

        /// <summary>
        /// Asserts that the OpenGL.<see cref="UniformMatrix4(uint, bool, ref Matrix4)"/>
        /// method is invoked the given amount of times.
        /// </summary>
        /// <param name="times">The amount of times that it is invoked.</param>
        /// <param name="transform">The transform to be used in each invoke.</param>
        private void AssertTransformUpdate(uint times, Matrix4 transform)
        {
            // Verify with given transform
            this.mockGLInvoker.Verify(m => m.UniformMatrix4(It.IsAny<uint>(), true, ref transform),
                Times.Exactly((int)times),
                "Transformation matrix not updated on GPU");
        }
    }
}
