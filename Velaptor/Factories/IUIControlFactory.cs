﻿// <copyright file="IUIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using UI;

/// <summary>
/// Creates UI controls.
/// </summary>
internal interface IUIControlFactory
{
    /// <summary>
    /// Creates a new <see cref="Label"/> control to display text.
    /// </summary>
    /// <param name="labelText">The text to display in the label.</param>
    /// <returns>The label to render.</returns>
    Label CreateLabel(string labelText);
}
