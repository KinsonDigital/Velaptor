using FileIO.Core;
using Moq;
using OpenToolkit.Graphics.OpenGL4;
using Raptor.Graphics;
using Raptor.OpenGL;
using RaptorTests.Helpers;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using OpenToolkit.Mathematics;
using System.Diagnostics.CodeAnalysis;

namespace RaptorTests.Graphics
{
    public class SpriteBatchTests
    {
        private readonly Mock<ITexture> mockTextureOne;
        private readonly Mock<ITexture> mockTextureTwo;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<ITextFile> mockTextFile;

        public SpriteBatchTests()
        {
            this.mockTextureOne = new Mock<ITexture>();
            this.mockTextureTwo = new Mock<ITexture>();

            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.ShaderCompileSuccess(It.IsAny<int>())).Returns(true);
            this.mockGL.Setup(m => m.LinkProgramSuccess(It.IsAny<int>())).Returns(true);

            this.mockTextFile = new Mock<ITextFile>();
        }

        //[Fact]
        public void Ctor_WhenInvoked_SetsUpShaderProgram()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.UseProgram(It.IsAny<int>()), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_EnablesBlending()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.Enable(EnableCap.Blend), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_SetsUpBlending()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_SetsUpClearColor()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.ClearColor(0.2f, 0.3f, 0.3f, 1.0f), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_SetsTextureUnitToSlot0()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.ActiveTexture(TextureUnit.Texture0), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_GetTransformLocation()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Assert
            this.mockGL.Verify(m => m.GetUniformLocation(It.IsAny<int>(), "uTransform"), Times.Once());
        }

        //[Fact]
        public void Ctor_WhenInvoked_SetsUpGPUBuffer()
        {
            //Act
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            batch.Begin();

            batch.Render(this.mockTextureOne.Object, 10, 20);

            batch.End();

            //Assert
            this.mockGL.Verify(m => m.BufferSubData(It.IsAny<BufferTarget>(),
                                                    It.IsAny<IntPtr>(),
                                                    It.IsAny<int>(),
                                                    It.IsAny<QuadData>()), Times.Once());
        }

        [Fact]
        public void Render_WhenUsingOverloadWithFourParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                batch.Render(this.mockTextureOne.Object, 10, 20);
            }, $"The '{nameof(SpriteBatch.Begin)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWith4ParamsAndNullTexture_ThrowsException()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);
            
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.Begin();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                batch.Render(null, 10, 20);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WhenUsingOverloadWith4Params_RendersBatch()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20, 2);

            batch.Begin();

            //Act
            batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            batch.End();
            batch.End();

            //Assert
            AssertBatchRendered(2, 1, 1);
        }

        [Fact]
        public void Render_WhenUsingOverloadWithSixParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var srcRect = new Rectangle(1, 2, 3, 4);
                var destRect = new Rectangle(5, 6, 7, 8);
                var tintClr = Color.FromArgb(11, 22, 33, 44);

                batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            }, $"The '{nameof(SpriteBatch.Begin)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void Render_WhenUsingOverloadWithSixParamsAndWithNullTexture_ThrowsException()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20);

            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.Begin();
                batch.Render(null, It.IsAny<Rectangle>(), It.IsAny<Rectangle>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<Color>());
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Render_WhenSwitchingTextures_RendersBatch()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 20, 1);

            //Act
            //WARNING - Changing these values will change the trans matrix
            var srcRect = new Rectangle(1, 2, 3, 4);
            var destRect = new Rectangle(5, 6, 7, 8);
            var tintClr = Color.FromArgb(11, 22, 33, 44);

            batch.Begin();
            batch.Render(this.mockTextureOne.Object, srcRect, destRect, 0.5f, 90, tintClr);
            batch.Render(this.mockTextureTwo.Object, srcRect, destRect, 0.5f, 90, tintClr);
            var transMatrix = new Matrix4()
            {
                Column0 = new Vector4(-6.5567085E-09f, 0.15f, 0f, 0f),
                Column1 = new Vector4(-0.1f, -4.371139E-09F, 0f, 0.39999998f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f)
            };

            //Assert
            AssertBatchRendered(2, 1, 1, transMatrix);
        }

        [Fact]
        public void Render_WhenBatchIsNotFull_OnlyRendersRequiredItems()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 2, 2);

            //Act
            batch.Begin();

            batch.Render(
                this.mockTextureOne.Object,
                It.IsAny<Rectangle>(),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            batch.End();

            //Assert
            AssertBatchRendered(2, 1, 1);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            //Arrange
            var batch = new SpriteBatch(this.mockGL.Object, this.mockTextFile.Object, 10, 2, 2);

            //Act
            batch.Dispose();

            //Assert

        }

        /// <summary>
        /// Assserts that a single batch was rendered the given amount of <paramref name="totalBatchUpdates"/>.
        /// </summary>
        /// <param name="totalTexturesInBatch">The total amount of textures to be expected in the batch.</param>
        /// <param name="totalBatchUpdates">The total amount of batch data updates.</param>
        private void AssertBatchRendered(int totalTexturesInBatch, int totalBatchUpdates, int totalDrawCalls)
            => AssertBatchRendered(totalTexturesInBatch, totalBatchUpdates, totalDrawCalls, new Matrix4());

        /// <summary>
        /// Assserts that a single batch was rendered the given amount of <paramref name="totalBatchUpdates"/>.
        /// </summary>
        /// <param name="totalTexturesInBatch">The total amount of textures to be expected in the batch.</param>
        /// <param name="totalBatchUpdates">The total amount of batch data updates.</param>
        /// <param name="transform">The transform that was sent to the GPU.  An empty transform means any transform data would assert true.</param>
        private void AssertBatchRendered(int totalTexturesInBatch, int totalBatchUpdates, int totalDrawCalls, Matrix4 transform)
        {
            this.mockGL.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<int>()),
                Times.Exactly(totalBatchUpdates),
                "Did not bind texture");

            if (transform.IsEmpty())
            {
                //Verify with any transform
                AssertTransformUpdate(totalBatchUpdates);
            }
            else
            {
                //Verify with given transform
                AssertTransformUpdate(totalBatchUpdates, transform);
            }

            //Invoked in the GPUBuffer.UpdateQuad() method
            this.mockGL.Verify(m => m.BufferSubData(BufferTarget.ArrayBuffer,
                                                    It.IsAny<IntPtr>(),
                                                    It.IsAny<int>(),
                                                    It.IsAny<QuadData>()),
                Times.Exactly(totalBatchUpdates),
                "Quad was not updated on GPU.");

            this.mockGL.Verify(m => m.DrawElements(PrimitiveType.Triangles, 6 * totalTexturesInBatch,
                                                   DrawElementsType.UnsignedInt, IntPtr.Zero),
                Times.Exactly(totalDrawCalls),
                $"Expected total draw calls of {totalDrawCalls} not reached.");
        }

        private void AssertTransformUpdate(int times)
        {
            //Verify with any transform
            this.mockGL.Verify(m => m.UniformMatrix4(It.IsAny<int>(), true, It.IsAny<Matrix4>()),
                Times.Exactly(times),
                "Transformation matrix not updated on GPU");
        }

        private void AssertTransformUpdate(int times, Matrix4 transform)
        {
            //Verify with given transform
            this.mockGL.Verify(m => m.UniformMatrix4(It.IsAny<int>(), true, transform),
                Times.Exactly(times),
                "Transformation matrix not updated on GPU");
        }
    }
}
