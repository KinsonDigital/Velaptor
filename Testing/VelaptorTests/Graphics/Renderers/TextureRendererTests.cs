// <copyright file="TextureRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Drawing;
using System.IO.Abstractions;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Shouldly;
using Helpers;
using NSubstitute;
using NSubstitute.Core;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Graphics.Renderers.Exceptions;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;
using Velaptor.WebGPU.Batching;
using TextureRenderItem = Carbonate.Core.OneWay.IReceiveSubscription<System.Memory<Velaptor.WebGPU.Batching.RenderItem<
            Velaptor.OpenGL.Batching.TextureBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="TextureRenderer"/> class.
/// </summary>
public class TextureRendererTests : TestsBase
{
    private const uint TextureId = 456u;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IGpuBuffer<TextureBatchItem> mockGpuBuffer;
    private readonly IShaderProgram mockShader;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable mockPushReactable;
    private readonly IRenderBatchReactable<TextureBatchItem> mockTextureRenderBatchReactable;
    private IReceiveSubscription? batchHasBegunReactor;
    private TextureRenderItem? renderReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRendererTests"/> class.
    /// </summary>
    public TextureRendererTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockShader = Substitute.For<IShaderProgram>();
        this.mockGpuBuffer = Substitute.For<IGpuBuffer<TextureBatchItem>>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable
            .When(m => m.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci => this.batchHasBegunReactor = ci.Arg<IReceiveSubscription>());

