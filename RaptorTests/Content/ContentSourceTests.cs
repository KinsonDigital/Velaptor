// <copyright file="ContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0017 // Simplify object initialization
namespace RaptorTests.Content
{
    using System.IO;
    using System.Reflection;
    using FileIO.Core;
    using Moq;
    using Raptor.Content;
    using Xunit;

    public class ContentSourceTests
    {
        private static readonly string baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly Mock<IDirectory> mockDirectory;

        public ContentSourceTests()
        {
            this.mockDirectory = new Mock<IDirectory>();
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(@"C:\temp\", @"C:\temp\Content\")]
        [InlineData(@"C:\temp", @"C:\temp\Content\")]
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
    }
}
