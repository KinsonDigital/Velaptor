// <copyright file="ITextSelection.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System.Drawing;
using Graphics;

/// <summary>
/// Represents selected text in a text box.
/// </summary>
internal interface ITextSelection
{
    /// <summary>
    /// Gets the selection rectangle.
    /// </summary>
    RectShape SelectionRect { get; }

    /// <summary>
    /// Gets the selected text.
    /// </summary>
    string SelectedText { get; }

    /// <summary>
    /// Gets or sets the color of the section rectangle.
    /// </summary>
    Color SelectionColor { get; set; }

    /// <summary>
    /// Updates the text selection.
    /// </summary>
    void Update();

    /// <summary>
    /// Clears the selected state and state.
    /// </summary>
    void Clear();
}
