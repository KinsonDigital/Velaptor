// <copyright file="SoundContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundPathResolver"/> class.
    /// </summary>
    public class SoundContentSourceTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
        {
            // Arrange
            var mockDirectory = new Mock<IDirectory>();

            // Act
            var source = new SoundPathResolver(mockDirectory.Object);
            var actual = source.FileDirectoryName;

            // Assert
            Assert.Equal("Sounds", actual);
        }
        #endregion
    }
}
