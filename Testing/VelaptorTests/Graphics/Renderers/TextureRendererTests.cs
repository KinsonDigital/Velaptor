// <copyright file="TextureRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Drawing;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Factories;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;

using TextureRenderItem = Carbonate.Core.UniDirectional.IReceiveReactor<
    System.Memory<
        Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.TextureBatchItem
        >
    >
>;


/// <summary>
/// Tests the <see cref="TextureRenderer"/> class.
/// </summary>
public class TextureRendererTests
{
    private const uint TextureShaderId = 1111u;
    private const uint TextureId = 456u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IGPUBuffer<TextureBatchItem>> mockGPUBuffer;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchBegunUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private IReceiveReactor? batchHasBegunReactor;
    private TextureRenderItem? renderReactor;
    private IReceiveReactor? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRendererTests"/> class.
    /// </summary>
    public TextureRendererTests()
    {
        this.mockGL = new Mock<IGLInvoker>();

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

        this.mockShader = new Mock<IShaderProgram>();
        this.mockShader.SetupGet(p => p.ShaderId).Returns(TextureShaderId);

        this.mockGPUBuffer = new Mock<IGPUBuffer<TextureBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();
        this.mockBatchingManager.Name = nameof(this.mockBatchingManager);

        this.mockBatchBegunUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        var mockRenderUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                if (reactor.Id == PushNotifications.RenderTexturesId)
                {
                    return mockRenderUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.BatchHasBegunId)
                {
                    return this.mockBatchBegunUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            })
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasBegunId)
                {
                    this.batchHasBegunReactor = reactor;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            });

        var mockTextureRenderBatchReactable = new Mock<IRenderBatchReactable<TextureBatchItem>>();
        mockTextureRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<TextureRenderItem>()))
            .Returns<TextureRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                return mockRenderUnsubscriber.Object;
            })
            .Callback<TextureRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"TextureRendererTests.Ctor - {nameof(PushNotifications.RenderTexturesId)}");

                this.renderReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderTextureReactable())
            .Returns(mockTextureRenderBatchReactable.Object);

        var mockFontTextureAtlas = new Mock<ITexture>();
        mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
        mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                null,
                this.mockGPUBuffer.Object,
                this.mockShader.Object,
                this.mockBatchingManager.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                this.mockGLService.Object,
                null,
                this.mockShader.Object,
                this.mockBatchingManager.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'buffer')");
    }

    [Fact]
    public void Ctor_WithNullShaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                this.mockGLService.Object,
                this.mockGPUBuffer.Object,
                null,
                this.mockBatchingManager.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'shader')");
    }

    [Fact]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                this.mockGLService.Object,
                this.mockGPUBuffer.Object,
                this.mockShader.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'batchManager')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Render_WhenNotCallingBeginFirst_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<InvalidOperationException>(() =>
        {
            sut.Render(
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
    public void Render_WithSourceRectWithNoWidthOrHeight_ThrowsException(int width, int height)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
        {
            sut.Render(
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
    public void Render_WithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            sut.Render(
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
    public void Render_WithNoTextureItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestTextureShader";
        this.mockShader.SetupGet(p => p.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render Texture Process - Nothing To Render"));
        this.mockGLService.VerifyOnce(m => m.EndGroup());
        this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Texture Process With {shaderName} Shader"));
        this.mockShader.VerifyNever(m => m.Use());
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Texture Data - TextureID"))));
        this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
        this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
        this.mockGPUBuffer.VerifyNever(m =>
            m.UploadData(It.IsAny<TextureBatchItem>(), It.IsAny<uint>()));
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
        this.mockGL.VerifyNever(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()));
    }

    [Fact]
    public void Render_WhenInvoking4ParamOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        var expectedSrcRect = new RectangleF(0f, 0f, expectedWidth, expectedHeight);
        var expectedDestRect = new RectangleF(expectedX, expectedY, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingManager.Setup(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), It.IsAny<int>()))
            .Callback<TextureBatchItem, int>((item, _) =>
            {
                actualBatchItem = item;
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, 123);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), 123));
        actualBatchItem.Should().BeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    public void Render_WhenInvoking5ParamOverloadWithEffects_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipHorizontally;
        var expectedSrcRect = new RectangleF(0f, 0f, expectedWidth, expectedHeight);
        var expectedDestRect = new RectangleF(expectedX, expectedY, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            expectedRenderEffects,
            textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingManager.Setup(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), It.IsAny<int>()))
            .Callback<TextureBatchItem, int>((item, _) =>
            {
                actualBatchItem = item;
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), 123));
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoking5ParamOverloadWithColor_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        var expectedSrcRect = new RectangleF(0f, 0f, expectedWidth, expectedHeight);
        var expectedDestRect = new RectangleF(expectedX, expectedY, expectedWidth, expectedHeight);
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            RenderEffects.None,
            textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingManager.Setup(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), It.IsAny<int>()))
            .Callback<TextureBatchItem, int>((item, _) =>
            {
                actualBatchItem = item;
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedClr, 123);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), 123));
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoking6ParamOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipVertically;
        var expectedSrcRect = new RectangleF(0f, 0f, expectedWidth, expectedHeight);
        var expectedDestRect = new RectangleF(expectedX, expectedY, expectedWidth, expectedHeight);
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            expectedRenderEffects,
            textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingManager.Setup(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), It.IsAny<int>()))
            .Callback<TextureBatchItem, int>((item, _) =>
            {
                actualBatchItem = item;
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedClr, expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddTextureItem(It.IsAny<TextureBatchItem>(), 123));
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoked_RendersTexture()
    {
        // Arrange
        const uint itemABatchIndex = 0;
        const uint itemBBatchIndex = 1;
        const uint expectedTotalElements = 12;

        var batchItemA = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            1,
            45,
            Color.Empty,
            RenderEffects.None,
            TextureId);

        var batchItemB = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            90,
            Color.Empty,
            RenderEffects.None,
            TextureId);

        var renderItemA = new RenderItem<TextureBatchItem> { Layer = 1, Item = batchItemA, };
        var renderItemB = new RenderItem<TextureBatchItem> { Layer = 2, Item = batchItemB, };

        var items = new Memory<RenderItem<TextureBatchItem>>(new[] { renderItemA, renderItemB });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        sut.Render(
            MockTexture(TextureId),
            new Rectangle(0, 0, 1, 2),
            It.IsAny<Rectangle>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>(),
            It.IsAny<RenderEffects>());

        // Act
        this.renderReactor.OnReceive(items);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 12 Texture Elements"));
        this.mockGL.VerifyOnce(m
            => m.DrawElements(GLPrimitiveType.Triangles, expectedTotalElements, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGLService.VerifyOnce(m => m.BindTexture2D(TextureId));
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(batchItemA, itemABatchIndex));
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(batchItemB, itemBBatchIndex));
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WithShutDownNotification_ShutsDownRenderer()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnReceive();
        this.shutDownReactor.OnReceive();

        // Assert
        this.mockBatchBegunUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates an <see cref="ITexture"/> instance for the purpose of testing the <see cref="TextureRenderer"/> class.
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
    /// Creates a new instance of <see cref="TextureRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGPUBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);
}
