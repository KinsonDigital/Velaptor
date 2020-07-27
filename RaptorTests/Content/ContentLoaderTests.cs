// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices.WindowsRuntime;
    using FileIO.Core;
    using Moq;
    using Raptor;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Graphics;
    using Xunit;

    /// <summary>
    /// Unit tests to test the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        private readonly string RootDir = @"C:\Content\";
        private readonly string GraphicsDirName = "Graphics";
        private readonly string SoundsDirName = "Sounds";
        private readonly string AtlasDirName = "Atlas";
        private readonly ContentLoader contentLoader;
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<AtlasRegionRectangle[]>> mockAtlasDataLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;
        private readonly Mock<IContentSource> mockContentSrc;
        private readonly Mock<IImageFile> mockImageFile;
        private readonly Mock<ITextFile> mockTextFile;

        public ContentLoaderTests()
        {
            this.mockContentSrc = new Mock<IContentSource>();
            this.mockContentSrc.SetupGet(p => p.ContentRootDirectory).Returns(this.RootDir);
            this.mockContentSrc.SetupGet(p => p.GraphicsDirectoryName).Returns(this.GraphicsDirName);
            this.mockContentSrc.SetupGet(p => p.SoundsDirectoryName).Returns(this.SoundsDirName);
            this.mockContentSrc.SetupGet(p => p.AtlasDirectoryName).Returns(this.AtlasDirName);

            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockAtlasDataLoader = new Mock<ILoader<AtlasRegionRectangle[]>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();
            this.mockImageFile = new Mock<IImageFile>();
            this.mockTextFile = new Mock<ITextFile>();

            this.contentLoader = new ContentLoader(this.mockContentSrc.Object,
                                                   this.mockTextureLoader.Object,
                                                   this.mockAtlasDataLoader.Object,
                                                   this.mockSoundLoader.Object,
                                                   this.mockImageFile.Object,
                                                   this.mockTextFile.Object);
        }

        #region Prop Tests
        #endregion

        #region Method Tests
        [Fact]
        public void LoadTexture_WhenInvoked_LoadsTexture()
        {
            // Act
            this.mockContentSrc.Setup(m => m.GetContentPath(ContentType.Graphics, "test-texture.png"))
                .Returns("test-path");
            this.contentLoader.LoadTexture("test-texture.png");

            // Assert
            this.mockContentSrc.Verify(m => m.GetContentPath(ContentType.Graphics, "test-texture.png"), Times.Once());
            this.mockTextureLoader.Verify(m => m.Load("test-path"), Times.Once());
        }

        [Fact]
        public void LoadAtlasData_WhenInvoked_LoadsAtlasData()
        {
            // Act
            this.mockContentSrc.Setup(m => m.GetContentPath(ContentType.Atlas, "test-atlas.json"))
                .Returns("test-path");
            this.contentLoader.LoadAtlasData("test-atlas.json");

            // Assert
            this.mockContentSrc.Verify(m => m.GetContentPath(ContentType.Atlas, "test-atlas.json"), Times.Once());
            this.mockAtlasDataLoader.Verify(m => m.Load("test-path"), Times.Once());
        }
        #endregion
    }
}
