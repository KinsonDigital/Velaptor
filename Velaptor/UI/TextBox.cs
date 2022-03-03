// <copyright file="TextBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Velaptor.Content;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides the ability to enter text into a box.
    /// </summary>
    [ExcludeFromCodeCoverage] // TODO: Remove this once implementation is being worked on
    // TODO: Left as internal to prevent library users from using the control until it is ready
    internal class TextBox : ControlBase
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

            // TODO: Setup textbox border rect

            this.text = new Label(contentLoader, null)
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
        // TODO: Map this value to the label text
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
        public override void Render(ISpriteBatch spriteBatch)
        {
            if (IsLoaded is false || Visible is false)
            {
                return;
            }

            this.text.Render(spriteBatch);
        }

        /// <summary>
        /// Processes any keyboard input inside of the <see cref="TextBox"/>.
        /// </summary>
        private static void ProcessKeys()
        {
            // TODO: Need to get this working with new Keyboard implementation using the KeyboardState struct
            // this.keyboard.UpdateCurrentState();

            // if (this.keyboard.IsKeyPressed(KeyCode.Right))
            // {
            //    this.lastDirectionOfTravel = 1;

            // this.characterPosition = this.characterPosition > Text.Length - 1 ?
            //        this.characterPosition :
            //        this.characterPosition + 1;

            // this.visibleTextCharPosition = this.visibleTextCharPosition > (this.visibleText is null ? 0 : this.visibleText.Text.Length - 1) ?
            //        this.visibleTextCharPosition :
            //        this.visibleTextCharPosition + 1;

            // this.charPosDelta = Math.Abs(this.characterPosition - this.visibleTextCharPosition);

            // this.keyboard.UpdatePreviousState();
            //    return;
            // }

            // if (this.keyboard.IsKeyPressed(KeyCode.Left))
            // {
            //    this.lastDirectionOfTravel = -1;

            // this.characterPosition = this.characterPosition <= 0 ?
            //        this.characterPosition :
            //        this.characterPosition - 1;

            // this.visibleTextCharPosition = this.visibleTextCharPosition == 0 ?
            //        this.visibleTextCharPosition :
            //        this.visibleTextCharPosition - 1;

            // this.charPosDelta = Math.Abs(this.characterPosition - this.visibleTextCharPosition);

            // this.keyboard.UpdatePreviousState();
            //    return;
            // }

            // var isShiftDown = this.keyboard.IsKeyDown(KeyCode.LeftShift) || this.keyboard.IsKeyDown(KeyCode.RightShift);

            // if (this.visibleText is not null && !string.IsNullOrEmpty(this.visibleText.Text))
            // {
            //    // The delete keys. This is the standard one and the numpad one
            //    if (this.keyboard.IsDeleteKeyPressed())
            //    {
            //        this.visibleText.Text = this.visibleText.Text.Remove(this.characterPosition, 1);
            //    }

            // if (this.keyboard.IsKeyPressed(KeyCode.Back) && this.characterPosition > 0)
            //    {
            //        RemoveCharacterUsingBackspace();
            //    }
            // }

            // TODO: Need to change how we can check for pressed letters AND to return the letter itself.  Tuples?
            // If a letter is pressed, add it to the <see cref="TextBox"/>
            // if (this.keyboard.AnyLettersPressed(out var letter))
            // {
            //    string letterText;

            // if (letter == KeyCode.Space)
            //    {
            //        letterText = " ";
            //    }
            //    else
            //    {
            //        letterText = isShiftDown || this.keyboard.CapsLockOn ?
            //            letter.ToString() :
            //            letter.ToString().ToUpperInvariant();
            //    }

            // if (this.visibleText is not null)
            //        this.visibleText.Text = this.visibleText.Text.Insert(this.characterPosition, letterText);

            // this.characterPosition += 1;
            // }

            // TODO: Need to change how we can check for pressed numbers AND to return the number itself.  Tuples?
            //// If a number was pressed on the keyboard
            // if (this.keyboard.AnyNumbersPressed(out var number))
            // {
            //    var character = this.keyboard.KeyToChar(number).ToString(CultureInfo.InvariantCulture);

            // if (this.visibleText is not null)
            //        this.visibleText.Text = this.visibleText.Text.Insert(this.characterPosition, character);

            // this.characterPosition += 1;
            // }

            // this.keyboard.UpdatePreviousState();
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
}
