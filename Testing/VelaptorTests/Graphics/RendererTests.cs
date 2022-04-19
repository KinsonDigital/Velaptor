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
        private const uint TextureShaderId = 1111;
        private const uint FontShaderId = 2222;
        private const uint RectShaderId = 3333;
        private const char InvalidCharacter = '□';
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IOpenGLService> mockGLService;
        private readonly Mock<IShaderProgram> mockTextureShader;
        private readonly Mock<IGPUBuffer<TextureBatchItem>> mockTextureBuffer;
        private readonly Mock<IBatchingService<TextureBatchItem>> mockTextureBatchingService;
        private readonly Mock<IShaderProgram> mockFontShader;
        private readonly Mock<IGPUBuffer<FontGlyphBatchItem>> mockFontBuffer;
        private readonly Mock<IBatchingService<FontGlyphBatchItem>> mockFontBatchingService;
        private readonly Mock<IFont> mockFont;
        private readonly Mock<IShaderProgram> mockRectShader;
        private readonly Mock<IGPUBuffer<RectShape>> mockRectBuffer;
        private readonly Mock<IBatchingService<RectShape>> mockRectBatchingService;
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

            this.mockTextureShader = new Mock<IShaderProgram>();
            this.mockTextureShader.SetupGet(p => p.ShaderId).Returns(TextureShaderId);

            this.mockFontShader = new Mock<IShaderProgram>();
            this.mockFontShader.SetupGet(p => p.ShaderId).Returns(FontShaderId);

            this.mockRectShader = new Mock<IShaderProgram>();
            this.mockRectShader.SetupGet(p => p.ShaderId).Returns(RectShaderId);

            this.mockTextureBuffer = new Mock<IGPUBuffer<TextureBatchItem>>();
            this.mockFontBuffer = new Mock<IGPUBuffer<FontGlyphBatchItem>>();
            this.mockRectBuffer = new Mock<IGPUBuffer<RectShape>>();

            this.mockTextureBatchingService = new Mock<IBatchingService<TextureBatchItem>>();
            this.mockTextureBatchingService.SetupProperty(p => p.BatchSize);
            this.mockTextureBatchingService.SetupGet(p => p.BatchItems)
                .Returns(Array.Empty<(bool shouldRender, TextureBatchItem item)>().ToReadOnlyDictionary());

            this.mockFontBatchingService = new Mock<IBatchingService<FontGlyphBatchItem>>();
            this.mockFontBatchingService.SetupProperty(p => p.BatchSize);
            this.mockFontBatchingService.SetupGet(p => p.BatchItems)
                .Returns(Array.Empty<(bool shouldRender, FontGlyphBatchItem item)>().ToReadOnlyDictionary());

            this.mockRectBatchingService = new Mock<IBatchingService<RectShape>>();
            this.mockRectBatchingService.SetupProperty(p => p.BatchSize);
            this.mockRectBatchingService.SetupProperty(p => p.BatchItems);
            this.mockRectBatchingService.SetupGet(p => p.BatchItems)
                .Returns(Array.Empty<(bool shouldRender, RectShape item)>().ToReadOnlyDictionary());

            this.mockGLInitUnsubscriber = new Mock<IDisposable>();
            this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
            this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
                .Returns(this.mockGLInitUnsubscriber.Object)
                .Callback<IReactor<GLInitData>>(reactor =>
                {
                    if (reactor is null)
                    {
                        Assert.True(false, "Shutdown reactable subscription failed.  Reactor is null.");
                    }

                    this.glInitReactor = reactor;
                });

            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();

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
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'gl')");
        }

        [Fact]
        public void Ctor_WithNullTextureShaderParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    null,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'textureShader')");
        }

        [Fact]
        public void Ctor_WithNullFontShaderParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    null,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'fontShader')");
        }

        [Fact]
        public void Ctor_WithNullTextureGPUBufferParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    null,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'textureBuffer')");
        }

        [Fact]
        public void Ctor_WithNullRectShaderParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    null,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'rectShader')");
        }

        [Fact]
        public void Ctor_WithNullFontGPUBufferParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    null,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'fontBuffer')");
        }

        [Fact]
        public void Ctor_WithNullRectGPUBufferParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    null,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'rectBuffer')");
        }

        [Fact]
        public void Ctor_WithNullTextureBatchServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    null,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'textureBatchingService')");
        }

        [Fact]
        public void Ctor_WithNullFontBatchServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    null,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'fontBatchingService')");
        }

        [Fact]
        public void Ctor_WithNullRectBatchServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new Renderer(
                    this.mockGL.Object,
                    this.mockGLService.Object,
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    null,
                    this.mockGLInitReactable.Object,
                    this.mockShutDownReactable.Object);
            }, "The parameter must not be null. (Parameter 'rectBatchingService')");
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
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
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
                    this.mockTextureShader.Object,
                    this.mockFontShader.Object,
                    this.mockRectShader.Object,
                    this.mockTextureBuffer.Object,
                    this.mockFontBuffer.Object,
                    this.mockRectBuffer.Object,
                    this.mockTextureBatchingService.Object,
                    this.mockFontBatchingService.Object,
                    this.mockRectBatchingService.Object,
                    this.mockGLInitReactable.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownReactable')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SubscribesToBatchingServicesFilledEvent()
        {
            // Act
            var unused = CreateRenderer();

            // Assert
            this.mockTextureBatchingService
                .VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
            this.mockFontBatchingService
                .VerifyAdd(e => e.BatchFilled += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
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
            this.mockTextureBuffer.VerifySet(p => p.ViewPortSize = new SizeU(11u, 22u));
            this.mockFontBuffer.VerifySet(p => p.ViewPortSize = new SizeU(11u, 22u));
            this.mockRectBuffer.VerifySet(p => p.ViewPortSize = new SizeU(11u, 22u));
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
            this.mockTextureShader.SetupGet(p => p.Name).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockTextureBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Texture Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Texture Process With {shaderName} Shader"));
            this.mockTextureShader.VerifyNever(m => m.Use());
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Texture Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockTextureBuffer.VerifyNever(m =>
                m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockFontBatchingService.VerifyNever(m => m.EmptyBatch());
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

            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20);

            // Assert
            this.mockTextureBatchingService.Verify(m => m.Add(It.IsAny<TextureBatchItem>()), Times.Once);
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

            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedRenderEffects);

            // Assert
            this.mockTextureBatchingService.Verify(m => m.Add(It.IsAny<TextureBatchItem>()), Times.Once);
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

            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr);

            // Assert
            this.mockTextureBatchingService.Verify(m => m.Add(It.IsAny<TextureBatchItem>()), Times.Once);
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

            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(rect => actualBatchItem = rect);
            var batch = CreateRenderer();
            this.glInitReactor.OnNext(default);
            batch.Begin();

            // Act
            batch.Render(mockTexture.Object, 10, 20, expectedClr, expectedRenderEffects);

            // Assert
            this.mockTextureBatchingService.Verify(m => m.Add(It.IsAny<TextureBatchItem>()), Times.Once);
            AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
        }

        [Fact]
        public void RenderTexture_WhenInvoked_RendersTexture()
        {
            // Arrange
            const uint textureId = 1;
            const uint batchIndex = 0;

            var shouldRenderItem = default(TextureBatchItem);
            shouldRenderItem.Angle = 45;
            shouldRenderItem.TextureId = textureId;

            var shouldNotRenderItem = default(TextureBatchItem);
            var items = new[] { (true, shouldRenderItem), (false, shouldNotRenderItem) };

            // TODO: Fix this
            var batch = CreateRenderer();
            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(_ =>
                {
                    MockTextureBatchItems(items);
                    this.mockTextureBatchingService.Raise(m => m.BatchFilled += null, EventArgs.Empty);
                });
            this.glInitReactor.OnNext(default);

            // Act
            batch.Begin();

            batch.Render(
                MockTexture(textureId),
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
            this.mockGLService.Verify(m => m.BindTexture2D(textureId), Times.Once);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldRenderItem, batchIndex), Times.Once);
            this.mockTextureBuffer.Verify(m => m.UploadData(shouldNotRenderItem, batchIndex), Times.Never);
            this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
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
            this.mockFontShader.SetupGet(p => p.Name).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockFontBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Text Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Text Process With {shaderName} Shader"));
            this.mockFontShader.VerifyNever(m => m.Use());
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Character Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockFontBuffer.VerifyNever(m =>
                m.UploadData(It.IsAny<FontGlyphBatchItem>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Font Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockFontBatchingService.VerifyNever(m => m.EmptyBatch());
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()), Times.Never);
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()), Times.Never);
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
            const string renderText = "hello\nworld";

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
            var actualBatchResultData = new List<FontGlyphBatchItem>();// Array.Empty<FontGlyphBatchItem>();

            const string renderText = "Font_Testing";
            MockFontMetrics();
            MockToGlyphMetrics(renderText);
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()), Times.Exactly(renderText.Length));
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
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
            const string renderText = $"{line1}\n{line2}";
            var totalGlyphs = line1.Length + line2.Length;

            MockFontMetrics();
            MockToGlyphMetrics("hello");
            MockToGlyphMetrics("world");
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
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
            this.mockFontBatchingService.Verify(m => m.Add(It.IsAny<FontGlyphBatchItem>()),
                Times.Exactly(totalGlyphs));
            Assert.Equal(totalGlyphs, actualBatchResultData.Count);
            AssertExtensions.ItemsEqual(expectedBatchResultData, actualBatchResultData);
        }

        [Fact]
        public void RenderFont_WhenInvoked_RendersFont()
        {
            // Arrange
            const uint textureId = 2;
            const string textBeingRendered = "font";
            const string textNotBeingRendered = "testing";
            const string renderText = $"{textBeingRendered} {textNotBeingRendered}";

            MockFontMetrics();
            MockToGlyphMetrics(renderText);

            var mockFontTextureAtlas = new Mock<ITexture>();
            mockFontTextureAtlas.SetupGet(p => p.Id).Returns(textureId);

            this.mockFont.SetupGet(p => p.FontTextureAtlas).Returns(mockFontTextureAtlas.Object);

            var batch = CreateRenderer();

            this.mockFontBatchingService.Setup(m => m.AddRange(It.IsAny<IEnumerable<FontGlyphBatchItem>>()))
                .Callback<IEnumerable<FontGlyphBatchItem>>(items =>
                {
                    var itemsToMock = items.Select(item => (shouldRender: false, item)).ToArray();

                    // Set only the text characters of the "font" section to be rendered
                    for (var i = 0; i < textBeingRendered.Length; i++)
                    {
                        itemsToMock[i].shouldRender = true;
                    }

                    MockFontBatchItems(itemsToMock);
                    this.mockFontBatchingService.Raise(m => m.BatchFilled += null, EventArgs.Empty);
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
            this.mockGLService.Verify(m => m.BindTexture2D(textureId), Times.Once);
            this.mockFontBuffer.Verify(m => m.UploadData(It.IsAny<FontGlyphBatchItem>(),
                    It.IsAny<uint>()),
                Times.Exactly(textBeingRendered.Length));
            this.mockFontBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
        }

        [Fact]
        public void RenderRectangle_WithNoRectItemsToRender_SetsUpCorrectDebugGroupAndExits()
        {
            // Arrange
            const string shaderName = "TestRectShader";
            this.mockFontShader.SetupGet(p => p.Name).Returns(shaderName);
            var unused = CreateRenderer();

            // Act
            this.mockRectBatchingService.Raise(e => e.BatchFilled += null, EventArgs.Empty);

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Render Rectangle Process - Nothing To Render"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
            this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Rectangle Process With {shaderName} Shader"));
            this.mockRectShader.VerifyNever(m => m.Use());
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Rectangle Data - TextureID"))));
            this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
            this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
            this.mockRectBuffer.VerifyNever(m =>
                m.UploadData(It.IsAny<RectShape>(), It.IsAny<uint>()));
            this.mockGLService.VerifyNever(m =>
                m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
            this.mockGL.VerifyNever(m => m.DrawElements(
                It.IsAny<GLPrimitiveType>(),
                It.IsAny<uint>(),
                It.IsAny<GLDrawElementsType>(),
                It.IsAny<IntPtr>()));
            this.mockRectBatchingService.VerifyNever(m => m.EmptyBatch());
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

            this.mockRectBatchingService.Setup(m => m.Add(It.IsAny<RectShape>()));

            var batch = CreateRenderer();

            // Act
            batch.Render(expected);

            // Assert
            this.mockRectBatchingService.Verify(m => m.Add(expected), Times.Once);
        }

        [Fact]
        public void EndBatch_WithEmptyTextureBatch_DoesNotAttemptRender()
        {
            // Arrange
            var textureBatchItems = new[] { (false, default(TextureBatchItem)) };

            var batch = CreateRenderer();

            // Mock that the batch items will return a single batch item that is marked to NOT render
            this.mockTextureBatchingService.Setup(m => m.Add(It.IsAny<TextureBatchItem>()))
                .Callback<TextureBatchItem>(_ =>
                {
                    MockTextureBatchItems(textureBatchItems);
                });

            this.glInitReactor.OnNext(default);
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
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void EndBatch_WithEmptyFontBatch_DoesNotAttemptRender()
        {
            // Arrange
            const string textToRender = "test-text";
            var fontBatchItems = new[] { (false, default(TextureBatchItem)) };

            var batch = CreateRenderer();
            this.mockFontBatchingService.Setup(m => m.Add(It.IsAny<FontGlyphBatchItem>()))
                .Callback<FontGlyphBatchItem>(_ =>
                {
                    MockTextureBatchItems(fontBatchItems);
                });
            MockToGlyphMetrics(textToRender);
            this.glInitReactor.OnNext(default);
            batch.BeginBatch();

            batch.Render(
                this.mockFont.Object,
                textToRender,
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

            // Act
            batch.EndBatch();

            // Assert
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Never());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Never);
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void EndBatch_WithBatchOfRectangles_SetsUpOpenGLDebugGroups()
        {
            // Arrange
            const string rectShaderName = "rect-test-shader";
            this.mockRectShader.SetupGet(p => p.Name).Returns(rectShaderName);
            this.mockGLService.Setup(m => m.BeginGroup($"Render Rectangle Process With {rectShaderName} Shader"))
                .CallbackInOrder(nameof(IOpenGLService.BeginGroup), 10, 1);
            this.mockGLService.Setup(m => m.BeginGroup("Update Rectangle Data - BatchItem(0)"))
                .CallbackInOrder(nameof(IOpenGLService.BeginGroup), 20, 2);

            var rectBatchItems = new[] { (true, GenerateRectShape(0)) };
            var batch = CreateRenderer();

            this.mockRectBatchingService.SetupGet(p => p.BatchItems)
                .Returns(rectBatchItems.ToReadOnlyDictionary());

            // Act
            batch.EndBatch();

            // Assert
            this.mockGLService.Verify(m => m.BeginGroup("Update Rectangle Data - BatchItem(0)"), Times.Once);
            this.mockGLService.Verify(m => m.EndGroup(), Times.AtLeastOnce);
        }

        [Fact]
        public void EndBatch_WithEmptyRectBatch_DoesNotAttemptRender()
        {
            // Arrange
            var rectBatchItems = new[] { (false, default(RectShape)) };

            this.mockRectBatchingService.SetupGet(p => p.BatchItems)
                .Returns(rectBatchItems.ToReadOnlyDictionary());

            var batch = CreateRenderer();

            this.glInitReactor.OnNext(default);
            batch.BeginBatch();

            var rect = new RectShape
            {
                Position = new Vector2(11, 22),
                Width = 33,
                Height = 44,
            };

            batch.Render(rect);

            // Act
            batch.EndBatch();

            // Assert
            this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, IntPtr.Zero), Times.Never());
            this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Never);
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
            this.mockTextureBuffer.Verify(m => m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfRenderer()
        {
            // Arrange
            IReactor<ShutDownData>? shutDownReactor = null;

            this.mockShutDownReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<ShutDownData>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IReactor<ShutDownData>>(reactor => shutDownReactor = reactor);

            CreateRenderer();

            // Act
            shutDownReactor?.OnNext(default);
            shutDownReactor?.OnNext(default);

            // Assert
            this.mockTextureBatchingService
                .VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once());
            this.mockFontBatchingService
                .VerifyRemove(e => e.BatchFilled -= It.IsAny<EventHandler<EventArgs>>(), Times.Once());
            this.mockGLInitUnsubscriber.Verify(m => m.Dispose(), Times.Once());
            this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
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

        /// <summary>
        /// Generates a <see cref="RectShape"/> with unique sequential data for its values for the purpose of testing.
        /// </summary>
        /// <param name="start">The start of the data sequence.</param>
        /// <returns>The instance to use for testing.</returns>
        private static RectShape GenerateRectShape(float start)
        {
            return new RectShape
            {
                Position = new Vector2(start + 1f, start + 2f),
                Width = start + 3f,
                Height = start + 4f,
                Color = Color.FromArgb((byte)start + 5, (byte)start + 6, (byte)start + 7, (byte)start + 8),
                IsFilled = true,
                BorderThickness = start + 9,
                CornerRadius = new CornerRadius(10, start + 11, start + 12, start + 13),
                GradientType = ColorGradient.None,
                GradientStart = Color.FromArgb((byte)start + 14, (byte)start + 15, (byte)start + 16, (byte)start + 17),
                GradientStop = Color.FromArgb((byte)start + 18, (byte)start + 19, (byte)start + 20, (byte)start + 21),
            };
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
                this.mockTextureShader.Object,
                this.mockFontShader.Object,
                this.mockRectShader.Object,
                this.mockTextureBuffer.Object,
                this.mockFontBuffer.Object,
                this.mockRectBuffer.Object,
                this.mockTextureBatchingService.Object,
                this.mockFontBatchingService.Object,
                this.mockRectBatchingService.Object,
                this.mockGLInitReactable.Object,
                this.mockShutDownReactable.Object);

            return result;
        }

        /// <summary>
        /// Mocks the batch items property of the texture batch service.
        /// </summary>
        /// <param name="items">The items to store in the service.</param>
        private void MockTextureBatchItems((bool shouldRender, TextureBatchItem item)[] items)
        {
            this.mockTextureBatchingService.SetupProperty(p => p.BatchItems);
            this.mockTextureBatchingService.Object.BatchItems = items.ToReadOnlyDictionary();
        }

        /// <summary>
        /// Mocks the batch items property of the font batch service.
        /// </summary>
        /// <param name="items">The items to store in the service.</param>
        private void MockFontBatchItems((bool shouldRender, FontGlyphBatchItem item)[] items)
        {
            this.mockFontBatchingService.SetupProperty(p => p.BatchItems);
            this.mockFontBatchingService.Object.BatchItems = items.ToReadOnlyDictionary();
        }
    }
}