        this.mockTextureRenderBatchReactable = Substitute.For<IRenderBatchReactable<TextureBatchItem>>();
        this.mockTextureRenderBatchReactable
            .When(m => m.Subscribe(Arg.Any<TextureRenderItem>()))
            .Do(ci => this.renderReactor = ci.Arg<TextureRenderItem>());

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory
            .CreateNoDataPushReactable()
            .Returns(this.mockPushReactable);
        this.mockReactableFactory
            .CreateRenderTextureReactable()
            .Returns(this.mockTextureRenderBatchReactable);
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL,
                this.mockReactableFactory,
                null,
                this.mockGpuBuffer,
                this.mockShader,
                this.mockBatchingManager);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL,
                this.mockReactableFactory,
                this.mockGLService,
                null,
                this.mockShader,
                this.mockBatchingManager);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'buffer')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullShaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL,
                this.mockReactableFactory,
                this.mockGLService,
                this.mockGpuBuffer,
                null,
                this.mockBatchingManager);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'shader')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureRenderer(
                this.mockGL,
                this.mockReactableFactory,
                this.mockGLService,
                this.mockGpuBuffer,
                this.mockShader,
                null);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'batchManager')");
    }
    #endregion

    #region Method Tests
    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenNotCallingBeginFirst_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
        {
            sut.Render(
                Substitute.For<ITexture>(),
                new Rectangle(10, 20, 30, 40),
                default,
                default,
                default,
                default,
                default);
        });

        exception.Message.ShouldBe("The 'Begin()' method must be invoked first before any 'Render()' methods.");
    }

    [Theory]
    [Trait("Category", Method)]
    [InlineData(0, 20)]
    [InlineData(-10, 20)]
    [InlineData(10, 0)]
    [InlineData(10, -20)]
    public void Render_WithSourceRectWithNoWidthOrHeight_ThrowsException(int width, int height)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
        {
            sut.Render(
                Substitute.For<ITexture>(),
                new Rectangle(1, 2, width, height),
                default,
                default,
                default,
                default,
                default);
        });

        exception.Message.ShouldBe("The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() =>
        {
            sut.Render(
                null,
                new Rectangle(10, 20, 30, 40),
                default,
                default,
                default,
                default,
                default);
        });

        exception.Message.ShouldBe($"Cannot render a null '{nameof(ITexture)}'. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WithZeroWidthOrHeightTexture_ThrowsException()
    {
        // Arrange
        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Width.Returns(0u);
        mockTexture.Height.Returns(0u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
        {
            sut.Render(
                texture: mockTexture,
                x: 10,
                y: 20,
                color: default,
                effects: default);
        });

        exception.Message.ShouldBe("The source rectangle must have a width and height greater than zero. (Parameter 'rects')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithNoTextureItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestTextureShader";
        this.mockShader.Name.Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render Texture Process - Nothing To Render");
        this.mockGLService.Received(1).EndGroup();
        this.mockGLService.DidNotReceive().BeginGroup($"Render Texture Process With {shaderName} Shader");
        this.mockShader.DidNotReceive().Use();
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Update Texture Data - TextureID")));
        this.mockGL.DidNotReceive().ActiveTexture(Arg.Any<GLTextureUnit>());
        this.mockGLService.DidNotReceive().BindTexture2D(Arg.Any<uint>());
        this.mockGpuBuffer
            .DidNotReceive()
            .UploadData(Arg.Any<TextureBatchItem>(), Arg.Any<uint>());
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements")));
        this.mockGL
            .DidNotReceive()
            .DrawElements(
                Arg.Any<GLPrimitiveType>(),
                Arg.Any<uint>(),
                Arg.Any<GLDrawElementsType>(),
                Arg.Any<nint>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With4ParamAndIntPosOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndIntPosOverloadWithAngle_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            180f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, 180, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamAndIntPosOverloadWithSize_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1.5f,
            180f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, 180, 1.5f, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamAndVectorPosOverloadWithSizeAndColor_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1.5f,
            180f,
            Color.FromArgb(11, 22, 33, 44),
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, 180, 1.5f, Color.FromArgb(11, 22, 33, 44), 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndIntPosOverloadWithEffects_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipHorizontally;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            expectedRenderEffects,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndIntPosOverloadWithColor_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, expectedClr, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamAndIntPosOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipVertically;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            expectedRenderEffects,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, 10, 20, expectedClr, expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With4ParamAndVectorPosOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndVectorPosOverloadWithAngle_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            180f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), 180, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndVectorPosOverloadWithAngleAndSize_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1.5f,
            180f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), 180, 1.5f, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamAndVectorPosOverloadWithAngleAndSizeAndColor_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1.5f,
            180f,
            Color.FromArgb(30, 40, 50, 60),
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), 180, 1.5f, Color.FromArgb(30, 40, 50, 60), 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndVectorPosOverloadWithEffects_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipHorizontally;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            expectedRenderEffects,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamAndVectorPosOverloadWithColor_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            RenderEffects.None,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), expectedClr, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamAndVectorPosOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipVertically;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects();
        var expectedClr = Color.FromArgb(11, 22, 33, 44);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            expectedClr,
            expectedRenderEffects,
            TextureId);

        var mockTexture = CreateTextureMock(TextureId, expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture, new Vector2(10, 20), expectedClr, expectedRenderEffects, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Theory]
    [Trait("Category", Method)]
    [InlineData(0f, 10f)]
    [InlineData(10f, 0f)]
    public void Render_With8ParamOverloadAndSrcRectWidthOrHeightIsZero_ThrowsException(
        int srcRectWidth,
        int srcRectHeight)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        var act = () => sut.Render(
            MockTexture(TextureId),
            new Rectangle(10, 20, srcRectWidth, srcRectHeight),
            default,
            default,
            default,
            default,
            default);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With8ParamOverload_RendersTexture()
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
            default,
            default,
            default,
            default,
            default);

        // Act
        this.renderReactor.OnReceive(items);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render 12 Texture Elements");
        this.mockGL
            .Received(1)
            .DrawElements(GLPrimitiveType.Triangles, expectedTotalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.mockGLService.Received(1).BindTexture2D(TextureId);
        this.mockGpuBuffer.Received(1).UploadData(batchItemA, itemABatchIndex);
        this.mockGpuBuffer.Received(1).UploadData(batchItemB, itemBBatchIndex);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamOverloadWithNullAtlasData_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20));

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithNullAtlasData_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20), Color.CornflowerBlue);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithNullAtlasDataAndAngle_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20), 25f);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithNullAtlasDataAndAngleAndSize_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20), 35f, 1.4f);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamOverloadWithNullAtlasDataAndAngleAndColor_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20), 45f, Color.IndianRed);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With8ParamOverloadWithNullAtlasDataAndAngleAndSizeAndColor_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(10, 20), 15f, 1.2f, Color.IndianRed);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With9ParamOverloadWithNullAtlasDataAndAngleAndSizeAndColorAndEffects_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(
            null,
            "test-texture",
            new Vector2(10, 20),
            15f,
            1.2f,
            Color.IndianRed,
            RenderEffects.FlipVertically);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamOverloadWithInvalidFrameNumber_ThrowsException()
    {
        // Arrange
        const string expectedMsg = "The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () => sut.Render(mockAtlas, "test-sub-texture", new Vector2(10, 20), 1234);

        // Assert
        var exception = act.ShouldThrow<RendererException>();
        exception.Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithInvalidFrameNumber_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () =>
            sut.Render(mockAtlas, "test-sub-texture", new Vector2(10, 20), Color.CornflowerBlue, 1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithInvalidFrameNumberAndAngle_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () => sut.Render(mockAtlas, "test-sub-texture", new Vector2(10, 20), 25f, 1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamOverloadWithInvalidFrameNumberAndAngleAndSize_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () =>
            sut.Render(mockAtlas, "test-sub-texture", new Vector2(10, 20), 35f, 1.4f, 1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamOverloadWithInvalidFrameNumberAndAngleAndColor_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () =>
            sut.Render(mockAtlas, "test-sub-texture", new Vector2(10, 20), 45f, Color.IndianRed, 1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With9ParamOverloadWithInvalidFrameNumberAndAngleAndSizeAndColor_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () =>
            sut.Render(
                mockAtlas,
                "test-sub-texture",
                new Vector2(10, 20),
                15f,
                1.2f,
                Color.IndianRed,
                1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With9ParamOverloadWithInvalidFrameNumberAndAngleAndSizeAndColorAndEffects_ThrowsException()
    {
        // Arrange
        const string expectedMsg = $"The frame number '1234' is invalid for atlas 'test-atlas-texture' and sub-texture 'test-sub-texture'." +
                                   "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var sut = CreateSystemUnderTest();
        var mockAtlas = CreateAtlasDataMock(10, 20);

        // Act
        var act = () =>
            sut.Render(
                mockAtlas,
                "test-sub-texture",
                new Vector2(10, 20),
                15f,
                1.2f,
                Color.IndianRed,
                RenderEffects.FlipHorizontally,
                1234);

        // Assert
        act.ShouldThrow<RendererException>().Message.ShouldBe(expectedMsg);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With5ParamOverloadWithPos_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithPosAndColor_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        var color = Color.FromArgb(20, 30, 40, 50);
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            0f,
            color,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, color, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With6ParamOverloadWithPosAndAngle_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const float angle = 45f;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            angle,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, angle, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamOverloadWithPosAndAngleAndSize_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const float angle = 45f;
        const float size = 1.5f;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            size,
            angle,
            Color.White,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, angle, size, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With7ParamOverloadWithPosAndAngleAndColor_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const float angle = 45f;
        var color = Color.FromArgb(30, 40, 50, 60);
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            1f,
            angle,
            color,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, angle, color, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With8ParamOverloadWithPosAndAngleAndSizeAndColor_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const float size = 1.4f;
        const float angle = 45f;
        var color = Color.FromArgb(30, 40, 50, 60);
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            size,
            angle,
            color,
            RenderEffects.None,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, angle, size, color, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_With8ParamOverloadWithPosAndAngleAndSizeAndColorAndEffects_AddsItemToBatch()
    {
        // Arrange
        var pos = new Vector2(10, 20);
        const float size = 1.4f;
        const float angle = 45f;
        var color = Color.FromArgb(30, 40, 50, 60);
        const RenderEffects effects = RenderEffects.FlipBothDirections;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        (RectangleF expectedSrcRect, RectangleF expectedDestRect) = CreateExpectedRects(pos, expectedWidth, expectedHeight);

        var expectedBatchItem = BatchItemFactory.CreateTextureItem(
            expectedSrcRect,
            expectedDestRect,
            size,
            angle,
            color,
            effects,
            TextureId);

        var mockAtlasData = CreateAtlasDataMock(expectedWidth, expectedHeight);

        TextureBatchItem actualBatchItem = default;

        MockAddTextureItem().Do(ci => actualBatchItem = ci.Arg<TextureBatchItem>());

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockAtlasData, "test-texture", pos, angle, size, color, effects, 0, 123);

        // Assert
        this.mockBatchingManager
            .Received(1)
            .AddTextureItem(Arg.Any<TextureBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchItem.ShouldBeEquivalentTo(expectedBatchItem);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void PushReactable_WhenCreatingAndDisposingOfSubscription_CreatesAndDisposesOfSubscriptionCorrectly()
    {
        // Arrange
        IReceiveSubscription? reactor = null;
        IDisposable mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockPushReactable
            .Subscribe(Arg.Any<IReceiveSubscription>())
            .Returns(mockUnsubscriber)
            .AndDoes(ci =>
            {
                var reactorParam = ci.Arg<IReceiveSubscription>();
                reactorParam.ShouldNotBeNull("It is required for unit testing.");
                reactor = reactorParam;
            });

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnUnsubscribe();

        // Assert
        mockUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void TextureRenderBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange
        TextureRenderItem? reactor = null;
        IDisposable mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockTextureRenderBatchReactable
            .Subscribe(Arg.Any<TextureRenderItem>())
            .Returns(mockUnsubscriber)
            .AndDoes(ci =>
            {
                var reactorParam = ci.Arg<TextureRenderItem>();
                reactorParam.ShouldNotBeNull("It is required for unit testing.");
                reactor = reactorParam;
            });

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnUnsubscribe();

        // Assert
        mockUnsubscriber.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates an <see cref="ITexture"/> instance for the purpose of testing the <see cref="TextureRenderer"/> class.
    /// </summary>
    /// <param name="textureId">The texture ID to use for the test.</param>
    /// <returns>The instance to use for testing.</returns>
    private static ITexture MockTexture(uint textureId)
    {
        var mockResult = Substitute.For<ITexture>();
        mockResult.Id.Returns(textureId);

        return mockResult;
    }

    /// <summary>
    /// Mocks the <see cref="ITexture"/> interface.
    /// </summary>
    /// <param name="textureId">The texture ID to mock.</param>
    /// <param name="expectedWidth">The texture width to mock.</param>
    /// <param name="expectedHeight">The texture height to mock.</param>
    /// <returns>The mocked texture.</returns>
    private static ITexture CreateTextureMock(
        uint textureId,
        uint expectedWidth,
        uint expectedHeight)
    {
        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(textureId);
        mockTexture.Width.Returns(expectedWidth);
        mockTexture.Height.Returns(expectedHeight);

        return mockTexture;
    }

    /// <summary>
    /// Creates two <see cref="RectangleF"/>s that are expected to be used in the tests.
    /// </summary>
    /// <returns>The expected rectangles.</returns>
    private static (RectangleF, RectangleF) CreateExpectedRects()
    {
        var srcRect = new RectangleF(0f, 0f, 111, 222);
        var destRect = new RectangleF(10, 20, 111, 222);

        return (srcRect, destRect);
    }

    /// <summary>
    /// Creates two <see cref="RectangleF"/>s that are expected to be used in the tests.
    /// </summary>
    /// <param name="pos">The position to render.</param>
    /// <param name="width">The width of the src and des rects.</param>
    /// <param name="height">The height of the src and des rects.</param>
    /// <returns>The expected rectangles.</returns>
    private static (RectangleF, RectangleF) CreateExpectedRects(Vector2 pos, int width, int height)
    {
        var srcRect = new RectangleF(0f, 0f, width, height);
        var destRect = new RectangleF(pos.X, pos.Y, width, height);

        return (srcRect, destRect);
    }

    /// <summary>
    /// Creates a mock object of the <see cref="IAtlasData"/> interface.
    /// </summary>
    /// <param name="width">The width of the bounds.</param>
    /// <param name="height">The height of the bounds.</param>
    /// <returns>The mocked object.</returns>
    private static IAtlasData CreateAtlasDataMock(int width, int height)
    {
        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(TextureId);
        mockTexture.Width.Returns((uint)width);
        mockTexture.Height.Returns((uint)height);

        var mockPath = Substitute.For<IPath>();
        mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns("test-atlas");

        var subTextureData = new AtlasSubTextureData { Name = "test-sub-texture", Bounds = new Rectangle(0, 0, width, height), FrameIndex = 0, };
        var subTextureDataItems = new[] { subTextureData };

        var mock = Substitute.For<IAtlasData>();
        mock.Name.Returns("test-atlas-texture");
        mock.Texture.Returns(mockTexture);
        mock.GetFrames(Arg.Any<string>()).Returns(subTextureDataItems);

        return mock;
    }

    /// <summary>
    /// Mocks the <see cref="IBatchingManager.AddTextureItem(TextureBatchItem, int, DateTime)"/> method.
    /// </summary>
    /// <returns>The mock setup.</returns>
    private WhenCalled<IBatchingManager> MockAddTextureItem()
    {
        return this.mockBatchingManager.When(m => m.AddTextureItem(Arg.Any<TextureBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()));
    }

    /// <summary>
    /// Creates a new instance of <see cref="TextureRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureRenderer CreateSystemUnderTest()
        => new (this.mockGL,
            this.mockReactableFactory,
            this.mockGLService,
            this.mockGpuBuffer,
            this.mockShader,
            this.mockBatchingManager);
}
