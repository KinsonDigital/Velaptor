// <copyright file="SpriteBatchTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO.Abstractions;
    using FreeTypeSharp.Native;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using Raptor.OpenGL;
    using RaptorTests.Helpers;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;

    /// <summary>
    /// Tests the <see cref="SpriteBatch"/> class.
    /// </summary>
    public class SpriteBatchTests : IDisposable
    {
        private const uint ProgramId = 1111;
        private const uint UniformTransformLocation = 2222;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IShaderProgram> mockShader;
        private readonly Mock<IGPUBuffer> mockBuffer;
        private readonly Mock<IFile> mockFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchTests"/> class.
        /// </summary>
        public SpriteBatchTests()
        {
            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockGLInvoker.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvoker.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(11, 22));
            this.mockGLInvoker.Setup(m => m.GetUniformLocation(ProgramId, "uTransform")).Returns(UniformTransformLocation);

            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();

            this.mockShader = new Mock<IShaderProgram>();
            this.mockShader.SetupGet(p => p.ProgramId).Returns(ProgramId);

            this.mockBuffer = new Mock<IGPUBuffer>();

            this.mockFile = new Mock<IFile>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(null, this.mockFreeTypeInvoker.Object, this.mockShader.Object, this.mockBuffer.Object);
            }, $"The '{nameof(IGLInvoker)}' must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullShader_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(this.mockGLInvoker.Object, this.mockFreeTypeInvoker.Object, null, this.mockBuffer.Object);
            }, $"The '{nameof(IShaderProgram)}' must not be null. (Parameter 'shader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGPUBuffer_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(this.mockGLInvoker.Object, this.mockFreeTypeInvoker.Object, this.mockShader.Object, null);
            }, $"The '{nameof(IGPUBuffer)}' must not be null. (Parameter 'gpuBuffer')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public unsafe void Width_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act
            batch.RenderSurfaceWidth = 100;
            _ = batch.RenderSurfaceWidth;

            // Assert
            this.mockGLInvoker.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLInvoker.Verify(m => m.SetViewPortSize(new Vector2(100, 22)), Times.Once());
        }

        [Fact]
        public unsafe void Height_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act
            batch.RenderSurfaceHeight = 100;
            _ = batch.RenderSurfaceHeight;

            // Assert
            this.mockGLInvoker.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLInvoker.Verify(m => m.SetViewPortSize(new Vector2(11, 100)), Times.Once());
        }

        [Fact]
        public void ClearColor_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrerctResult()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act
            batch.ClearColor = Color.FromArgb(11, 22, 33, 44);
            var actual = batch.ClearColor;

            // Assert
            this.mockGLInvoker.Verify(m => m.GetFloat(GetPName.ColorClearValue, It.IsAny<float[]>()), Times.Once());
            this.mockGLInvoker.Verify(m => m.ClearColor(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()),
            Times.Exactly(2));
        }

        [Fact]
        public void BatchSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var batch = CreateSpriteBatch();
            batch.BatchSize = 3;

            // Act
            var actual = batch.BatchSize;

            // Assert
            Assert.Equal(3u, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Clear_WhenInvoked_ClearsBuffer()
        {
            // Act
            var batch = CreateSpriteBatch();
            batch.Clear();

            // Assert
            this.mockGLInvoker.Verify(m => m.Clear(ClearBufferMask.ColorBufferBit), Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenUsingOverloadWithFourParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            // Arrange
            var mockTexture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<Exception>(() =>
            {
                batch.Render(mockTexture.Object, 10, 20);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void RenderTexture_WhenUsingOverloadWithFourParamsAndNullTexture_ThrowsException()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.BeginBatch();
                batch.Render(null, 10, 20);
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void RenderTexture_WhenUsingOverloadWithFourParams_RendersBatch()
        {
            // Arrange
            var texture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();

            batch.BatchSize = 1;

            batch.BeginBatch();

            // Act
            batch.Render(
                texture.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            batch.EndBatch();
            batch.EndBatch();

            // Assert
            this.mockGLInvoker.Verify2DTextureIsBound(0, Times.Once());
            this.mockGLInvoker.VerifyTotalBatchItemsDrawn(1, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdated(Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenExcedingBatchSize_RendersBatchTwoTimes()
        {
            // Arrange
            var textureA = CreateTextureMock(0, 11, 22);
            var textureB = CreateTextureMock(1, 44, 33);

            var batch = CreateSpriteBatch();

            batch.BatchSize = 1;

            batch.BeginBatch();

            // Act
            batch.Render(
                textureA.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            batch.Render(
                textureB.Object,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>());

            // Assert
            this.mockGLInvoker.Verify(m => m.UniformMatrix4(UniformTransformLocation, true, ref It.Ref<Matrix4>.IsAny),
                Times.Exactly(1),
                "Transformation matrix not updated on GPU");
        }

        [Fact]
        public void RenderTexture_WhenUsingOverloadWithSixParamsAndWithoutCallingBeginFirst_ThrowsException()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);
            var batch = CreateSpriteBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<Exception>(() =>
            {
                var srcRect = new Rectangle(1, 2, 3, 4);
                var destRect = new Rectangle(5, 6, 7, 8);
                var tintClr = Color.FromArgb(11, 22, 33, 44);

                batch.Render(texture.Object, srcRect, destRect, 0.5f, 90, tintClr, RenderEffects.None);
            }, $"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
        }

        [Fact]
        public void RenderTexture_WhenUsingOverloadWithSixParamsAndWithNullTexture_ThrowsException()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 10, 20);
                batch.BeginBatch();
                batch.Render(null, srcRect, It.IsAny<Rectangle>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<Color>(), RenderEffects.None);
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void RenderTexture_WithNoSourceRectWidth_ThrowsException()
        {
            // Arrange
            var texture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();

            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 0, 20);
                var destRect = new Rectangle(10, 20, 100, 200);

                batch.Render(texture.Object, srcRect, destRect, 1, 1, Color.White, RenderEffects.None);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void RenderTexture_WithNoSourceRectHeight_ThrowsException()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);
            var batch = CreateSpriteBatch();

            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 10, 0);
                var destRect = new Rectangle(10, 20, 100, 200);

                batch.Render(texture.Object, srcRect, destRect, 1, 1, Color.White, RenderEffects.None);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void RenderTexture_WhenSwitchingTextures_RendersBatchOnce()
        {
            // Arrange
            var textureA = CreateTextureMock(0, 11, 22);
            var textureB = CreateTextureMock(1, 33, 44);

            var batch = CreateSpriteBatch();

            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(10, 20));
            batch.RenderSurfaceWidth = 10;
            batch.RenderSurfaceHeight = 20;
            batch.BatchSize = 3;

            var transMatrix = new Matrix4()
            {
                Column0 = new Vector4(-6.5567085E-09f, 0.15f, 0f, 0f),
                Column1 = new Vector4(-0.1f, -4.371139E-09F, 0f, 0.39999998f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f),
            };

            // WARNING - Changing these values will change the trans matrix
            var textureWidth = 7;
            var textureHeight = 8;
            var srcRect = new Rectangle(1, 2, 3, 4);
            var destRect = new Rectangle(5, 6, textureWidth, textureHeight);
            var tintClr = Color.FromArgb(11, 22, 33, 44);

            batch.BeginBatch();

            // Act
            batch.Render(textureA.Object, srcRect, destRect, 0.5f, 90, tintClr, RenderEffects.None);
            batch.Render(textureB.Object, srcRect, destRect, 0.5f, 90, tintClr, RenderEffects.None);

            // Assert
            this.mockGLInvoker.Verify2DTextureIsBound(0, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectQuadID(0, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectSrcRectangle(srcRect, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureWidth(textureWidth, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureHeight(textureHeight, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectColor(tintClr, Times.Once());
            this.mockGLInvoker.VerifyDrawingWithTriangles(Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenBatchIsFull_RendersBatch()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            batch.RenderSurfaceWidth = 10;
            batch.RenderSurfaceHeight = 20;
            batch.BatchSize = 2u;

            // Act
            batch.BeginBatch();
            batch.Render(texture.Object, It.IsAny<int>(), It.IsAny<int>());
            batch.Render(texture.Object, It.IsAny<int>(), It.IsAny<int>());
            batch.Render(texture.Object, It.IsAny<int>(), It.IsAny<int>());

            // Assert
            this.mockGLInvoker.Verify2DTextureIsBound(0, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdated(Times.Exactly(2));
            this.mockGLInvoker.VerifyDrawingWithTriangles(Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenBatchIsNotFull_OnlyRendersRequiredItems()
        {
            // Arrange
            var texture = CreateTextureMock(0, 10, 20);

            var batch = CreateSpriteBatch();

            batch.BatchSize = 2;
            batch.BeginBatch();

            // Act
            batch.Render(
                texture.Object,
                new Rectangle(1, 2, 3, 4),
                new Rectangle(5, 6, 7, 8),
                9f,
                10f,
                Color.FromArgb(11, 22, 33, 44),
                RenderEffects.None);

            batch.EndBatch();

            // Assert
            this.mockGLInvoker.Verify2DTextureIsBound(0, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectQuadID(0, Times.Once());
            this.mockGLInvoker.VerifyDrawingWithTriangles(Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenInvoking4ParamOverloadWithRenderEffect_RendersTexture()
        {
            // Arrange
            var texture = CreateTextureMock(0, 10, 20);

            var batch = CreateSpriteBatch();

            batch.RenderSurfaceWidth = 10;
            batch.RenderSurfaceHeight = 20;
            batch.BatchSize = 1u;

            var expectedMatrix = default(Matrix4);
            expectedMatrix.Row0 = new Vector4(-0.90909094f, 0f, 0f, 0f);
            expectedMatrix.Row1 = new Vector4(0f, 0.90909094f, 0f, 0f);
            expectedMatrix.Row2 = new Vector4(0f, 0f, 1f, 0f);
            expectedMatrix.Row3 = new Vector4(0.8181819f, -0.8181819f, 0f, 1f);

            batch.BeginBatch();

            // Act
            batch.Render(texture.Object, 10, 20, RenderEffects.FlipHorizontally);
            batch.Render(texture.Object, 30, 40, RenderEffects.FlipHorizontally);

            // Assert
            this.mockGLInvoker.Verify2DTextureIsBound(0, Times.Once());
            this.mockGLInvoker.VerifyTransformIsUpdated(expectedMatrix, Times.Once());
            this.mockGLInvoker.VerifyTotalBatchItemsDrawn(1, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdated(Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenInvoking7ParamOverload_RendersTextureFlippedHorizontally()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(11, 22));
            batch.BatchSize = 1;
            batch.BeginBatch();

            var expectedMatrix = default(Matrix4);
            expectedMatrix.Row0 = new Vector4(-2.4172554f, -0.28415158f, 0f, 0f);
            expectedMatrix.Row1 = new Vector4(-0.42622736f, 1.6115037f, 0f, 0f);
            expectedMatrix.Row2 = new Vector4(0f, 0f, 1f, 0f);
            expectedMatrix.Row3 = new Vector4(-0.090909064f, 0.45454544f, 0f, 1f);

            // Act
            batch.Render(
                            texture.Object,
                            new Rectangle(1, 2, 3, 4),
                            new Rectangle(5, 6, 7, 8),
                            9f,
                            10f,
                            Color.FromArgb(11, 22, 33, 44),
                            RenderEffects.FlipHorizontally);

            batch.EndBatch();

            // Assert
            this.mockGLInvoker.Verify(m => m.UniformMatrix4(UniformTransformLocation, true, ref expectedMatrix), Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenInvoking7ParamOverload_RendersTextureFlippedVertically()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            batch.BatchSize = 1;
            batch.BeginBatch();

            var expectedMatrix = default(Matrix4);
            expectedMatrix.Row0 = new Vector4(2.4172554f, 0.28415158f, 0f, 0f);
            expectedMatrix.Row1 = new Vector4(0.42622736f, -1.6115037f, 0f, 0f);
            expectedMatrix.Row2 = new Vector4(0f, 0f, 1f, 0f);
            expectedMatrix.Row3 = new Vector4(-0.090909064f, 0.45454544f, 0f, 1f);

            // Act
            batch.Render(
                            texture.Object,
                            new Rectangle(1, 2, 3, 4),
                            new Rectangle(5, 6, 7, 8),
                            9f,
                            10f,
                            Color.FromArgb(11, 22, 33, 44),
                            RenderEffects.FlipVertically);

            batch.EndBatch();

            // Assert
            this.mockGLInvoker.Verify(m => m.UniformMatrix4(UniformTransformLocation, true, ref expectedMatrix), Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenInvoking7ParamOverload_RendersTextureFlippedBothDirections()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            batch.BatchSize = 1;
            batch.BeginBatch();

            var expectedMatrix = default(Matrix4);
            expectedMatrix.Row0 = new Vector4(-2.4172554f, 0.28415158f, 0f, 0f);
            expectedMatrix.Row1 = new Vector4(-0.42622736f, -1.6115037f, 0f, 0f);
            expectedMatrix.Row2 = new Vector4(0f, 0f, 1f, 0f);
            expectedMatrix.Row3 = new Vector4(-0.090909064f, 0.45454544f, 0f, 1f);

            // Act
            batch.Render(
                            texture.Object,
                            new Rectangle(1, 2, 3, 4),
                            new Rectangle(5, 6, 7, 8),
                            9f,
                            10f,
                            Color.FromArgb(11, 22, 33, 44),
                            RenderEffects.FlipBothDirections);

            batch.EndBatch();

            // Assert
            this.mockGLInvoker.VerifyTransformIsUpdated(expectedMatrix, Times.Once());
        }

        [Fact]
        public void RenderTexture_WithUnknownRenderEffect_ThrowsException()
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            batch.BatchSize = 1;
            batch.BeginBatch();

            var expectedMatrix = default(Matrix4);
            expectedMatrix.Row0 = new Vector4(-2.4172554f, 0.28415158f, 0f, 0f);
            expectedMatrix.Row1 = new Vector4(-0.42622736f, -1.6115037f, 0f, 0f);
            expectedMatrix.Row2 = new Vector4(0f, 0f, 1f, 0f);
            expectedMatrix.Row3 = new Vector4(-0.090909064f, 0.45454544f, 0f, 1f);

            // Act & Assert
            batch.Render(
                            texture.Object,
                            new Rectangle(1, 2, 3, 4),
                            new Rectangle(5, 6, 7, 8),
                            9f,
                            10f,
                            Color.FromArgb(11, 22, 33, 44),
                            (RenderEffects)44);

            Assert.ThrowsWithMessage<InvalidRenderEffectsException>(() =>
            {
                batch.EndBatch();
            }, $"The '{nameof(RenderEffects)}' value of '44' is not valid.");
        }

        [Theory]
        [InlineData(0, 22, "The port size width cannot be a negative or zero value.")]
        [InlineData(11, 0, "The port size height cannot be a negative or zero value.")]
        public void RenderTexture_WhenPortSizeWithIsZero_ThrowsException(int width, int height, string expectedMessage)
        {
            // Arrange
            var texture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            this.mockGLInvoker.Setup(m => m.GetViewPortSize()).Returns(new Vector2(width, height));
            batch.BatchSize = 1;
            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<Exception>(() =>
            {
                batch.Render(texture.Object, 10, 20);
                batch.EndBatch();
            }, expectedMessage);
        }

        [Fact]
        public void RenderFont_With4ParamOverload_CorrectlySetsQuadAndTransformSetup()
        {
            // Arrange
            var stringToRender = "c";
            var metrics = new List<GlyphMetrics>
            {
                new GlyphMetrics()
                {
                    Glyph = 'c',
                    AtlasBounds = new Rectangle(0, 0, 10, 20),
                },
            };

            const int textureWidth = 100;
            const int textureHeight = 200;
            var mockFontTexture = CreateTextureMock(0, textureWidth, textureHeight);

            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics.ToArray()));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters()).Returns(stringToRender.ToCharArray());

            var expectedTransform = new Matrix4()
            {
                Column0 = new Vector4(0.90909094f, 0f, 0f, 18.09091f),
                Column1 = new Vector4(0f, 0.90909094f, 0f, -18.09091f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f),
            };

            var batch = CreateSpriteBatch();
            batch.BatchSize = 1;
            batch.BeginBatch();

            // Act
            batch.Render(mockFont.Object, stringToRender, 100, 200);
            batch.EndBatch();

            // Assert
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectSrcRectangle(new Rectangle(0, 0, 10, 20), Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureWidth(textureWidth, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureHeight(textureHeight, Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectColor(Color.White, Times.Once());
            this.mockGLInvoker.VerifyTransformIsUpdated(expectedTransform, Times.Once());
        }

        [Fact]
        public void RenderFont_WhenInvokingAllParamOverload_CorrectlySetsKerning()
        {
            // Arrange
            var stringToRender = "ca";
            var metrics = new List<GlyphMetrics>
            {
                new GlyphMetrics()
                {
                    Glyph = 'c',
                    AtlasBounds = new Rectangle(0, 0, 10, 20),
                    CharIndex = 1,
                },
                new GlyphMetrics()
                {
                    Glyph = 'a',
                    AtlasBounds = new Rectangle(0, 0, 11, 22),
                    CharIndex = 2,
                },
            };

            const int textureWidth = 100;
            const int textureHeight = 200;
            var facePtr = new IntPtr(1234);
            var mockFontTexture = CreateTextureMock(0, textureWidth, textureHeight);

            this.mockFreeTypeInvoker.Setup(m => m.GetFace()).Returns(facePtr);

            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.HasKerning).Returns(true);
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics.ToArray()));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters()).Returns(stringToRender.ToCharArray());

            var expectedTransform = new Matrix4()
            {
                Column0 = new Vector4(0.90909094f, 0f, 0f, 18.09091f),
                Column1 = new Vector4(0f, 0.90909094f, 0f, -18.09091f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f),
            };

            var batch = CreateSpriteBatch();
            batch.BatchSize = 1;
            batch.BeginBatch();

            // Act
            batch.Render(mockFont.Object, stringToRender, 100, 200);
            batch.EndBatch();

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Kerning(facePtr, 1, 2, (int)FT_Kerning_Mode.FT_KERNING_DEFAULT), Times.Once());
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectSrcRectangle(new Rectangle(0, 0, 11, 22), Times.Once());
            this.mockGLInvoker.VerifyTransformIsUpdated(expectedTransform, Times.Once());
        }

        [Fact]
        public void RenderFont_WhenInvokingAllParamOverload_CorrectlySetsQuadAndTransformSetup()
        {
            // Arrange
            var stringToRender = "cÆ";
            var metrics = new List<GlyphMetrics>
            {
                new GlyphMetrics()
                {
                    Glyph = 'c',
                    AtlasBounds = new Rectangle(0, 0, 10, 20),
                    CharIndex = 1,
                },
                // This character is the invalid character and is rendered when a requested character
                // in the render string is not available to render.  Adding this struct item
                // is simulating it existing in the font texture atlas which it does exist
                // by it being added by the FontAtlasService
                new GlyphMetrics()
                {
                    Glyph = '□',
                    AtlasBounds = new Rectangle(0, 0, 10, 20),
                    CharIndex = 100,
                },
            };

            const int textureWidth = 100;
            const int textureHeight = 200;

            var mockFontTexture = CreateTextureMock(0, textureWidth, textureHeight);

            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.HasKerning).Returns(true);
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics.ToArray()));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters()).Returns("c".ToCharArray());

            var expectedTransform = new Matrix4()
            {
                Column0 = new Vector4(0.90909094f, 0f, 0f, 18.09091f),
                Column1 = new Vector4(0f, 0.90909094f, 0f, -18.09091f),
                Column2 = new Vector4(0f, 0f, 1f, 0f),
                Column3 = new Vector4(0f, 0f, 0f, 1f),
            };

            var batch = CreateSpriteBatch();
            batch.BatchSize = 1;
            batch.BeginBatch();

            // Act
            batch.Render(mockFont.Object, stringToRender, 100, 200, Color.Blue);
            batch.EndBatch();

            // Assert
            // NOTE: Invocation will be performed once per glyph character
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectSrcRectangle(new Rectangle(0, 0, 10, 20), Times.Exactly(metrics.Count));
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureWidth(textureWidth, Times.Exactly(metrics.Count));
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectTextureHeight(textureHeight, Times.Exactly(metrics.Count));
            this.mockBuffer.VerifyQuadIsUpdatedWithCorrectColor(Color.Blue, Times.Exactly(metrics.Count));
            this.mockGLInvoker.VerifyTransformIsUpdated(expectedTransform, Times.Exactly(metrics.Count));
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Act
            var batch = CreateSpriteBatch();

            batch.Dispose();
            batch.Dispose();

            // Assert
            this.mockShader.Verify(m => m.Dispose(), Times.Once());
            this.mockBuffer.Verify(m => m.Dispose(), Times.Once());
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

        /// <summary>
        /// Creates a new instance of <see cref="SpriteBatch"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private SpriteBatch CreateSpriteBatch()
            => new SpriteBatch(
                this.mockGLInvoker.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockShader.Object,
                this.mockBuffer.Object);
    }
}
