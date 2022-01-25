// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureLoader"/> class.
    /// </summary>
    public class TextureLoaderTests
    {
        private const string TextureExtension = ".png";
        private const string TextureDirPath = @"C:\textures\";
        private const string TextureFileName = "test-texture";
        private readonly string textureFilePath = $"{TextureDirPath}{TextureFileName}{TextureExtension}";
        private readonly Mock<IDisposableItemCache<string, ITexture>> mockTextureCache;
        private readonly Mock<IPathResolver> mockTexturePathResolver;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<IObservable<bool>> mockShutDownObservable;
        private readonly Mock<IDisposable> mockShutDownUnsubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
        /// </summary>
        public TextureLoaderTests()
        {
            this.mockTexturePathResolver = new Mock<IPathResolver>();
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(TextureFileName))
                .Returns(this.textureFilePath);

            this.mockTextureCache = new Mock<IDisposableItemCache<string, ITexture>>();

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.Exists(this.textureFilePath)).Returns(true);

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetExtension(this.textureFilePath)).Returns(TextureExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}")).Returns(TextureFileName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}{TextureExtension}")).Returns(TextureFileName);

            this.mockShutDownUnsubscriber = new Mock<IDisposable>();
            this.mockShutDownObservable = new Mock<IObservable<bool>>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullTextureCacheParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureLoader(
                    null,
                    this.mockTexturePathResolver.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'textureCache')");
        }

        [Fact]
        public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureLoader(
                    this.mockTextureCache.Object,
                    null,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'texturePathResolver')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureLoader(
                    this.mockTextureCache.Object,
                    this.mockTexturePathResolver.Object,
                    null,
                    this.mockPath.Object,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureLoader(
                    this.mockTextureCache.Object,
                    this.mockTexturePathResolver.Object,
                    this.mockFile.Object,
                    null,
                    this.mockShutDownObservable.Object);
            }, "The parameter must not be null. (Parameter 'path')");
        }

        [Fact]
        public void Ctor_WithNullShutDownObservableParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new TextureLoader(
                    this.mockTextureCache.Object,
                    this.mockTexturePathResolver.Object,
                    this.mockFile.Object,
                    this.mockPath.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'shutDownObservable')");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Load_WhenLoadingContentWithFullPath_LoadsTexture()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            this.mockTextureCache.Setup(m => m.GetItem(this.textureFilePath))
                .Returns(mockTexture.Object);

            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.textureFilePath);

            // Assert
            Assert.NotNull(actual);
            Assert.Same(mockTexture.Object, actual);
        }

        [Theory]
        [InlineData(TextureFileName, "")]
        [InlineData(TextureFileName, ".txt")]
        public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();

            this.mockTextureCache.Setup(m => m.GetItem(this.textureFilePath))
                .Returns(mockTexture.Object);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}")).Returns(contentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}")).Returns(contentName);

            var loader = CreateLoader();

            // Act
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            Assert.NotNull(actual);
            Assert.Same(mockTexture.Object, actual);
        }

        [Fact]
        public void Load_WhenFileDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                loader.Load(this.textureFilePath);
            }, $"The texture file '{this.textureFilePath}' does not exist.");
        }

        [Fact]
        public void Load_WhenFileExistsButIsNotATextureContentFile_ThrowsException()
        {
            // Arrange
            const string extension = ".txt";
            var filePathWithInvalidExtension = $"{TextureDirPath}{TextureFileName}{extension}";
            this.mockFile.Setup(m => m.Exists(filePathWithInvalidExtension)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(filePathWithInvalidExtension)).Returns(extension);

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadTextureException>(() =>
            {
                loader.Load(filePathWithInvalidExtension);
            }, $"The file '{filePathWithInvalidExtension}' must be a texture file with the extension '{TextureExtension}'.");
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsCachedTextures()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.textureFilePath);

            // Assert
            this.mockTextureCache.Verify(m => m.Unload(this.textureFilePath), Times.Once);
        }

        [Fact]
        public void WithShutDownNotification_DisposesOfLoader()
        {
            // Arrange
            IObserver<bool>? shutDownObserver = null;
            this.mockShutDownObservable.Setup(m => m.Subscribe(It.IsAny<IObserver<bool>>()))
                .Returns(this.mockShutDownUnsubscriber.Object)
                .Callback<IObserver<bool>>(observer =>
                {
                    if (observer is null)
                    {
                        Assert.True(false, "Shutdown observable subscription failed.  Observer is null.");
                    }

                    shutDownObserver = observer;
                });

            CreateLoader();

            // Act
            shutDownObserver?.OnNext(true);
            shutDownObserver?.OnNext(true);

            // Assert
            this.mockTextureCache.Verify(m => m.Dispose(), Times.Once);
            this.mockShutDownUnsubscriber.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureLoader CreateLoader()
            => new (this.mockTextureCache.Object,
             this.mockTexturePathResolver.Object,
             this.mockFile.Object,
             this.mockPath.Object,
             this.mockShutDownObservable.Object);
    }
}
