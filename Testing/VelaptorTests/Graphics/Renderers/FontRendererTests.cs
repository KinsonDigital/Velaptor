// <copyright file="FontRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics.Renderers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Xunit;
using FontRenderItem = Carbonate.Core.OneWay.IReceiveSubscription<System.Memory<Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.FontGlyphBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="FontRenderer"/> class.
/// </summary>
public class FontRendererTests
{
    private const string GlyphTestDataFileName = "glyph-test-data.json";
    private const string BatchTestDataDirPath = "BatchItemTestData";
    private const uint AtlasTextureId = 1234u;
    private const uint FontShaderId = 2222u;
    private const char InvalidCharacter = '□';
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IGpuBuffer<FontGlyphBatchItem>> mockGpuBuffer;
    private readonly Mock<IShaderProgram> mockShader;
    private readonly Mock<IFont> mockFont;
    private readonly Mock<IBatchingManager> mockBatchingManager;
    private readonly Mock<IReactableFactory> mockReactableFactory;

    private readonly char[] glyphChars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '~', '_', '+',
        '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
    };
    private IReceiveSubscription? batchHasBegunReactor;
    private FontRenderItem? renderReactor;

    private List<GlyphMetrics> allGlyphMetrics = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="FontRendererTests"/> class.
    /// </summary>
    public FontRendererTests()
    {
        this.mockGL = new Mock<IGLInvoker>();

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockGLService.Setup(m => m.ProgramLinkedSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.ShaderCompiledSuccessfully(It.IsAny<uint>())).Returns(true);
        this.mockGLService.Setup(m => m.GetViewPortSize()).Returns(new Size(800, 600));

        this.mockShader = new Mock<IShaderProgram>();
        this.mockShader.SetupGet(p => p.ShaderId).Returns(FontShaderId);

        this.mockGpuBuffer = new Mock<IGpuBuffer<FontGlyphBatchItem>>();

        this.mockBatchingManager = new Mock<IBatchingManager>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasBegunId)
                {
                    reactor.Name.Should().Be($"FontRendererTests.Ctor - {nameof(PushNotifications.BatchHasBegunId)}");
                    this.batchHasBegunReactor = reactor;
                }
            });

        var mockFontRenderBatchReactable = new Mock<IRenderBatchReactable<FontGlyphBatchItem>>();
        mockFontRenderBatchReactable
            .Setup(m => m.Subscribe(It.IsAny<FontRenderItem>()))
            .Callback<FontRenderItem>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"FontRendererTests.Ctor - {nameof(PushNotifications.RenderFontsId)}");

                this.renderReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderFontReactable())
            .Returns(mockFontRenderBatchReactable.Object);

        var mockFontTextureAtlas = new Mock<ITexture>();
        mockFontTextureAtlas.SetupGet(p => p.Width).Returns(200);
        mockFontTextureAtlas.SetupGet(p => p.Height).Returns(100);

        this.mockFont = new Mock<IFont>();
        this.mockFont.SetupGet(p => p.Atlas).Returns(mockFontTextureAtlas.Object);
        this.mockFont.SetupGet(p => p.Size).Returns(12u);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontRenderer(
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
            .WithMessage("The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontRenderer(
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
            _ = new FontRenderer(
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
            .WithMessage("The parameter must not be null. (Parameter 'shader')");
    }

    [Fact]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontRenderer(
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
            .WithMessage("The parameter must not be null. (Parameter 'batchManager')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Render_WithNullFont_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test", 10, 20, 1f, 0f, Color.White);

        // Asset
        act.Should().Throw<ArgumentNullException>()
            .WithMessage($"Cannot render a null '{nameof(IFont)}'. (Parameter 'font')");
    }

    [Fact]
    public void Render_WithNoFontItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestFontShader";
        this.mockShader.Setup(m => m.Name).Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.VerifyOnce(m => m.BeginGroup("Render Text Process - Nothing To Render"));
        this.mockGLService.VerifyOnce(m => m.EndGroup());
        this.mockGLService.VerifyNever(m => m.BeginGroup($"Render Text Process With {shaderName} Shader"));
        this.mockShader.VerifyNever(m => m.Use());
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Update Character Data - TextureID"))));
        this.mockGL.VerifyNever(m => m.ActiveTexture(It.IsAny<GLTextureUnit>()));
        this.mockGLService.VerifyNever(m => m.BindTexture2D(It.IsAny<uint>()));
        this.mockGpuBuffer.VerifyNever(m =>
            m.UploadData(It.IsAny<FontGlyphBatchItem>(), It.IsAny<uint>()));
        this.mockGLService.VerifyNever(m =>
            m.BeginGroup(It.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Font Elements"))));
        this.mockGL.VerifyNever(m => m.DrawElements(
            It.IsAny<GLPrimitiveType>(),
            It.IsAny<uint>(),
            It.IsAny<GLDrawElementsType>(),
            It.IsAny<nint>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Render_WithNullOrEmptyText_DoesNotRenderText(string renderText)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            new Mock<IFont>().Object,
            renderText,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>());

        // Assert
        this.mockFont.VerifyNever(m => m.Measure(It.IsAny<string>()));
        this.mockFont.VerifyNever(m => m.ToGlyphMetrics(It.IsAny<string>()));
        this.mockBatchingManager
            .VerifyNever(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()));
    }

    [Fact]
    public void Render_WithFontSizeSetToZero_DoesNotRenderText()
    {
        // Arrange
        this.mockFont.SetupGet(p => p.Size).Returns(0);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            "test-text",
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>());

        // Assert
        this.mockFont.VerifyNever(m => m.Measure(It.IsAny<string>()));
        this.mockFont.VerifyNever(m => m.ToGlyphMetrics(It.IsAny<string>()));
        this.mockBatchingManager
            .VerifyNever(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()));
    }

    [Fact]
    public void Render_WhenNotCallingBeginFirst_ThrowsException()
    {
        // Arrange
        const string renderText = "hello world";
        MockFontMetrics();
        MockToGlyphMetrics(renderText);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(this.mockFont.Object,
                renderText,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>());

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The 'Begin()' method must be invoked first before any 'Render()' methods.");
    }

    [Fact]
    public void Render_WhenTextIsOnlyNewLineCharacters_DoesNotRenderText()
    {
        // Arrange
        const string renderText = "\n\r\r\n";

        MockFontMetrics();

        MockToGlyphMetrics(renderText);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>());

        // Assert
        this.mockFont.VerifyNever(m => m.ToGlyphMetrics(It.IsAny<string>()));
        this.mockFont.VerifyNever(m => m.GetKerning(It.IsAny<uint>(), It.IsAny<uint>()));
        this.mockBatchingManager
            .VerifyNever(m =>
                m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()));
    }

    [Fact]
    public void Render_WhenInvoked_MeasuresText()
    {
        // Arrange
        const string renderText = "hello world";
        MockFontMetrics();
        MockToGlyphMetrics(renderText);
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>());

        // Assert
        this.mockFont.VerifyOnce(m => m.Measure(renderText));
    }

    [Fact]
    public void Render_WhenRenderingMultilineText_ConvertsEachLineToGlyphMetrics()
    {
        // Arrange
        var renderText = $"hello{Environment.NewLine}world";

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>());

        // Assert
        this.mockFont.VerifyOnce(m => m.ToGlyphMetrics("hello"));
        this.mockFont.VerifyOnce(m => m.ToGlyphMetrics("world"));
    }

    [Fact]
    public void Render_WhenInvoked_AddsCorrectBatchItems()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoked_AddsCorrectBatchItems)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);
        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string renderText = "Font_Testing";
        MockFontMetrics();
        MockToGlyphMetrics(renderText);

        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            400,
            300,
            1.5f,
            45,
            Color.FromArgb(11, 22, 33, 44),
            500);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m =>
                m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()), renderText.Length);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking4ParamsWithXAndYOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking4ParamsWithXAndYOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            11,
            22,
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking3ParamsWithPositionOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking3ParamsWithPositionOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            new Vector2(33, 44),
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking6ParamsWithXAndYOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking6ParamsWithXAndYOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            321,
            202,
            2.25f,
            230f,
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m =>
                m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking5ParamsWithPositionOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking5ParamsWithPositionOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            new Vector2(66, 77),
            1.25f,
            8f,
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m =>
            m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking5ParamsWithColorOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking5ParamsWithColorOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            456,
            635,
            Color.DarkOrange,
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m =>
            m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking4ParamsWithPositionAndColorOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            new Vector2(758, 137),
            Color.MediumPurple,
            123);

        // Assert
        this.mockBatchingManager
            .VerifyExactly(m =>
            m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking6ParamsWithColorOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking6ParamsWithColorOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            147,
            185,
            16f,
            Color.IndianRed,
            123);

        // Assert
        this.mockBatchingManager.VerifyExactly(m =>
            m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont()
    {
        // Arrange
        const string expectedTestDataFileName = $"{nameof(Render_WhenInvoking5ParamsWithPositionAndColorOverload_RendersFont)}.json";
        var expectedBatchResultData =
            TestDataLoader.LoadTestData<FontGlyphBatchItem[]>(BatchTestDataDirPath, expectedTestDataFileName);

        var actualBatchResultData = new List<FontGlyphBatchItem>();

        const string line1 = "hello";
        const string line2 = "world";
        var renderText = $"{line1}{Environment.NewLine}{line2}";
        var totalGlyphs = line1.Length + line2.Length;

        MockFontMetrics();
        MockToGlyphMetrics("hello");
        MockToGlyphMetrics("world");
        this.mockBatchingManager
            .Setup(m => m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Callback<FontGlyphBatchItem, int, DateTime>((item, _, _) =>
            {
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont.Object,
            renderText,
            new Vector2(1255, 79),
            88f,
            Color.CornflowerBlue,
            123);

        // Assert
        this.mockBatchingManager.VerifyExactly(m =>
            m.AddFontItem(It.IsAny<FontGlyphBatchItem>(), 123, It.IsAny<DateTime>()), totalGlyphs);
        actualBatchResultData.Should().BeEquivalentTo(expectedBatchResultData);
    }

    [Fact]
    public void Render_WhenInvoked_RendersFont()
    {
        // Arrange
        const string renderText = "font";

        MockFontMetrics();
        MockToGlyphMetrics(renderText);

        var renderItems = CreateFontRenderItems(renderText);

        var mockFontTextureAtlas = new Mock<ITexture>();
        mockFontTextureAtlas.SetupGet(p => p.Id).Returns(AtlasTextureId);

        var sut = CreateSystemUnderTest();

        this.renderReactor.OnReceive(renderItems);

        // Act
        this.batchHasBegunReactor.OnReceive();

        sut.Render(
            this.mockFont.Object,
            renderText,
            11,
            22);

        // Assert
        this.mockGL.VerifyOnce(m => m.DrawElements(GLPrimitiveType.Triangles,
                6u * (uint)renderText.Length,
                GLDrawElementsType.UnsignedInt,
                nint.Zero));
        this.mockGLService.VerifyOnce(m => m.BindTexture2D(AtlasTextureId));
        this.mockGpuBuffer
            .VerifyExactly(m =>
                m.UploadData(It.IsAny<FontGlyphBatchItem>(), It.IsAny<uint>()), renderText.Length);
    }
    #endregion

    /// <summary>
    /// Creates batch items for the purpose of testing.
    /// </summary>
    /// <param name="batchGlyphs">The glyphs to mock.</param>
    private static Memory<RenderItem<FontGlyphBatchItem>> CreateFontRenderItems(string batchGlyphs)
    {
        var renderItems = new List<RenderItem<FontGlyphBatchItem>>();

        foreach (var t in batchGlyphs)
        {
            var batchItem = new FontGlyphBatchItem(
                RectangleF.Empty,
                RectangleF.Empty,
                t,
                0,
                0,
                Color.Empty,
                RenderEffects.None,
                AtlasTextureId);

            renderItems.Add(new RenderItem<FontGlyphBatchItem>
            {
                Layer = 0,
                Item = batchItem,
            });
        }

        return new Memory<RenderItem<FontGlyphBatchItem>>(renderItems.ToArray());
    }

    /// <summary>
    /// Creates a new instance of <see cref="FontRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontRenderer CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockReactableFactory.Object,
            this.mockGLService.Object,
            this.mockGpuBuffer.Object,
            this.mockShader.Object,
            this.mockBatchingManager.Object);

        /// <summary>
    /// Mocks the font metrics for testing.
    /// </summary>
    private void MockFontMetrics()
    {
        this.allGlyphMetrics = TestDataLoader.LoadTestData<GlyphMetrics[]>(string.Empty, GlyphTestDataFileName).ToList();
        this.mockFont.SetupGet(p => p.Metrics).Returns(() => this.allGlyphMetrics.ToArray().AsReadOnly());
    }

    /// <summary>
    /// Mocks the given <paramref name="text"/> to glyph metrics for testing.
    /// </summary>
    /// <param name="text">The text of glyphs to mock.</param>
    private void MockToGlyphMetrics(string text)
    {
        this.mockFont.Setup(m => m.ToGlyphMetrics(text)).Returns(() =>
        {
            var textGlyphs = this.allGlyphMetrics.Where(m => text.Contains(m.Glyph)).ToList();

            return text.Select(character
                => (from m in textGlyphs
                    where m.Glyph == (this.glyphChars.Contains(character)
                        ? character
                        : InvalidCharacter)
                    select m).FirstOrDefault()).ToArray();
        });
    }
}
