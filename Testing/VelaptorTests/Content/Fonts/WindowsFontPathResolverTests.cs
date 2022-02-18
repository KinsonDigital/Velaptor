// <copyright file="WindowsFontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using System;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content.Fonts;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="WindowsFontPathResolver"/> class.
    /// </summary>
    public class WindowsFontPathResolverTests
    {
        private readonly Mock<IDirectory> mockDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFontPathResolverTests"/> class.
        /// </summary>
        public WindowsFontPathResolverTests() => this.mockDirectory = new Mock<IDirectory>();

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullDirectoryParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new WindowsFontPathResolver(null);
            }, "The parameter must not be null. (Parameter 'directory')");
        }
        #endregion

        #region Prop tests
        [Fact]
        public void RootDirectoryPath_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var actual = resolver.RootDirectoryPath;

            // Assert
            Assert.Equal(@"C:\Windows\", actual);
        }

        [Fact]
        public void ContentDirectoryName_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal("Fonts", actual);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ResolveFilePath_WhenNullOrEmpty_ThrowsException(string contentName)
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                resolver.ResolveFilePath(contentName);
            }, "The string parameter must not be null or empty. (Parameter 'contentName')");
        }

        [Fact]
        public void ResolveFilePath_WhenContentNameEndsWithDirectorySeparator_ThrowsException()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
            {
                resolver.ResolveFilePath(@"test-content\");
            }, @"The 'test-content\' cannot end with a folder.  It must end with a file name with or without the extension. (Parameter 'contentName')");
        }

        [Fact]
        public void ResolveFilePath_WhenInvoking_ReturnsCorrectResolvedFilePath()
        {
            // Arrange
            const string rootDir = @"C:\Windows\";
            const string contentDirName = "Fonts";
            const string contentName = "test-content";
            const string extension = ".ttf";
            var fullContentDirPath = $@"{rootDir}{contentDirName}\";
            var expected = $@"{fullContentDirPath}{contentName}{extension}";

            var files = new[]
            {
                $"{fullContentDirPath}other-file.txt",
                expected,
            };

            this.mockDirectory.Setup(m => m.GetFiles(fullContentDirPath, $"*{extension}"))
                .Returns(files);

            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResolveDirPath_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            const string expected = @"C:\Windows\Fonts\";
            var resolver = CreateResolver();

            // Act
            var actual = resolver.ResolveDirPath();

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="WindowsFontPathResolver"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private WindowsFontPathResolver CreateResolver() => new WindowsFontPathResolver(this.mockDirectory.Object);
    }
}
