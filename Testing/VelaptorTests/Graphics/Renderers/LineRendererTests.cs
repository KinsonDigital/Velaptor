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

using LineRenderItem = Carbonate.Core.UniDirectional.IReceiveReactor<
    System.Memory<
        Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.LineBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="LineRenderer"/> class.
/// </summary>
public class LineRendererTests
{
    private const string Category = nameof(Category);
    private const uint LineShaderId = 3333u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IGPUBuffer<LineBatchItem>> mockGPUBuffer;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchBegunUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private IReceiveReactor? shutDownReactor;
    private LineRenderItem? renderReactor;
    private IReceiveReactor? batchHasBegunReactor;

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

        this.mockGPUBuffer = new Mock<IGPUBuffer<LineBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();

        this.mockBatchBegunUnsubscriber = new Mock<IDisposable>();
        var mockRenderUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
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
            })
            .Returns<IReceiveReactor>(reactor =>
            {
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
            });

        var mockLineRenderBatchReactable = new Mock<IRenderBatchReactable<LineBatchItem>>();
        mockLineRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<LineRenderItem>()))
            .Callback<LineRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.RenderLinesId)
                {
                    reactor.Name.Should().Be($"LineRendererTests.Ctor - {nameof(PushNotifications.RenderLinesId)}");

                    this.renderReactor = reactor;
                }
            })
            .Returns<LineRenderItem>(reactor =>
            {
                if (reactor.Id == PushNotifications.RenderLinesId)
                {
                    return mockRenderUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            });

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
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineRenderer(
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
            .WithMessage("The parameter must not be null. (Parameter 'buffer')");
    }

    [Fact]
    public void Ctor_WithNullShaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineRenderer(
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
            _ = new LineRenderer(
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
        this.mockGPUBuffer.VerifyNever(m =>
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
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(batchItem, batchIndex));
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait(Category, "Reactable Tests")]
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
    /// Creates a new instance of <see cref="LineRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGPUBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);
}
