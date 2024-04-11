// <copyright file="TextBoxStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using System.Drawing;
using System.Numerics;
using System.Text;
using Graphics;
using Input;
using UI;

/// <summary>
/// Represents the state of the <see cref="TextBox"/> control.
/// </summary>
internal readonly record struct TextBoxStateData
{
    private readonly int charIndex;

    /// <summary>
    /// Gets the text of the text box.
    /// </summary>
    public StringBuilder Text { get; init; }

    /// <summary>
    /// Gets the length of the text in the text box.
    /// </summary>
    /// <remarks>
    /// This is important because this is not a reference to the same text in the pre-state
    /// vs the post-state.
    /// </remarks>
    public int TextLength { get; init; }

    /// <summary>
    /// Gets the X position of the left end of the text.
    /// </summary>
    public int TextLeft { get; init; }

    /// <summary>
    /// Gets the X position of the right end of the text.
    /// </summary>
    public int TextRight { get; init; }

    /// <summary>
    /// Gets the rectangle shape that represents the viewable area in the text box.
    /// </summary>
    public RectShape TextView { get; init; }

    /// <summary>
    /// Gets the keyboard key that was pressed in the text box.
    /// </summary>
    public KeyCode Key { get; init; }

    /// <summary>
    /// Gets the index of the cursor.
    /// </summary>
    public int CharIndex
    {
        readonly get => this.charIndex;
        init => this.charIndex = value < 0 ? 0 : value;
    }

    /// <summary>
    /// Gets the X position of the left side of character at the cursor.
    /// </summary>
    public int CurrentCharLeft { get; init; }

    /// <summary>
    /// Gets the X position of the right side of character at the cursor.
    /// </summary>
    public int CurrentCharRight { get; init; }

    /// <summary>
    /// Gets a value indicating whether the text box is in selection mode.
    /// </summary>
    public bool InSelectionMode { get; init; }

    /// <summary>
    /// Gets the bounds of the character at the start of the selected text.
    /// </summary>
    public RectangleF SelStartCharBounds { get; init; }

    /// <summary>
    /// Gets the bounds of the character at the end of the selected text.
    /// </summary>
    public RectangleF SelStopCharBounds { get; init; }

    /// <summary>
    /// Gets the bounds of the first character that is visible in the viewable area in the text box.
    /// </summary>
    public RectangleF FirstVisibleCharBounds { get; init; }

    /// <summary>
    /// Gets the bounds of the last character that is visible in the viewable area in the text box.
    /// </summary>
    public RectangleF LastVisibleCharBounds { get; init; }

    /// <summary>
    /// Gets the character index of the first character in the selected text.
    /// </summary>
    public int SelectionStartIndex { get; init; }

    /// <summary>
    /// Gets the character index of the last character in the selected text.
    /// </summary>
    public int SelectionStopIndex { get; init; }

    /// <summary>
    /// Gets the height of the selection.
    /// </summary>
    public int SelectionHeight { get; init; }

    /// <summary>
    /// Gets a value indicating whether the selection is a right side of the last
    /// character of all the text in the text box.
    /// </summary>
    public bool SelectionAtRightEnd { get; init; }

    /// <summary>
    /// Gets the position of the text box.
    /// </summary>
    /// <remarks>This is relative to the vertical and horizontal center.</remarks>
    public Vector2 Position { get; init; }

    /// <summary>
    /// Gets a value indicating whether the cursor is at the right side of the
    /// last character in all the text of the text box.
    /// </summary>
    public bool CursorAtEnd { get; init; }

    /// <summary>
    /// Gets the kind of mutation that the state represents.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <see cref="MutateType.PreMutate"/> represents that state before the text was mutated.
    /// </para>
    /// <para>
    ///     <see cref="MutateType.PostMutate"/> represents that state before the text was mutated.
    /// </para>
    /// </remarks>
    public MutateType TextMutateType { get; init; }

    /// <summary>
    /// Gets the type of event that has occured in the text box.
    /// </summary>
    public TextBoxEvent Event { get; init; }

    /// <summary>
    /// Gets the total width of the text box.
    /// </summary>
    public int Width { get; init; }
}
