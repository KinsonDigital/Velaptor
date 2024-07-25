// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No implementation to test")]
internal class AppService : IAppService
{
    private readonly string appDirectory = string.Empty;
    private bool alreadyInitialized;

    /// <inheritdoc/>
    public string AppDirectory => string.IsNullOrEmpty(this.appDirectory) ? Assembly.GetExecutingAssembly().Location : this.appDirectory;

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
