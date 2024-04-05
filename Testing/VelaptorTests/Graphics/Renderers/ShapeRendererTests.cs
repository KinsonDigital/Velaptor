// <copyright file="ShapeRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Drawing;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
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
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;

// Type aliases
using RectRenderItem = Carbonate
    .Core.OneWay.IReceiveSubscription<
        System.Memory<
            Velaptor.OpenGL.Batching.RenderItem<
                Velaptor.OpenGL.Batching.ShapeBatchItem
            >
        >
    >;

/// <summary>
/// Tests the <see cref="ShapeRenderer"/> class.
/// </summary>
public class ShapeRendererTests : TestsBase
{
    private const uint ShapeShaderId = 3333u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IGpuBuffer<ShapeBatchItem>> mockGpuBuffer;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription? batchHasBegunReactor;
    private RectRenderItem? renderReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRendererTests"/> class.
    /// </summary>
    public ShapeRendererTests()
    {
        this.mockGL = new Mock<IGLInvoker>();

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

        this.mockShader = new Mock<IShaderProgram>();
        this.mockShader.SetupGet(p => p.ShaderId).Returns(ShapeShaderId);

        this.mockGpuBuffer = new Mock<IGpuBuffer<ShapeBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();
        this.mockBatchingManager.Name = nameof(this.mockBatchingManager);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor => this.batchHasBegunReactor = reactor);

        var mockShapeRenderBatchReactable = new Mock<IRenderBatchReactable<ShapeBatchItem>>();
        mockShapeRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<RectRenderItem>()))
            .Callback<RectRenderItem>(reactor => this.renderReactor = reactor);

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderShapeReactable())
            .Returns(mockShapeRenderBatchReactable.Object);

        var mockFontTextureAtlas = new Mock<ITexture>();
        mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
        mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);
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
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                null,
                this.mockGpuBuffer.Object,
                this.mockShader.Object,
                this.mockBatchingManager.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ShapeRenderer(
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
            .WithMessage("Value cannot be null. (Parameter 'buffer')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullShaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ShapeRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                this.mockGLService.Object,
                this.mockGpuBuffer.Object,
                null,
                this.mockBatchingManager.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'shader')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ShapeRenderer(
                this.mockGL.Object,
                this.mockReactableFactory.Object,
                this.mockGLService.Object,
                this.mockGpuBuffer.Object,
                this.mockShader.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'batchManager')");
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
        this.mockBatchingManager.VerifyOnce(m => m.AddShapeItem(expected, 123, It.IsAny<DateTime>()));
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
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 6 Shape Elements"));
        this.mockGLService.VerifyExactly(m => m.EndGroup(), 3);
        this.mockGL.VerifyOnce(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGpuBuffer.VerifyOnce(m => m.UploadData(batchItem, batchIndex));
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
        act.Should().Throw<InvalidOperationException>()
            .WithMessage(expected);
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
        this.mockBatchingManager.VerifyOnce(m => m.AddShapeItem(expected, 123, It.IsAny<DateTime>()));
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
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 6 Shape Elements"));
        this.mockGLService.VerifyExactly(m => m.EndGroup(), 3);
        this.mockGL.VerifyOnce(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGpuBuffer.VerifyOnce(m => m.UploadData(batchItem, batchIndex));
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
        act.Should().Throw<InvalidOperationException>()
            .WithMessage(expected);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Render_WithNoRectItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestShapeShader";
        this.mockShader.SetupGet(p => p.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render Shape Process - Nothing To Render"));
        this.mockGLService.VerifyOnce(m => m.EndGroup());
        this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Shape Process With {shaderName} Shader"));
        this.mockShader.VerifyNever(m => m.Use());
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Rectangle Data - TextureID"))));
        this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
        this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
        this.mockGpuBuffer.VerifyNever(m =>
            m.UploadData(It.IsAny<ShapeBatchItem>(), It.IsAny<uint>()));
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
        this.mockGL.VerifyNever(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()));
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void PushReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"ShapeRenderer.ctor() - {PushNotifications.BatchHasBegunId}");
            });
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void ShapeRenderBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockShapeRenderBatchReactable = new Mock<IRenderBatchReactable<ShapeBatchItem>>();
        mockShapeRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<RectRenderItem>()))
            .Callback<RectRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"ShapeRenderer.ctor() - {PushNotifications.RenderShapesId}");
            });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ShapeRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGpuBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);
}
