// <copyright file="SpriteBatchTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Moq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SpriteBatch"/> class.
    /// </summary>
    public class SpriteBatchTests
    {
        private const string RootRelativeTestDataDirPath = @"SampleTestData\";
        private const string GlyphTestDataFileName = "glyph-test-data.json";
        private const uint TextureShaderId = 1111;
        private const uint FontShaderId = 2222;
        private const char InvalidCharacter = '□';
        private readonly string batchTestDataDirPath = @$"{RootRelativeTestDataDirPath}BatchItemTestData\";
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IShaderProgram> mockTextureShader;
        private readonly Mock<IGPUBuffer<SpriteBatchItem>> mockTextureBuffer;
        private readonly Mock<IBatchManagerService<SpriteBatchItem>> mockTextureBatchService;
        private readonly Mock<IShaderProgram> mockFontShader;
        private readonly Mock<IGPUBuffer<SpriteBatchItem>> mockFontBuffer;
        private readonly Mock<IBatchManagerService<SpriteBatchItem>> mockFontBatchService;
        private readonly Mock<IFont> mockFont;
        private readonly OpenGLInitObservable glInitObservable;
        private readonly char[] glyphChars =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };

        private List<GlyphMetrics> allGlyphMetrics = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchTests"/> class.
        /// </summary>
        public SpriteBatchTests()
        {
            this.mockGL = new Mock<IGLInvoker>();

            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
            this.mockGLExtensions.Setup(m => m.LinkProgramSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLExtensions.Setup(m => m.ShaderCompileSuccess(It.IsAny<uint>())).Returns(true);
            this.mockGLExtensions.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

            this.mockTextureShader = new Mock<IShaderProgram>();
            this.mockTextureShader.SetupGet(p => p.ShaderId).Returns(TextureShaderId);

            this.mockFontShader = new Mock<IShaderProgram>();
            this.mockFontShader.SetupGet(p => p.ShaderId).Returns(FontShaderId);

            this.mockTextureBuffer = new Mock<IGPUBuffer<SpriteBatchItem>>();
            this.mockFontBuffer = new Mock<IGPUBuffer<SpriteBatchItem>>();

            this.mockTextureBatchService = new Mock<IBatchManagerService<SpriteBatchItem>>();
            this.mockTextureBatchService.SetupProperty(p => p.BatchSize);

            this.mockFontBatchService = new Mock<IBatchManagerService<SpriteBatchItem>>();
            this.mockFontBatchService.SetupProperty(p => p.BatchSize);
            this.mockFontBatchService.SetupProperty(p => p.BatchItems);
            this.mockFontBatchService.Object.BatchItems =
                new ReadOnlyDictionary<uint, (bool shouldRender, SpriteBatchItem item)>(new Dictionary<uint, (bool shouldRender, SpriteBatchItem item)>());

            this.glInitObservable = new OpenGLInitObservable();

            var mockFontTextureAtlas = new Mock<ITexture>();
            mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
            mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);

            this.mockFont = new Mock<IFont>();
            this.mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTextureAtlas.Object);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SubscribesToBatchingServicesFilledEvent()
        {
            // Act
            var unused = CreateSpriteBatch();

            // Assert
            this.mockTextureBatchService
                .VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockFontBatchService
                .VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullGLInvoker_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SpriteBatch(
                    null,
                    this.mockGLExtensions.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockTextureBatchService.Object,
                    this.mockFontBatchService.Object,
                    this.glInitObservable);
            }, $"The '{nameof(IGLInvoker)}' must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullTextureShader_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SpriteBatch(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    null,
                    this.mockFontShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockTextureBatchService.Object,
                    this.mockFontBatchService.Object,
                    this.glInitObservable);
            }, $"The 'textureShader' must not be null. (Parameter 'textureShader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullFontShader_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SpriteBatch(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockTextureShader.Object,
                    null,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockTextureBatchService.Object,
                    this.mockFontBatchService.Object,
                    this.glInitObservable);
            }, $"The 'fontShader' must not be null. (Parameter 'fontShader')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullTextureGPUBuffer_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SpriteBatch(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    null,
                    this.mockFontBuffer.Object,
                    this.mockTextureBatchService.Object,
                    this.mockFontBatchService.Object,
                    this.glInitObservable);
            }, $"The 'textureBuffer' must not be null. (Parameter 'textureBuffer')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithNullFontGPUBuffer_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new SpriteBatch(
                    this.mockGL.Object,
                    this.mockGLExtensions.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockTextureBuffer.Object,
                    null,
                    this.mockTextureBatchService.Object,
                    this.mockFontBatchService.Object,
                    this.glInitObservable);
            }, $"The 'fontBuffer' must not be null. (Parameter 'fontBuffer')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Width_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLExtensions.Setup(m => m.GetViewPortSize()).Returns(new Size(0, 22));
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            batch.RenderSurfaceWidth = 100;
            _ = batch.RenderSurfaceWidth;

            // Assert
            this.mockGLExtensions.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLExtensions.Verify(m => m.SetViewPortSize(new Size(100, 22)), Times.Once());
        }

        [Fact]
        public void Height_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLExtensions.Setup(m => m.GetViewPortSize()).Returns(new Size(11, 0));
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            batch.RenderSurfaceHeight = 100;
            _ = batch.RenderSurfaceHeight;

            // Assert
            this.mockGLExtensions.Verify(m => m.SetViewPortSize(new Size(11, 100)), Times.Once());
            this.mockGLExtensions.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
        }

        [Fact]
        public void ClearColor_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            const float expectedRed = 0.0431372561f;
            const float expectedGreen = 0.0862745121f;
            const float expectedBlue = 0.129411772f;
            const float expectedAlpha = 0.172549024f;
            var expected = Color.FromArgb(44, 11, 22, 33);
            var expectedClrValues = new float[4];
            this.mockGL.Setup(m => m.GetFloat(GLGetPName.ColorClearValue, expectedClrValues))
                .Callback<GLGetPName, float[]>((_, data) =>
                {
                    data[0] = expectedRed;
                    data[1] = expectedGreen;
                    data[2] = expectedBlue;
                    data[3] = expectedAlpha;
                });

            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            var actual = batch.ClearColor;

            // Assert
            this.mockGL.Verify(m => m.GetFloat(GLGetPName.ColorClearValue, It.IsAny<float[]>()), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ClearColor_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            batch.ClearColor = Color.FromArgb(11, 22, 33, 44);

            // Assert
            this.mockGL.Verify(m => m.ClearColor(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()),
            Times.Exactly(2));
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
            this.mockGL.Verify(m => m.Clear(GLClearBufferMask.ColorBufferBit), Times.Once());
        }

        [Fact]
        public void RenderTexture_WhenNotCallingBeginFirst_ThrowsException()
        {
            // Arrange
            var batch = CreateSpriteBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<InvalidOperationException>(() =>
            {
                batch.Render(
                    new Mock<ITexture>().Object,
                    It.IsAny<Rectangle>(),
                    It.IsAny<Rectangle>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>(),
                    It.IsAny<RenderEffects>());
            }, "The 'BeginBatch()' method must be invoked first before any 'Render()' methods.");
        }

        [Theory]
        [InlineData(0, 20)]
        [InlineData(10, 0)]
        public void RenderTexture_WithSourceRectWithNoWidthOrHeight_ThrowsException(int width, int height)
        {
            // Arrange
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
            {
                batch.Render(
                    new Mock<ITexture>().Object,
                    new Rectangle(1, 2, width, height),
                    It.IsAny<Rectangle>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>(),
                    It.IsAny<RenderEffects>());
            }, "The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
        }

        [Fact]
        public void RenderTexture_WithNullTexture_ThrowsException()
        {
            // Arrange
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.Render(
                    null,
                    It.IsAny<Rectangle>(),
                    It.IsAny<Rectangle>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>(),
                    It.IsAny<RenderEffects>());
            }, "The texture must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void RenderTexture_WithDisposedTexture_ThrowsException()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.IsDisposed).Returns(true);
            mockTexture.SetupGet(p => p.Name).Returns("test-texture");

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<InvalidOperationException>(() =>
            {
                batch.Render(
                    mockTexture.Object,
                    It.IsAny<Rectangle>(),
                    It.IsAny<Rectangle>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>(),
                    It.IsAny<RenderEffects>());
            }, "Cannot render texture.  The texture 'test-texture' has been disposed.");
        }

        [Fact]
        public void RenderTexture_WhenInvoking3ParamOverload_AddsCorrectItemToBatch()
        {
            // Arrange
            const int textureId = 1234;
            const int expectedX = 10;
            const int expectedY = 20;
            const int expectedWidth = 111;
            const int expectedHeight = 222;
            var expectedBatchItem = CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, RenderEffects.None, Color.White, textureId);

            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.Id).Returns(textureId);
            mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
            mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

            SpriteBatchItem actualBatchItem = default;

            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, 10, 20);

            // Assert
            this.mockTextureBatchService.Verify(m => m.Add(It.IsAny<SpriteBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The sprite batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoking4ParamOverloadWithEffects_AddsCorrectItemToBatch()
        {
            // Arrange
            const int textureId = 1234;
            const int expectedX = 10;
            const int expectedY = 20;
            const int expectedWidth = 111;
            const int expectedHeight = 222;
            const RenderEffects expectedRenderEffects = RenderEffects.FlipHorizontally;
            var expectedBatchItem = CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, expectedRenderEffects, Color.White, textureId);

            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.Id).Returns(textureId);
            mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
            mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

            SpriteBatchItem actualBatchItem = default;

            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedRenderEffects);

            // Assert
            this.mockTextureBatchService.Verify(m => m.Add(It.IsAny<SpriteBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The sprite batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoking4ParamOverloadWithColor_AddsCorrectItemToBatch()
        {
            // Arrange
            const int textureId = 1234;
            const int expectedX = 10;
            const int expectedY = 20;
            const int expectedWidth = 111;
            const int expectedHeight = 222;
            var expectedClr = Color.FromArgb(11, 22, 33, 44);
            var expectedBatchItem = CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, RenderEffects.None, expectedClr, textureId);

            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.Id).Returns(textureId);
            mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
            mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

            SpriteBatchItem actualBatchItem = default;

            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr);

            // Assert
            this.mockTextureBatchService.Verify(m => m.Add(It.IsAny<SpriteBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The sprite batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoking5ParamOverload_AddsCorrectItemToBatch()
        {
            // Arrange
            const int textureId = 1234;
            const int expectedX = 10;
            const int expectedY = 20;
            const int expectedWidth = 111;
            const int expectedHeight = 222;
            var expectedClr = Color.FromArgb(11, 22, 33, 44);
            const RenderEffects expectedRenderEffects = RenderEffects.FlipVertically;
            var expectedBatchItem = CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, expectedRenderEffects, expectedClr, textureId);

            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(p => p.Id).Returns(textureId);
            mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
            mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

            SpriteBatchItem actualBatchItem = default;

            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateSpriteBatch();
            this.glInitObservable.OnOpenGLInitialized();
            batch.BeginBatch();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr, expectedRenderEffects);

            // Assert
            this.mockTextureBatchService.Verify(m => m.Add(It.IsAny<SpriteBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The sprite batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoked_RendersTexture()
        {
            // Arrange
            const uint textureId = 1;
            const uint batchIndex = 0;

            var shouldRenderItem = default(SpriteBatchItem);
            shouldRenderItem.Angle = 45;
            shouldRenderItem.TextureId = textureId;

            var shouldNotRenderItem = default(SpriteBatchItem);
            var items = new[] { (true, shouldRenderItem), (false, shouldNotRenderItem) };

            // TODO: Fix this
            var batch = CreateSpriteBatch();
            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(_ =>
                {
                    MockTextureBatchItems(items);
                    this.mockTextureBatchService.Raise(m => m.BatchFilled += null, EventArgs.Empty);
                });
            this.glInitObservable.OnOpenGLInitialized();

            // Act
            batch.BeginBatch();

            batch.Render(
                MockTexture(textureId),
                new Rectangle(0, 0, 1, 2),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Assert
            this.mockGLExtensions.Verify(m => m.BeginGroup("Render 6 Texture Elements"), Times.Once);
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Once());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Once);
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, textureId), Times.Once);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldRenderItem, batchIndex), Times.Once);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldNotRenderItem, batchIndex), Times.Never);
            this.mockTextureBatchService.Verify(m => m.EmptyBatch(), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RenderFont_WithNullOrEmptyText_DoesNotRenderText(string renderText)
        {
            // Arrange
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                It.IsAny<IFont>(),
                renderText,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Assert
            this.mockFont.Verify(m => m.Measure(It.IsAny<string>()), Times.Never);
            this.mockFont.Verify(m => m.ToGlyphMetrics(It.IsAny<string>()), Times.Never);
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Never);
        }

        [Fact]
        public void RenderFont_WithDisposedFont_ThrowsException()
        {
            // Arrange
            this.mockFont.SetupGet(m => m.Name).Returns("test-font");
            this.mockFont.SetupGet(m => m.IsDisposed).Returns(true);

            var batch = CreateSpriteBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<InvalidOperationException>(() =>
            {
                batch.Render(
                    this.mockFont.Object,
                    "test-test",
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>());
            }, "Cannot render font.  The font 'test-font' has been disposed.");
        }

        [Fact]
        public void RenderFont_WhenNotCallingBeginFirst_ThrowsException()
        {
            // Arrange
            const string renderText = "hello world";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            var batch = CreateSpriteBatch();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<InvalidOperationException>(() =>
            {
                batch.Render(
                    this.mockFont.Object,
                    renderText,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>());
            }, "The 'BeginBatch()' method must be invoked first before any 'Render()' methods.");
        }

        [Fact]
        public void RenderFont_WhenInvoked_MeasuresText()
        {
            // Arrange
            const string renderText = "hello world";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Assert
            this.mockFont.Verify(m => m.Measure(renderText), Times.Once);
        }

        [Fact]
        public void RenderFont_WhenRenderingMultilineText_ConvertsEachLineToGlyphMetrics()
        {
            // Arrange
            const string renderText = "hello\nworld";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Assert
            this.mockFont.Verify(m => m.ToGlyphMetrics("hello"), Times.Once);
            this.mockFont.Verify(m => m.ToGlyphMetrics("world"), Times.Once);
        }

        [Fact]
        public void RenderFont_WhenInvoked_AddsCorrectBatchItems()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoked_AddsCorrectBatchItems)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);
            var actualSpriteBatchResultData = Array.Empty<SpriteBatchItem>();

            const string renderText = "Font_Testing";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData = rects.ToArray();
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                400,
                300,
                1.5f,
                45,
                Color.FromArgb(11, 22, 33, 44));

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Once);
            Assert.Equal(12, actualSpriteBatchResultData.Length);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking4ParamsWithXAndYOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking4ParamsWithXAndYOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                11,
                22);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking3ParamsWithPositionOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking3ParamsWithPositionOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(33, 44));

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking6ParamsWithXAndYOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking6ParamsWithXAndYOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                321,
                202,
                2.25f,
                230f);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithPositionOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithPositionOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(66, 77),
                1.25f,
                8f);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithColorOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithColorOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                456,
                635,
                Color.DarkOrange);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(758, 137),
                Color.MediumPurple);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking6ParamsWithColorOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking6ParamsWithColorOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                147,
                185,
                16f,
                Color.IndianRed);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont()
        {
            // Arrange
            var expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont)}.json";
            var expectedSpriteBatchResultData =
                TestDataLoader.LoadTestData<SpriteBatchItem>(this.batchTestDataDirPath, expectedTestDataFileName);

            var actualSpriteBatchResultData = new List<SpriteBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}\n{line2}";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(rects =>
                {
                    actualSpriteBatchResultData.AddRange(rects.ToArray());
                });

            var batch = CreateSpriteBatch();
            batch.BeginBatch();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(1255, 79),
                88f,
                Color.CornflowerBlue);

            // Assert
            this.mockFontBatchService.Verify(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()), Times.Exactly(2));
            Assert.Equal(line1.Length + line2.Length, actualSpriteBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedSpriteBatchResultData, actualSpriteBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoked_RendersFont()
        {
            // Arrange
            const uint textureId = 2;
            const string textBeingRendered = "font";
            const string textNotBeingRendered = "testing";
            var renderText = $"{textBeingRendered} {textNotBeingRendered}";

            MockFontMetrics();
            MockToGlyphMetrics(renderText);

            var mockFontTextureAtlas = new Mock<ITexture>();
            mockFontTextureAtlas.SetupGet(p => p.Id).Returns(textureId);

            this.mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTextureAtlas.Object);

            var batch = CreateSpriteBatch();

            this.mockFontBatchService.Setup(m => m.AddRange(It.IsAny<IEnumerable<SpriteBatchItem>>()))
                .Callback<IEnumerable<SpriteBatchItem>>(items =>
                {
                    var itemsToMock = items.Select(item => (shouldRender: false, item)).ToArray();

                    // Set only the text characters of the "font" section to be rendered
                    for (var i = 0; i < textBeingRendered.Length; i++)
                    {
                        itemsToMock[i].shouldRender = true;
                    }

                    MockFontBatchItems(itemsToMock);
                    this.mockFontBatchService.Raise(m => m.BatchFilled += null, EventArgs.Empty);
                });

            this.glInitObservable.OnOpenGLInitialized();

            // Act
            batch.BeginBatch();

            batch.Render(
                this.mockFont.Object,
                renderText,
                11,
                22);

            // Assert
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles,
                    6u * (uint)textBeingRendered.Length,
                    GLDrawElementsType.UnsignedInt,
                    IntPtr.Zero),
                Times.Once());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture1), Times.Once);
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, textureId), Times.Once);
            this.mockFontBuffer.Verify(m => m.UploadData(It.IsAny<SpriteBatchItem>(),
                    It.IsAny<uint>()),
                Times.Exactly(textBeingRendered.Length));
            this.mockFontBatchService.Verify(m => m.EmptyBatch(), Times.Once);
        }

        [Fact]
        public void EndBatch_WithEntireBatchEmpty_DoesNotRenderBatch()
        {
            // Arrange
            var shouldRenderItem = default(SpriteBatchItem);
            var shouldNotRenderItem = default(SpriteBatchItem);
            var items = new[] { (false, shouldRenderItem), (false, shouldNotRenderItem) };

            var batch = CreateSpriteBatch();
            // TODO: Take care of this mock of the Add method by converting it to a mock of the AddRange method
            this.mockTextureBatchService.Setup(m => m.Add(It.IsAny<SpriteBatchItem>()))
                .Callback<SpriteBatchItem>(_ =>
                {
                    MockTextureBatchItems(items);
                });

            this.glInitObservable.OnOpenGLInitialized();
            batch.BeginBatch();

            batch.Render(
                new Mock<ITexture>().Object,
                new Rectangle(0, 0, 1, 2),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Act
            batch.EndBatch();

            // Assert
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Never());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Never);
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldRenderItem, It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldNotRenderItem, It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfMangedResources()
        {
            // Arrange
            var mockGlObservableUnsubscriber = new Mock<IDisposable>();
            var mockGlInitObservable = new Mock<IObservable<bool>>();
            mockGlInitObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(mockGlObservableUnsubscriber.Object);
            var batch = CreateSpriteBatch(mockGlInitObservable.Object);

            // Act
            batch.Dispose();
            batch.Dispose();

            // Assert
            this.mockTextureBatchService
                .VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once());
            this.mockFontBatchService
                .VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once());
            this.mockTextureShader.Verify(m => m.Dispose(), Times.Once());
            this.mockTextureBuffer.Verify(m => m.Dispose(), Times.Once());
            this.mockFontShader.Verify(m => m.Dispose(), Times.Once());
            this.mockFontBuffer.Verify(m => m.Dispose(), Times.Once());
            mockGlObservableUnsubscriber.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates an <see cref="ITexture"/> instance for the purpose of testing the <see cref="SpriteBatch"/> class.
        /// </summary>
        /// <param name="textureId">The texture ID to use for the test.</param>
        /// <returns>The instance to use for testing.</returns>
        private static ITexture MockTexture(uint textureId)
        {
            var mockResult = new Mock<ITexture>();
            mockResult.SetupGet(p => p.Id).Returns(textureId);

            return mockResult.Object;
        }

        /// <summary>
        /// Creates a <see cref="SpriteBatchItem"/> using the given parameters for the purpose of testing.
        /// </summary>
        /// <param name="x">The X location of the item.</param>
        /// <param name="y">The Y location of the item.</param>
        /// <param name="width">The width of the item.</param>
        /// <param name="height">The height of the item.</param>
        /// <param name="effects">The type of render effects to perform for the item.</param>
        /// <param name="clr">The color of the item.</param>
        /// <param name="textureId">The ID of the texture.</param>
        /// <returns>The instance to use for testing.</returns>
        private static SpriteBatchItem CreateBatchItem(int x, int y, int width, int height, RenderEffects effects, Color clr, int textureId)
        {
            var result = default(SpriteBatchItem);
            result.SrcRect = new RectangleF(0f, 0f, width, height);
            result.DestRect = new RectangleF(x, y, width, height);
            result.Size = 1f;
            result.Angle = 0f;
            result.TintColor = clr;
            result.Effects = effects;
            result.ViewPortSize = new SizeF(800f, 600f);
            result.TextureId = (uint)textureId;

            return result;
        }

        private void MockFontMetrics()
        {
            this.allGlyphMetrics = TestDataLoader.LoadTestData<GlyphMetrics>(RootRelativeTestDataDirPath, GlyphTestDataFileName).ToList();
            this.mockFont.SetupGet(p => p.Metrics).Returns(() => this.allGlyphMetrics.ToArray().ToReadOnlyCollection());
        }

        private void MockToGlyphMetrics(string text)
        {
            this.mockFont.Setup(m => m.ToGlyphMetrics(text)).Returns(() =>
            {
                var textGlyphs = this.allGlyphMetrics.Where(m => text.Contains(m.Glyph)).ToList();

                return text.Select(character
                    => (from m in textGlyphs
                        where m.Glyph == (this.glyphChars.Contains(character)
                            ? character
                            : InvalidCharacter)
                        select m).FirstOrDefault()).ToArray();
            });
        }

        /// <summary>
        /// Creates a new instance of <see cref="SpriteBatch"/> for the purpose of testing.
        /// </summary>
        /// <param name="openGLInitObservable">The observable to use for OpenGL initialization.</param>
        /// <returns>The instance to test with.</returns>
        /// <summary>
        ///     If <paramref name="openGLInitObservable"/> is null, then this <see cref="glInitObservable"/> will be used.
        /// </summary>
        private SpriteBatch CreateSpriteBatch(IObservable<bool>? openGLInitObservable = null)
        {
            var result = new SpriteBatch(
                this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockTextureShader.Object,
                this.mockFontShader.Object,
                this.mockTextureBuffer.Object,
                this.mockFontBuffer.Object,
                this.mockTextureBatchService.Object,
                this.mockFontBatchService.Object,
                openGLInitObservable ?? this.glInitObservable);

            return result;
        }

        private void MockTextureBatchItems((bool shouldRender, SpriteBatchItem item)[] items)
        {
            this.mockTextureBatchService.SetupProperty(p => p.BatchItems);
            this.mockTextureBatchService.Object.BatchItems = items.ToReadOnlyDictionary();
        }

        private void MockFontBatchItems((bool shouldRender, SpriteBatchItem item)[] items)
        {
            this.mockFontBatchService.SetupProperty(p => p.BatchItems);
            this.mockFontBatchService.Object.BatchItems = items.ToReadOnlyDictionary();
        }
    }
}
