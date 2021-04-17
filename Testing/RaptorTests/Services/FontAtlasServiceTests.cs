// <copyright file="FontAtlasServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Services
{
    using System;
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

    /// <summary>
    /// Tests the <see cref="FontAtlasService"/> class.
    /// </summary>
    public unsafe class FontAtlasServiceTests : IDisposable
    {
        private const string FontFilePath = @"C:\temp\test-font.ttf";
        private const int GlyphWidth = 5;
        private const int GlyphHeight = 6;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
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
        };
        private readonly Mock<IFile> mockFile;
        private byte[]? bitmapBufferData;
        private GCHandle bitmapBufferDataHandle;
        private FT_FaceRec faceRec = default;
        private FT_GlyphSlotRec glyphSlotRec = default;
        private FT_SizeRec sizeRec = default;
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
            this.mockFreeTypeInvoker.Setup(m => m.FT_New_Face(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => this.facePtr);

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
        public void CreateFontAtlas_WhenInvoked_LoadsFontFace()
        {
            // Arrange
            var fontFilePath = $@"C:\temp\test-font.ttf";
            var service = CreateService();

            // Act
            var (actualAtlasTexture, atlasData) = service.CreateFontAtlas(fontFilePath, It.IsAny<int>());

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_New_Face(this.freeTypeLibPtr, fontFilePath, 0), Times.Once());
        }

        [Theory]
        [InlineData(new[] { '1', })]
        [InlineData(new[] { '2', '3', })]
        [InlineData(new[] { '4', '5', '6' })]
        [InlineData(new[] { '7', '8', '9', '0' })]
        public void CreateFontAtlas_WhenInvoked_CalculatesAtlasMetrics(char[] glyphChars)
        {
            // Arrange
            var fontFilePath = $@"C:\temp\test-font.ttf";
            var service = CreateService();
            service.SetAvailableCharacters(glyphChars);

            // Act
            var (actualAtlasTexture, atlasData) = service.CreateFontAtlas(fontFilePath, It.IsAny<int>());

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_New_Face(this.freeTypeLibPtr, fontFilePath, 0), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_GetsGlyphIndices()
        {
            // Arrange
            var fontFilePath = $@"C:\temp\test-font.ttf";
            var glyphChars = new char[] { 'a', 'b' };
            var service = CreateService();
            service.SetAvailableCharacters(glyphChars);

            // Act
            var (actualAtlasTexture, atlasData) = service.CreateFontAtlas(fontFilePath, It.IsAny<int>());

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Char_Index(this.facePtr, 'a'), Times.Once());
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Char_Index(this.facePtr, 'b'), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_CreatesGlyphMetrics()
        {
            // Arrange
            var fontFilePath = $@"C:\temp\test-font.ttf";
            var glyphChars = new char[] { 'a', 'b' };
            this.mockFreeTypeInvoker.Setup(m => m.FT_Get_Char_Index(It.IsAny<IntPtr>(), 97))
                .Returns(11u);
            this.mockFreeTypeInvoker.Setup(m => m.FT_Get_Char_Index(It.IsAny<IntPtr>(), 98))
                .Returns(22u);

            var service = CreateService();
            service.SetAvailableCharacters(glyphChars);

            // Act
            var (actualAtlasTexture, atlasData) = service.CreateFontAtlas(fontFilePath, It.IsAny<int>());

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Load_Glyph(It.IsAny<IntPtr>(), 11, FT.FT_LOAD_BITMAP_METRICS_ONLY), Times.Once());
            this.mockFreeTypeInvoker.Verify(m => m.FT_Load_Glyph(It.IsAny<IntPtr>(), 22, FT.FT_LOAD_BITMAP_METRICS_ONLY), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenSettingCharacterSizeWithNullMainMonitor_ThrowsException()
        {
            // Arrange
            this.mockMonitorService.SetupGet(p => p.MainMonitor).Returns(() => null);

            var service = CreateService();

            // Act & Assert
            Assert.ThrowsWithMessage<SystemDisplayException>(() =>
            {
                service.CreateFontAtlas(FontFilePath, It.IsAny<int>());
            }, "The main system display must not be null.");
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
            this.mockFreeTypeInvoker.Verify(m => m.FT_Set_Char_Size(
                this.facePtr,
                sizeInPointsPtr,
                sizeInPointsPtr,
                96,
                96), Times.Once());
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
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Char_Index(this.facePtr, It.IsAny<uint>()), Times.Exactly(totalGlyphs));
            this.mockFreeTypeInvoker.Verify(m => m.FT_Load_Glyph(this.facePtr, It.IsAny<uint>(), FT.FT_LOAD_RENDER), Times.Exactly(totalGlyphs - 1));
            this.mockImageService.Verify(m => m.Draw(It.IsAny<ImageData>(), It.IsAny<ImageData>(), It.IsAny<Point>()), Times.Exactly(totalGlyphs - 1));

            Assert.Equals(16, actualImage.Width, $"The resulting font atlas texture width is invalid.");
            Assert.Equals(36, actualImage.Height, $"The resulting font atlas texture height is invalid.");
            Assert.Equals(576, actualImage.Pixels.Length, $"The number of atlas image pixels is invalid.");
            Assert.Equal(this.glyphChars.Length + 1, actualData.Length);
        }

        [Fact]
        public void CreateFontAtlas_WithIssueCreatingFontFace_ThrowsException()
        {
            // Arrange
            this.mockFreeTypeInvoker.Setup(m => m.FT_New_Face(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(IntPtr.Zero);

            var service = CreateService();

            // Act & Assert
            Assert.ThrowsWithMessage<LoadFontException>(() =>
            {
                service.CreateFontAtlas(FontFilePath, 12);
            }, "An invalid pointer value of zero was returned when creating a new font face.");
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
            this.mockFreeTypeInvoker.Verify(m => m.FT_Done_Face(this.facePtr), Times.Once());
            //this.mockFreeTypeInvoker.Verify(m => m.FT_Done_Glyph(TestHelpers.ToIntPtr(ref this.glyphSlotRec)), Times.Once());
            this.mockFreeTypeInvoker.Verify(m => m.FT_Done_FreeType(this.freeTypeLibPtr), Times.Once());
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
        /// <returns>The size metric data to use for testing.</returns>
        [ExcludeFromCodeCoverage]
        private static FT_Size_Metrics CreateSizeMetrics(int width, int height)
        {
            FT_Size_Metrics sizeMetrics = default;
            sizeMetrics.ascender = new IntPtr(width << 6);
            sizeMetrics.descender = new IntPtr(height << 6);

            return sizeMetrics;
        }

        /// <summary>
        /// Creates native <see cref="FT_Glyph_Metrics"/> data for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the glyph.</param>
        /// <param name="height">The height of the glyph.</param>
        /// <param name="horiAdvance">The horizontal advance of the glyph.</param>
        /// <param name="horiBearingX">The X coordinate of the horizontal bearing.</param>
        /// <param name="horiBearingY">The Y coordinate of the horizontal bearing.</param>
        /// <returns>The glyph metrics used for testing.</returns>
        private static FT_Glyph_Metrics CreateGlypMetrics(int width, int height, int horiAdvance, int horiBearingX, int horiBearingY)
        {
            FT_Glyph_Metrics glyphMetrics = default;
            glyphMetrics.width = new IntPtr(width << 6);
            glyphMetrics.height = new IntPtr(height << 6);
            glyphMetrics.horiAdvance = new IntPtr(horiAdvance << 6);
            glyphMetrics.horiBearingX = new IntPtr(horiBearingX << 6);
            glyphMetrics.horiBearingY = new IntPtr(horiBearingY << 6);

            return glyphMetrics;
        }

        /// <summary>
        /// Creats all of the glyph data for testing purposes.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupTestGlyphData()
        {
            var faceBitmap = CreateGlyphBMPData(GlyphWidth, GlyphHeight);

            this.glyphSlotRec.bitmap = faceBitmap;

            // Setup the size metric data
            this.sizeRec.metrics = CreateSizeMetrics(8, 5);

            this.faceRec.size = TestHelpers.ToUnsafePointer(ref this.sizeRec);

            // Setup the glyph metrics
            this.glyphSlotRec.metrics = CreateGlypMetrics(5, 6, 13, 15, 7);

            this.faceRec.glyph = TestHelpers.ToUnsafePointer(ref this.glyphSlotRec);

            this.facePtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.faceRec));
            Marshal.StructureToPtr(this.faceRec, this.facePtr, false);
        }

        /// <summary>
        /// Creates native <see cref="FT_Bitmap"/> data for the purpose of testing.
        /// </summary>
        /// <param name="width">The width of the glyph bitmap.</param>
        /// <param name="height">The height of the glyph bitmap.</param>
        /// <returns>The glyph bitmap data to use for testing.</returns>
        [ExcludeFromCodeCoverage]
        private FT_Bitmap CreateGlyphBMPData(uint width, uint height)
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
            var faceBitmap = default(FT_Bitmap);
            faceBitmap.width = width;
            faceBitmap.rows = height;
            faceBitmap.buffer = this.bitmapBufferDataHandle.AddrOfPinnedObject();

            return faceBitmap;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FontAtlasService"/> for the purposes of testing.
        /// </summary>
        /// <returns>An instance to use for testing.</returns>
        private FontAtlasService CreateService(bool setGlyphChars = true)
        {
            var result = new FontAtlasService(
                this.mockFreeTypeInvoker.Object,
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
