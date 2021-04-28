// <copyright file="FontAtlasServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Services
{
#pragma warning disable IDE0001 // Name can be simplified
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.IO.Abstractions;
    using System.Runtime.InteropServices;
    using FreeTypeSharp.Native;
    using Moq;
    using Raptor;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.Hardware;
    using Raptor.NativeInterop;
    using Raptor.Services;
    using RaptorTests.Helpers;
    using Xunit;
    using Assert = RaptorTests.Helpers.AssertExtensions;
#pragma warning restore IDE0001 // Name can be simplified

    /// <summary>
    /// Tests the <see cref="FontAtlasService"/> class.
    /// </summary>
    public unsafe class FontAtlasServiceTests : IDisposable
    {
        private const string FontFilePath = @"C:\temp\test-font.ttf";
        private const int GlyphWidth = 5;
        private const int GlyphHeight = 6;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IFreeTypeExtensions> mockFreeTypeExtensions;
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<ISystemMonitorService> mockMonitorService;
        private readonly Mock<IPlatform> mockPlatform;
        private readonly IntPtr freeTypeLibPtr = new IntPtr(1234);
        private readonly char[] glyphChars = new[]
        { // This represents how it would be layed out in the atlas
            'h', 'e',
            'l', 'o',
            'w', 'r',
            'd', ' ',
            '□',
        };
        private readonly Dictionary<char, uint> glyphIndices = new Dictionary<char, uint>()
        {
            { 'h', 0 },
            { 'e', 1 },
            { 'l', 2 },
            { 'o', 3 },
            { 'w', 4 },
            { 'r', 5 },
            { 'd', 6 },
            { ' ', 7 },
            { '□', 8 },
        };
        private readonly Mock<IFile> mockFile;
        private byte[]? bitmapBufferData;
        private GCHandle bitmapBufferDataHandle;
        private FT_Bitmap faceBitmap = default;
        private FT_FaceRec faceRec = default;
        private FT_GlyphSlotRec glyphSlotRec = default;
        private FT_SizeRec sizeRec = default;
        private FT_Size_Metrics sizeMetrics = default;
        private FT_Glyph_Metrics glyphMetrics = default;
        private IntPtr facePtr;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAtlasServiceTests"/> class.
        /// </summary>
        public FontAtlasServiceTests()
        {
            TestHelpers.SetupTestResultDirPath();

            SetupTestGlyphData();

            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();
            this.mockFreeTypeInvoker.Setup(m => m.FT_Init_FreeType()).Returns(this.freeTypeLibPtr);

            this.mockFreeTypeExtensions = new Mock<IFreeTypeExtensions>();
            this.mockFreeTypeExtensions.Setup(m => m.CreateFontFace(this.freeTypeLibPtr, FontFilePath))
                .Returns(() => this.facePtr);

            this.mockFreeTypeExtensions.Setup(m => m.GetGlyphIndices(this.facePtr, this.glyphChars))
                .Returns(() => this.glyphIndices);

            this.mockFreeTypeExtensions.Setup(m => m.CreateGlyphMetrics(
                this.facePtr,
                this.mockFreeTypeExtensions.Object.GetGlyphIndices(this.facePtr, this.glyphChars)))
                    .Returns(() =>
                    {
                        var result = new Dictionary<char, GlyphMetrics>();

                        for (var i = 0; i < this.glyphChars.Length; i++)
                        {
                            GlyphMetrics newMetric = default;

                            newMetric.Glyph = this.glyphChars[i];
                            newMetric.CharIndex = this.glyphIndices[this.glyphChars[i]];
                            newMetric.Ascender = (i + 1) * 10;
                            newMetric.Descender = (i + 2) * 10;
                            newMetric.GlyphWidth = (i + 3) * 10;
                            newMetric.GlyphHeight = (i + 4) * 10;
                            newMetric.HoriBearingX = (i + 5) * 10;
                            newMetric.HoriBearingY = (i + 6) * 10;
                            newMetric.HorizontalAdvance = (i + 7) * 10;
                            newMetric.XMin = (i + 8) * 10;
                            newMetric.XMax = (i + 9) * 10;
                            newMetric.YMin = (i + 10) * 10;
                            newMetric.YMax = (i + 11) * 10;

                            result.Add(this.glyphChars[i], newMetric);
                        }

                        return result;
                    });

            this.mockFreeTypeExtensions.Setup(m => m.CreateGlyphImage(this.facePtr, It.IsAny<char>(), It.IsAny<uint>()))
                .Returns(() =>
                {
                    return (new byte[]
                    {
                        255, 255,
                    }, 1, 2);
                });

            this.mockImageService = new Mock<IImageService>();

            this.mockMonitorService = new Mock<ISystemMonitorService>();
            this.mockMonitorService.SetupGet(p => p.MainMonitor)
                .Returns(() =>
                {
                    return new SystemMonitor(this.mockPlatform.Object)
                    {
                        HorizontalScale = 1,
                        VerticalScale = 1,
                    };
                });

            this.mockPlatform = new Mock<IPlatform>();
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.Exists(FontFilePath)).Returns(true);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_InitializesFreeType()
        {
            // Act
            var service = CreateService();

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Init_FreeType(), Times.Once());
        }
        #endregion

        #region Method Tests
        [Fact]
        public void CreateFontAtlas_WithUnsetGlyphCharacters_ThrowsException()
        {
            // Arrange
            var service = CreateService(false);

            // Act & Assert
            Assert.ThrowsWithMessage<InvalidOperationException>(() =>
            {
                service.CreateFontAtlas(It.IsAny<string>(), It.IsAny<int>());
            }, "The available glyph characters must be set first before creating a font texture atlas.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateFontAtlas_WithNullOrEmptyFilePath_ThrowsException(string fontFilePath)
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            Assert.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                service.CreateFontAtlas(fontFilePath, It.IsAny<int>());
            }, "The font file path argument must not be null. (Parameter 'fontFilePath')");
        }

        [Fact]
        public void CreateFontAtlas_WhenFontFilePathDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            var service = CreateService();

            // Act & Assert
            Assert.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                service.CreateFontAtlas(FontFilePath, It.IsAny<int>());
            }, $"The file '{FontFilePath}' does not exist.");
        }

        [Fact]
        public void CreateFontAtlas_WhenSettingCharacterSizeWithNullMainMonitor_ThrowsException()
        {
            // Arrange
            this.mockMonitorService.SetupGet(p => p.MainMonitor).Returns(() => null);

            var service = CreateService();

            // Act & Assert
            Assert.ThrowsWithMessage<SystemMonitorException>(() =>
            {
                service.CreateFontAtlas(FontFilePath, It.IsAny<int>());
            }, "The main system monitor must not be null.");
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_SetsCharacterSize()
        {
            // Arrange
            var fontSize = 12;
            var sizeInPointsPtr = (IntPtr)(fontSize << 6);

            var service = CreateService();

            // Act
            service.CreateFontAtlas(FontFilePath, fontSize);

            // Assert
            this.mockFreeTypeExtensions.Verify(m => m.SetCharacterSize(this.facePtr, 12, 96, 96), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_CreatesAllGlyphImages()
        {
            // Arrange
            this.mockImageService.Setup(m => m.Draw(It.IsAny<ImageData>(), It.IsAny<ImageData>(), It.IsAny<Point>()))
                .Returns<ImageData, ImageData, Point>((src, dest, location) =>
                {
                    return TestHelpers.Draw(src, dest, location);
                });

            // This is to account for the extra '□' that is used to
            // render something to the screen when a character/glyph
            // does not exist as part of the glyphs to render
            var totalGlyphs = this.glyphChars.Length + 1;
            var service = CreateService();

            // Act
            var (actualImage, actualData) = service.CreateFontAtlas(FontFilePath, 12);

            // Save the the results
            TestHelpers.SaveImageForTest(actualImage);

            // Assert
            this.mockFreeTypeExtensions.Verify(
                m => m.CreateGlyphImage(this.facePtr, It.IsAny<char>(), It.IsAny<uint>()), Times.Exactly(8));
        }

        [Fact]
        public void Dispose_WhenInvoked_ProperlyDisposesOfFreeType()
        {
            // TODO: This needs to be figured out.  The Dispose() method has some code that is doing
            // some casting from IntPtr to unsafe pointers and it is causing issues

            // Arrange
            var service = CreateService();
            service.CreateFontAtlas(FontFilePath, It.IsAny<int>());

            // Act
            service.Dispose();
            service.Dispose();

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            /*NOTE:
             * The purpose of 'bitmapBufferDataHandle' is to create a handle to
             * the byte data that represents the bitmap data of the glyph.  It is
             * pinned so the GC cannot collect it.  This is necessary so there
             * is no way for the GC to clean up the data "before" it is used
             * in the FontAtlasService which would cause issues while the test is running.
             */
            if (this.bitmapBufferDataHandle.IsAllocated is false)
            {
                return;
            }

            // This is required to clean up the data manually because it is pinned in place.
            // If we did not do this, then we would have a memory leak.
            this.bitmapBufferDataHandle.Free();

            // Clean up the fact data
            Marshal.FreeHGlobal(this.facePtr);
        }

        /// <summary>
        /// Creates native <see cref="FT_Size_Metrics"/> data for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the size.</param>
        /// <param name="height">The height of the size.</param>
        [ExcludeFromCodeCoverage]
        private void CreateSizeMetrics(int width, int height)
        {
            this.sizeMetrics.ascender = new IntPtr(width << 6);
            this.sizeMetrics.descender = new IntPtr(height << 6);
        }

        /// <summary>
        /// Creates native <see cref="FT_Glyph_Metrics"/> data for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the glyph.</param>
        /// <param name="height">The height of the glyph.</param>
        /// <param name="horiAdvance">The horizontal advance of the glyph.</param>
        /// <param name="horiBearingX">The X coordinate of the horizontal bearing.</param>
        /// <param name="horiBearingY">The Y coordinate of the horizontal bearing.</param>
        private void CreateGlypMetrics(int width, int height, int horiAdvance, int horiBearingX, int horiBearingY)
        {
            this.glyphMetrics.width = new IntPtr(width << 6);
            this.glyphMetrics.height = new IntPtr(height << 6);
            this.glyphMetrics.horiAdvance = new IntPtr(horiAdvance << 6);
            this.glyphMetrics.horiBearingX = new IntPtr(horiBearingX << 6);
            this.glyphMetrics.horiBearingY = new IntPtr(horiBearingY << 6);
        }

        /// <summary>
        /// Creats all of the glyph data for testing purposes.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupTestGlyphData()
        {
            CreateGlyphBMPData(GlyphWidth, GlyphHeight);

            this.glyphSlotRec.bitmap = this.faceBitmap;

            // Setup the size metric data
            CreateSizeMetrics(8, 5);
            this.sizeRec.metrics = this.sizeMetrics;

            this.faceRec.size = TestHelpers.ToUnsafePointer(ref this.sizeRec);

            // Setup the glyph metrics
            CreateGlypMetrics(5, 6, 13, 15, 7);
            this.glyphSlotRec.metrics = this.glyphMetrics;

            this.faceRec.glyph = TestHelpers.ToUnsafePointer(ref this.glyphSlotRec);

            this.facePtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.faceRec));
            Marshal.StructureToPtr(this.faceRec, this.facePtr, false);
        }

        /// <summary>
        /// Creates native <see cref="FT_Bitmap"/> data for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the glyph bitmap.</param>
        /// <param name="height">The height of the glyph bitmap.</param>
        [ExcludeFromCodeCoverage]
        private void CreateGlyphBMPData(uint width, uint height)
        {
            this.bitmapBufferData = new byte[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var arrayIndex = (width * (height == 0 ? height : height - 1)) + x;

                    this.bitmapBufferData[arrayIndex] = 255;
                }
            }

            this.bitmapBufferDataHandle = GCHandle.Alloc(this.bitmapBufferData, GCHandleType.Pinned);

            // Setup the face data required to satisfy the test
            this.faceBitmap.width = width;
            this.faceBitmap.rows = height;
            this.faceBitmap.buffer = this.bitmapBufferDataHandle.AddrOfPinnedObject();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FontAtlasService"/> for the purposes of testing.
        /// </summary>
        /// <returns>An instance to use for testing.</returns>
        private FontAtlasService CreateService(bool setGlyphChars = true)
        {
            var result = new FontAtlasService(
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.mockImageService.Object,
                this.mockMonitorService.Object,
                this.mockFile.Object);

            if (setGlyphChars)
            {
                result.SetAvailableCharacters(this.glyphChars);
            }

            return result;
        }
    }
}
