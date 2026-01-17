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
using System.Text;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Content.Fonts.Services;
using Velaptor.ExtensionMethods;
using Velaptor.Graphics;
using Velaptor.NativeInterop.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="Font"/> class.
/// </summary>
public class FontTests
{
    private const char InvalidCharacter = '□';
    private const string DirPath = "C:/test-dir/fonts";
    private const string FontName = "test-font";
    private const string FontExtension = ".ttf";
    private readonly string fontFilePath;
    private readonly nint facePtr = new (5678);
    private readonly IFreeTypeService mockFreeTypeService;
    private readonly IFontStatsService mockFontStatsService;
    private readonly ITexture mockTexture;
    private readonly string sampleTestDataDirPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .ToCrossPlatPath() + "/SampleTestData";
    private readonly Dictionary<char, GlyphMetrics> glyphMetrics = new ();

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

        this.mockFreeTypeService = Substitute.For<IFreeTypeService>();
        this.mockFreeTypeService.CreateFontFace(this.fontFilePath).Returns(this.facePtr);
        this.mockFreeTypeService.CreateGlyphMetrics(this.facePtr, null)
            .Returns(this.glyphMetrics);
        this.mockFreeTypeService.HasKerning(this.facePtr).Returns(true);

        this.mockFontStatsService = Substitute.For<IFontStatsService>();

