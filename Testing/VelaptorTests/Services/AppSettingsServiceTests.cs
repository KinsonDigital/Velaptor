// <copyright file="AppSettingsServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System;
    using System.IO.Abstractions;
    using System.Text.Json;
    using FluentAssertions;
    using Moq;
    using Velaptor;
    using Velaptor.Exceptions;
    using Velaptor.Services;
    using Xunit;

    public class AppSettingsServiceTests
    {
        private const string AppSettingsFileName = "app-settings.json";
        private const string BaseDirPath = "C:/velaptor";
        private const string SettingsFilePath = $"{BaseDirPath}/{AppSettingsFileName}";
        private readonly Mock<IJSONService> mockJsonService;
        private readonly Mock<IDirectory> mockDirService;
        private readonly Mock<IFile> mockFileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsServiceTests"/> class.
        /// </summary>
        public AppSettingsServiceTests()
        {
            this.mockJsonService = new Mock<IJSONService>();

            this.mockDirService = new Mock<IDirectory>();
            this.mockDirService.Setup(m => m.GetCurrentDirectory()).Returns(BaseDirPath);

            this.mockFileService = new Mock<IFile>();
            this.mockFileService.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullJSONServiceParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new AppSettingsService(
                    null,
                    this.mockDirService.Object,
                    this.mockFileService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'jsonService')");
        }

        [Fact]
        public void Ctor_WithNullDirectoryParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new AppSettingsService(
                    this.mockJsonService.Object,
                    null,
                    this.mockFileService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'directory')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new AppSettingsService(
                    this.mockJsonService.Object,
                    this.mockDirService.Object,
                    null);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WhenSettingsFileDoesNotExist_CreatesSettingsFileWithDefaultValues()
        {
            // Arrange
            this.mockFileService.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            this.mockJsonService.Setup(m => m.Serialize(It.IsAny<object?>()))
                .Returns("test-data");

            // Act
            _ = CreateService();

            // Assert
            this.mockFileService.Verify(m => m.Exists(SettingsFilePath), Times.Once);
            this.mockJsonService.Verify(m => m.Serialize(It.IsAny<AppSettings>()), Times.Once);
            this.mockFileService.Verify(m => m.WriteAllText(SettingsFilePath, "test-data"), Times.Once);
        }

        [Fact]
        public void Ctor_WhenSettingsFileExistsWithProblemLoadingData_ThrowsException()
        {
            // Arrange
            var expected = $"There was an issue loading the application settings at the path '{SettingsFilePath}'.";
            expected += $"{Environment.NewLine}The file could be corrupt.";

            this.mockJsonService.Setup(m => m.Deserialize<AppSettings>(It.IsAny<string>()))
                .Throws<JsonException>();

            // Act
            var act = () => _ = CreateService();

            // Assert
            act.Should().Throw<AppSettingsException>()
                .WithMessage(expected).WithInnerException<JsonException>();
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Settings_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var sut = CreateService();

            // Act
            var actual = sut.Settings;

            // Assert
            actual.Should().NotBeNull();
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="AppSettingsService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private AppSettingsService CreateService()
            => new (this.mockJsonService.Object, this.mockDirService.Object, this.mockFileService.Object);
    }
}
