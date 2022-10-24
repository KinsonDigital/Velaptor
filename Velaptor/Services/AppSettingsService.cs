// <copyright file="AppSettingsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Text.Json;
    using Velaptor.Exceptions;
    using Velaptor.Guards;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    internal sealed class AppSettingsService : IAppSettingsService
    {
        private const uint DefaultWidth = 1500;
        private const uint DefaultHeight = 800;
        private const string AppSettingsFileName = "app-settings.json";
        private readonly IJSONService jsonService;
        private readonly IFile file;
        private readonly string appSettingsFilePath;
        private KeyValuePair<string, string>[] settings = Array.Empty<KeyValuePair<string, string>>();

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
        public uint WindowWidth
        {
            get
            {
                var setting = this.settings.FirstOrDefault(s => s.Key == nameof(WindowWidth));

                var doesNotExist = string.IsNullOrEmpty(setting.Key) && string.IsNullOrEmpty(setting.Value);

                if (doesNotExist)
                {
                    return DefaultWidth;
                }

                var isParsed = uint.TryParse(setting.Value, out var width);

                return isParsed ? width : 0u;
            }
        }

        /// <inheritdoc/>
        public uint WindowHeight
        {
            get
            {
                var setting = this.settings.FirstOrDefault(s => s.Key == nameof(WindowHeight));

                var doesNotExist = string.IsNullOrEmpty(setting.Key) && string.IsNullOrEmpty(setting.Value);

                if (doesNotExist)
                {
                    return DefaultHeight;
                }

                var isParsed = uint.TryParse(setting.Value, out var height);

                return isParsed ? height : 0u;
            }
        }

        /// <summary>
        /// Loads the app settings data.
        /// </summary>
        private void LoadSettings()
        {
            if (this.file.Exists(this.appSettingsFilePath) is false)
            {
                CreateSettings();

                return;
            }

            var jsonData = this.file.ReadAllText(this.appSettingsFilePath);

            try
            {
                this.settings = this.jsonService.Deserialize<KeyValuePair<string, string>[]>(jsonData) ?? Array.Empty<KeyValuePair<string, string>>();
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
            this.settings = new KeyValuePair<string, string>[]
            {
                new (nameof(WindowWidth), DefaultWidth.ToString()),
                new (nameof(WindowHeight), DefaultHeight.ToString()),
            };

            var settingData = this.jsonService.Serialize(this.settings);

            this.file.WriteAllText(this.appSettingsFilePath, settingData);
        }
    }
}
