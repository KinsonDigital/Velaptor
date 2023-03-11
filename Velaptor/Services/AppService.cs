// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using Graphics.Renderers;

/// <inheritdoc/>
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
