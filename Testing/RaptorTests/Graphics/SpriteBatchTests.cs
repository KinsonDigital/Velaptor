// <copyright file="SpriteBatchTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0002 // Name can be simplified
namespace VelaptorTests.Graphics
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO.Abstractions;
    using System.Numerics;
    using FreeTypeSharp.Native;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;
    using Assert = VelaptorTests.Helpers.AssertExtensions;
#pragma warning restore IDE0001 // Name can be simplified

    /// <summary>
    /// Tests the <see cref="SpriteBatch"/> class.
    /// </summary>
    public class SpriteBatchTests
    {
        private const uint ProgramId = 1111;
        private const int UniformTransformLocation = 2222;
        private readonly Mock<IGLInvoker> mockGLInvoker;
        private readonly Mock<IGLInvokerExtensions> mockGLInvokerExtensions;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IShaderProgram> mockShader;
        private readonly Mock<IGPUBuffer> mockBuffer;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IDisposable> mockGLUnsubscriber;
        private readonly Mock<OpenGLInitObservable> mockGLObservable;
        private readonly Mock<IBatchManagerService> mockBatchManagerService;
        private IObserver<bool>? spriteBatchGLObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchTests"/> class.
        /// </summary>
        public SpriteBatchTests()
        {
            this.mockGLInvoker = new Mock<IGLInvoker>();
            this.mockGLInvoker.Setup(m => m.GetUniformLocation(ProgramId, "uTransform")).Returns(UniformTransformLocation);

            this.mockGLInvokerExtensions = new Mock<IGLInvokerExtensions>();
            this.mockGLInvokerExtensions.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLInvokerExtensions.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);

            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();

            this.mockShader = new Mock<IShaderProgram>();
            this.mockShader.SetupGet(p => p.ProgramId).Returns(ProgramId);

            this.mockBuffer = new Mock<IGPUBuffer>();

            this.mockFile = new Mock<IFile>();

            this.mockGLUnsubscriber = new Mock<IDisposable>();
            this.mockGLObservable = new Mock<OpenGLInitObservable>();
            this.mockGLObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(this.mockGLUnsubscriber.Object)
                .Callback<IObserver<bool>>(observer =>
                {
                    this.spriteBatchGLObserver = observer;
                });

            this.mockBatchManagerService = new Mock<IBatchManagerService>();
            this.mockBatchManagerService.SetupProperty(p => p.BatchSize);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SubscribesToBatchReadyEvent()
        {
            // Act
            var batch = CreateSpriteBatch();

            // Assert
            this.mockBatchManagerService.VerifyAdd(s => s.BatchReady += It.IsAny<EventHandler<EventArgs>>(), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(
                    null,
                    this.mockGLInvokerExtensions.Object,
                    this.mockFreeTypeInvoker.Object,
                    this.mockShader.Object,
                    this.mockBuffer.Object,
                    this.mockBatchManagerService.Object,
                    this.mockGLObservable.Object);
            }, $"The '{nameof(IGLInvoker)}' must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullShader_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(
                    this.mockGLInvoker.Object,
                    this.mockGLInvokerExtensions.Object,
                    this.mockFreeTypeInvoker.Object,
                    null,
                    this.mockBuffer.Object,
                    this.mockBatchManagerService.Object,
                    this.mockGLObservable.Object);
            }, $"The '{nameof(IShaderProgram)}' must not be null. (Parameter 'shader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGPUBuffer_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var buffer = new SpriteBatch(
                    this.mockGLInvoker.Object,
                    this.mockGLInvokerExtensions.Object,
                    this.mockFreeTypeInvoker.Object,
                    this.mockShader.Object,
                    null,
                    this.mockBatchManagerService.Object,
                    this.mockGLObservable.Object);
            }, $"The '{nameof(IGPUBuffer)}' must not be null. (Parameter 'gpuBuffer')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public unsafe void Width_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLInvokerExtensions.Setup(m => m.GetViewPortSize()).Returns(new Size(0, 22));
            var batch = CreateSpriteBatch();

            // Act
            batch.RenderSurfaceWidth = 100;
            _ = batch.RenderSurfaceWidth;

            // Assert
            this.mockGLInvokerExtensions.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLInvokerExtensions.Verify(m => m.SetViewPortSize(new Size(100, 22)), Times.Once());
        }

        [Fact]
        public unsafe void Height_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLInvokerExtensions.Setup(m => m.GetViewPortSize()).Returns(new Size(11, 0));
            var batch = CreateSpriteBatch();

            // Act
            batch.RenderSurfaceHeight = 100;
            _ = batch.RenderSurfaceHeight;

            // Assert
            this.mockGLInvokerExtensions.Verify(m => m.SetViewPortSize(new Size(11, 100)), Times.Once());
            this.mockGLInvokerExtensions.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
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
            this.mockGLInvoker.Verify(m => m.GetFloat(GLGetPName.ColorClearValue, It.IsAny<float[]>()), Times.Once());
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

            // Act
            batch.BatchSize = 3;

            // Assert
            Assert.Equal(3u, batch.BatchSize);
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
            this.mockGLInvoker.Verify(m => m.Clear(GLClearBufferMask.ColorBufferBit), Times.Once());
        }

        [Fact]
        public void RenderTexture_WithEmptyBatchItem_DoesNotRenderBatchItem()
        {
            // Arrange
            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var result = new Dictionary<uint, SpriteBatchItem>
                    {
                        { 0, default },
                    };

                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(result);
                });
            var batch = CreateSpriteBatch();

            // Act
            batch.BeginBatch();
            batch.Render(
                new Mock<ITexture>().Object,
                new Rectangle(0, 0, 1, 2),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Assert
            this.mockBatchManagerService.VerifyAnyBuildTransformationMatrix(Times.Never());
            this.mockGLInvoker.Setup(m => m.UniformMatrix4(It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<bool>(), It.IsAny<Matrix4x4>()));
            this.mockBuffer.Verify(m => m.UpdateQuad(
                It.IsAny<uint>(),
                It.IsAny<Rectangle>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>()), Times.Never());
        }

        [Fact]
        public void RenderTexture_WhenInvoked_RendersTexture()
        {
            // Arrange
            this.mockBatchManagerService.SetupGet(p => p.TotalItemsToRender).Returns(2);
            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var result = new Dictionary<uint, SpriteBatchItem>
                    {
                        { 0, default },
                    };

                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(result);
                });
            var batch = CreateSpriteBatch();

            // Act
            batch.BeginBatch();
            batch.Render(
                new Mock<ITexture>().Object,
                new Rectangle(0, 0, 1, 2),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Assert
            this.mockGLInvoker.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 12, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenInvoking4ParamOverload_UsesCorrectRenderEffect()
        {
            // Arrange
            const int expectedWidth = 1234;
            const int expectedHeight = 5678;

            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var batchItem = new SpriteBatchItem()
                    {
                        Effects = RenderEffects.None,
                        SrcRect = new Rectangle(0, 0, expectedWidth, expectedHeight),
                    };
                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(
                        new Dictionary<uint, SpriteBatchItem>()
                        {
                            { 0, batchItem },
                        });
                });

            var mockTexture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Color>());

            // Assert
            this.mockBatchManagerService.VerifyBuildTransformationMatrixSrcRectWidth(expectedWidth, Times.Once());
            this.mockBatchManagerService.VerifyBuildTransformationMatrixSrcRectHeight(expectedHeight, Times.Once());
        }

        [Fact]
        public void RenderTexture_WithoutCallingBeginFirst_ThrowsException()
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
        public void RenderTexture_WhenInvoking6ParamOverlloadWithoutCallingBeginFirst_ThrowsException()
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
        public void RenderTexture_WhenInvoking5ParamOverloadAndNullTexture_ThrowsException()
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
        public void RenderTexture_WhenInvoking6ParamOverloadAndNullTexture_ThrowsException()
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
            var mockTexture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();

            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 0, 20);
                var destRect = new Rectangle(10, 20, 100, 200);

                batch.Render(mockTexture.Object, srcRect, destRect, 1, 1, Color.White, RenderEffects.None);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void RenderTexture_WithNoSourceRectHeight_ThrowsException()
        {
            // Arrange
            var mockTexture = CreateTextureMock(0, 11, 22);
            var batch = CreateSpriteBatch();

            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentException>(() =>
            {
                var srcRect = new Rectangle(0, 0, 10, 0);
                var destRect = new Rectangle(10, 20, 100, 200);

                batch.Render(mockTexture.Object, srcRect, destRect, 1, 1, Color.White, RenderEffects.None);
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Theory]
        [InlineData(RenderEffects.None, 1234, 5678, 1234, 5678)]
        [InlineData(RenderEffects.FlipHorizontally, 1234, 5678, -1234, 5678)]
        [InlineData(RenderEffects.FlipVertically, 1234, 5678, 1234, -5678)]
        [InlineData(RenderEffects.FlipBothDirections, 1234, 5678, -1234, -5678)]
        public void RenderTexture_WithNoRenderEffect_RendersTextureWithCorrectEffects(
            RenderEffects effects,
            int srcRectWidth,
            int srcRectHeight,
            int expectedWidth,
            int expectedHeight)
        {
            // Arrange
            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var batchItem = new SpriteBatchItem()
                    {
                        Effects = effects,
                        SrcRect = new Rectangle(0, 0, srcRectWidth, srcRectHeight),
                    };
                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(
                        new Dictionary<uint, SpriteBatchItem>()
                        {
                            { 0, batchItem },
                        });
                });

            var mockTexture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RenderEffects>());

            // Assert
            this.mockBatchManagerService.VerifyBuildTransformationMatrixSrcRectWidth(expectedWidth, Times.Once());
            this.mockBatchManagerService.VerifyBuildTransformationMatrixSrcRectHeight(expectedHeight, Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenTextureIsNotBound_BindsTexture()
        {
            // Arrange
            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var batchItem = new SpriteBatchItem()
                    {
                        Effects = RenderEffects.None,
                        SrcRect = new Rectangle(0, 0, 10, 20),
                    };
                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(
                        new Dictionary<uint, SpriteBatchItem>()
                        {
                            { 0, batchItem },
                        });
                });

            var mockTexture = CreateTextureMock(0, 10, 20);
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RenderEffects>());

            // Assert
            this.mockGLInvoker.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, 0), Times.Once());
        }

        [Fact]
        public void RenderTexture_WithUnknownRenderEffect_ThrowsException()
        {
            // Arrange
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() =>
                {
                    var batchItem = new SpriteBatchItem()
                    {
                        Effects = (RenderEffects)1234,
                    };
                    return new ReadOnlyDictionary<uint, SpriteBatchItem>(
                        new Dictionary<uint, SpriteBatchItem>()
                        {
                            { 0, batchItem },
                        });
                });
            this.mockBatchManagerService.SetupRaiseEventWithUpdateBatch();

            var mockTexture = CreateTextureMock(0, 11, 22);

            var batch = CreateSpriteBatch();

            batch.BeginBatch();

            // Act & Assert
            Assert.ThrowsWithMessage<InvalidRenderEffectsException>(() =>
            {
                batch.Render(
                    mockTexture.Object,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<RenderEffects>());
            }, $"The '{nameof(RenderEffects)}' value of '1234' is not valid.");
        }

        [Fact]
        public void RenderFont_WhenUsingDefaultTintColor_ColorWhiteIsUsed()
        {
            // Arrange
            const int atlasWidth = 100;
            const int atlasHeight = 200;
            var batch = CreateSpriteBatch();
            var mockTexture = CreateTextureMock(1, atlasWidth, atlasHeight);
            var metrics = new GlyphMetrics[]
            {
                new GlyphMetrics()
                {
                    Glyph = 'a',
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 6,
                },
            };
            var fontSettings = new FontSettings()
            {
                Size = 12,
                Style = FontStyle.Regular,
            };
            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters())
                .Returns(() => new[] { 'a' });

            // Act
            batch.BeginBatch();
            batch.Render(mockFont.Object, "a", 10, 20);

            // Assert
            this.mockBatchManagerService.VerifyColorForUpdateBatch(Color.White, Times.Once());
        }

        [Fact]
        public void RenderFont_WhenRenderingValidCharacter_UpdatesBatchWithCorrectData()
        {
            // Arrange
            const int atlasWidth = 100;
            const int atlasHeight = 200;

            var batch = CreateSpriteBatch();
            var mockTexture = CreateTextureMock(1, atlasWidth, atlasHeight);
            var metrics = new GlyphMetrics[]
            {
                new GlyphMetrics()
                {
                    Glyph = 'a',
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 6,
                },
            };
            var fontSettings = new FontSettings()
            {
                Size = 12,
                Style = FontStyle.Regular,
            };
            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters())
                .Returns(() => new[] { 'a' });

            // Act
            batch.BeginBatch();
            batch.Render(mockFont.Object, "a", 10, 20, Color.Blue);

            // Assert
            this.mockBatchManagerService.VerifySrcRectForUpdateBatch(new Rectangle(0, 0, atlasWidth, atlasHeight), Times.Once());
            this.mockBatchManagerService.VerifyDestRectForUpdateBatch(new Rectangle(60, 114, atlasWidth, atlasHeight), Times.Once());
            this.mockBatchManagerService.VerifySizeForUpdateBatch(1f, Times.Once());
            this.mockBatchManagerService.VerifyAngleForUpdateBatch(0f, Times.Once());
            this.mockBatchManagerService.VerifyColorForUpdateBatch(Color.Blue, Times.Once());
            this.mockBatchManagerService.VerifyRenderEffectForUpdateBatch(RenderEffects.None, Times.Once());
        }

        [Fact]
        public void RenderFont_WhenCharacterDoesNotExist_UpdatesBatchWithInvalidCharacter()
        {
            // Arrange
            const int atlasWidth = 100;
            const int atlasHeight = 200;
            var batch = CreateSpriteBatch();
            var mockTexture = CreateTextureMock(1, atlasWidth, atlasHeight);
            var metrics = new GlyphMetrics[]
            {
                new GlyphMetrics()
                {
                    Glyph = 'a',
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 6,
                },
                new GlyphMetrics()
                {
                    Glyph = '□',
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 2,
                },
            };
            var fontSettings = new FontSettings()
            {
                Size = 12,
                Style = FontStyle.Regular,
            };
            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics));
            mockFont.Setup(m => m.GetAvailableGlyphCharacters())
                .Returns(() => new[] { 'a' });

            // Act
            batch.BeginBatch();
            batch.Render(mockFont.Object, "b", 10, 20, Color.Blue);

            // Assert
            this.mockBatchManagerService.VerifySrcRectForUpdateBatch(new Rectangle(0, 0, atlasWidth, atlasHeight), Times.Once());
            this.mockBatchManagerService.VerifyDestRectForUpdateBatch(new Rectangle(60, 118, atlasWidth, atlasHeight), Times.Once());
        }

        [Fact]
        public void RenderFont_WhenFontHasKerning_UpdatesBatchWithCorrectData()
        {
            // Arrange
            const int atlasWidth = 100;
            const int atlasHeight = 200;
            var batch = CreateSpriteBatch();
            var mockTexture = CreateTextureMock(1, atlasWidth, atlasHeight);
            var metrics = new GlyphMetrics[]
            {
                new GlyphMetrics()
                {
                    Glyph = 'a',
                    CharIndex = 1,
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 6,
                },
                new GlyphMetrics()
                {
                    Glyph = 'b',
                    CharIndex = 2,
                    AtlasBounds = new Rectangle(0, 0, atlasWidth, atlasHeight),
                    HoriBearingY = 2,
                },
            };
            var fontSettings = new FontSettings()
            {
                Size = 12,
                Style = FontStyle.Regular,
            };

            var facePtr = new IntPtr(1234);
            var glyphIndex2KerningVector = new FT_Vector()
            {
                // This value represents the value 12 bit shifted to the left 6 places
                // This wil be bit shifted to the right 6 places to get the value of 12
                x = new IntPtr(768),
                y = IntPtr.Zero,
            };
            this.mockFreeTypeInvoker.Setup(m => m.FT_Get_Kerning(
                facePtr, // face
                1, // left glyph index
                2, // right glyph index
                (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT)) // kerning mode
                    .Returns(() => glyphIndex2KerningVector);
            this.mockFreeTypeInvoker.Setup(m => m.GetFace()).Returns(facePtr);
            var mockFont = new Mock<IFont>();
            mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockTexture.Object);
            mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(metrics));
            mockFont.SetupGet(p => p.HasKerning).Returns(true);
            mockFont.Setup(m => m.GetAvailableGlyphCharacters())
                .Returns(() => new[] { 'a', 'b' });

            // Act
            batch.BeginBatch();
            batch.Render(mockFont.Object, "ab", 10, 20, Color.Blue);

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Kerning(facePtr, 1, 2, (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT), Times.Once());
            this.mockBatchManagerService.VerifyDestRectForUpdateBatch(new Rectangle(72, 118, atlasWidth, atlasHeight), Times.Once());
        }

        [Fact]
        public void EndBatch_WithEntireBatchEmpty_DoesNotRenderBatch()
        {
            // Arrange
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() => new ReadOnlyDictionary<uint, SpriteBatchItem>(new Dictionary<uint, SpriteBatchItem>()));
            this.mockBatchManagerService.SetupGet(p => p.EntireBatchEmpty).Returns(true);
            var batch = CreateSpriteBatch();

            // Act
            batch.EndBatch();

            // Assert
            this.mockBatchManagerService.Verify(m => m.EmptyBatch(), Times.Never());
        }

        [Fact]
        public void EndBatch_WithNoEmptyBatch_RenderBatch()
        {
            // Arrange
            this.mockBatchManagerService.SetupGet(p => p.BatchItems)
                .Returns(() => new ReadOnlyDictionary<uint, SpriteBatchItem>(new Dictionary<uint, SpriteBatchItem>()));
            this.mockBatchManagerService.SetupGet(p => p.EntireBatchEmpty).Returns(false);
            var batch = CreateSpriteBatch();

            // Act
            batch.EndBatch();

            // Assert
            this.mockBatchManagerService.Verify(m => m.EmptyBatch(), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Arrange
            var batch = CreateSpriteBatch();
            var mockTexture = CreateTextureMock(1, 10, 20);

            // Act
            batch.Dispose();
            batch.Dispose();

            // Assert
            this.mockBatchManagerService.VerifyRemove(m => m.BatchReady -= It.IsAny<EventHandler<EventArgs>>(), Times.Once());
            this.mockShader.Verify(m => m.Dispose(), Times.Once());
            this.mockBuffer.Verify(m => m.Dispose(), Times.Once());
            this.mockGLUnsubscriber.Verify(m => m.Dispose(), Times.Once());
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
        {
            var result = new SpriteBatch(
                this.mockGLInvoker.Object,
                this.mockGLInvokerExtensions.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockShader.Object,
                this.mockBuffer.Object,
                this.mockBatchManagerService.Object,
                this.mockGLObservable.Object);

            SimulateGLInitPushNotification();

            return result;
        }

        /// <summary>
        /// Simulates an OpenGL intialized push notification for the <see cref="OpenGLInitObservable"/>
        /// subscription that is setup inside of <see cref="SpriteBatch"/>.
        /// </summary>
        private void SimulateGLInitPushNotification() => this.spriteBatchGLObserver.OnNext(It.IsAny<bool>());
    }
}
