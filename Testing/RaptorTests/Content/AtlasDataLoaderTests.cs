// <copyright file="AtlasDataLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.Drawing;
    using System.IO.Abstractions;
    using System.Text.Json;
    using Moq;
    using Raptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasDataLoader{T}"/> class.
    /// </summary>
    public class AtlasDataLoaderTests
    {
        private readonly Mock<IFile> mockTextFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataLoaderTests"/> class.
        /// </summary>
        public AtlasDataLoaderTests()
        {
            this.mockTextFile = new Mock<IFile>();

            var textFileData = new Rectangle[]
            {
                new Rectangle(11, 22, 33, 44),
                new Rectangle(55, 66, 77, 88),
            };

            this.mockTextFile.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns(() =>
            {
                return JsonSerializer.Serialize(textFileData);
            });
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsTextureAtlasData()
        {
            // Arrange
            var loader = new AtlasDataLoader<Rectangle>(this.mockTextFile.Object);
            var expected = new Rectangle[]
            {
                new Rectangle(11, 22, 33, 44),
                new Rectangle(55, 66, 77, 88),
            };

            // Act
            var actual = loader.Load("test-file");

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
