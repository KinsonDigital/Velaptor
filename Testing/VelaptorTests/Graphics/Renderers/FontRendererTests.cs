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
using Shouldly;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Content.Fonts;
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
using FontRenderItem = Carbonate.Core.OneWay.IReceiveSubscription<System.Memory<Velaptor.OpenGL.Batching.RenderItem<
            Velaptor.OpenGL.Batching.FontGlyphBatchItem
        >
    >
>;

/// <summary>
/// Tests the <see cref="FontRenderer"/> class.
/// </summary>
public class FontRendererTests : TestsBase
{
    private const string GlyphTestDataFileName = "glyph-test-data.json";
    private const string BatchTestDataDirPath = "BatchItemTestData";
    private const uint AtlasTextureId = 1234u;
    private const uint FontShaderId = 2222u;
    private const char InvalidCharacter = '□';
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IGpuBuffer<FontGlyphBatchItem> mockGpuBuffer;
    private readonly IShaderProgram mockShader;
    private readonly IFont mockFont;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;

    private readonly char[] glyphChars =
    [
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '~', '_', '+',
        '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
    ];

    private IReceiveSubscription? batchHasBegunReactor;
    private FontRenderItem? renderReactor;

    private List<GlyphMetrics> allGlyphMetrics = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="FontRendererTests"/> class.
    /// </summary>
    public FontRendererTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();

        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockGLService.ProgramLinkedSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.ShaderCompiledSuccessfully(Arg.Any<uint>()).Returns(true);
        this.mockGLService.GetViewPortSize().Returns(new Size(800, 600));

        this.mockShader = Substitute.For<IShaderProgram>();
        this.mockShader.ShaderId.Returns(FontShaderId);

