// <copyright file="RendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Moq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Buffers;
    using Velaptor.OpenGL.Shaders;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Renderer"/> class.
    /// </summary>
    public class RendererTests
    {
        private const string GlyphTestDataFileName = "glyph-test-data.json";
        private const string BatchTestDataDirPath = @"BatchItemTestData\";
        private const uint TextureShaderId = 1111u;
        private const uint FontShaderId = 2222u;
        private const uint RectShaderId = 3333u;
        private const uint TextureId = 456u;
        private const uint FontAtlasTextureId = 1234u;
        private const char InvalidCharacter = '□';
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IOpenGLService> mockGLService;
        private readonly Mock<IShaderManager> mockShaderManager;
        private readonly Mock<IBufferManager> mockBufferManager;
        private readonly Mock<IFont> mockFont;
        private readonly Mock<IBatchServiceManager> mockBatchServiceManager;
        private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
        private readonly Mock<IDisposable> mockGLInitUnsubscriber;
        private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;
        private readonly Mock<IDisposable> mockShutDownUnsubscriber;
        private readonly char[] glyphChars =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };

        private List<GlyphMetrics> allGlyphMetrics = new ();
        private IReactor<GLInitData>? glInitReactor;
        private IReactor<ShutDownData>? shutDownReactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererTests"/> class.
        /// </summary>
        public RendererTests()
        {
            this.mockGL = new Mock<IGLInvoker>();

            this.mockGLService = new Mock<IOpenGLService>();
            this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
            this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
            this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

            this.mockShaderManager = new Mock<IShaderManager>();
            this.mockShaderManager.Setup(m => m.GetShaderId(ShaderType.Texture)).Returns(TextureShaderId);
            this.mockShaderManager.Setup(m => m.GetShaderId(ShaderType.Font)).Returns(FontShaderId);
            this.mockShaderManager.Setup(m => m.GetShaderId(ShaderType.Rectangle)).Returns(RectShaderId);

            this.mockBufferManager = new Mock<IBufferManager>();

            this.mockBatchServiceManager = new Mock<IBatchServiceManager>();
            this.mockBatchServiceManager.SetupGet(p => p.TextureBatchItems)
                .Returns(Array.Empty<(bool shouldRender, TextureBatchItem item)>().ToReadOnlyDictionary());
            this.mockBatchServiceManager.SetupGet(p => p.FontGlyphBatchItems)
                .Returns(Array.Empty<(bool shouldRender, FontGlyphBatchItem item)>().ToReadOnlyDictionary());
            this.mockBatchServiceManager.SetupGet(p => p.RectBatchItems)
                .Returns(Array.Empty<(bool shouldRender, RectShape item)>().ToReadOnlyDictionary());

            this.mockGLInitUnsubscriber = new Mock<IDisposable>();
            this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
            this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
                .Returns(this.mockGLInitUnsubscriber.Object)
                .Callback<IReactor<GLInitData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "GL Init reactable subscription failed.  Reactor is null.");
                    }

                    this.glInitReactor = reactor;
                });

            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
            this.mockShutDownReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<ShutDownData>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IReactor<ShutDownData>>((reactor) =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "Shutdown reactable subscription failed.  Reactor is null.");
                    }

                    this.shutDownReactor = reactor;
                });

            var mockFontTextureAtlas = new Mock<ITexture>();
            mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
            mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);

            this.mockFont = new Mock<IFont>();
            this.mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTextureAtlas.Object);
            this.mockFont.SetupGet(p => p.Size).Returns(12u);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullGLInvokerParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    null,
                    this.mockGLService.Object,
                    this.mockShaderManager.Object,
                    this.mockBufferManager.Object,
                    this.mockBatchServiceManager.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WithNullShaderManagerParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    null,
                    this.mockBufferManager.Object,
                    this.mockBatchServiceManager.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'shaderManager')");
        }

        [Fact]
        public void Ctor_WithNullBufferManagerParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderManager.Object,
                    null,
                    this.mockBatchServiceManager.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'bufferManager')");
        }

        [Fact]
        public void Ctor_WithNullBatchServiceManagerParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderManager.Object,
                    this.mockBufferManager.Object,
                    null,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'batchServiceManager')");
        }

        [Fact]
        public void Ctor_WithNullGLInitReactableParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderManager.Object,
                    this.mockBufferManager.Object,
                    this.mockBatchServiceManager.Object,
                    null,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'glInitReactable')");
        }

        [Fact]
        public void Ctor_WithNullShutDownReactableParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockShaderManager.Object,
                    this.mockBufferManager.Object,
                    this.mockBatchServiceManager.Object,
                    this.mockGLInitReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SubscribesToBatchEvents()
        {
            // Arrange & Act
            var unused = CreateRenderer();

            // Assert
            this.mockBatchServiceManager.VerifyAdd(a => a.TextureBatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockBatchServiceManager.VerifyAdd(a => a.FontGlyphBatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockBatchServiceManager.VerifyAdd(a => a.RectBatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Width_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(0, 22));
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);

            // Act
            batch.RenderSurfaceWidth = 100;
            _ = batch.RenderSurfaceWidth;

            // Assert
            this.mockGLService.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
            this.mockGLService.Verify(m => m.SetViewPortSize(new Size(100, 22)), Times.Once());
        }

        [Fact]
        public void Height_WhenSettingValueAfterOpenGLInitialized_ReturnsCorrectResult()
        {
            // Arrange
            this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(11, 0));
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);

            // Act
            batch.RenderSurfaceHeight = 100;
            _ = batch.RenderSurfaceHeight;

            // Assert
            this.mockGLService.Verify(m => m.SetViewPortSize(new Size(11, 100)), Times.Once());
            this.mockGLService.Verify(m => m.GetViewPortSize(), Times.Exactly(4));
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

            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);

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
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);

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
            var batch = CreateRenderer();
            batch.Clear();

            // Assert
            this.mockGL.Verify(m => m.Clear(GLClearBufferMask.ColorBufferBit), Times.Once());
        }

        [Fact]
        public void OnResize_WhenInvoked_SetsBufferViewPortSizes()
        {
            // Arrange
            var batch = CreateRenderer();

            // Act
            batch.OnResize(new SizeU(11u, 22u));

            // Assert
            this.mockBufferManager.VerifyOnce(m => m.SetViewPortSize(VelaptorBufferType.Texture, new SizeU(11u, 22u)));
            this.mockBufferManager.VerifyOnce(m => m.SetViewPortSize(VelaptorBufferType.Font, new SizeU(11u, 22u)));
            this.mockBufferManager.VerifyOnce(m => m.SetViewPortSize(VelaptorBufferType.Rectangle, new SizeU(11u, 22u)));
        }

        [Fact]
        public void RenderTexture_WhenNotCallingBeginFirst_ThrowsException()
        {
            // Arrange
            var batch = CreateRenderer();

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
            }, "The 'Begin()' method must be invoked first before any 'Render()' methods.");
        }

        [Theory]
        [InlineData(0, 20)]
        [InlineData(10, 0)]
        public void RenderTexture_WithSourceRectWithNoWidthOrHeight_ThrowsException(int width, int height)
        {
            // Arrange
            var batch = CreateRenderer();
            batch.Begin();

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
            var batch = CreateRenderer();
            batch.Begin();

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
            }, $"Cannot render a null '{nameof(ITexture)}'. (Parameter 'texture')");
        }

        [Fact]
        public void RenderTexture_WithNoTextureItemsToRender_SetsUpCorrectDebugGroupAndExits()
        {
            // Arrange
            const string shaderName = "TestTextureShader";
            this.mockShaderManager.Setup(m => m.GetShaderName(ShaderType.Texture)).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockBatchServiceManager.Raise(e => e.TextureBatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Texture Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Texture Process With {shaderName} Shader"));
            this.mockShaderManager.VerifyNever(m => m.Use(It.IsAny<ShaderType>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Texture Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockBufferManager.VerifyNever(m =>
                m.UploadTextureData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockBatchServiceManager.VerifyNever(m => m.EmptyBatch(BatchServiceType.FontGlyph));
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

            TextureBatchItem actualBatchItem = default;

            this.mockBatchServiceManager.Setup(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
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

            TextureBatchItem actualBatchItem = default;

            this.mockBatchServiceManager.Setup(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedRenderEffects);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
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

            TextureBatchItem actualBatchItem = default;

            this.mockBatchServiceManager.Setup(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
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

            TextureBatchItem actualBatchItem = default;

            this.mockBatchServiceManager.Setup(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr, expectedRenderEffects);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoked_RendersTexture()
        {
            // Arrange
            const uint batchIndex = 0;

            var shouldRenderItem = default(TextureBatchItem);
            shouldRenderItem.Angle = 45;
            shouldRenderItem.TextureId = TextureId;

            var shouldNotRenderItem = default(TextureBatchItem);
            var items = new[] { (true, shouldRenderItem), (false, shouldNotRenderItem) };

            var batch = CreateRenderer();
            this.mockBatchServiceManager.Setup(m => m.AddTextureBatchItem(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(_ =>
                {
                    MockTextureBatchItems(items);
                    this.mockBatchServiceManager.Raise(m => m.TextureBatchFilled += null, EventArgs.Empty);
                });
            this.glInitReactor.OnNext(default);

            // Act
            batch.Begin();

            batch.Render(
                MockTexture(TextureId),
                new Rectangle(0, 0, 1, 2),
                It.IsAny<Rectangle>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<RenderEffects>());

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render 6 Texture Elements"), Times.Once);
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Once());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Once);
            this.mockGLService.Verify(m => m.BindTexture2D(TextureId), Times.Once);
            this.mockBufferManager.VerifyOnce(m => m.UploadTextureData(shouldRenderItem, batchIndex));
            this.mockBufferManager.VerifyNever(m => m.UploadTextureData(shouldNotRenderItem, batchIndex));
            this.mockBatchServiceManager.Verify(m => m.EmptyBatch(BatchServiceType.Texture), Times.Once);
        }

        [Fact]
        public void RenderFont_WithNullFont_ThrowsException()
        {
            // Arrange
            var batch = CreateRenderer();

            // Act & Asset
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                batch.Render(null, "test", 10, 20, 1f, 0f, Color.White);
            }, $"Cannot render a null '{nameof(IFont)}'. (Parameter 'font')");
        }

        [Fact]
        public void RenderFont_WithNoFontItemsToRender_SetsUpCorrectDebugGroupAndExits()
        {
            // Arrange
            const string shaderName = "TestFontShader";
            this.mockShaderManager.Setup(m => m.GetShaderName(ShaderType.Font)).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockBatchServiceManager.Raise(e => e.FontGlyphBatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Text Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Text Process With {shaderName} Shader"));
            this.mockShaderManager.VerifyNever(m => m.Use(It.IsAny<ShaderType>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Character Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockBufferManager.VerifyNever(m =>
                m.UploadFontGlyphData(It.IsAny<FontGlyphBatchItem>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Font Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockBatchServiceManager.VerifyNever(m => m.EmptyBatch(BatchServiceType.FontGlyph));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RenderFont_WithNullOrEmptyText_DoesNotRenderText(string renderText)
        {
            // Arrange
            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                new Mock<IFont>().Object,
                renderText,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Assert
            this.mockFont.Verify(m => m.Measure(It.IsAny<string>()), Times.Never);
            this.mockFont.Verify(m => m.ToGlyphMetrics(It.IsAny<string>()), Times.Never);
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()), Times.Never);
        }

        [Fact]
        public void RenderFont_WithFontSizeSetToZero_DoesNotRenderText()
        {
            // Arrange
            this.mockFont.SetupGet(p => p.Size).Returns(0);

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                "test-text",
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Assert
            this.mockFont.Verify(m => m.Measure(It.IsAny<string>()), Times.Never);
            this.mockFont.Verify(m => m.ToGlyphMetrics(It.IsAny<string>()), Times.Never);
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()), Times.Never);
        }

        [Fact]
        public void RenderFont_WhenNotCallingBeginFirst_ThrowsException()
        {
            // Arrange
            const string renderText = "hello world";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            var batch = CreateRenderer();

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
            }, "The 'Begin()' method must be invoked first before any 'Render()' methods.");
        }

        [Fact]
        public void RenderFont_WhenInvoked_MeasuresText()
        {
            // Arrange
            const string renderText = "hello world";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            var batch = CreateRenderer();
            batch.Begin();

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
            var renderText = $"hello{Environment.NewLine}world";

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");

            var batch = CreateRenderer();
            batch.Begin();

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
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoked_AddsCorrectBatchItems)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);
            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string renderText = "Font_Testing";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

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
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()), Times.Exactly(renderText.Length));
            Assert.Equal(12, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking4ParamsWithXAndYOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking4ParamsWithXAndYOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                11,
                22);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking3ParamsWithPositionOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking3ParamsWithPositionOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(33, 44));

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking6ParamsWithXAndYOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking6ParamsWithXAndYOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                321,
                202,
                2.25f,
                230f);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithPositionOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithPositionOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(66, 77),
                1.25f,
                8f);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithColorOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithColorOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                456,
                635,
                Color.DarkOrange);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(758, 137),
                Color.MediumPurple);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking6ParamsWithColorOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking6ParamsWithColorOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                147,
                185,
                16f,
                Color.IndianRed);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont()
        {
            // Arrange
            const string expectedTestDataFileName = $"{nameof(RenderFont_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont)}.json";
            var expectedBatchResultData =
                TestDataLoader.LoadTestData<FontGlyphBatchItem>(BatchTestDataDirPath, expectedTestDataFileName);

            var actualBatchResultData = new List<FontGlyphBatchItem>();

            const string line1 = "hello";
            const string line2 = "world";
            var renderText = $"{line1}{Environment.NewLine}{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(batchItem =>
                {
                    actualBatchResultData.Add(batchItem);
                });

            var batch = CreateRenderer();
            batch.Begin();

            // Act
            batch.Render(
                this.mockFont.Object,
                renderText,
                new Vector2(1255, 79),
                88f,
                Color.CornflowerBlue);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoked_RendersFont()
        {
            // Arrange
            const string textBeingRendered = "font";
            const string textNotBeingRendered = "testing";
            const string renderText = $"{textBeingRendered}{textNotBeingRendered}";

            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            MockFontBatchItems(renderText, 4, 11);

            var mockFontTextureAtlas = new Mock<ITexture>();
            mockFontTextureAtlas.SetupGet(p => p.Id).Returns(FontAtlasTextureId);

            this.mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTextureAtlas.Object);

            var batch = CreateRenderer();
            var totalAddFontGlyphBatchItemInvokes = 0;
            var doNotRaise = false;

            this.mockBatchServiceManager.Setup(m => m.AddFontGlyphBatchItem(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(_ =>
                {
                    totalAddFontGlyphBatchItemInvokes += 1;

                    if (totalAddFontGlyphBatchItemInvokes > textBeingRendered.Length || doNotRaise)
                    {
                        return;
                    }

                    this.mockBatchServiceManager.Raise(m => m.FontGlyphBatchFilled += null, EventArgs.Empty);
                    doNotRaise = true;
                });

            this.glInitReactor.OnNext(default);

            // Act
            batch.Begin();

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
            this.mockGLService.Verify(m => m.BindTexture2D(FontAtlasTextureId), Times.Once);
            this.mockBufferManager.Verify(m => m.UploadFontGlyphData(It.IsAny<FontGlyphBatchItem>(),
                    It.IsAny<uint>()),
                    Times.Exactly(textBeingRendered.Length));
            this.mockBatchServiceManager.VerifyOnce(m => m.EmptyBatch(BatchServiceType.FontGlyph));
        }

        [Fact]
        public void RenderRectangle_WithNoRectItemsToRender_SetsUpCorrectDebugGroupAndExits()
        {
            // Arrange
            const string shaderName = "TestRectShader";
            this.mockShaderManager.Setup(m => m.GetShaderName(ShaderType.Rectangle)).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockBatchServiceManager.Raise(e => e.RectBatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Rectangle Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Rectangle Process With {shaderName} Shader"));
            this.mockShaderManager.VerifyNever(m => m.Use(It.IsAny<ShaderType>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Rectangle Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockBufferManager.VerifyNever(m =>
                m.UploadRectData(It.IsAny<RectShape>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockBatchServiceManager.VerifyNever(m => m.EmptyBatch(BatchServiceType.Rectangle));
        }

        [Fact]
        public void RenderRectangle_WhenInvoked_AddsRectToBatch()
        {
            // Arrange
            var expected = new RectShape
            {
                Position = new Vector2(11, 22),
                Width = 33u,
                Height = 44u,
                IsFilled = true,
                BorderThickness = 0,
                CornerRadius = CornerRadius.Empty(),
                Color = Color.White,
                GradientType = ColorGradient.None,
                GradientStart = Color.Magenta,
                GradientStop = Color.Magenta,
            };

            var batch = CreateRenderer();

            // Act
            batch.Render(expected);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.AddRectBatchItem(expected), Times.Once);
        }

        [Fact]
        public void RenderRectangle_WhenInvoked_RendersRectangle()
        {
            // Arrange
            const uint batchIndex = 0;

            var shouldRenderItem = default(RectShape);
            shouldRenderItem.Position = new Vector2(1, 2);
            shouldRenderItem.Width = 3;
            shouldRenderItem.Height = 4;
            shouldRenderItem.BorderThickness = 5;
            shouldRenderItem.CornerRadius = new CornerRadius(6f, 7f, 8f, 9f);
            shouldRenderItem.GradientStart = Color.FromArgb(11, 22, 33, 44);
            shouldRenderItem.GradientStop = Color.FromArgb(55, 66, 77, 88);
            shouldRenderItem.GradientType = ColorGradient.Horizontal;

            var shouldNotRenderItem = default(RectShape);
            shouldNotRenderItem.Position = new Vector2(99, 111);
            shouldNotRenderItem.Width = 222;
            shouldNotRenderItem.Height = 333;
            shouldNotRenderItem.BorderThickness = 444;
            shouldNotRenderItem.CornerRadius = new CornerRadius(555f, 666f, 777f, 888f);
            shouldNotRenderItem.GradientStart = Color.FromArgb(112, 113, 114, 115);
            shouldNotRenderItem.GradientStop = Color.FromArgb(116, 117, 118, 119);
            shouldNotRenderItem.GradientType = ColorGradient.Vertical;

            var shouldNotRenderEmptyItem = default(RectShape);

            var items = new[] { (true, shouldRenderItem), (false, shouldNotRenderItem), (true, shouldNotRenderEmptyItem) };

            var batch = CreateRenderer();
            this.mockBatchServiceManager.Setup(m => m.AddRectBatchItem(It.IsAny<RectShape>()))
                .Callback<RectShape>(_ =>
                {
                    MockRectBatchItems(items);
                    this.mockBatchServiceManager.Raise(m => m.RectBatchFilled += null, EventArgs.Empty);
                });
            this.glInitReactor.OnNext(default);

            // Act
            batch.Begin();
            batch.Render(shouldRenderItem);

            // Assert
            this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 6 Rectangle Elements"));
            this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(3));
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Once());
            this.mockBufferManager.VerifyOnce(m => m.UploadRectData(shouldRenderItem, batchIndex));
            this.mockBufferManager.VerifyNever(m => m.UploadRectData(shouldNotRenderItem, batchIndex));
            this.mockBufferManager.VerifyNever(m => m.UploadRectData(shouldNotRenderEmptyItem, batchIndex));
            this.mockBatchServiceManager.Verify(m => m.EmptyBatch(BatchServiceType.Rectangle), Times.Once);
        }

        [Fact]
        public void End_WhenInvoked_EndsBatches()
        {
            // Arrange
            var manager = CreateRenderer();

            // Act
            manager.End();

            // Assert
            this.mockBatchServiceManager.Verify(m => m.EndBatch(BatchServiceType.Texture), Times.Once);
            this.mockBatchServiceManager.Verify(m => m.EndBatch(BatchServiceType.Rectangle), Times.Once);
            this.mockBatchServiceManager.Verify(m => m.EndBatch(BatchServiceType.FontGlyph), Times.Once);
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfRenderer()
        {
            // Arrange
            this.mockShutDownReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<ShutDownData>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IReactor<ShutDownData>>(reactor => this.shutDownReactor = reactor);

            CreateRenderer();

            // Act
            this.shutDownReactor?.OnNext(default);
            this.shutDownReactor?.OnNext(default);

            // Assert
            this.mockBatchServiceManager.Verify(m => m.Dispose(), Times.Once);
            this.mockBatchServiceManager.
                VerifyRemove(e => e.TextureBatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockBatchServiceManager.
                VerifyRemove(e => e.FontGlyphBatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockBatchServiceManager.
                VerifyRemove(e => e.RectBatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        }
        #endregion

        #region Indirect Tests
        [Fact]
        public void GlInitReactable_WhenCompleted_DisposesOfSubscription()
        {
            // Arrange
            var unused = CreateRenderer();

            // Act
            this.glInitReactor.OnCompleted();

            // Assert
            this.mockGLInitUnsubscriber.VerifyOnce(m => m.Dispose());
        }

        [Fact]
        public void ShutDownReactable_WhenCompleted_DisposesOfSubscription()
        {
            // Arrange
            var unused = CreateRenderer();

            // Act
            this.shutDownReactor.OnCompleted();

            // Assert
            this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        }
        #endregion

        /// <summary>
        /// Creates an <see cref="ITexture"/> instance for the purpose of testing the <see cref="Renderer"/> class.
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
        /// Creates a <see cref="TextureBatchItem"/> using the given parameters for the purpose of testing.
        /// </summary>
        /// <param name="x">The X location of the item.</param>
        /// <param name="y">The Y location of the item.</param>
        /// <param name="width">The width of the item.</param>
        /// <param name="height">The height of the item.</param>
        /// <param name="effects">The type of render effects to perform for the item.</param>
        /// <param name="clr">The color of the item.</param>
        /// <param name="textureId">The ID of the texture.</param>
        /// <returns>The instance to use for testing.</returns>
        private static TextureBatchItem CreateBatchItem(int x, int y, int width, int height, RenderEffects effects, Color clr, int textureId)
        {
            var result = default(TextureBatchItem);
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
            this.allGlyphMetrics = TestDataLoader.LoadTestData<GlyphMetrics>(string.Empty, GlyphTestDataFileName).ToList();
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
        /// Creates a new instance of <see cref="Renderer"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test with.</returns>
        private Renderer CreateRenderer()
        {
            var result = new Renderer(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderManager.Object,
                this.mockBufferManager.Object,
                this.mockBatchServiceManager.Object,
                this.mockGLInitReactable.Object,
                this.mockShutDownReactable.Object);

            return result;
        }

        /// <summary>
        /// Mocks the <see cref="BatchServiceManager.TextureBatchItems"/> property of the <see cref="IBatchServiceManager"/>.
        /// </summary>
        /// <param name="items">The items to store in the service.</param>
        private void MockTextureBatchItems((bool shouldRender, TextureBatchItem item)[] items)
        {
            this.mockBatchServiceManager.SetupProperty(p => p.TextureBatchItems);
            this.mockBatchServiceManager.Object.TextureBatchItems = items.ToReadOnlyDictionary();
        }

        /// <summary>
        /// Mocks the <see cref="BatchServiceManager.FontGlyphBatchItems"/> property of the <see cref="IBatchServiceManager"/>.
        /// </summary>
        /// <param name="batchGlyphs">The glyphs to mock.</param>
        /// <param name="excludeIndexStart">The starting index of the glyph to inclusively exclude from mocking.</param>
        /// <param name="excludeIndexStop">The ending index of the glyph to inclusively exclude from mocking.</param>
        /// <remarks>
        ///     The <paramref name="excludeIndexStart"/> and <paramref name="excludeIndexStop"/> are the index start and stop
        ///     of the given glyph string param <paramref name="batchGlyphs"/>.
        /// </remarks>
        private void MockFontBatchItems(string batchGlyphs, int excludeIndexStart, int excludeIndexStop)
        {
            var glyphsToMock = new List<(bool, FontGlyphBatchItem)>();

            for (var i = 0; i < batchGlyphs.Length; i++)
            {
                var shouldRender = i < excludeIndexStart || i > excludeIndexStop;
                var batchItem = default(FontGlyphBatchItem);
                batchItem.Glyph = batchGlyphs[i];
                batchItem.TextureId = FontAtlasTextureId;

                var glyphInBatchToMock = (shouldRender, batchItem);
                glyphsToMock.Add(glyphInBatchToMock);
            }

            this.mockBatchServiceManager.SetupProperty(p => p.FontGlyphBatchItems);
            this.mockBatchServiceManager.Object.FontGlyphBatchItems = glyphsToMock.ToReadOnlyDictionary();
        }

        /// <summary>
        /// Mocks the <see cref="BatchServiceManager.RectBatchItems"/> property of the <see cref="IBatchServiceManager"/>.
        /// </summary>
        /// <param name="items">The items to store in the service.</param>
        private void MockRectBatchItems((bool shouldRender, RectShape item)[] items)
        {
            this.mockBatchServiceManager.SetupProperty(p => p.RectBatchItems);
            this.mockBatchServiceManager.Object.RectBatchItems = items.ToReadOnlyDictionary();
        }
    }
}
