// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics.CodeAnalysis;
using Graphics.Renderers;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "No implementation to test")]
internal class AppService : IAppService
{
    private bool alreadyInitialized;

    /// <inheritdoc/>
    public void Init()
    {
        if (this.alreadyInitialized)
        {
            return;
        }

        IRenderer.Init();
        this.alreadyInitialized = true;
    }
}
