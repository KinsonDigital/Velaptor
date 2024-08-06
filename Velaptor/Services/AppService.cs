// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No implementation to test")]
internal class AppService : IAppService
{
    private readonly string appDirectory = string.Empty;
    private bool alreadyInitialized;

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "No tests written for this.")]
    public string AppDirectory => string.IsNullOrEmpty(this.appDirectory)
        ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty
        : this.appDirectory;

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "No tests written for this.")]
    public void Init()
    {
        if (this.alreadyInitialized)
        {
            return;
        }

        this.alreadyInitialized = true;
    }
}
