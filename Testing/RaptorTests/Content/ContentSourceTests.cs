// <copyright file="ContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace RaptorTests.Content
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;
    using System.Text;
    using Moq;
    using Raptor.Content;
    using Raptor.Exceptions;
    using RaptorTests.Fakes;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentSource"/> class.
    /// </summary>
    public class ContentSourceTests
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly Mock<IDirectory> mockDirectory;
        private readonly string contentDirName = "test-dir";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSourceTests"/> class.
        /// </summary>
        public ContentSourceTests() => this.mockDirectory = new Mock<IDirectory>();

        /// <summary>
        /// Gets test data for the <see cref="ContentRootDirectory_WhenSettingValue_ReturnsCorrectValue"/> test.
        /// </summary>
        public static IEnumerable<object[]> ContentRootPaths =>
            new List<object[]>
            {
                new object[] { null, @$"{BaseDir}Content\" },
                new object[] { @"C:\temp\", @"C:\temp\Content\" },
                new object[] { @"C:\temp", @"C:\temp\Content\" },
            };

        #region Prop Tests
        [Theory]
        [MemberData(nameof(ContentRootPaths))]
        public void ContentRootDirectory_WhenSettingValue_ReturnsCorrectValue(string rootDirectory, string expected)
        {
            // Arrange
            var source = CreateContentSource();

            // Act
            source.ContentRootDirectory = rootDirectory;
            var actual = source.ContentRootDirectory;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ContentDirectoryName_WhenSettingWithDirectoryPath_CorrectlySetsValue()
        {
            // Arrange
            var source = CreateContentSource();

            // Act
            source.ContentDirectoryName = @"C:\temp\test-dir-name";
            var actual = source.ContentDirectoryName;

            // Assert
            Assert.Equal("test-dir-name", actual);
        }

        [Fact]
        public void GraphicsDirectoryName_WhenValueIsNullOrEmpty_ThrowsExeption()
        {
            // Arrange
            var source = CreateContentSource();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                source.ContentDirectoryName = null;
            }, "The 'ContentDirectoryName' must not be null or empty.");
        }

        [Fact]
        public void GetContentPath_WhenContentNameIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var source = CreateContentSource();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<StringNullOrEmptyException>(() =>
            {
                source.GetContentPath(null);
            }, "The string must not be null or empty.");
        }

        [Fact]
        public void GetContentPath_WhenContentNameEndsWithDirSeparator_ThrowsException()
        {
            // Arrange
            var source = CreateContentSource();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                source.GetContentPath(@"content-item\");
            }, @"The 'content-item\' cannot end with folder.  It must end with a file name with or without the extension.");
        }

        [Theory]
        [InlineData("content-item.png", @"C:\temp\Content\test-dir\content-item.png")]
        [InlineData("content-item.ogg", @"C:\temp\Content\test-dir\content-item.ogg")]
        [InlineData("content-item.json", @"C:\temp\Content\test-dir\content-item.json")]
        public void GetContentPath_WhenInvokedWithFileExtension_ReturnsCorrectResult(string contentName, string expected)
        {
            // Arrange
            var contentSubFolder = string.Empty;

            this.mockDirectory.Setup(m => m.GetFiles(@$"C:\temp\Content\test-dir"))
                .Returns(new[] { expected });

            var loader = CreateContentSource();
            loader.ContentRootDirectory = @"C:\temp\";

            // Act
            var actual = loader.GetContentPath(contentName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContentPath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockDirectory.Setup(m => m.GetFiles(It.IsAny<string>()))
                .Returns(new[] { "content-item.ogg", "content-item.mp3" });

            var source = CreateContentSource();
            source.ContentRootDirectory = @"C:\temp\";

            var expectedMsg = new StringBuilder();
            expectedMsg.AppendLine("Multiple items match the content item name.");
            expectedMsg.AppendLine("The content item name must be unique and the file extension is not taken into account.");
            expectedMsg.AppendLine("\tcontent-item.ogg");
            expectedMsg.AppendLine("\tcontent-item.mp3");

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                source.GetContentPath("content-item");
            }, expectedMsg.ToString());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ContentSourceFake"/> for testing.
        /// </summary>
        /// <returns>The instance used for testing.</returns>
        private ContentSourceFake CreateContentSource() => new ContentSourceFake(this.contentDirName, this.mockDirectory.Object);
    }
}
