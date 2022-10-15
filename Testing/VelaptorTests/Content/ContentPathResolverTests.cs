// <copyright file="ContentPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Velaptor.Content;
    using VelaptorTests.Fakes;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentPathResolver"/> class.
    /// </summary>
    public class ContentPathResolverTests
    {
        private const string ContentName = "test-content.png";
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .Replace('\\', '/');

        /// <summary>
        /// Gets test data for the <see cref="RootDirectory_WhenSettingValue_ReturnsCorrectResult"/> test.
        /// </summary>
        public static IEnumerable<object[]> ContentRootPaths =>
            new List<object[]>
            {
                new object[] { null, @$"{BaseDir}/Content" },
                new object[] { @"C:\base-content\", @"C:/base-content" },
                new object[] { @"C:\base-content", @"C:/base-content" },
            };

        #region Prop Tests
        [Theory]
        [MemberData(nameof(ContentRootPaths))]
        public void RootDirectoryPath_WhenSettingValue_ReturnsCorrectResult(string rootDirectory, string expected)
        {
            // Arrange
            // ReSharper disable UseObjectOrCollectionInitializer
            var resolver = new ContentPathResolverFake();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Act
            resolver.RootDirectoryPath = rootDirectory;
            var actual = resolver.RootDirectoryPath;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FileDirectoryName_WhenSettingWithDirectoryPath_CorrectlySetsResult()
        {
            // Arrange
            // ReSharper disable UseObjectOrCollectionInitializer
            var resolver = new ContentPathResolverFake();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Act
            resolver.ContentDirectoryName = @"C:\temp\test-dir-name";
            var actual = resolver.ContentDirectoryName;

            // Assert
            Assert.Equal("test-dir-name", actual);
        }

        [Fact]
        public void ContentDirectoryName_WhenValueIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var resolver = new ContentPathResolverFake();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                resolver.ContentDirectoryName = null;
            }, "The 'ContentDirectoryName' must not be null or empty.");
        }

        [Theory]
        [InlineData("test-content.png")]
        public void ResolveFilePath_WhenInvoked_ResolvesContentFilePath(string contentName)
        {
            // Arrange
            var resolver = new ContentPathResolverFake();

            // Act
            var actual = resolver.ResolveFilePath(contentName);

            // Assert
            Assert.Equal(ContentName, actual);
        }

        [Fact]
        public void ResolveDirPath_WhenInvoked_ResolvesContentDirPath()
        {
            // Arrange
            // ReSharper disable UseObjectOrCollectionInitializer
            var resolver = new ContentPathResolverFake();
            // ReSharper restore UseObjectOrCollectionInitializer
            resolver.RootDirectoryPath = @"C:\temp\my-content\";
            resolver.ContentDirectoryName = "test-content";

            // Act
            var actual = resolver.ResolveDirPath();

            // Assert
            Assert.Equal(@"C:\temp\my-content\test-content\", actual);
        }

        [Fact]
        public void ResolveFilePath_WhenContentNameIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var resolver = new ContentPathResolverFake();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                resolver.ResolveFilePath(null);
            }, "The string parameter must not be null or empty. (Parameter 'contentName')");
        }

        [Fact]
        public void ResolveFilePath_WhenContentNameEndsWithDirSeparator_ThrowsException()
        {
            // Arrange
            var resolver = new ContentPathResolverFake();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
            {
                resolver.ResolveFilePath($@"{ContentName}\");
            }, $@"The '{ContentName}\' cannot end with a folder.  It must end with a file name with or without the extension. (Parameter 'contentName')");
        }
        #endregion
    }
}
