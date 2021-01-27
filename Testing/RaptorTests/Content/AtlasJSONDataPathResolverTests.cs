// <copyright file="AtlasJSONDataPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasJSONDataPathResolver"/> class.
    /// </summary>
    public class AtlasJSONDataPathResolverTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var source = new AtlasJSONDataPathResolver(mockDirectory.Object);
            var actual = source.FileDirectoryName;

            // Assert
            Assert.Equal("Atlas", actual);
        }
        #endregion
    }
}
