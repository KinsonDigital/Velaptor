// <copyright file="MeasureTextBenchmarks.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace MeasureTextPerf;

using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using Moq;
using Newtonsoft.Json;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Fonts;
using Velaptor.Content.Fonts.Services;
using Velaptor.Graphics;
using Velaptor.NativeInterop.Services;
using Velaptor.Services;

/// <summary>
/// Performance benchmarks for measuring the time it takes to measure text.
/// </summary>
[MemoryDiagnoser]
public class MeasureTextBenchmarks
{
    private readonly IFont font;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeasureTextBenchmarks"/> class.
    /// </summary>
    /// <exception cref="Exception">Thrown if there is an issue loading the sample data for benchmarking.</exception>
    public MeasureTextBenchmarks()
    {
        var baseDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var metricDataFilePath = $"{baseDirPath}/glyph-test-data.json";

        var jsonData = File.ReadAllText(metricDataFilePath);
        var metricData = JsonConvert.DeserializeObject<GlyphMetrics[]>(jsonData);

        if (metricData is null)
        {
            throw new Exception("Failed to load glyph metrics data");
        }

        var mockTexture = new Mock<ITexture>();

        var mockFontService = new Mock<IFreeTypeService>();
        mockFontService.Setup(m => m.HasKerning(It.IsAny<nint>())).Returns(false);

        var mockFontStatsService = new Mock<IFontStatsService>();
        var mockFontAtlasService = new Mock<IFontAtlasService>();
        var mockTextureCache = new Mock<IItemCache<string, ITexture>>();

        this.font = new Font(
            mockTexture.Object,
            mockFontService.Object,
            mockFontStatsService.Object,
            mockFontAtlasService.Object,
            mockTextureCache.Object,
            "test-font",
            "test-font-path",
            12u,
            true,
            metricData);
        this.font.CacheEnabled = true;
    }

    /// <summary>
    /// Gets or sets the total number of characters of text to use for the benchmarks.
    /// </summary>
    [Params(10, 100, 1_000, 10_000, 100_000)]
    public int TotalCharacters { get; set; }

    /// <summary>
    /// Gets or sets the total number of lines of text to use for the benchmarks.
    /// </summary>
    [Params(1, 10, 50, 100)]
    public int TotalLines { get; set; }

    /// <summary>
    /// Gets or sets the text to use for the benchmarks.
    /// </summary>
    private string Text { get; set; } = string.Empty;

    /// <summary>
    /// Sets up the test.
    /// </summary>
    [IterationSetup]
    public void IterationSetup()
    {
        var textToMeasure = new StringBuilder();

        var random = new Random();

        for (var i = 1; i <= TotalCharacters; i++)
        {
            var randomChar = (char)random.Next(32, 126);
            textToMeasure.Append(randomChar);

            var addNewLine = i % TotalLines == 0;

            if (addNewLine)
            {
                textToMeasure.Append('\n');
            }
        }

        Text = textToMeasure.ToString();
    }

    /// <summary>
    /// Runs performance testing on the text measure process.
    /// </summary>
    [Benchmark(Description = "Measure String()")]
    public void MeasureString() => this.font.Measure(Text);
}
