// <copyright file="ITextCursor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System.Drawing;
using System.Numerics;
using Graphics;

/// <summary>
/// Represents a text cursor in a text box.
/// </summary>
internal interface ITextCursor
{
    /// <summary>
    /// Gets the cursor.
    /// </summary>
    RectShape Cursor { get; }

    /// <summary>
    /// Gets or sets the width of the cursor.
    /// </summary>
    int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the cursor.
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// Gets or sets the position of the cursor.
    /// </summary>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the color of the cursor.
    /// </summary>
    Color Color { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cursor is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Updates the state of the cursor.
    /// </summary>
    void Update();

    /// <summary>
    /// Snaps the cursor to the left end of the text box.
    /// </summary>
    void SnapCursorToLeft();

    /// <summary>
    /// Snaps the cursor to the right end of the text box.
    /// </summary>
    void SnapCursorToRight();

    /// <summary>
    /// Snaps the the cursor to the first visible character in the text box.
    /// </summary>
    void SnapToFirstVisibleChar();

    /// <summary>
    /// Snaps the the cursor to the last visible character in the text box.
    /// </summary>
    void SnapToLastVisibleChar();
}
