// <copyright file="FontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using System;
    using System.IO.Abstractions;
    using System.Runtime.InteropServices;
    using Moq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontPathResolver"/> class.
    /// </summary>
    public class FontPathResolverTests
    {
        private const string OnlyWindowsSupportMessage = "Currently loading system fonts is only supported on Windows.";
        private const string RootDirInContentLocation = @"C:\app-dir\Content\";
        private const string ContentDirNameInContentLocation = "ContentFonts";
        private const string RootDirInWindowsLocation = @"C:\Windows\";
        private const string ContentDirNameInWindowsLocation = "WinFonts";
        private readonly Mock<IPathResolver> mockContentPathResolver;
        private readonly Mock<IPathResolver> mockWindowsPathResolver;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IDirectory> mockDirectory;
        private readonly Mock<IPlatform> mockPlatform;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontPathResolverTests"/> class.
        /// </summary>
        public FontPathResolverTests()
        {
            this.mockContentPathResolver = new Mock<IPathResolver>();
            this.mockContentPathResolver.Setup(p => p.RootDirectoryPath).Returns(RootDirInContentLocation);
            this.mockContentPathResolver.Setup(p => p.ContentDirectoryName).Returns(ContentDirNameInContentLocation);

            this.mockWindowsPathResolver = new Mock<IPathResolver>();
            this.mockWindowsPathResolver.Setup(p => p.RootDirectoryPath).Returns(RootDirInWindowsLocation);
            this.mockWindowsPathResolver.Setup(p => p.ContentDirectoryName).Returns(ContentDirNameInWindowsLocation);

            this.mockFile = new Mock<IFile>();
            this.mockDirectory = new Mock<IDirectory>();

            this.mockPlatform = new Mock<IPlatform>();
            this.mockPlatform.SetupGet(p => p.CurrentPlatform)
                .Returns(() => throw new Exception("Platform mock not setup for test."));
        }

        #region Prop Tests
        [Fact]
        public void RootDirectory_WhenFontExistsInContentDirectory_ReturnsContentRootDirectoryForContentLocation()
        {
            // Arrange
            MockWindowsPlatform();
            this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(true);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.RootDirectoryPath;

            // Assert
            Assert.Equal(RootDirInContentLocation, actual);
        }

        [Fact]
        public void RootDirectory_WhenFontDoesNotExistInContentDirectory_ReturnsWindowsRootDirectoryForWindowsLocation()
        {
            // Arrange
            MockWindowsPlatform();
            this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(false);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.RootDirectoryPath;

            // Assert
            Assert.Equal(RootDirInWindowsLocation, actual);
        }

        [Fact]
        public void RootDirectory_WhenNotOnWindowsAndFontDoesNotExistInContentDirectory_ThrowsException()
        {
            // Arrange
            MockOSXPlatform();
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<NotImplementedException>(() =>
            {
                var unused = resolver.RootDirectoryPath;
            }, OnlyWindowsSupportMessage);
        }

        [Fact]
        public void ContentDirectoryName_WhenFontExistsInContentDirectory_ReturnsContentDirectoryNameForContentLocation()
        {
            // Arrange
            MockWindowsPlatform();
            this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(true);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal(ContentDirNameInContentLocation, actual);
        }

        [Fact]
        public void ContentDirectoryName_WhenFontDoesNotExistInContentDirectory_ReturnsWindowsDirectoryNameForContentLocation()
        {
            // Arrange
            MockWindowsPlatform();
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal(ContentDirNameInWindowsLocation, actual);
        }

        [Fact]
        public void ContentDirectoryName_WhenNotOnWindowsAndFontDoesNotExistInContentDirectory_ThrowsException()
        {
            // Arrange
            MockOSXPlatform();
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<NotImplementedException>(() =>
            {
                var unused = resolver.ContentDirectoryName;
            }, OnlyWindowsSupportMessage);
        }
        #endregion

        #region Public Methods
        [Fact]
        public void ResolveFilePath_IfNotWindows_ThrowsException()
        {
            // Arrange
            MockOSXPlatform();
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<NotImplementedException>(() =>
            {
                resolver.ResolveFilePath("test-content");
            }, OnlyWindowsSupportMessage);
        }

        [Fact]
        public void ResolveFilePath_WhenFontExistsInContentDirectory_ReturnsCorrectResult()
        {
            // Arrange
            const string expected = @"C:\app-dir\test-content.ttf";
            MockWindowsPlatform();
            this.mockFile.Setup(m => m.Exists(expected)).Returns(true);
            this.mockContentPathResolver.Setup(m => m.ResolveFilePath("test-content"))
                .Returns(expected);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveFilePath("test-content");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResolveFilePath_WhenFontDoesNotExistInContentDirectory_ReturnsCorrectResult()
        {
            // Arrange
            const string expected = @"C:\Windows\Fonts\test-content.ttf";
            MockWindowsPlatform();
            this.mockFile.Setup(m => m.Exists(expected)).Returns(false);
            this.mockWindowsPathResolver.Setup(m => m.ResolveFilePath("test-content"))
                .Returns(expected);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveFilePath("test-content");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResolveDirPath_IfNotWindows_ThrowsException()
        {
            // Arrange
            MockOSXPlatform();
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<NotImplementedException>(() =>
            {
                resolver.ResolveDirPath();
            }, OnlyWindowsSupportMessage);
        }

        [Fact]
        public void ResolveDirPath_WhenFontExistsInContentDirectory_ReturnsCorrectResult()
        {
            // Arrange
            const string expected = @"C:\app-dir\";
            MockWindowsPlatform();
            this.mockDirectory.Setup(m => m.Exists(expected)).Returns(true);
            this.mockContentPathResolver.Setup(m => m.ResolveDirPath())
                .Returns(expected);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveDirPath();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResolveDirPath_WhenFontDoesNotExistInContentDirectory_ReturnsCorrectResult()
        {
            // Arrange
            const string expected = @"C:\Windows\Fonts\";
            MockWindowsPlatform();
            this.mockDirectory.Setup(m => m.Exists(expected)).Returns(false);
            this.mockWindowsPathResolver.Setup(m => m.ResolveDirPath())
                .Returns(expected);
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveDirPath();

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="FontPathResolver"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private FontPathResolver CreateResolver()
        {
            return new FontPathResolver(
                this.mockContentPathResolver.Object,
                this.mockWindowsPathResolver.Object,
                this.mockFile.Object,
                this.mockDirectory.Object,
                this.mockPlatform.Object);
        }

        /// <summary>
        /// Mocks the platform to be Windows.
        /// </summary>
        private void MockWindowsPlatform() =>
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

        /// <summary>
        /// Mocks the platform to be Mac OSX.
        /// </summary>
        private void MockOSXPlatform() =>
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.OSX);
    }
}
