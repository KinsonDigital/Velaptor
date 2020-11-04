// <copyright file="GraphicsContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="GraphicsContentSource"/> class.
    /// </summary>
    public class GraphicsContentSourceTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var source = new GraphicsContentSource(mockDirectory.Object);
            var actual = source.ContentDirectoryName;

            // Assert
            Assert.Equal("Graphics", actual);
        }
        #endregion
    }
}
