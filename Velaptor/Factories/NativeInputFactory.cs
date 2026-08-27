// <copyright file="NativeInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using Silk.NET.Input;
using Silk.NET.Windowing;

/// <inheritdoc/>
internal sealed class NativeInputFactory : INativeInputFactory
{
    private readonly IWindow window;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInputFactory"/> class.
    /// </summary>
    /// <param name="window">The silk window object.</param>
    public NativeInputFactory(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        this.window = window;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Too complicated to mock SILK static method 'IWindow.CreateInput()'")]
    public IInputContext CreateInput() => this.window.CreateInput();
}
