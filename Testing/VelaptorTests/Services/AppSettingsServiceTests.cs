// <copyright file="AppSettingsServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.IO.Abstractions;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AppSettingsService"/> class.
/// </summary>
public class AppSettingsServiceTests
{
    private const string AppSettingsFileName = "app-settings.json";
    private const string BaseDirPath = "C:/velaptor";
    private const string SettingsFilePath = $"{BaseDirPath}/{AppSettingsFileName}";
    private readonly IJsonService mockJsonService;
    private readonly IDirectory mockDirService;
    private readonly IFile mockFileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsServiceTests"/> class.
    /// </summary>
    public AppSettingsServiceTests()
    {
        this.mockJsonService = Substitute.For<IJsonService>();

        this.mockDirService = Substitute.For<IDirectory>();
        this.mockDirService.GetCurrentDirectory().Returns(BaseDirPath);

        this.mockFileService = Substitute.For<IFile>();
        this.mockFileService.Exists(Arg.Any<string>()).Returns(true);
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
                this.mockDirService,
                this.mockFileService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'jsonService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AppSettingsService(
                this.mockJsonService,
                null,
                this.mockFileService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AppSettingsService(
                this.mockJsonService,
                this.mockDirService,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WhenSettingsFileDoesNotExist_CreatesSettingsFileWithDefaultValues()
    {
        // Arrange
        this.mockFileService.Exists(Arg.Any<string>()).Returns(false);
        this.mockJsonService.Serialize(Arg.Any<object?>()).Returns("test-data");

        // Act
        _ = CreateService();

        // Assert
        this.mockFileService.Received(1).Exists(SettingsFilePath);
        this.mockJsonService.Received(1).Serialize(Arg.Any<AppSettings>());
        this.mockFileService.Received(1).WriteAllText(SettingsFilePath, "test-data");
    }

    [Fact]
    public void Ctor_WhenSettingsFileExistsWithProblemLoadingData_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue loading the application settings at the path '{SettingsFilePath}'.";
        expected += $"{Environment.NewLine}The file could be corrupt.";

        this.mockJsonService
            .When(x => x.Deserialize<AppSettings>(Arg.Any<string>()))
            .Throw<JsonException>();

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
        => new (this.mockJsonService, this.mockDirService, this.mockFileService);
}
