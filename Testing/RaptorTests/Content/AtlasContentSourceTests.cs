// <copyright file="AtlasContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasContentSource"/> class.
    /// </summary>
    public class AtlasContentSourceTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var source = new AtlasContentSource(mockDirectory.Object);
            var actual = source.ContentDirectoryName;

            // Assert
            Assert.Equal("Atlases", actual);
        }
        #endregion
    }
}
