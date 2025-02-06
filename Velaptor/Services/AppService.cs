// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No implementation to test")]
internal class AppService : IAppService
{
    private bool alreadyInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppService"/> class.
    /// </summary>
    public AppService() => AppDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

    /// <inheritdoc/>
    public string AppDirectory { get; }

    /// <inheritdoc/>
    public void Init()
    {
        if (this.alreadyInitialized)
        {
            return;
        }

        this.alreadyInitialized = true;
    }
}
