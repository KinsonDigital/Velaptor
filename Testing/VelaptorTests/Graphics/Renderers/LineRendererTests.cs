// <copyright file="LineRendererTests.cs" company="KinsonDigital">
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
using LineRenderItem = Carbonate.Core.OneWay.IReceiveSubscription<System.Memory<Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.LineBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="LineRenderer"/> class.
/// </summary>
public class LineRendererTests : TestsBase
{
    private const uint LineShaderId = 3333u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IGpuBuffer<LineBatchItem>> mockGpuBuffer;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private LineRenderItem? renderReactor;
    private IReceiveSubscription? batchHasBegunReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineRendererTests"/> class.
    /// </summary>
    public LineRendererTests()
    {
        this.mockGL = new Mock<IGLInvoker>();

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

        this.mockShader = new Mock<IShaderProgram>();
        this.mockShader.Setup(m => m.ShaderId).Returns(LineShaderId);

        this.mockGpuBuffer = new Mock<IGpuBuffer<LineBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();

        var mockRenderUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor => this.batchHasBegunReactor = reactor)
            .Returns<IReceiveSubscription>(_ => Substitute.For<IDisposable>());

        var mockLineRenderBatchReactable = new Mock<IRenderBatchReactable<LineBatchItem>>();
        mockLineRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<LineRenderItem>()))
            .Callback<LineRenderItem>(reactor => this.renderReactor = reactor)
            .Returns<LineRenderItem>(_ => mockRenderUnsubscriber.Object);

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderLineReactable())
            .Returns(mockLineRenderBatchReactable.Object);

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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
    public void Render_WhenBegunHasNotBeenInvoked_ThrowsException()
    {
        // Arrange
        const string expected = "The 'Begin()' method must be invoked first before any 'Render()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(default(Line));

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoking2ParamMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9);

        var line = new Line(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9);
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(line, 10);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddLineItem(expected, 10, It.IsAny<DateTime>()));
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoking3ParamMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.White,
            1u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(new Vector2(1, 2), new Vector2(3, 4), 10);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddLineItem(expected, 10, It.IsAny<DateTime>()));
     }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoking4ParamWithColorMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            1u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            10);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddLineItem(expected, 10, It.IsAny<DateTime>()));
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoking4ParamWithThicknessMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.White,
            11u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(
            new Vector2(1, 2),
            new Vector2(3, 4),
            11u,
            10);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddLineItem(expected, 10, It.IsAny<DateTime>()));
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvokingOverloadWithAllParams_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9u,
            10);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddLineItem(expected, 10, It.IsAny<DateTime>()));
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithNoLineItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestLineShader";
        this.mockShader.SetupGet(p => p.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render Line Process - Nothing To Render"));
        this.mockGLService.VerifyOnce(m => m.EndGroup());
        this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Line Process With {shaderName} Shader"));
        this.mockShader.VerifyNever(m => m.Use());
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Line Data - TextureID"))));
        this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
        this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
        this.mockGpuBuffer.VerifyNever(m =>
            m.UploadData(It.IsAny<LineBatchItem>(), It.IsAny<uint>()));
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
        this.mockGL.VerifyNever(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()));
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoked_RendersLine()
    {
        // Arrange
        const uint batchIndex = 0;

        var line = new Line(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            5);

        var batchItem = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9);

        var renderItem = new RenderItem<LineBatchItem> { Layer = 0, Item = batchItem };

        var renderItems = new Memory<RenderItem<LineBatchItem>>(new[] { renderItem });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();
        sut.Render(line);

        // Act
        this.renderReactor.OnReceive(renderItems);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 6 Line Elements"));
        this.mockGLService.VerifyExactly(m => m.EndGroup(), 3);
        this.mockGL.VerifyOnce(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGpuBuffer.VerifyOnce(m => m.UploadData(batchItem, batchIndex));
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void PushReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");
            });
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void LineRenderReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockLineRenderBatchReactable = new Mock<IRenderBatchReactable<LineBatchItem>>();
        mockLineRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<LineRenderItem>()))
            .Callback<LineRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");
                reactor.Name.Should().Be($"LineRenderer.ctor() - {PushNotifications.RenderLinesId}");
            });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGpuBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);
}
