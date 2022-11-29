// <copyright file="NativeInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Silk.NET.Input;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
internal sealed class NativeInputFactory : INativeInputFactory
{
    private readonly IWindowFactory windowFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInputFactory"/> class.
    /// </summary>
    /// <param name="windowFactory">Creates a window object.</param>
    public NativeInputFactory(IWindowFactory windowFactory) => this.windowFactory = windowFactory;

    /// <inheritdoc/>
    public IInputContext CreateInput()
    {
        var window = this.windowFactory.CreateSilkWindow();

        return window.CreateInput();
    }
}
