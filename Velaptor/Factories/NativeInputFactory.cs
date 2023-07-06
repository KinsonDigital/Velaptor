// <copyright file="NativeInputFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Guards;
using Silk.NET.Input;

/// <inheritdoc/>
internal sealed class NativeInputFactory : INativeInputFactory
{
    private readonly IWindowFactory windowFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInputFactory"/> class.
    /// </summary>
    /// <param name="windowFactory">Creates a window object.</param>
    public NativeInputFactory(IWindowFactory windowFactory)
    {
        EnsureThat.ParamIsNotNull(windowFactory);
        this.windowFactory = windowFactory;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Too complicated to mock SILK static method 'IWindow.CreateInput()'")]
    public IInputContext CreateInput()
    {
        var window = this.windowFactory.CreateSilkWindow();

        return window.CreateInput();
    }
}
