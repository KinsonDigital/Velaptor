// <copyright file="TextureRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Collections.Generic;
using System.Drawing;
using Carbonate;
using Carbonate.Core;
using FluentAssertions;
using Helpers;
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
    private readonly Mock<IBatchingService<TextureBatchItem>> mockBatchingService;
    private readonly Mock<IPushReactable> mockReactable;
    private readonly Mock<IDisposable> mockBatchBegunUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private IReceiveReactor? batchHasBegunReactor;
    private IReceiveReactor? renderReactor;
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

        this.mockBatchingService = new Mock<IBatchingService<TextureBatchItem>>();
        this.mockBatchingService.Name = nameof(this.mockBatchingService);
        this.mockBatchingService.SetupGet(p => p.BatchItems)
            .Returns(Array.Empty<TextureBatchItem>().AsReadOnly());

        this.mockBatchBegunUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        var mockRenderUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IPushReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                if (reactor.Id == NotificationIds.RenderTexturesId)
                {
                    return mockRenderUnsubscriber.Object;
                }

                if (reactor.Id == NotificationIds.RenderBatchBegunId)
                {
                    return this.mockBatchBegunUnsubscriber.Object;
                }

                if (reactor.Id == NotificationIds.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
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

                if (reactor.Id == NotificationIds.RenderTexturesId)
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
        this.renderReactor.OnReceive();

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup("Render Texture Process - Nothing To Render"), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Once);
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
        this.mockBatchingService.VerifyNever(m => m.EmptyBatch());
    }

    [Fact]
    public void Render_WhenInvoking3ParamOverload_AddsCorrectItemToBatch()
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

        this.mockBatchingService.Setup(m => m.Add(It.Ref<TextureBatchItem>.IsAny))
            .Callback((in TextureBatchItem item) =>
            {
                actualBatchItem = item;
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(It.Ref<TextureBatchItem>.IsAny), Times.Once);
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoking4ParamOverloadWithEffects_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        const RenderEffects expectedRenderEffects = RenderEffects.FlipHorizontally;
        var expectedBatchItem =
            CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, expectedRenderEffects, Color.White, textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingService.Setup(m => m.Add(It.Ref<TextureBatchItem>.IsAny))
            .Callback((in TextureBatchItem item) =>
            {
                actualBatchItem = item;
            });
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedRenderEffects);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(It.Ref<TextureBatchItem>.IsAny), Times.Once);
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoking4ParamOverloadWithColor_AddsCorrectItemToBatch()
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

        this.mockBatchingService.Setup(m => m.Add(It.Ref<TextureBatchItem>.IsAny))
            .Callback((in TextureBatchItem item) =>
            {
                actualBatchItem = item;
            });
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedClr);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(It.Ref<TextureBatchItem>.IsAny), Times.Once);
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoking5ParamOverload_AddsCorrectItemToBatch()
    {
        // Arrange
        const int textureId = 1234;
        const int expectedX = 10;
        const int expectedY = 20;
        const int expectedWidth = 111;
        const int expectedHeight = 222;
        var expectedClr = Color.FromArgb(11, 22, 33, 44);
        const RenderEffects expectedRenderEffects = RenderEffects.FlipVertically;
        var expectedBatchItem =
            CreateBatchItem(expectedX, expectedY, expectedWidth, expectedHeight, expectedRenderEffects, expectedClr, textureId);

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(textureId);
        mockTexture.SetupGet(p => p.Width).Returns(expectedWidth);
        mockTexture.SetupGet(p => p.Height).Returns(expectedHeight);

        TextureBatchItem actualBatchItem = default;

        this.mockBatchingService.Setup(m => m.Add(It.Ref<TextureBatchItem>.IsAny))
            .Callback((in TextureBatchItem item) =>
            {
                actualBatchItem = item;
            });
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(mockTexture.Object, 10, 20, expectedClr, expectedRenderEffects);

        // Assert
        this.mockBatchingService.Verify(m => m.Add(It.Ref<TextureBatchItem>.IsAny), Times.Once);
        AssertExtensions.EqualWithMessage(expectedBatchItem, actualBatchItem, "The texture batch item being added is incorrect.");
    }

    [Fact]
    public void Render_WhenInvoked_RendersTexture()
    {
        // Arrange
        const uint itemABatchIndex = 0;
        const uint itemBBatchIndex = 1;
        const uint expectedTotalElements = 12;

        var itemA = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            1,
            45,
            Color.Empty,
            RenderEffects.None,
            TextureId,
            0);

        var itemB = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            90,
            Color.Empty,
            RenderEffects.None,
            TextureId,
            1);

        var shouldNotRenderItem = default(TextureBatchItem);
        var items = new[] { itemA, itemB, shouldNotRenderItem };
        MockTextureBatchItems(items);

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
        this.renderReactor.OnReceive();

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render 12 Texture Elements"));
        this.mockGL.VerifyOnce(m
            => m.DrawElements(GLPrimitiveType.Triangles, expectedTotalElements, GLDrawElementsType.UnsignedInt, nint.Zero));
        this.mockGLService.VerifyOnce(m => m.BindTexture2D(TextureId));
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(itemA, itemABatchIndex));
        this.mockGPUBuffer.VerifyOnce(m => m.UploadData(itemB, itemBBatchIndex));
        this.mockGPUBuffer.VerifyNever(m => m.UploadData(shouldNotRenderItem, It.IsAny<uint>()));
        this.mockBatchingService.VerifyOnce(m => m.EmptyBatch());
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
        this.mockBatchBegunUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }
    #endregion

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
        var result = new TextureBatchItem(
            new RectangleF(0f, 0f, width, height),
            new RectangleF(x, y, width, height),
            1f,
            0f,
            clr,
            effects,
            (uint)textureId,
            0);

        return result;
    }

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
    /// Mocks the <see cref="IBatchingService{T}.BatchItems"/> property of the <see cref="IBatchingService{T}"/>.
    /// </summary>
    /// <param name="items">The items to store in the service.</param>
    private void MockTextureBatchItems(IList<TextureBatchItem> items)
    {
        this.mockBatchingService.SetupProperty(p => p.BatchItems);
        this.mockBatchingService.Object.BatchItems = items.AsReadOnly();
    }

    /// <summary>
    /// Creates a new instance of <see cref="TextureRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactable.Object,
            this.mockGLService.Object,
            this.mockGPUBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingService.Object);
}
