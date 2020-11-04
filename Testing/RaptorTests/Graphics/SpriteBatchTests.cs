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

    public class SpriteBatchTests
    {
        private readonly Mock<ITexture> mockTextureOne;
        private readonly Mock<ITexture> mockTextureTwo;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IShaderProgram> mockShader;
        private readonly Mock<IGPUBuffer> mockBuffer;
        private readonly Mock<IFile> mockFile;

        public SpriteBatchTests()
        {
            this.mockTextureOne = new Mock<ITexture>();
            this.mockTextureTwo = new Mock<ITexture>();
            this.mockTextureTwo.SetupGet(p => p.ID).Returns(1);

            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGL.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);

            this.mockShader = new Mock<IShaderProgram>();

            this.mockBuffer = new Mock<IGPUBuffer>();

            this.mockFile = new Mock<IFile>();
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
                var buffer = new SpriteBatch(this.mockGL.Object, null, this.mockBuffer.Object);
            }, $"The '{nameof(IShaderProgram)}' must not be null. (Parameter 'shader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGPUBuffer_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, null);
            }, $"The '{nameof(IGPUBuffer)}' must not be null. (Parameter 'gpuBuffer')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsUpShaderProgram()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockShader.Verify(m => m.UseProgram(), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_EnablesBlending()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockGL.Verify(m => m.Enable(EnableCap.Blend), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsUpBlending()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockGL.Verify(m => m.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsUpClearColor()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockGL.Verify(m => m.ClearColor(0.2f, 0.3f, 0.3f, 1.0f), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsTextureUnitToSlot0()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockGL.Verify(m => m.ActiveTexture(TextureUnit.Texture0), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_GetTransformLocation()
        {
            // Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Assert
            this.mockGL.Verify(m => m.GetUniformLocation(It.IsAny<uint>(), "uTransform"), Times.Once());
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Render_WhenUsingOverloadWithFourParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                batch.Render(this.mockTextureOne.Object, 10, 20);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParamsAndNullTexture_ThrowsException()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.BeginBatch();
                batch.Render(null, 10, 20);
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParams_RendersBatch()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object)
            {
                BatchSize = 1,
            };

            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            batch.EndBatch();
            batch.EndBatch();

            // Assert
            AssertBatchRendered(1, 1, 1, 1);
        }

        [Fact]
        public void Render_WhenExcedingBatchSize_RendersBatchTwoTimes()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object)
            {
                BatchSize = 1,
            };
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            batch.Render(
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
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var srcRect = new Rectangle(1, 2, 3, 4);
                var destRect = new Rectangle(5, 6, 7, 8);
                var tintClr = Color.FromArgb(11, 22, 33, 44);

                batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithSixParamsAndWithNullTexture_ThrowsException()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.BeginBatch();
                batch.Render(null, It.IsAny<Rectangle>(), It.IsAny<Rectangle>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<Color>());
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WhenSwitchingTextures_RendersBatchOnce()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object)
            {
                RenderSurfaceWidth = 10,
                RenderSurfaceHeight = 20,
                BatchSize = 3,
            };

            // Act
            // WARNING - Changing these values will change the trans matrix
            var srcRect = new Rectangle(1, 2, 3, 4);
            var destRect = new Rectangle(5, 6, 7, 8);
            var tintClr = Color.FromArgb(11, 22, 33, 44);

            batch.BeginBatch();
            batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            batch.Render(this.mockTextureTwo.Object, srcRect, destRect, 0.5f, 90, tintClr);
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
            var batchSize = 2u;
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object)
            {
                RenderSurfaceWidth = 10,
                RenderSurfaceHeight = 20,
                BatchSize = batchSize,
            };

            // Act
            batch.BeginBatch();
            batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());
            batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());
            batch.Render(this.mockTextureOne.Object, It.IsAny<int>(), It.IsAny<int>());

            // Assert
            AssertBatchRendered(2, batchSize, 1, 1);
        }

        [Fact]
        public void Render_WhenBatchIsNotFull_OnlyRendersRequiredItems()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object)
            {
                BatchSize = 2,
            };

            // Act
            batch.BeginBatch();

            batch.Render(
                this.mockTextureOne.Object,
                new Rectangle(1, 2, 3, 4),
                new Rectangle(5, 6, 7, 8),
                9f,
                10f,
                Color.FromArgb(11, 22, 33, 44));

            batch.EndBatch();

            // Assert
            AssertBatchRendered(1, 1, 1, 1);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockShader.Object, this.mockBuffer.Object);

            // Act
            batch.Dispose();
            batch.Dispose();

            // Assert
            this.mockShader.Verify(m => m.Dispose(), Times.Once());
            this.mockBuffer.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

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
            this.mockGL.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<uint>()),
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

            this.mockGL.Verify(m => m.DrawElements(PrimitiveType.Triangles,
                    6 * totalItemsInBatch,
                    DrawElementsType.UnsignedInt,
                    IntPtr.Zero),
                Times.Exactly((int)totalDrawCalls),
                $"Expected total draw calls of {totalDrawCalls} not reached.");
        }

        private void AssertTransformUpdate(uint times)
        {
            // Verify with any transform
            this.mockGL.Verify(m => m.UniformMatrix4(It.IsAny<uint>(), true, ref It.Ref<Matrix4>.IsAny),
                Times.Exactly((int)times),
                "Transformation matrix not updated on GPU");
        }

        private void AssertTransformUpdate(uint times, Matrix4 transform)
        {
            // Verify with given transform
            this.mockGL.Verify(m => m.UniformMatrix4(It.IsAny<uint>(), true, ref transform),
                Times.Exactly((int)times),
                "Transformation matrix not updated on GPU");
        }
    }
}
