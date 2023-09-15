// <copyright file="AppSettingsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.IO.Abstractions;
using System.Text.Json;
using Exceptions;
using ExtensionMethods;
using Guards;

/// <inheritdoc/>
internal sealed class AppSettingsService : IAppSettingsService
{
    private const string AppSettingsFileName = "app-settings.json";
    private readonly IJSONService jsonService;
    private readonly IFile file;
    private readonly string appSettingsFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsService"/> class.
    /// </summary>
    /// <param name="jsonService">Provides JSON related services.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <exception cref="AppSettingsException">Occurs if there are issues with loading the application settings file.</exception>
    public AppSettingsService(IJSONService jsonService, IDirectory directory, IFile file)
    {
        EnsureThat.ParamIsNotNull(jsonService);
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(file);

        this.jsonService = jsonService;
        this.file = file;

        var baseDirPath = directory.GetCurrentDirectory().ToCrossPlatPath();
        this.appSettingsFilePath = $"{baseDirPath}/{AppSettingsFileName}";
        LoadSettings();
    }

    /// <inheritdoc/>
    public AppSettings Settings { get; private set; } = new ();

    /// <summary>
    /// Loads the app settings data.
    /// </summary>
    private void LoadSettings()
    {
        if (!this.file.Exists(this.appSettingsFilePath))
        {
            CreateSettings();

            return;
        }

        var jsonData = this.file.ReadAllText(this.appSettingsFilePath);

        try
        {
            Settings = this.jsonService.Deserialize<AppSettings>(jsonData) ?? new AppSettings();
        }
        catch (JsonException e)
        {
            var exMsg = $"There was an issue loading the application settings at the path '{this.appSettingsFilePath}'.";
            exMsg += $"{Environment.NewLine}The file could be corrupt.";
            throw new AppSettingsException(exMsg, e);
        }
    }

    /// <summary>
    /// Creates default settings and an associated file for storage.
    /// </summary>
    private void CreateSettings()
    {
        Settings = new AppSettings();

        var settingData = this.jsonService.Serialize(Settings);

        this.file.WriteAllText(this.appSettingsFilePath, settingData);
    }
}
