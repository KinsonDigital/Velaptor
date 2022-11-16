// <copyright file="IUIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Content.Fonts;
using Velaptor.UI;

namespace Velaptor.Factories;

/// <summary>
/// Creates UI controls.
/// </summary>
public interface IUIControlFactory
{
    /// <summary>
    /// Creates a new <see cref="Label"/> control to display text.
    /// </summary>
    /// <param name="labelText">The text to display in the label.</param>
    /// <returns>The label to render.</returns>
    Label CreateLabel(string labelText);

    /// <summary>
    /// Creates a new <see cref="Label"/> control to display text.
    /// </summary>
    /// <param name="labelText">The text to display in the label.</param>
    /// <param name="font">The font to use for the label.</param>
    /// <returns>The label to render.</returns>
    Label CreateLabel(string labelText, IFont font);
}
