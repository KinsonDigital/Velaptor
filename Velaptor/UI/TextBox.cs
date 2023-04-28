// <copyright file="TextBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using Content;
using Content.Fonts;
using Factories;

/// <summary>
/// Provides the ability to enter text into a box.
/// </summary>
public sealed class TextBox : ControlBase
{
    private readonly ILoader<IFont> contentLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox"/> class.
    /// </summary>
    public TextBox()
    {
        this.contentLoader = ContentLoaderFactory.CreateFontLoader();
    }

    /// <summary>
    /// Gets or sets the text in the <see cref="TextBox"/>.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Loads the content of the <see cref="TextBox"/>.
    /// </summary>
    public override void LoadContent()
    {
    }

    /// <summary>
    /// Updates the text box.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    public override void Update(FrameTime frameTime)
    {
        if (IsLoaded is false || Enabled is false)
        {
            return;
        }
    }

    /// <inheritdoc/>
    public override void Render()
    {
        if (IsLoaded is false || Visible is false)
        {
            return;
        }

    }

    /// <summary>
    /// Processes any keyboard input inside of the <see cref="TextBox"/>.
    /// </summary>
    private static void ProcessKeys()
    {
    }
}