        this.mockTexture = Substitute.For<ITexture>();
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
                this.mockFreeTypeService,
                this.mockFontStatsService,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Ctor_WithNullFreeTypeServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture,
                null,
                this.mockFontStatsService,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'freeTypeService')");
    }

    [Fact]
    public void Ctor_WithNullFontStatsServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture,
                this.mockFreeTypeService,
                null,
                FontName,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fontStatsService')");
    }

    [Fact]
    public void Ctor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture,
                this.mockFreeTypeService,
                this.mockFontStatsService,
                null,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WithEmptyName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Font(
                this.mockTexture,
                this.mockFreeTypeService,
                this.mockFontStatsService,
                string.Empty,
                this.fontFilePath,
                12u,
                true,
                this.glyphMetrics.Values.ToArray());
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsPropertyValues()
    {
        // Arrange
        this.mockFreeTypeService.GetFontStyle(this.facePtr, this.fontFilePath).Returns(FontStyle.Italic);
        this.mockFreeTypeService.GetFamilyName(this.facePtr, this.fontFilePath).Returns("test-font-family");

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Atlas.ShouldBeEquivalentTo(this.mockTexture);
        sut.Metrics.Count.ShouldBe(this.glyphMetrics.Count);
        sut.Name.ShouldBe(FontName);
        sut.FamilyName.ShouldBe("test-font-family");
        sut.HasKerning.ShouldBeTrue();
        sut.FilePath.ShouldBe(this.fontFilePath);
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

        this.mockFreeTypeService.GetFamilyName(this.facePtr, this.fontFilePath).Returns(familyName);
        this.mockFreeTypeService.GetFontStyle(this.facePtr, this.fontFilePath).Returns(FontStyle.Bold);
        this.mockFontStatsService.GetContentStatsForFontFamily(familyName).Returns(contentFontStats);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Source.ShouldBe(FontSource.AppContent);
    }

    [Fact]
    public void Ctor_WithNoFontStyles_SetsFontSourceToUnknown()
    {
        // Arrange
        this.mockFreeTypeService.GetFamilyName(this.facePtr, this.fontFilePath).Returns("test-font-family");
        this.mockFreeTypeService.GetFontStyle(this.facePtr, this.fontFilePath).Returns(FontStyle.Bold);
        this.mockFontStatsService.GetContentStatsForFontFamily("test-font-family").Returns([]);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Source.ShouldBe(FontSource.Unknown);
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
        actualName.ShouldBe(FontName);
        actualFilePath.ShouldBe(this.fontFilePath);
        actualIsDefaultFont.ShouldBeTrue();
    }

    [Fact]
    public void Ctor_WhenInvoked_PropsSetToCorrectDefaultValues()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        sut.CacheEnabled.ShouldBeTrue();
        sut.MaxMeasureCacheSize.ShouldBe(1000);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void AvailableStylesForFamily_WhenNoStylesExist_ReturnsEmpty()
    {
        // Arrange
        this.mockFontStatsService.GetContentStatsForFontFamily(Arg.Any<string>()).Returns([]);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AvailableStylesForFamily;

        // Assert
        actual.ShouldBeEmpty();
    }

    [Fact]
    public void AvailableStylesForFamily_WhenAnyStylesExist_Returns()
    {
        // Arrange
        this.mockFontStatsService.GetContentStatsForFontFamily(Arg.Any<string>())
            .Returns([new () { Style = FontStyle.Bold }]);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AvailableStylesForFamily;

        // Assert
        actual.ShouldHaveSingleItem();
    }

    [Fact]
    public void LineSpacing_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFreeTypeService.GetFontScaledLineSpacing(this.facePtr, 12)
            .Returns(0.5f);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LineSpacing;

        // Assert
        actual.ShouldBe(0.5f);
    }

    [Fact]
    public void Style_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontStatsService.GetContentStatsForFontFamily(Arg.Any<string>())
            .Returns([new () { Style = FontStyle.Italic, FontFilePath = this.fontFilePath }]);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Style = FontStyle.Italic;
        var actual = sut.Style;

        // Assert
        actual.ShouldBe(FontStyle.Italic);
    }

    [Fact]
    public void Size_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFontStatsService.GetContentStatsForFontFamily(Arg.Any<string>())
            .Returns([new () { Style = FontStyle.Regular, FontFilePath = this.fontFilePath }]);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Size = 22;
        var actual = sut.Size;

        // Assert
        actual.ShouldBe(22u);
    }

    [Fact]
    public void Size_WhenValueIsEqualToZero_DoesNotBuildAtlas()
    {
        // Arrange
        this.mockFreeTypeService.GetFontScaledLineSpacing(this.facePtr, 12)
            .Returns(123u);
        this.mockFontStatsService.GetContentStatsForFontFamily(Arg.Any<string>())
            .Returns([new () { Style = FontStyle.Regular, FontFilePath = this.fontFilePath }]);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Size = 0;

        // Assert
        sut.Atlas.ShouldBe(this.mockTexture);
        sut.LineSpacing.ShouldBe(123);
        this.mockFreeTypeService.DidNotReceive().GetFontScaledLineSpacing(Arg.Any<nint>(), 0u);
    }

    [Fact]
    public void CacheEnabled_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.CacheEnabled = false;
        var actual = sut.CacheEnabled;

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void CacheMeasurements_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MaxMeasureCacheSize = 100;
        var actual = sut.MaxMeasureCacheSize;

        // Assert
        actual.ShouldBe(100);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetKerning_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFreeTypeService.GetKerning(this.facePtr, 11, 22).Returns(33);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetKerning(11, 22);

        // Assert
        this.mockFreeTypeService.Received(1).GetKerning(this.facePtr, 11, 22);
        actual.ShouldBe(33);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Measure_WithNullOrEmptyText_ReturnsEmptySize(string? text)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Measure(text);

        // Assert
        actual.Width.ShouldBe(0);
        actual.Height.ShouldBe(0);
    }

    [Theory]
    [InlineData(true, 10)]
    [InlineData(false, 20)]
    public void Measure_WhenInvoked_ReturnsCorrectResult(bool useCaching, int executeKerningCount)
    {
        // ReSharper disable once CommentTypo
        /* NOTE:
         * The kerning invoke count is 20 because the Measure() method is being called twice.
         * The text 'hello\nworld' contains 10 render-capable characters and kerning is invoked for each character.
         */
        // Arrange
        const string text = "hello\r\nworld";

        this.mockFreeTypeService.GetFontScaledLineSpacing(this.facePtr, 12).Returns(2f);
        this.mockFreeTypeService.HasKerning(this.facePtr).Returns(true);
        MockGlyphKernings(text);

        var font = CreateSystemUnderTest();
        font.CacheEnabled = useCaching;

        // Act
        var actual = font.Measure(text);
        font.Measure(text);

        // Assert
        actual.Width.ShouldBe(137);
        actual.Height.ShouldBe(33);

        this.mockFreeTypeService.Received(executeKerningCount).GetKerning(Arg.Any<nint>(), Arg.Any<uint>(), Arg.Any<uint>());
    }

    [Fact]
    public void Measure_WhenInvoked_Something()
    {
        // Arrange
        const string text = "hello\r\nworld";
        MockGlyphKernings(text);

        var font = CreateSystemUnderTest();
        font.CacheEnabled = true;
        font.MaxMeasureCacheSize = 0;

        // Act
        font.Measure(text);

        // Assert
        font.CurrentMeasureCacheSize.ShouldBe(0);
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
        actual.Length.ShouldBe(5);
        actual[0].Glyph.ShouldBe('t');
        actual[1].Glyph.ShouldBe('e');
        actual[2].Glyph.ShouldBe('s');
        actual[3].Glyph.ShouldBe('t');
        actual[4].Glyph.ShouldBe(InvalidCharacter);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetCharacterBounds_WithNullOrEmptyText_ReturnsEmptyResult(string? value)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetCharacterBounds(value, Vector2.Zero);

        // Assert
        actual.ShouldBeEmpty();
    }

    [Fact]
    public void GetCharacterBounds_WhenInvokedWithStringParam_ReturnsCorrectResult()
    {
        // Arrange
        const string testText = "test-value";
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetCharacterBounds(testText, Vector2.Zero).ToArray();

        // Assert
        actual.ShouldAllBe(data => testText.Contains(data.character.ToString()), $"the character should be in the text '{testText}'.");

        // Assert that the test text characters all have a bounds Y position of 0
        actual.ShouldAllBe(data => data.character == '-' || data.bounds.Y == 0);

        // Assert that the character 't' has the correct height
        actual.Where(i => i.character == 't').ShouldAllBe(data => Math.Abs(data.bounds.Height - 26) <= 0);

        // Assert that the character '-' has the correct height
        actual.Where(i => i.character == '-').ShouldAllBe(data => Math.Abs(data.bounds.Height - 4) <= 0);

        // Assert that the character 'l' has the correct height
        actual.Where(i => i.character == 'l').ShouldAllBe(data => Math.Abs(data.bounds.Height - 31) <= 0);

        // Assert that all the characters 'e', 's', 'v', 'a', and 'u' all have a height of 20
        actual.Where(i => "esvau".Contains(i.character)).ShouldAllBe(data => Math.Abs(data.bounds.Height - 20) <= 0);

        Assert.Equal(10, actual.Length);
        actual.Length.ShouldBe(10);
    }

    [Fact]
    public void GetCharacterBounds_WhenInvokedWithStringBuilderParam_ReturnsCorrectResult()
    {
        // Arrange
        var testText = new StringBuilder("test-value");
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetCharacterBounds(testText, Vector2.Zero).ToArray();

        // Assert
        actual.ShouldAllBe(data => testText.ToString().Contains(data.character.ToString()), $"the character should be in the text '{testText}'.");

        // Assert that the test text characters all have a bounds Y position of 0
        const float epsilon = 1e-5f;
        actual.ShouldAllBe(data => data.character == '-' || Math.Abs(data.bounds.Y - 0f) <= epsilon);

        // Assert that the character 't' has the correct height
        actual.Where(i => i.character == 't').ShouldAllBe(data => Math.Abs(data.bounds.Height - 26) <= 0);

        // Assert that the character '-' has the correct height
        actual.Where(i => i.character == '-').ShouldAllBe(data => Math.Abs(data.bounds.Height - 4) <= 0);

        // Assert that the character 'l' has the correct height
        actual.Where(i => i.character == 'l').ShouldAllBe(data => Math.Abs(data.bounds.Height - 31) <= 0);

        // Assert that all the characters 'e', 's', 'v', 'a', and 'u' all have a height of 20
        actual.Where(i => "esvau".Contains(i.character)).ShouldAllBe(data => Math.Abs(data.bounds.Height - 20) <= 0);

        Assert.Equal(10, actual.Length);
        actual.Length.ShouldBe(10);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Font"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Font CreateSystemUnderTest(uint size = 12)
        => new (
            this.mockTexture,
            this.mockFreeTypeService,
            this.mockFontStatsService,
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
        if (this.glyphMetrics.Count <= 0)
        {
            Assert.Fail($"Cannot run test with the static class member '{this.glyphMetrics}' being null or empty.");
        }

        // Strip new line and carriage feed characters. The white space characters
        // do not contribute to the kerning values.
        text = text.Replace("\r", string.Empty);
        text = text.Replace("\n", string.Empty);

        // ReSharper disable once CommentTypo
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
            this.mockFreeTypeService.GetKerning(this.facePtr, leftIndex, rightIndex)
                .Returns(i + 1);

            leftGlyphIndex = rightGlyphIndex;
        }
    }
}
