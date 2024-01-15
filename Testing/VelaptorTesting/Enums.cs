// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

/// <summary>
/// The different layers for textures to be rendered for testing purposes.
/// </summary>
public enum RenderLayer
{
    /// <summary>
    /// Layer 1.
    /// </summary>
    One = -10, // White default layer (White changes layers)

    /// <summary>
    /// Layer 2.
    /// </summary>
    Two = 10, // Orange layer

    /// <summary>
    /// Layer 3.
    /// </summary>
    Three = 20, // Free spot

    /// <summary>
    /// Layer 4.
    /// </summary>
    Four = 30, // Blue layer

    /// <summary>
    /// Layer 5.
    /// </summary>
    Five = 40, // Free spot
}

/// <summary>
/// Different kinds of shapes.
/// </summary>
public enum ShapeType
{
    /// <summary>
    /// A rectangle.
    /// </summary>
    Rectangle = 0,

    /// <summary>
    /// A circle.
    /// </summary>
    Circle = 1,
}

/// <summary>
/// Represents the different text box settings for the text box scene.
/// </summary>
public enum TextBoxSetting
{
    /// <summary>
    /// The text box color.
    /// </summary>
    TextColor,

    /// <summary>
    /// The color of the text selection rectangle.
    /// </summary>
    SelectionColor,

    /// <summary>
    /// The color of the cursor.
    /// </summary>
    CursorColor,

    /// <summary>
    /// Moves the text box.
    /// </summary>
    MoveTextBox,

    /// <summary>
    /// The text box width.
    /// </summary>
    TextBoxWidth,

    /// <summary>
    /// The changing of the font.
    /// </summary>
    FontChange,
}

/// <summary>
/// Represents the different components of a color.
/// </summary>
public enum TxtBoxColorComponent
{
    /// <summary>
    /// The red component.
    /// </summary>
    Red,

    /// <summary>
    /// The green component.
    /// </summary>
    Green,

    /// <summary>
    /// The blue component.
    /// </summary>
    Blue,
}
