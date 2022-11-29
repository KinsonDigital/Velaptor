// <copyright file="UIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Content.Fonts;
using Input;
using UI;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
public class UIControlFactory : IUIControlFactory
{
    /// <inheritdoc/>
    public Label CreateLabel(string labelText)
    {
        var label = new Label(
            ContentLoaderFactory.CreateContentLoader(),
            IoC.Container.GetInstance<IFont>(),
            IoC.Container.GetInstance<IAppInput<MouseState>>())
        {
            Text = labelText,
        };

        return label;
    }

    /// <inheritdoc/>
    public Label CreateLabel(string labelText, IFont font)
    {
        var label = new Label(
            ContentLoaderFactory.CreateContentLoader(),
            font,
            IoC.Container.GetInstance<IAppInput<MouseState>>())
        {
            Text = labelText,
        };

        return label;
    }
}
