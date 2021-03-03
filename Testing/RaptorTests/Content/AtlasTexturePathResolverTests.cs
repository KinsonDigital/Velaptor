// <copyright file="AtlasTexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasTexturePathResolver"/> class.
    /// </summary>
    public class AtlasTexturePathResolverTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var resolver = new AtlasTexturePathResolver(mockDirectory.Object);
            var actual = resolver.FileDirectoryName;

            // Assert
            Assert.Equal("Atlas", actual);
        }
        #endregion
    }
}
