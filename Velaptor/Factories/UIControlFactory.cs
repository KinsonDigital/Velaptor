// <copyright file="UIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Content.Fonts;
using Graphics.Renderers;
using Input;
using UI;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal class UIControlFactory : IUIControlFactory
{
    /// <inheritdoc/>
    public Label CreateLabel(string labelText)
    {
        var label = new Label(
            ContentLoaderFactory.CreateFontLoader(),
            IoC.Container.GetInstance<IAppInput<KeyboardState>>(),
            IoC.Container.GetInstance<IAppInput<MouseState>>(),
            IoC.Container.GetInstance<IRendererFactory>())
        {
            Text = labelText,
        };

        return label;
    }

    /// <inheritdoc/>
    public Label CreateLabel(string labelText, IFont font)
    {
        var label = new Label(
            ContentLoaderFactory.CreateFontLoader(),
            IoC.Container.GetInstance<IAppInput<KeyboardState>>(),
            IoC.Container.GetInstance<IAppInput<MouseState>>(),
            IoC.Container.GetInstance<IRendererFactory>())
        {
            Text = labelText,
        };

        return label;
    }
}
