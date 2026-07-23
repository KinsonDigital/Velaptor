// <copyright file="ShapeRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Drawing;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Shouldly;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;
using Velaptor.WebGPU.Batching;

// Type aliases
using RectRenderItem = Carbonate
    .Core.OneWay.IReceiveSubscription<
        System.Memory<
            Velaptor.WebGPU.Batching.RenderItem<
                Velaptor.WebGPU.Batching.ShapeBatchItem
            >
        >
    >;

/// <summary>
/// Tests the <see cref="ShapeRenderer"/> class.
/// </summary>
public class ShapeRendererTests : TestsBase
{
    private const uint ShapeShaderId = 3333u;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IShaderProgram mockShader;
    private readonly IGpuBuffer<ShapeBatchItem> mockGpuBuffer;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription? batchHasBegunReactor;
    private RectRenderItem? renderReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRendererTests"/> class.
    /// </summary>
    public ShapeRendererTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();

        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockGLService.ProgramLinkedSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.ShaderCompiledSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.GetViewPortSize().Returns(new Size(800, 600));

        this.mockShader = Substitute.For<IShaderProgram>();
        this.mockShader.ShaderId.Returns(ShapeShaderId);

        this.mockGpuBuffer = Substitute.For<IGpuBuffer<ShapeBatchItem>>();

        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable
            .When(m => m.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                this.batchHasBegunReactor = reactor;
            });

        var mockShapeRenderBatchReactable = Substitute.For<IRenderBatchReactable<ShapeBatchItem>>();
        mockShapeRenderBatchReactable
            .When(m => m.Subscribe(Arg.Any<RectRenderItem>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<RectRenderItem>();
                this.renderReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory
            .CreateNoDataPushReactable()
            .Returns(mockPushReactable);
        this.mockReactableFactory
            .CreateRenderShapeReactable()
            .Returns(mockShapeRenderBatchReactable);

        var mockFontTextureAtlas = Substitute.For<ITexture>();
        mockFontTextureAtlas.Width.Returns(200u);
        mockFontTextureAtlas.Height.Returns(100u);
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ShapeRenderer(
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
            _ = new ShapeRenderer(
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
            _ = new ShapeRenderer(
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
            _ = new ShapeRenderer(
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
    public void Render_WhenRenderingShape_AddsShapeToBatch()
    {
        // Arrange
        var rectShape = new RectShape
        {
            Position = new Vector2(11, 22),
            Width = 33u,
            Height = 44u,
            IsSolid = true,
            BorderThickness = 20,
            CornerRadius = CornerRadius.Empty(),
            Color = Color.White,
            GradientType = ColorGradient.None,
            GradientStart = Color.Magenta,
            GradientStop = Color.Magenta,
        };

        var expected = new ShapeBatchItem(
            new Vector2(11, 22),
            33u,
            44u,
            Color.White,
            true,
            20,
            CornerRadius.Empty(),
            ColorGradient.None,
            Color.Magenta,
            Color.Magenta);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(rectShape, 123);

        // Assert
        this.mockBatchingManager.Received(1).AddShapeItem(expected, 123, Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenRenderingRect_RendersRectangle()
    {
        // Arrange
        const uint batchIndex = 0;

        var rect = default(RectShape);
        rect.Position = new Vector2(1, 2);
        rect.Width = 3;
        rect.Height = 4;
        rect.Color = Color.FromArgb(99, 100, 110, 120);
        rect.IsSolid = true;
        rect.BorderThickness = 5;
        rect.CornerRadius = new CornerRadius(6f, 7f, 8f, 9f);
        rect.GradientStart = Color.FromArgb(11, 22, 33, 44);
        rect.GradientStop = Color.FromArgb(55, 66, 77, 88);
        rect.GradientType = ColorGradient.Horizontal;

        var batchItem = new ShapeBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(99, 100, 110, 120),
            true,
            5,
            new CornerRadius(6f, 7f, 8f, 9f),
            ColorGradient.Horizontal,
            Color.FromArgb(11, 22, 33, 44),
            Color.FromArgb(55, 66, 77, 88));

        var renderItem = new RenderItem<ShapeBatchItem> { Layer = 0, Item = batchItem };

        var renderItems = new Memory<RenderItem<ShapeBatchItem>>(new[] { renderItem });

        _ = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        this.renderReactor.OnReceive(renderItems);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render 6 Shape Elements");
        this.mockGLService.Received(3).EndGroup();

        this.mockGL
            .Received(1)
            .DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.mockGpuBuffer.Received(1).UploadData(batchItem, batchIndex);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenRenderingRectAndBegunHasNotBeenInvoked_ThrowsException()
    {
        // Arrange
        const string expected = "The 'Begin()' method must be invoked first before any 'Render()' methods.";
        var rectShape = default(RectShape);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(rectShape);

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(expected);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenRenderingCircle_AddsCircleToBatch()
    {
        // Arrange
        var circle = new CircleShape
        {
            Position = new Vector2(11, 22),
            Diameter = 33u,
            IsSolid = true,
            BorderThickness = 10,
            Color = Color.White,
            GradientType = ColorGradient.None,
            GradientStart = Color.Magenta,
            GradientStop = Color.Magenta,
        };

        var expected = new ShapeBatchItem(
            new Vector2(11, 22),
            33u,
            33u,
            Color.White,
            true,
            10,
            new CornerRadius(16.5f),
            ColorGradient.None,
            Color.Magenta,
            Color.Magenta);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(circle, 123);

        // Assert
        this.mockBatchingManager.Received(1).AddShapeItem(expected, 123, Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenRenderingCircle_RendersCircle()
    {
        // Arrange
        const uint batchIndex = 0;

        var circle = default(CircleShape);
        circle.Position = new Vector2(1, 2);
        circle.Diameter = 3;
        circle.Color = Color.FromArgb(99, 100, 110, 120);
        circle.IsSolid = true;
        circle.BorderThickness = 5;
        circle.GradientStart = Color.FromArgb(11, 22, 33, 44);
        circle.GradientStop = Color.FromArgb(55, 66, 77, 88);
        circle.GradientType = ColorGradient.Horizontal;

        var batchItem = new ShapeBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(99, 100, 110, 120),
            true,
            5,
            new CornerRadius(4f),
            ColorGradient.Horizontal,
            Color.FromArgb(11, 22, 33, 44),
            Color.FromArgb(55, 66, 77, 88));

        var renderItem = new RenderItem<ShapeBatchItem> { Layer = 0, Item = batchItem };

        var renderItems = new Memory<RenderItem<ShapeBatchItem>>(new[] { renderItem });

        _ = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        this.renderReactor.OnReceive(renderItems);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render 6 Shape Elements");
        this.mockGLService.Received(3).EndGroup();
        this.mockGL
            .Received(1)
            .DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.mockGpuBuffer.Received(1).UploadData(batchItem, batchIndex);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenRenderingCircleAndBegunHasNotBeenInvoked_ThrowsException()
    {
        // Arrange
        const string expected = "The 'Begin()' method must be invoked first before any 'Render()' methods.";
        var circle = default(CircleShape);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(circle);

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(expected);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Render_WithNoRectItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestShapeShader";
        this.mockShader.Name.Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render Shape Process - Nothing To Render");
        this.mockGLService.Received(1).EndGroup();
        this.mockGLService.DidNotReceive().BeginGroup($"Render Shape Process With {shaderName} Shader");
        this.mockShader.DidNotReceive().Use();
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Update Rectangle Data - TextureID")));
        this.mockGL.DidNotReceive().ActiveTexture(Arg.Any<GLTextureUnit>());
        this.mockGLService.DidNotReceive().BindTexture2D(Arg.Any<uint>());
        this.mockGpuBuffer
            .DidNotReceive()
            .UploadData(Arg.Any<ShapeBatchItem>(), Arg.Any<uint>());
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
    [Trait("Category", Ctor)]
    public void PushReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable
            .When(m => m.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"ShapeRenderer.ctor() - {PushNotifications.BatchHasBegunId}");
            });
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void ShapeRenderBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockShapeRenderBatchReactable = Substitute.For<IRenderBatchReactable<ShapeBatchItem>>();
        mockShapeRenderBatchReactable
            .When(m => m.Subscribe(Arg.Any<RectRenderItem>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<RectRenderItem>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"ShapeRenderer.ctor() - {PushNotifications.RenderShapesId}");
            });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ShapeRenderer CreateSystemUnderTest()
        => new (this.mockGL,
            this.mockReactableFactory,
            this.mockGLService,
            this.mockGpuBuffer,
            this.mockShader,
            this.mockBatchingManager);
}
