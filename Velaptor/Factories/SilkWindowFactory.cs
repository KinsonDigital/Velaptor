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
    private static IWindow? window;

    /// <inheritdoc/>
    public IWindow CreateSilkWindow()
    {
        if (window is not null)
        {
            return window;
        }

        var windowOptions = WindowOptions.Default;
        windowOptions.ShouldSwapAutomatically = false;

        window = Window.Create(windowOptions);
        return window;
    }
}
