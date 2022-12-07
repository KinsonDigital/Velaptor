// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Fonts;
using Velaptor.Content.Fonts.Services;
using Velaptor.Graphics;
using Velaptor.Services;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="Font"/> class.
/// </summary>
public class FontTests : IDisposable
{
    private const char InvalidCharacter = '□';
    private const string DirPath = @"C:/test-dir/fonts";
    private const string FontName = "test-font";
    private const string FontExtension = ".ttf";
    private readonly string fontFilePath;
    private readonly IntPtr facePtr = new (5678);
    private readonly Mock<IFontService> mockFontService;
    private readonly Mock<IFontStatsService> mockFontStatsService;
    private readonly Mock<ITexture> mockTexture;
    private readonly Mock<IFontAtlasService> mockFontAtlasService;
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly string sampleTestDataDirPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .ToCrossPlatPath() + "/SampleTestData";
    private Dictionary<char, GlyphMetrics> glyphMetrics = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="FontTests"/> class.
    /// </summary>
    public FontTests()
    {
        this.fontFilePath = $"{DirPath}/{FontName}{FontExtension}";

        const string glyphTestDataFileName = "glyph-test-data.json";
        var glyphMetricFilePath = $"{this.sampleTestDataDirPath}/{glyphTestDataFileName}";
        var glyphMetricData = File.ReadAllText(glyphMetricFilePath);

        var glyphMetricItems = JsonConvert.DeserializeObject<GlyphMetrics[]>(glyphMetricData);

        foreach (var metric in glyphMetricItems)
        {
            this.glyphMetrics.Add(metric.Glyph, metric);
        }

        this.mockFontService = new Mock<IFontService>();
        this.mockFontService.Setup(m => m.CreateFontFace(this.fontFilePath)).Returns(this.facePtr);
        this.mockFontService.Setup(m => m.CreateGlyphMetrics(this.facePtr, null))
            .Returns(this.glyphMetrics);
        this.mockFontService.Setup(m => m.HasKerning(this.facePtr)).Returns(true);

        this.mockFontStatsService = new Mock<IFontStatsService>();

        this.mockFontAtlasService = new Mock<IFontAtlasService>();
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();

        this.mockTexture = new Mock<ITexture>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                null,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'texture')");
    }

    [Fact]
    public void Ctor_WithNullFontServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture.Object,
                null,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontService')");
    }

    [Fact]
    public void Ctor_WithNullFontStatsServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture.Object,
                this.mockFontService.Object,
                null,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontStatsService')");
    }

    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture.Object,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                null,
                this.mockTextureCache.Object,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture.Object,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                null,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'textureCache')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullNameParam_ThrowsException(string name)
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture.Object,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                name,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange
        this.mockTexture.SetupGet(p => p.FilePath);
        this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Italic);
        this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.FontTextureAtlas.Should().BeEquivalentTo(this.mockTexture.Object);
        sut.Metrics.Count.Should().Be(this.glyphMetrics.Count);
        sut.Name.Should().Be(FontName);
        sut.FamilyName.Should().Be("test-font-family");
        sut.HasKerning.Should().BeTrue();
        sut.FilePath.Should().Be(this.fontFilePath);
    }

    [Fact]
    public void Ctor_WithAtLeastOneOrMoreFontStyles_SetsFontSource()
    {
        // Arrange
        const string familyName = "test-font-family";
        const FontStyle boldItalic = FontStyle.Bold | FontStyle.Italic;
        var contentFontStats = new FontStats[]
        {
            new () { Style = FontStyle.Regular, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            new () { Style = FontStyle.Bold, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            new () { Style = FontStyle.Italic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            new () { Style = boldItalic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
        };

        this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns(familyName);
        this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Bold);
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(familyName))
            .Returns(contentFontStats);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Source.Should().Be(FontSource.AppContent);
    }

    [Fact]
    public void Ctor_WithMissingStylesInContentButExistsInSystem_SetsFontSource()
    {
        // Arrange
        const string familyName = "test-font-family";
        const FontStyle boldItalic = FontStyle.Bold | FontStyle.Italic;
        var contentFontStats = new FontStats[]
        {
            new () { Style = FontStyle.Regular, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            new () { Style = FontStyle.Bold, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            new () { Style = FontStyle.Italic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
        };
        var systemFontStats = new FontStats[]
        {
            new () { Style = boldItalic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
        };

        this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns(familyName);
        this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(boldItalic);
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(familyName))
            .Returns(contentFontStats);
        this.mockFontStatsService.Setup(m => m.GetSystemStatsForFontFamily(familyName))
            .Returns(systemFontStats);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Source.Should().Be(FontSource.AppContent);
        sut.Style.Should().Be(boldItalic);
    }

    [Fact]
    public void Ctor_WithNoFontStyles_SetsFontSourceToUnknown()
    {
        // Arrange
        this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");
        this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Bold);
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily("test-font-family"))
            .Returns(Array.Empty<FontStats>());

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Source.Should().Be(FontSource.Unknown);
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertiesToCorrectValues()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Actual
        var actualName = sut.Name;
        var actualFilePath = sut.FilePath;
        var actualIsDefaultFont = sut.IsDefaultFont;

        // Assert
        actualName.Should().Be(FontName);
        actualFilePath.Should().Be(this.fontFilePath);
        actualIsDefaultFont.Should().BeTrue();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void AvailableStylesForFamily_WhenNoStylesExist_ReturnsEmpty()
    {
        // Arrange
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(Array.Empty<FontStats>());

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AvailableStylesForFamily;

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void AvailableStylesForFamily_WhenAnyStylesExist_Returns()
    {
        // Arrange
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(new FontStats[] { new () { Style = FontStyle.Bold } });

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AvailableStylesForFamily;

        // Assert
        actual.Should().ContainSingle();
    }

    [Fact]
    public void LineSpacing_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontService.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12))
            .Returns(0.5f);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LineSpacing;

        // Assert
        actual.Should().Be(0.5f);
    }

    [Fact]
    public void Style_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(new FontStats[] { new () { Style = FontStyle.Italic, FontFilePath = this.fontFilePath } });

        var sut = CreateSystemUnderTest();

        // Act
        sut.Style = FontStyle.Italic;
        var actual = sut.Style;

        // Assert
        actual.Should().Be(FontStyle.Italic);
    }

    [Fact]
    public void Style_WhenUsingStyleThatDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(new FontStats[] { new () { Style = FontStyle.Bold } });

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Style = FontStyle.Italic;

        // Assert
        act.Should().Throw<LoadFontException>()
            .WithMessage("The font style 'Italic' does not exist for the font family 'test-font-family'.");
    }

    [Fact]
    public void Size_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(new FontStats[] { new () { Style = FontStyle.Regular, FontFilePath = this.fontFilePath } });

        var sut = CreateSystemUnderTest();

        // Act
        sut.Size = 22;
        var actual = sut.Size;

        // Assert
        actual.Should().Be(22u);
    }

    [Fact]
    public void Size_WhenValueIsEqualToZero_DoesNotBuildFontAtlasTexture()
    {
        // Arrange
        this.mockFontService.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12))
            .Returns(123u);
        this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
            .Returns(new FontStats[] { new () { Style = FontStyle.Regular, FontFilePath = this.fontFilePath } });

        var sut = CreateSystemUnderTest();

        // Act
        sut.Size = 0;

        // Assert
        sut.FontTextureAtlas.Should().Be(this.mockTexture.Object);
        sut.LineSpacing.Should().Be(123);
        this.mockFontAtlasService.VerifyNever(m => m.CreateFontAtlas(It.IsAny<string>(), It.IsAny<uint>()));
        this.mockFontService.VerifyNever(m => m.GetFontScaledLineSpacing(It.IsAny<IntPtr>(), 0u));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetKerning_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontService.Setup(m => m.GetKerning(this.facePtr, 11, 22)).Returns(33);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetKerning(11, 22);

        // Assert
        this.mockFontService.Verify(m => m.GetKerning(this.facePtr, 11, 22), Times.Once);
        actual.Should().Be(33);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Measure_WithNullOrEmptyText_ReturnsEmptySize(string text)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Measure(text);

        // Assert
        actual.Width.Should().Be(0);
        actual.Height.Should().Be(0);
    }

    [Fact]
    public void Measure_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var text = $"hello{Environment.NewLine}world";

        this.mockFontService.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12)).Returns(2f);
        this.mockFontService.Setup(m => m.HasKerning(this.facePtr)).Returns(true);
        MockGlyphKernings(text);

        var font = new Font(
            this.mockTexture.Object,
            this.mockFontService.Object,
            this.mockFontStatsService.Object,
            this.mockFontAtlasService.Object,
            this.mockTextureCache.Object,
            "test-name",
            DirPath,
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            this.glyphMetrics.Values.ToArray());

        // Act
        var actual = font.Measure(text);

        // Assert
        actual.Width.Should().Be(103);
        actual.Height.Should().Be(31);
    }

    [Fact]
    public void ToGlyphMetrics_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const string text = "test©";
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ToGlyphMetrics(text);

        // Assert
        actual.Should().HaveCount(5);
        actual[0].Glyph.Should().Be('t');
        actual[1].Glyph.Should().Be('e');
        actual[2].Glyph.Should().Be('s');
        actual[3].Glyph.Should().Be('t');
        actual[4].Glyph.Should().Be(InvalidCharacter);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetCharacterBounds_WithNullOrEmptyText_ReturnsEmptyResult(string value)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetCharacterBounds(value, Vector2.Zero);

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void GetCharacterBounds_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const string testText = "test-value";
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetCharacterBounds(testText, Vector2.Zero).ToArray();

        // Assert
        actual.Should().AllSatisfy(data =>
        {
            var character = data.character;
            testText.ToArray().Should().Contain(character, $"the character '{character}' should be in the text '{testText}'.");
        });

        // Assert that the test text characters all have a bounds Y position of 0
        actual.Should().AllSatisfy(data =>
        {
            if (data.character == '-')
            {
                return;
            }

            var bounds = data.bounds;
            bounds.Y.Should().Be(0);
        });

        // Assert that the character t has the correct height
        actual.Where(i => i.character == 't').Should().AllSatisfy(data =>
        {
            data.bounds.Height.Should().Be(26);
        });

        // Assert that the character - has the correct height
        actual.Where(i => i.character == '-').Should().AllSatisfy(data =>
        {
            data.bounds.Height.Should().Be(4);
        });

        // Assert that the character l has the correct height
        actual.Where(i => i.character == 'l').Should().AllSatisfy(data =>
        {
            data.bounds.Height.Should().Be(31);
        });

        // Assert that all the characters e, s, v, a, and u all have a height of 20
        actual.Where(i => "esvau".Contains(i.character)).Should().AllSatisfy(data =>
        {
            data.bounds.Height.Should().Be(20);
        });

        Assert.Equal(10, actual.Length);
        actual.Length.Should().Be(10);
    }
    #endregion

    /// <inheritdoc/>
    public void Dispose() => this.glyphMetrics = null;

    /// <summary>
    /// Creates a new instance of <see cref="Font"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Font CreateSystemUnderTest(uint size = 12)
        => new (
            this.mockTexture.Object,
            this.mockFontService.Object,
            this.mockFontStatsService.Object,
            this.mockFontAtlasService.Object,
            this.mockTextureCache.Object,
            FontName,
            this.fontFilePath,
            size,
            true,
            this.glyphMetrics.Values.ToArray());

    /// <summary>
    /// Mocks the kerning value for each character in the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to mock the kerning values for.</param>
    private void MockGlyphKernings(string text)
    {
        if (this.glyphMetrics is null || this.glyphMetrics.Count <= 0)
        {
            Assert.True(false, $"Cannot run test with the static class member '{this.glyphMetrics}' being null or empty.");
        }

        /* NOTE:
         * For the text 'hello\nworld', the kerning values should be mocked for each character below
         * h = 1
         * e = 2
         * l = 3
         * l = 4
         * o = 5
         * w = 6
         * o = 6
         * r = 7
         * l = 8
         * d = 9
         */
        var leftGlyphIndex = 0u;
        var rightGlyphIndex = 0u;

        for (var i = 0; i < text.Length; i++)
        {
            var glyphChar = text[i];

            if (i > 0)
            {
                var foundGlyphMetric = (from m in this.glyphMetrics.Values
                    where m.Glyph == glyphChar
                    select m).FirstOrDefault();

                rightGlyphIndex = foundGlyphMetric.CharIndex;
            }

            var leftIndex = leftGlyphIndex;
            var rightIndex = rightGlyphIndex;
            this.mockFontService.Setup(m => m.GetKerning(this.facePtr, leftIndex, rightIndex))
                .Returns(i + 1);

            leftGlyphIndex = rightGlyphIndex;
        }
    }
}
