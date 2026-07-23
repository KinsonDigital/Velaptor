// <copyright file="LineRendererTests.cs" company="KinsonDigital">
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
using Velaptor.WebGPU.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;
using LineRenderItem = Carbonate.Core.OneWay.IReceiveSubscription<System.Memory<Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.WebGPU.Batching.LineBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="LineRenderer"/> class.
/// </summary>
public class LineRendererTests : TestsBase
{
    private const uint LineShaderId = 3333u;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IShaderProgram mockShader;
    private readonly IGpuBuffer<LineBatchItem> mockGpuBuffer;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private LineRenderItem? renderReactor;
    private IReceiveSubscription? batchHasBegunReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineRendererTests"/> class.
    /// </summary>
    public LineRendererTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();

        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockGLService.ProgramLinkedSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.ShaderCompiledSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.GetViewPortSize().Returns(new Size(800, 600));

        this.mockShader = Substitute.For<IShaderProgram>();
        this.mockShader.ShaderId.Returns(LineShaderId);

        this.mockGpuBuffer = Substitute.For<IGpuBuffer<LineBatchItem>>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable
            .Subscribe(Arg.Any<IReceiveSubscription>())
            .Returns(Substitute.For<IDisposable>())
            .AndDoes(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                this.batchHasBegunReactor = reactor;
            });

        var mockLineRenderBatchReactable = Substitute.For<IRenderBatchReactable<LineBatchItem>>();
        mockLineRenderBatchReactable
            .Subscribe(Arg.Any<LineRenderItem>())
            .Returns(Substitute.For<IDisposable>())
            .AndDoes(ci =>
            {
                var reactor = ci.Arg<LineRenderItem>();
                this.renderReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory
            .CreateNoDataPushReactable()
            .Returns(mockPushReactable);
        this.mockReactableFactory
            .CreateRenderLineReactable()
            .Returns(mockLineRenderBatchReactable);

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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
    public void Render_WhenBegunHasNotBeenInvoked_ThrowsException()
    {
        // Arrange
        const string expected = "The 'Begin()' method must be invoked first before any 'Render()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(default(Line));

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(expected);
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
        this.mockBatchingManager.Received(1).AddLineItem(expected, 10, Arg.Any<DateTime>());
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
        this.mockBatchingManager.Received(1).AddLineItem(expected, 10, Arg.Any<DateTime>());
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
        this.mockBatchingManager.Received(1).AddLineItem(expected, 10, Arg.Any<DateTime>());
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
        this.mockBatchingManager.Received(1).AddLineItem(expected, 10, Arg.Any<DateTime>());
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
        this.mockBatchingManager.Received(1).AddLineItem(expected, 10, Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithNoLineItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestLineShader";
        this.mockShader.Name.Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render Line Process - Nothing To Render");
        this.mockGLService.Received(1).EndGroup();
        this.mockGLService.DidNotReceive().BeginGroup($"Render Line Process With {shaderName} Shader");
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Update Line Data - TextureID")));
        this.mockGLService.DidNotReceive().BindTexture2D(Arg.Any<uint>());
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements")));
        this.mockGL.DidNotReceive().ActiveTexture(Arg.Any<GLTextureUnit>());
        this.mockGL.DidNotReceive().DrawElements(
            Arg.Any<GLPrimitiveType>(),
            Arg.Any<uint>(),
            Arg.Any<GLDrawElementsType>(),
            Arg.Any<nint>());
        this.mockShader.DidNotReceive().Use();
        this.mockGpuBuffer
            .DidNotReceive()
            .UploadData(Arg.Any<LineBatchItem>(), Arg.Any<uint>());
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
        this.mockGLService.Received(1).BeginGroup("Render 6 Line Elements");
        this.mockGLService.Received(3).EndGroup();
        this.mockGL
            .Received(1)
            .DrawElements(
                GLPrimitiveType.Triangles,
                6,
                GLDrawElementsType.UnsignedInt,
                nint.Zero);
        this.mockGpuBuffer.Received(1).UploadData(batchItem, batchIndex);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
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
            });
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void LineRenderReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockLineRenderBatchReactable = Substitute.For<IRenderBatchReactable<LineBatchItem>>();
        mockLineRenderBatchReactable
            .When(m => m.Subscribe(Arg.Any<LineRenderItem>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<LineRenderItem>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"LineRenderer.ctor() - {PushNotifications.RenderLinesId}");
            });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineRenderer CreateSystemUnderTest()
        => new (this.mockGL,
            this.mockReactableFactory,
            this.mockGLService,
            this.mockGpuBuffer,
            this.mockShader,
            this.mockBatchingManager);
}