        this.mockGpuBuffer = Substitute.For<IGpuBuffer<FontGlyphBatchItem>>();

        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                this.batchHasBegunReactor = reactor;
            });

        var mockFontRenderBatchReactable = Substitute.For<IRenderBatchReactable<FontGlyphBatchItem>>();
        mockFontRenderBatchReactable
            .When(x => x.Subscribe(Arg.Any<FontRenderItem>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<FontRenderItem>();
                this.renderReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateRenderFontReactable().Returns(mockFontRenderBatchReactable);

        var mockFontTextureAtlas = Substitute.For<ITexture>();
        mockFontTextureAtlas.Width.Returns(200u);
        mockFontTextureAtlas.Height.Returns(100u);

        this.mockFont = Substitute.For<IFont>();
        this.mockFont.Atlas.Returns(mockFontTextureAtlas);
        this.mockFont.Size.Returns(12u);
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontRenderer(
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
            _ = new FontRenderer(
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
            _ = new FontRenderer(
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
            _ = new FontRenderer(
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
    public void Render_WithNullFont_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, "test", 10, 20, 1f, 0f, Color.White);

        // Asset
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe($"Cannot render a null '{nameof(IFont)}'. (Parameter 'font')");
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithNoFontItemsToRender_SetsUpCorrectDebugGroupAndExits()
    {
        // Arrange
        const string shaderName = "TestFontShader";
        this.mockShader.Name.Returns(shaderName);
        _ = CreateSystemUnderTest();

        // Act
        this.renderReactor.OnReceive(default);

        // Assert
        this.mockGLService.Received(1).BeginGroup("Render Text Process - Nothing To Render");
        this.mockGLService.Received(1).EndGroup();
        this.mockGLService.DidNotReceive().BeginGroup($"Render Text Process With {shaderName} Shader");
        this.mockShader.DidNotReceive().Use();
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Update Character Data - TextureID")));
        this.mockGL.DidNotReceive().ActiveTexture(Arg.Any<GLTextureUnit>());
        this.mockGLService
            .DidNotReceive()
            .BindTexture2D(Arg.Any<uint>());
        this.mockGpuBuffer
            .DidNotReceive()
            .UploadData(Arg.Any<FontGlyphBatchItem>(), Arg.Any<uint>());
        this.mockGLService
            .DidNotReceive()
            .BeginGroup(Arg.Is<string>(value => value.StartsWith("Render ") && value.EndsWith(" Font Elements")));
        this.mockGL
            .DidNotReceive()
            .DrawElements(
                Arg.Any<GLPrimitiveType>(),
                Arg.Any<uint>(),
                Arg.Any<GLDrawElementsType>(),
                Arg.Any<nint>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [Trait("Category", Method)]
    public void Render_WithNullOrEmptyText_DoesNotRenderText(string? renderText)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            font: Substitute.For<IFont>(),
            text: renderText,
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        this.mockFont.DidNotReceive().Measure(Arg.Any<string>());
        this.mockFont.DidNotReceive().ToGlyphMetrics(Arg.Any<string>());
        this.mockBatchingManager
            .DidNotReceive()
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WithFontSizeSetToZero_DoesNotRenderText()
    {
        // Arrange
        this.mockFont.Size.Returns(0u);

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            font: this.mockFont,
            text: "test-text",
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        this.mockFont.DidNotReceive().Measure(Arg.Any<string>());
        this.mockFont.DidNotReceive().ToGlyphMetrics(Arg.Any<string>());
        this.mockBatchingManager
            .DidNotReceive()
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenNotCallingBeginFirst_ThrowsException()
    {
        // Arrange
        const string renderText = "hello world";
        MockFontMetrics();
        MockToGlyphMetrics(renderText);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(
            font: this.mockFont,
            text: renderText,
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("The 'Begin()' method must be invoked first before any 'Render()' methods.");
    }

    [Fact]
    [Trait("Category", Method)]
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
            font: this.mockFont,
            text: renderText,
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        this.mockFont.DidNotReceive().ToGlyphMetrics(Arg.Any<string>());
        this.mockFont.DidNotReceive().GetKerning(Arg.Any<uint>(), Arg.Any<uint>());
        this.mockBatchingManager
            .DidNotReceive()
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    [Trait("Category", Method)]
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
            font: this.mockFont,
            text: renderText,
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        this.mockFont.Received(1).Measure(renderText);
    }

    [Fact]
    [Trait("Category", Method)]
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
            font: this.mockFont,
            text: renderText,
            x: default,
            y: default,
            renderSize: default,
            angle: default,
            color: default);

        // Assert
        this.mockFont.Received(1).ToGlyphMetrics("hello");
        this.mockFont.Received(1).ToGlyphMetrics("world");
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(call =>
            {
                var item = call.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            400,
            300,
            1.5f,
            45,
            Color.FromArgb(11, 22, 33, 44),
            500);

        // Assert
        this.mockBatchingManager
            .Received(renderText.Length)
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            11,
            22,
            123);

        // Assert
        this.mockBatchingManager.Received(totalGlyphs).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            new Vector2(33, 44),
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            321,
            202,
            2.25f,
            230f,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            new Vector2(66, 77),
            1.25f,
            8f,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Any<FontGlyphBatchItem>(), 123, Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            456,
            635,
            Color.DarkOrange,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Is<FontGlyphBatchItem>(x => true), 123, Arg.Any<DateTime>());
        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            new Vector2(758, 137),
            Color.MediumPurple,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Is<FontGlyphBatchItem>(x => true), 123, Arg.Any<DateTime>());

        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            147,
            185,
            16f,
            Color.IndianRed,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Is<FontGlyphBatchItem>(x => true), 123, Arg.Any<DateTime>());

        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
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
            .When(m => m.AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>()))
            .Do(ci =>
            {
                var item = ci.Arg<FontGlyphBatchItem>();
                actualBatchResultData.Add(item);
            });

        var sut = CreateSystemUnderTest();
        this.batchHasBegunReactor.OnReceive();

        // Act
        sut.Render(
            this.mockFont,
            renderText,
            new Vector2(1255, 79),
            88f,
            Color.CornflowerBlue,
            123);

        // Assert
        this.mockBatchingManager
            .Received(totalGlyphs)
            .AddFontItem(Arg.Is<FontGlyphBatchItem>(x => true), 123, Arg.Any<DateTime>());

        actualBatchResultData.ShouldBe(expectedBatchResultData);
    }

    [Fact]
    [Trait("Category", Method)]
    public void Render_WhenInvoked_RendersFont()
    {
        // Arrange
        const string renderText = "font";

        MockFontMetrics();
        MockToGlyphMetrics(renderText);

        var renderItems = CreateFontRenderItems(renderText);

        var mockFontTextureAtlas = Substitute.For<ITexture>();
        mockFontTextureAtlas.Id.Returns(AtlasTextureId);

        var sut = CreateSystemUnderTest();

        this.renderReactor.OnReceive(renderItems);

        // Act
        this.batchHasBegunReactor.OnReceive();

        sut.Render(
            this.mockFont,
            renderText,
            11,
            22);

        // Assert
        this.mockGL
            .Received(1)
            .DrawElements(GLPrimitiveType.Triangles,
                6u * (uint)renderText.Length,
                GLDrawElementsType.UnsignedInt,
                nint.Zero);

        this.mockGLService.Received(1).BindTexture2D(AtlasTextureId);
        this.mockGpuBuffer.Received(renderText.Length).UploadData(Arg.Any<FontGlyphBatchItem>(), Arg.Any<uint>());
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
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"FontRenderer.ctor() - {PushNotifications.BatchHasBegunId}");
            });
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void FontRenderBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        var mockFontRenderBatchReactable = Substitute.For<IRenderBatchReactable<FontGlyphBatchItem>>();
        mockFontRenderBatchReactable
            .When(x => x.Subscribe(Arg.Any<FontRenderItem>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<FontRenderItem>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"FontRenderer.ctor() - {PushNotifications.RenderFontsId}");
            });
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

            renderItems.Add(new RenderItem<FontGlyphBatchItem> { Layer = 0, Item = batchItem, });
        }

        return new Memory<RenderItem<FontGlyphBatchItem>>(renderItems.ToArray());
    }

    /// <summary>
    /// Creates a new instance of <see cref="FontRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontRenderer CreateSystemUnderTest()
        => new (this.mockGL,
            this.mockReactableFactory,
            this.mockGLService,
            this.mockGpuBuffer,
            this.mockShader,
            this.mockBatchingManager);

    /// <summary>
    /// Mocks the font metrics for testing.
    /// </summary>
    private void MockFontMetrics()
    {
        this.allGlyphMetrics = TestDataLoader.LoadTestData<GlyphMetrics[]>(string.Empty, GlyphTestDataFileName).ToList();
        this.mockFont.Metrics.Returns(this.allGlyphMetrics.ToArray().AsReadOnly());
    }

    /// <summary>
    /// Mocks the given <paramref name="text"/> to glyph metrics for testing.
    /// </summary>
    /// <param name="text">The text of glyphs to mock.</param>
    private void MockToGlyphMetrics(string text)
    {
        this.mockFont.ToGlyphMetrics(text).Returns(ci =>
        {
            var textGlyphs = this.allGlyphMetrics.Where(m => text.Contains(m.Glyph)).ToList();
            return text.Select(character
                    => textGlyphs.FirstOrDefault(m => m.Glyph == (this.glyphChars.Contains(character)
                        ? character
                        : InvalidCharacter)))
                .ToArray();
        });
    }
}
