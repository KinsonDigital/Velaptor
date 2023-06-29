// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

/// <summary>
/// Represents the various types of events that can occur with a text box cursor.
/// </summary>
internal enum TextBoxEvent
{
    /// <summary>
    /// No event has occured.
    /// </summary>
    None,

    /// <summary>
    /// A character is being added to the text box.
    /// </summary>
    AddingCharacter,

    /// <summary>
    /// A character is being removed from the text box.
    /// </summary>
    RemovingSingleChar,

    /// <summary>
    /// One or more characters are selected to be removed.
    /// </summary>
    RemovingSelectedChars,

    /// <summary>
    /// The cursor is moving.
    /// </summary>
    MovingCursor,

    /// <summary>
    /// The text box is being resized.
    /// </summary>
    DimensionalChange,

    /// <summary>
    /// The text box is being moved horizontally and/or vertically.
    /// </summary>
    Movement,
}

/// <summary>
/// The type of text mutation in a text box.
/// </summary>
internal enum MutateType
{
    /// <summary>
    /// Signifies before the text has been mutated.
    /// </summary>
    PreMutate,

    /// <summary>
    /// Signifies after the text has been mutated.
    /// </summary>
    PostMutate,
}
