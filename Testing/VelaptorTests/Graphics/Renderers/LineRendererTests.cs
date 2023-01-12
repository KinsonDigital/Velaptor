// <copyright file="LineRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Carbonate;
using Carbonate.Core;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Velaptor.Services;
using Xunit;

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
    private readonly Mock<IBatchingService<LineBatchItem>> mockBatchingService;
    private readonly Mock<IPushReactable> mockReactable;
    private readonly Mock<IDisposable> mockBatchBegunUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private IReceiveReactor? shutDownReactor;
    private IReceiveReactor? batchHasBegunReactor;
    private IReceiveReactor? renderReactor;

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

        this.mockBatchingService = new Mock<IBatchingService<LineBatchItem>>();
        this.mockBatchingService.SetupGet(p => p.BatchItems)
            .Returns(Array.Empty<LineBatchItem>().AsReadOnly());

        this.mockBatchBegunUnsubscriber = new Mock<IDisposable>();
        var mockRenderUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IPushReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                if (reactor.Id == NotificationIds.RenderBatchBegunId)
                {
                    return this.mockBatchBegunUnsubscriber.Object;
                }

                if (reactor.Id == NotificationIds.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                if (reactor.Id == NotificationIds.RenderLinesId)
                {
                    return mockRenderUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            })
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == NotificationIds.RenderBatchBegunId)
                {
                    this.batchHasBegunReactor = reactor;
                }

                if (reactor.Id == NotificationIds.RenderLinesId)
                {
                    this.renderReactor = reactor;
                }

                if (reactor.Id == NotificationIds.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
            });

        var mockFontTextureAtlas = new Mock<ITexture>();
        mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
        mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);
    }

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
    public void RenderLine_WhenInvoking2ParamMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9,
            10);

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
        this.mockBatchingService.Verify(m => m.Add(expected), Times.Once);
    }

    [Fact]
    public void RenderLine_WhenInvoking3ParamMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.White,
            1u,
            10);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(new Vector2(1, 2), new Vector2(3, 4), 10);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(expected), Times.Once);
     }

    [Fact]
    public void RenderLine_WhenInvoking4ParamWithColorMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            1u,
            10);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            10);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(expected), Times.Once);
    }

    [Fact]
    public void RenderLine_WhenInvoking4ParamWithThicknessMethodOverload_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.White,
            11u,
            10);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.RenderLine(
            new Vector2(1, 2),
            new Vector2(3, 4),
            11u,
            10);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(expected), Times.Once);
    }

    [Fact]
    public void RenderLine_WhenInvokingOverloadWithAllParams_AddsToBatch()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9u,
            10);

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
        this.mockBatchingService.Verify(m => m.Add(expected), Times.Once);
    }

    [Fact]
    public void RenderLine_WithNoLineItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestLineShader";
        this.mockShader.SetupGet(p => p.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive();

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Render Line Process - Nothing To Render"), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup($"Render Line Process With {shaderName} Shader"), Times.Never);
        this.mockShader.Verify(m => m.Use(), Times.Never);
        this.mockGLService.Verify(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Line Data - TextureID"))), Times.Never);
        this.mockGL.Verify(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()), Times.Never);
        this.mockGLService.Verify(m => m.BindTexture2D(It.IsAny<uint>()), Times.Never);
        this.mockGPUBuffer.Verify(m =>
            m.UploadData(It.IsAny<LineBatchItem>(), It.IsAny<uint>()), Times.Never);
        this.mockGLService.Verify(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Texture Elements"))), Times.Never);
        this.mockGL.Verify(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()), Times.Never);
        this.mockBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
    }

    [Fact]
    public void RenderLine_WhenInvoked_RendersLine()
    {
        // Arrange
        const uint batchIndex = 0;

        var rect = new Line(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            5);

        var batchItem = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9,
            10);

        var shouldNotRenderEmptyItem = default(LineBatchItem);

        var items = new[] { batchItem, shouldNotRenderEmptyItem };
        MockLineBatchItems(items);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();
        sut.Render(rect);

        // Act
        this.renderReactor.OnReceive();

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Render 6 Line Elements"), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(3));
        this.mockGL.Verify(m => m.DrawElements(GLPrimitiveType.Triangles, 6, GLDrawElementsType.UnsignedInt, nint.Zero), Times.Once());
        this.mockGPUBuffer.Verify(m => m.UploadData(batchItem, batchIndex), Times.Once);
        this.mockGPUBuffer.Verify(m => m.UploadData(shouldNotRenderEmptyItem, batchIndex), Times.Never);
        this.mockBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    [Trait(Category, "Indirect Tests")]
    public void Reactable_WithShutDownNotification_ShutsDownRenderer()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnReceive();
        this.shutDownReactor.OnReceive();

        // Assert
        this.mockBatchBegunUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }
    #endregion

    /// <summary>
    /// Mocks the <see cref="IBatchingService{T}.BatchItems"/> property of the <see cref="IBatchingService{T}"/>.
    /// </summary>
    /// <param name="items">The items to store in the service.</param>
    private void MockLineBatchItems(IList<LineBatchItem> items)
    {
        this.mockBatchingService.SetupProperty(p => p.BatchItems);
        this.mockBatchingService.Object.BatchItems = items.AsReadOnly();
    }

    /// <summary>
    /// Creates a new instance of <see cref="LineRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactable.Object,
            this.mockGLService.Object,
            this.mockGPUBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingService.Object);
}
