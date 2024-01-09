// <copyright file="SilkWindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Silk.NET.Windowing;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the SILK library.")]
internal sealed class SilkWindowFactory : IWindowFactory
{
    private IWindow? window;

    /// <inheritdoc/>
    public IWindow CreateSilkWindow()
    {
        if (this.window is not null)
        {
            return this.window;
        }

        var windowOptions = WindowOptions.Default;
        windowOptions.ShouldSwapAutomatically = false;

        this.window = Window.Create(windowOptions);

        return this.window;
    }
}
