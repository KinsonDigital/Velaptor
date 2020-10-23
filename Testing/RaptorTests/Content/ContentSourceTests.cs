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
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ContentSource"/> class.
    /// </summary>
    public class ContentSourceTests
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly Mock<IDirectory> mockDirectory;

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
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act
            loader.ContentRootDirectory = rootDirectory;
            var actual = loader.ContentRootDirectory;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GraphicsDirectoryName_WhenValueIsNullOrEmpty_ThrowsExeption()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                loader.GraphicsDirectoryName = null;
            }, "The 'GraphicsDirectoryName' must not be null or empty.");
        }

        [Fact]
        public void GraphicsDirectoryName_WhenInvoked_ReturnsCorrectValue()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act
            loader.GraphicsDirectoryName = "root-dir-test";
            var actual = loader.GraphicsDirectoryName;

            // Assert
            Assert.Equal("root-dir-test", actual);
        }

        [Fact]
        public void SoundsDirectoryName_WhenValueIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act &  Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                loader.SoundsDirectoryName = null;
            }, "The 'SoundsDirectoryName' must not be null or empty.");
        }

        [Fact]
        public void SoundsDirectoryName_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act
            loader.SoundsDirectoryName = "sounds-dir-test";
            var actual = loader.SoundsDirectoryName;

            // Assert
            Assert.Equal("sounds-dir-test", actual);
        }

        [Fact]
        public void AtlasDirectoryName_WhenValueIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act &  Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                loader.AtlasDirectoryName = null;
            }, "The 'AtlasDirectoryName' must not be null or empty.");
        }

        [Fact]
        public void AtlasDirectoryName_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act
            loader.AtlasDirectoryName = "atlas-dir-test";
            var actual = loader.AtlasDirectoryName;

            // Assert
            Assert.Equal("atlas-dir-test", actual);
        }

        [Fact]
        public void GetGraphicsPath_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = "test-content-dir";
            loader.GraphicsDirectoryName = "test-graphics";

            // Act
            var actual = loader.GetGraphicsPath();

            // Assert
            Assert.Equal(@"test-content-dir\Content\test-graphics\", actual);
        }

        [Fact]
        public void GetSoundsPath_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = "test-content-dir";
            loader.SoundsDirectoryName = "test-sounds";

            // Act
            var actual = loader.GetSoundsPath();

            // Assert
            Assert.Equal(@"test-content-dir\Content\test-sounds\", actual);
        }

        [Fact]
        public void GetAtlasPath_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = "test-content-dir";
            loader.AtlasDirectoryName = "test-atlas";

            // Act
            var actual = loader.GetAtlasPath();

            // Assert
            Assert.Equal(@"test-content-dir\Content\test-atlas\", actual);
        }

        [Fact]
        public void GetContentPath_WhenContentNameIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<StringNullOrEmptyException>(() =>
            {
                loader.GetContentPath(It.IsAny<ContentType>(), null);
            }, "The string must not be null or empty.");
        }

        [Fact]
        public void GetContentPath_WhenContentNameEndsWithDirSeparator_ThrowsException()
        {
            // Arrange
            var loader = new ContentSource(this.mockDirectory.Object);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                loader.GetContentPath(ContentType.Graphics, @"content-item\");
            }, @"The 'content-item\' cannot end with folder.  It must end with a file name with or without the extension.");
        }

        [Theory]
        [InlineData(ContentType.Graphics, "content-item.png", @"C:\temp\Content\Graphics\content-item.png")]
        [InlineData(ContentType.Sounds, "content-item.ogg", @"C:\temp\Content\Graphics\content-item.ogg")]
        [InlineData(ContentType.Atlas, "content-item.json", @"C:\temp\Content\Graphics\content-item.json")]
        public void GetContentPath_WhenInvokedWithFileExtension_ReturnsCorrectResult(ContentType contentType, string contentName, string expected)
        {
            // Arrange
            var contentSubFolder = string.Empty;

            switch (contentType)
            {
                case ContentType.Graphics:
                    contentSubFolder = "Graphics";
                    break;
                case ContentType.Sounds:
                    contentSubFolder = "Sounds";
                    break;
                case ContentType.Atlas:
                    contentSubFolder = "AtlasData";
                    break;
            }

            this.mockDirectory.Setup(m => m.GetFiles(@$"C:\temp\Content\{contentSubFolder}"))
                .Returns(new[] { expected });

            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = @"C:\temp\";

            // Act
            var actual = loader.GetContentPath(contentType, contentName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MemberName_Scenario_Expectation()
        {
            // Arrange
            this.mockDirectory.Setup(m => m.GetFiles(It.IsAny<string>()))
            .Returns(Array.Empty<string>());

            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = @"C:\temp\";

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                loader.GetContentPath(ContentType.Graphics, "content-item");
            }, "The content item 'content-item' does not exist.");
        }

        [Fact]
        public void GetContentPath_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockDirectory.Setup(m => m.GetFiles(It.IsAny<string>()))
                .Returns(new[] { "content-item.ogg", "content-item.mp3" });

            var loader = new ContentSource(this.mockDirectory.Object);
            loader.ContentRootDirectory = @"C:\temp\";

            var expectedMsg = new StringBuilder();
            expectedMsg.AppendLine("Multiple items match the content item name.");
            expectedMsg.AppendLine("The content item name must be unique and the file extension is not taken into account.");
            expectedMsg.AppendLine("\tcontent-item.ogg");
            expectedMsg.AppendLine("\tcontent-item.mp3");

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                loader.GetContentPath(ContentType.Graphics, "content-item");
            }, expectedMsg.ToString());
        }
        #endregion
    }
}
