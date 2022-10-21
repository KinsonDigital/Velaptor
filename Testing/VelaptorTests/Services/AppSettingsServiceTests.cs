// <copyright file="AppSettingsServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Text.Json;
    using FluentAssertions;
    using Moq;
    using Velaptor.Exceptions;
    using Velaptor.Services;
    using Xunit;

    public class AppSettingsServiceTests
    {
        private const uint DefaultWidth = 1500;
        private const uint DefaultHeight = 800;
        private const string AppSettingsFileName = "app-settings.json";
        private const string BaseDirPath = "C:/velaptor";
        private const string SettingsFilePath = $"{BaseDirPath}/{AppSettingsFileName}";
        private readonly Mock<IJSONService> mockJsonService;
        private readonly Mock<IDirectory> mockDirService;
        private readonly string defaultSettingsData = $"[{{\"Key\":\"WindowWidth\",\"Value\":\"{DefaultWidth}\"}},{{\"Key\":\"WindowHeight\",\"Value\":\"{DefaultHeight}\"}}]";
        private readonly Mock<IFile> mockFileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsServiceTests"/> class.
        /// </summary>
        public AppSettingsServiceTests()
        {
            this.mockJsonService = new Mock<IJSONService>();
            this.mockJsonService.Setup(m => m.Deserialize<KeyValuePair<string, string>[]>(It.IsAny<string>()))
                .Returns<string>(_ => new KeyValuePair<string, string>[]
                {
                    new (nameof(IAppSettingsService.WindowWidth), DefaultWidth.ToString()),
                    new (nameof(IAppSettingsService.WindowHeight), DefaultHeight.ToString()),
                });

            this.mockDirService = new Mock<IDirectory>();
            this.mockDirService.Setup(m => m.GetCurrentDirectory()).Returns(BaseDirPath);

            this.mockFileService = new Mock<IFile>();
            this.mockFileService.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            this.mockFileService.Setup(m => m.ReadAllText(It.IsAny<string>()))
                .Returns<string>(_ => this.defaultSettingsData);
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
            var expectedSettings = new KeyValuePair<string, string>[]
            {
                new (nameof(IAppSettingsService.WindowWidth), DefaultWidth.ToString()),
                new (nameof(IAppSettingsService.WindowHeight), DefaultHeight.ToString()),
            };
            this.mockFileService.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            this.mockJsonService.Setup(m => m.Serialize(It.IsAny<object?>()))
                .Returns("test-data");

            // Act
            _ = CreateService();

            // Assert
            this.mockFileService.Verify(m => m.Exists(SettingsFilePath), Times.Once);
            this.mockJsonService.Verify(m => m.Serialize(expectedSettings), Times.Once);
            this.mockFileService.Verify(m => m.WriteAllText(SettingsFilePath, "test-data"), Times.Once);
        }

        [Fact]
        public void Ctor_WhenSettingsFileExistsWithProblemLoadingData_ThrowsException()
        {
            // Arrange
            var expected = $"There was an issue loading the application settings at the path '{SettingsFilePath}'.";
            expected += $"{Environment.NewLine}The file could be corrupt.";

            this.mockJsonService.Setup(m => m.Deserialize<KeyValuePair<string, string>[]>(It.IsAny<string>()))
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
        public void WindowWidth_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var sut = CreateService();

            // Act
            var actual = sut.WindowWidth;

            // Assert
            actual.Should().Be(DefaultWidth);
        }

        [Fact]
        public void WindowHeight_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var sut = CreateService();

            // Act
            var actual = sut.WindowHeight;

            // Assert
            actual.Should().Be(DefaultHeight);
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
