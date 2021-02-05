// <copyright file="AtlasRepositoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System;
    using System.Drawing;
    using Moq;
    using Raptor.Graphics;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AtlasRepository"/> class.
    /// </summary>
    public class AtlasRepositoryTests : IDisposable
    {
        private readonly Mock<ITexture> mockTexture;
        private readonly IAtlasData atlasData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasRepositoryTests"/> class.
        /// </summary>
        public AtlasRepositoryTests()
        {
            this.mockTexture = new Mock<ITexture>()
            {
                Name = nameof(this.mockTexture),
            };

            var spriteData = new AtlasSubTextureData[]
            {
                new AtlasSubTextureData()
                {
                    Name = "sub-texture",
                    Bounds = new Rectangle(11, 22, 33, 44),
                },
                new AtlasSubTextureData()
                {
                    Name = "sub-texture",
                    Bounds = new Rectangle(55, 66, 77, 88),
                },
            };

            this.atlasData = new AtlasData(spriteData, this.mockTexture.Object, "test-atlas", $@"C:\temp\test-atlas.png");
            AtlasRepository.Instance.EmptyRepository();
        }

        #region Prop Tests
        [Fact]
        public void Instance_WhenInvoked_ReturnsSameObjectInstance()
        {
            // Act
            var instanceA = AtlasRepository.Instance;
            var instanceB = AtlasRepository.Instance;

            // Assert
            Assert.Same(instanceA, instanceB);
        }

        [Fact]
        public void TotalItems_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            var actual = repo.TotalItems;

            // Assert
            Assert.Equal(1, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void AtlasData_WithAlreadyAddedTextureAtlasID_ThrowsException()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                repo.AddAtlasData("test-atlas", this.atlasData);
            }, "Texture atlas data with the ID of 'test-atlas' already exists.");
        }

        [Fact]
        public void AtlasLoaded_WhenInvokedWithAlreadyLoadedAtlas_ReturnsTrue()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            var actual = repo.IsAtlasLoaded("test-atlas");

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EmptyRepository_WhenRemovingAtlasData_DisposesOfTexture()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            repo.EmptyRepository();

            // Assert
            this.mockTexture.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void EmptyRepository_WhenInvoked_RemovesData()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            repo.RemoveAtlasData("test-atlas");
            var actualAtlasLoaded = repo.IsAtlasLoaded("test-atlas");
            var actualTotalItems = repo.TotalItems;

            // Assert
            Assert.False(actualAtlasLoaded);
            Assert.Equal(0, actualTotalItems);
        }

        [Fact]
        public void RemoveAtlasData_WithNonExistingAtlas_ThrowsException()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                repo.RemoveAtlasData("non-existing-atlas");
            }, "Texture atlas data with the ID of 'non-existing-atlas' does not exist.");
        }

        [Fact]
        public void GetAtlasData_WhenAtlasExists_ReturnsAtlasData()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            var actual = repo.GetAtlasData("test-atlas");

            // Assert
            Assert.Same(this.atlasData, actual);
        }

        [Fact]
        public void GetAtlasData_WhenAtlasDoesNotExist_ThrowsException()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                repo.GetAtlasData("non-existing-atlas");
            }, "Texture atlas data with the ID of 'non-existing-atlas' does not exist.");
        }

        [Fact]
        public void GetAtlasTexture_WhenAtlasExists_ReturnsAtlasData()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            var actual = repo.GetAtlasTexture("test-atlas");

            // Assert
            Assert.Same(this.atlasData.Texture, actual);
        }

        [Fact]
        public void GetAtlasTexture_WhenAtlasDoesNotExist_ThrowsException()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                repo.GetAtlasTexture("non-existing-atlas");
            }, "Texture atlas data with the ID of 'non-existing-atlas' does not exist.");
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfAtlasData()
        {
            // Arrange
            var repo = AtlasRepository.Instance;
            repo.AddAtlasData("test-atlas", this.atlasData);

            // Act
            repo.Dispose();
            repo.Dispose();

            // Assert
            this.mockTexture.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            AtlasRepository.Instance.EmptyRepository();
        }
    }
}
