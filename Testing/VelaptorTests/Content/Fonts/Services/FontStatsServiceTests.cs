// <copyright file="FontStatsServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts.Services
{
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.NativeInterop.FreeType;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontStatsService"/> class.
    /// </summary>
    public class FontStatsServiceTests
    {
        private const string RootContentDirPath = @"C:\content-dir\";
        private const string DirNameForContentPath = "content-fonts";
        private const string RootSystemDirPath = @"C:\system-dir\";
        private const string DirNameForSystemPath = "system-fonts";
        private readonly string fullContentFontDirPath = $@"{RootContentDirPath}{DirNameForContentPath}\";
        private readonly string fullSystemFontDirPath = $@"{RootSystemDirPath}{DirNameForSystemPath}\";
        private readonly Mock<IFontService> mockFontService;
        private readonly Mock<IPathResolver> mockSystemFontPathResolver;
        private readonly Mock<IPathResolver> mockContentPathResolver;
        private readonly Mock<IDirectory> mockDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontStatsServiceTests"/> class.
        /// </summary>
        public FontStatsServiceTests()
        {
            this.mockFontService = new Mock<IFontService>();

            this.mockContentPathResolver = new Mock<IPathResolver>();
            this.mockContentPathResolver.Setup(m => m.ResolveDirPath())
                .Returns(this.fullContentFontDirPath);
            this.mockContentPathResolver.SetupGet(p => p.ContentDirectoryName)
                .Returns(DirNameForContentPath);

            this.mockSystemFontPathResolver = new Mock<IPathResolver>();
            this.mockSystemFontPathResolver.Setup(m => m.ResolveDirPath())
                .Returns(this.fullSystemFontDirPath);
            this.mockSystemFontPathResolver.SetupGet(p => p.ContentDirectoryName)
                .Returns(DirNameForSystemPath);

            this.mockDirectory = new Mock<IDirectory>();
        }

        #region Method Tests
        [Fact]
        public void GetContentStatsForFontFamily_WhenInvoked_ReturnsCorrectListOfFontStyles()
        {
            // Arrange
            const string fontFamily = "Times New Roman";
            var fontTimesRegular = BuildContentFontPath("TimesNewRoman-Regular.ttf");
            var fontTimesBold = BuildContentFontPath("TimesNewRoman-Bold.ttf");
            var fontTimesItalic = BuildContentFontPath("TimesNewRoman-Italic.ttf");
            var fontTimesBoldItalic = BuildContentFontPath("TimesNewRoman-BoldItalic.ttf");

            var fontFiles = new[]
            {
                fontTimesRegular,
                fontTimesBold,
                fontTimesItalic,
                fontTimesBoldItalic,
            };

            MockAllFontFamilies(fontFiles, fontFamily);

            MockFontStyle(fontTimesRegular, FontStyle.Regular);
            MockFontStyle(fontTimesBold, FontStyle.Bold);
            MockFontStyle(fontTimesItalic, FontStyle.Italic);
            MockFontStyle(fontTimesBoldItalic, FontStyle.Bold | FontStyle.Italic);

            this.mockDirectory.Setup(m => m.GetFiles(this.fullContentFontDirPath, "*.ttf"))
                .Returns(() => fontFiles);

            var expected = new[]
            {
                new FontStats { FontFilePath = fontTimesRegular, FamilyName = fontFamily, Style = FontStyle.Regular },
                new FontStats { FontFilePath = fontTimesBold, FamilyName = fontFamily, Style = FontStyle.Bold },
                new FontStats { FontFilePath = fontTimesItalic, FamilyName = fontFamily, Style = FontStyle.Italic },
                new FontStats { FontFilePath = fontTimesBoldItalic, FamilyName = fontFamily, Style = FontStyle.Bold | FontStyle.Italic },
            };

            var service = CreateService();

            // Act
            var unused = service.GetContentStatsForFontFamily(fontFamily);
            var actual = service.GetContentStatsForFontFamily(fontFamily);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSystemStatsForFontFamily_WhenInvoked_ReturnsCorrectListOfFontStyles()
        {
            // Arrange
            const string fontFamily = "Times New Roman";
            var fontTimesRegular = BuildSystemFontPath("TimesNewRoman-Regular.ttf");
            var fontTimesBold = BuildSystemFontPath("TimesNewRoman-Bold.ttf");
            var fontTimesItalic = BuildSystemFontPath("TimesNewRoman-Italic.ttf");
            var fontTimesBoldItalic = BuildSystemFontPath("TimesNewRoman-BoldItalic.ttf");

            var fontFiles = new[]
            {
                fontTimesRegular,
                fontTimesBold,
                fontTimesItalic,
                fontTimesBoldItalic,
            };

            MockAllFontFamilies(fontFiles, fontFamily);

            MockFontStyle(fontTimesRegular, FontStyle.Regular);
            MockFontStyle(fontTimesBold, FontStyle.Bold);
            MockFontStyle(fontTimesItalic, FontStyle.Italic);
            MockFontStyle(fontTimesBoldItalic, FontStyle.Bold | FontStyle.Italic);

            this.mockDirectory.Setup(m => m.GetFiles(this.fullSystemFontDirPath, "*.ttf"))
                .Returns(() => fontFiles);

            var expected = new[]
            {
                new FontStats { FontFilePath = fontTimesRegular, FamilyName = fontFamily, Style = FontStyle.Regular },
                new FontStats { FontFilePath = fontTimesBold, FamilyName = fontFamily, Style = FontStyle.Bold },
                new FontStats { FontFilePath = fontTimesItalic, FamilyName = fontFamily, Style = FontStyle.Italic },
                new FontStats { FontFilePath = fontTimesBoldItalic, FamilyName = fontFamily, Style = FontStyle.Bold | FontStyle.Italic },
            };

            var service = CreateService();

            // Act
            var unused = service.GetSystemStatsForFontFamily(fontFamily);
            var actual = service.GetSystemStatsForFontFamily(fontFamily);

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        /// <summary>
        /// Creates a new service for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private FontStatsService CreateService() => new (
                this.mockFontService.Object,
                this.mockContentPathResolver.Object,
                this.mockSystemFontPathResolver.Object,
                this.mockDirectory.Object);

        private string BuildContentFontPath(string fileName) => $"{this.fullContentFontDirPath}{fileName}";

        private string BuildSystemFontPath(string fileName) => $"{this.fullSystemFontDirPath}{fileName}";

        private void MockFontFamilyName(string filePath, string familyName)
        {
            this.mockFontService.Setup(m => m.GetFamilyName(filePath, true))
                .Returns(familyName);
        }

        private void MockAllFontFamilies(IEnumerable<string> filePaths, string familyName)
        {
            foreach (var path in filePaths)
            {
                MockFontFamilyName(path, familyName);
            }
        }

        private void MockFontStyle(string filePath, FontStyle style)
        {
            this.mockFontService.Setup(m => m.GetFontStyle(filePath, true))
                .Returns(style);
        }
    }
}
