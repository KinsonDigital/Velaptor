// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO;
    using System.Reflection;
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
        private readonly string baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly ContentLoader contentLoader;
        private readonly Mock<ILoader<ITexture>> mockTextureLoader;
        private readonly Mock<ILoader<AtlasRegionRectangle[]>> mockAtlasDataLoader;
        private readonly Mock<ILoader<ISound>> mockSoundLoader;

        public ContentLoaderTests()
        {
            this.mockTextureLoader = new Mock<ILoader<ITexture>>();
            this.mockAtlasDataLoader = new Mock<ILoader<AtlasRegionRectangle[]>>();
            this.mockSoundLoader = new Mock<ILoader<ISound>>();

            this.contentLoader = new ContentLoader(this.mockTextureLoader.Object,
                                                   this.mockAtlasDataLoader.Object,
                                                   this.mockSoundLoader.Object);
        }

        #region Prop Tests
        [Theory]
        [InlineData(@"C:\temp\", @"C:\temp\Content\")]
        [InlineData(@"C:\temp", @"C:\temp\Content\")]
        [InlineData(null, "")]
        public void ContentRootDirectory_WhenSettingValue_ReturnsCorrectValue(string rootDirectory, string expected)
        {
            // Act
            // If the root is null, use the base directory for the expected
            if (rootDirectory is null)
                expected = $@"{this.baseDir}Content\";

#pragma warning disable CS8601 // Possible null reference assignment.
            this.contentLoader.ContentRootDirectory = rootDirectory;
#pragma warning restore CS8601 // Possible null reference assignment.
            var actual = this.contentLoader.ContentRootDirectory;

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadTexture_WhenInvoked_LoadsTexture()
        {
            // Act
            this.contentLoader.LoadTexture("test-texture.png");

            // Assert
            this.mockTextureLoader.Verify(m => m.Load(@$"{this.baseDir}Content\Graphics\test-texture.png"), Times.Once());
        }

        [Fact]
        public void LoadAtlasData_WhenInvoked_LoadsAtlasData()
        {
            // Act
            this.contentLoader.LoadAtlasData("test-atlas.json");

            // Assert
            this.mockAtlasDataLoader.Verify(m => m.Load($@"{this.baseDir}Content\Graphics\test-atlas.json"), Times.Once());
        }
        #endregion
    }
}
