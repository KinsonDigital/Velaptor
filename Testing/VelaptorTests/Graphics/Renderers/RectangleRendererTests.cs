// <copyright file="RectangleRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Diagnostics.CodeAnalysis;
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

using RectRenderItem = Carbonate.Core.UniDirectional.IReceiveReactor<
    System.Memory<
        Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.RectBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="RectangleRenderer"/> class.
/// </summary>
public class RectangleRendererTests
{
    private const uint RectShaderId = 3333u;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IGPUBuffer<RectBatchItem>> mockGPUBuffer;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IDisposable> mockBatchBegunUnsubscriber;
    private readonly Mock<IDisposable> mockRenderUnsubscriber;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveReactor? batchHasBegunReactor;
    private RectRenderItem? renderReactor;
    private IReceiveReactor? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleRendererTests"/> class.
    /// </summary>
    public RectangleRendererTests()
    {
        this.mockGL = new Mock<IGLInvoker>();

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

        this.mockShader = new Mock<IShaderProgram>();
        this.mockShader.SetupGet(p => p.ShaderId).Returns(RectShaderId);

        this.mockGPUBuffer = new Mock<IGPUBuffer<RectBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();
        this.mockBatchingManager.Name = nameof(this.mockBatchingManager);

        this.mockBatchBegunUnsubscriber = new Mock<IDisposable>();
        this.mockRenderUnsubscriber = new Mock<IDisposable>();
        var mockShutDownUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasBegunId)
                {
                    reactor.Name.Should().Be($"RectangleRendererTests.Ctor - {nameof(PushNotifications.BatchHasBegunId)}");
                    this.batchHasBegunReactor = reactor;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            })
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasBegunId)
                {
                    return this.mockBatchBegunUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            });

        var mockRectRenderBatchReactable = new Mock<IRenderBatchReactable<RectBatchItem>>();
        mockRectRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<RectRenderItem>()))
            .Callback<RectRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"RectangleRendererTests.Ctor - {nameof(PushNotifications.RenderRectsId)}");

                this.renderReactor = reactor;
            })
            .Returns<RectRenderItem>(reactor =>
            {
                if (reactor.Id == PushNotifications.RenderRectsId)
                {
                    return this.mockRenderUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderRectReactable())
            .Returns(mockRectRenderBatchReactable.Object);

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
            _ = new RectangleRenderer(
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
            _ = new RectangleRenderer(
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
            _ = new RectangleRenderer(
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
            _ = new RectangleRenderer(
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
    public void Render_WithNoRectItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestRectShader";
        this.mockShader.SetupGet(p => p.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render Rectangle Process - Nothing To Render"));
        this.mockGLService.VerifyOnce(m => m.EndGroup());
        this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Rectangle Process With {shaderName} Shader"));
        this.mockShader.VerifyNever(m => m.Use());
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Rectangle Data - TextureID"))));
        this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
        this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
        this.mockGPUBuffer.VerifyNever(m =>
            m.UploadData(It.IsAny<RectBatchItem>(), It.IsAny<uint>()));
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))));
        this.mockGL.VerifyNever(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()));
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Used for testing")]
    public void Render_WhenInvoked_AddsRectToBatch()
    {
        // Arrange
        var rect = new RectShape
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

        var expected = new RectBatchItem(
            new Vector2(11, 22),
            33u,
            44u,
            Color.White,
            true,
            0,
            CornerRadius.Empty(),
            ColorGradient.None,
            Color.Magenta,
            Color.Magenta);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(rect, 123);

        // Assert
        this.mockBatchingManager.VerifyOnce(m => m.AddRectItem(expected, 123, It.IsAny<DateTime>()));
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Used for testing")]
    public void Render_WhenInvoked_RendersRectangle()
    {
        // Arrange
        const uint batchIndex = 0;

        var rect = default(RectShape);
        rect.Position = new Vector2(1, 2);
        rect.Width = 3;
        rect.Height = 4;
        rect.Color = Color.FromArgb(99, 100, 110, 120);
        rect.IsFilled = true;
        rect.BorderThickness = 5;
        rect.CornerRadius = new CornerRadius(6f, 7f, 8f, 9f);
        rect.GradientStart = Color.FromArgb(11, 22, 33, 44);
        rect.GradientStop = Color.FromArgb(55, 66, 77, 88);
        rect.GradientType = ColorGradient.Horizontal;

        var batchItem = new RectBatchItem(
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

        var renderItem = new RenderItem<RectBatchItem> { Layer = 0, Item = batchItem };

        var renderItems = new Memory<RenderItem<RectBatchItem>>(new[] { renderItem });

        _ = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        this.renderReactor.OnReceive(renderItems);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 6 Rectangle Elements"));
        this.mockGLService.VerifyExactly(m => m.EndGroup(), 3);
        this.mockGL.VerifyOnce(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(batchItem, batchIndex));
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Used for testing")]
    public void Render_WhenBegunHasNotBeenInvoked_ThrowsException()
    {
        // Arrange
        const string expected = "The 'Begin()' method must be invoked first before any 'Render()' methods.";
        var rect = default(RectShape);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(rect);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage(expected);
    }
    #endregion

    #region Reactable Tests
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
        this.mockRenderUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="RectangleRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RectangleRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGPUBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);
}
