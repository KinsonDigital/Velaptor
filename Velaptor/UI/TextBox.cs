// <copyright file="TextBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Velaptor.Content;
using Velaptor.Graphics;

namespace Velaptor.UI;

/// <summary>
/// Provides the ability to enter text into a box.
/// </summary>
[ExcludeFromCodeCoverage] // TODO: Remove this once implementation is being worked on
// TODO: Left as internal to prevent library users from using the control until it is ready
internal sealed class TextBox : ControlBase
{
    private const int LEFTMARGIN = 5;
    private const int RIGHTMARGIN = 5;
    private readonly int visibleTextCharPosition;
    private readonly int charPosDelta;
    private readonly int characterPosition;
    private readonly int lastDirectionOfTravel;
    private Texture backgroundTexture;
    private int rightSide;
    private int leftSide;
    private bool cursorVisible;
    private int cursorElapsedMilliseconds;
    private Label text;
    private readonly IContentLoader contentLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the control.</param>
    public TextBox(IContentLoader contentLoader)
    {
        this.contentLoader = contentLoader;

        this.text = new Label(contentLoader, null, null)
        {
            Position = Point.Empty,
            Text = "Textbox Text",
        };

        // Set position with left and to margin taken into account
    }

    /// <summary>
    /// Gets the width of the <see cref="TextBox"/>.
    /// </summary>
    public override uint Width => this.backgroundTexture.Width;

    /// <summary>
    /// Gets the height of the <see cref="TextBox"/>.
    /// </summary>
    public override uint Height => this.backgroundTexture.Height;

    /// <summary>
    /// Gets or sets the font name of the <see cref="TextBox"/>.
    /// </summary>
    public string FontName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text in the <see cref="TextBox"/>.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Loads the content of the <see cref="TextBox"/>.
    /// </summary>
    public override void LoadContent() => this.text.LoadContent();

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

        this.text.Update(frameTime);

        ProcessKeys();

        this.cursorElapsedMilliseconds += frameTime.ElapsedTime.Milliseconds;

        if (this.cursorElapsedMilliseconds >= 500)
        {
            this.cursorElapsedMilliseconds = 0;
            this.cursorVisible = !this.cursorVisible;
        }
    }

    /// <inheritdoc/>
    public override void Render(IRenderer renderer)
    {
        if (IsLoaded is false || Visible is false)
        {
            return;
        }

        this.text.Render(renderer);
    }

    /// <summary>
    /// Processes any keyboard input inside of the <see cref="TextBox"/>.
    /// </summary>
    private static void ProcessKeys()
    {
    }

    /// <summary>
    /// Removes the characters using the backspace key.
    /// </summary>
    private static void RemoveCharacterUsingBackspace()
    {
        // Text = Text.Remove(Text.IndexOf(this.visibleText.Text, StringComparison.Ordinal) + this.characterPosition - 1, 1);
    }

    public void Dispose() => throw new NotImplementedException();
}
