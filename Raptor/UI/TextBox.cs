// <copyright file="TextBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.UI
{
    using System;
    using System.Globalization;
    using System.Numerics;
    using Raptor.Content;
    using Raptor.Graphics;

    /// <summary>
    /// Provides the ability to enter text into a box.
    /// </summary>
    public class TextBox : IControl
    {
        private const int LEFTMARGIN = 5;
        private const int RIGHTMARGIN = 5;
        private readonly RenderText? textRuler; // Used for measuring text with and height
        private readonly RenderText? visibleText;
        private int visibleTextCharPosition;
        private int charPosDelta;
        private int characterPosition;
        private int cursorElapsedMilliseconds;
        private bool cursorVisible;
        private int rightSide;
        private int leftSide;
        private int lastDirectionOfTravel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBox"/> class.
        /// </summary>
        /// <param name="backgroundTexture">The background texture to be displayed in the text box.</param>
        public TextBox(Texture backgroundTexture)
        {
            BackgroundTexture = backgroundTexture;
            this.textRuler = new RenderText();
            this.visibleText = new RenderText();
        }

        /// <summary>
        /// Gets or sets the position of the text box.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets the width of the <see cref="TextBox"/>.
        /// </summary>
        public int Width => BackgroundTexture.Width;

        /// <summary>
        /// Gets the height of the <see cref="TextBox"/>.
        /// </summary>
        public int Height => BackgroundTexture.Height;

        /// <summary>
        /// Gets the background UI of the <see cref="TextBox"/>.
        /// </summary>
        public Texture BackgroundTexture { get; }

        /// <summary>
        /// Gets or sets the font name of the <see cref="TextBox"/>.
        /// </summary>
        public string FontName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text in the <see cref="TextBox"/>.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Initializes the <see cref="TextBox"/>.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Loads the content of the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="contentLoader">The content loader used to load the content.</param>
        public void LoadContent(ContentLoader contentLoader)
        {
            if (contentLoader is null)
            {
                throw new ArgumentNullException(nameof(contentLoader), "The content loader must not be null.");
            }

                // TODO: Currently for every single instance of this class TextBox, there will be 2 RenderText objects created.
                // One of them is for the TextBox itself and the other is for the purpose of measuring the text inside of the box.
                // This is not ideal.  Try to figure out a way to measure text without the use of another RenderText object.  This is
                // not best for performance as well as taking extra memory.
                // _visibleText = contentLoader.LoadText(FontName);
                // _textRuler = contentLoader.LoadText(FontName);
            //this.deferredActions.ExecuteAll();
        }

        /// <summary>
        /// Updates the text box.
        /// </summary>
        /// <param name="engineTime">The update iteration time.</param>
        public void Update(FrameTime engineTime)
        {
            UpdateSideLocations();

            ProcessKeys();

            this.cursorElapsedMilliseconds += engineTime.ElapsedTime.Milliseconds;

            if (this.cursorElapsedMilliseconds >= 500)
            {
                this.cursorElapsedMilliseconds = 0;
                this.cursorVisible = !this.cursorVisible;
            }
        }

        /// <summary>
        /// Renders the <see cref="TextBox"/> to the graphics surface.
        /// </summary>
        /// <param name="renderer">The renderer used to render the <see cref="TextBox"/>.</param>
        public void Render(object renderer)
        {
            // if (renderer is null)
            //    throw new ArgumentNullException(nameof(renderer), "The renderer must not be null.");

            // renderer.Render(BackgroundTexture, Position);

            ////Update the X position of the text
            // _textPosition = new Vector2(_leftSide, Position.Y - (_visibleText is null ? 0 : _visibleText.Height / 2f));

            ////Render the text inside of the text box
            // if (!(_visibleText is null))
            // {
            //    _visibleText.Text = ClipText(Text);
            //    renderer.Render(_visibleText, _textPosition, Color.FromArgb(255, 0, 0, 0));
            // }

            ////Render the end to cover any text that has passed the end of the render area
            // var topLeftCorner = new Vector2(Position.X - Width / 2, Position.Y - Height / 2);

            // var areaWidth = Width - (_rightSide - topLeftCorner.X);

            // var coverArea = new Rectangle((int)(Width - areaWidth), 0, (int)areaWidth, (int)Height);

            // var coverPosition = new Vector2(454, 250);// new Vector2(_rightSide, Position.Y);

            // renderer.RenderTextureArea(BackgroundTexture, coverArea, coverPosition);

            ////TODO: Figure out is the code in the debugging comment section below is needed and if not, remove it
            ////DEBUGGING
            ////Render the dot at the right side line
            // renderer.FillCircle(new Vector2(_rightSide, Position.Y - Height / 2), 5, Color.FromArgb(255, 125, 125, 0));

            ////Render the margins for visual debugging
            // var leftMarginStart = new Vector2(_leftSide, Position.Y - 50);
            // var leftMarginStop = new Vector2(_leftSide, Position.Y + 50);
            // renderer.Line(leftMarginStart, leftMarginStop, Color.FromArgb(255, 0, 255, 0));

            // var rightMarginStart = new Vector2(_rightSide, Position.Y - 50);
            // var rightMarginStop = new Vector2(_rightSide, Position.Y + 50);
            // renderer.Line(rightMarginStart, rightMarginStop, Color.FromArgb(255, 0, 255, 0));
            /////////////

            ////Render the blinking cursor
            // var lineStart = CalcCursorStart();// new Vector2(cursorPositionX, Position.Y - (Background.Height / 2) + 3);
            // var lineStop = CalcCursorStop();// new Vector2(cursorPositionX, Position.Y + (Background.Height / 2) - 3);

            // lineStart.X = lineStart.X > _rightSide ? _rightSide : lineStart.X;
            // lineStop.X = lineStop.X > _rightSide ? _rightSide : lineStop.X;

            // if (_cursorVisible)
            //    renderer.Line(lineStart, lineStop, Color.FromArgb(255, 255, 0, 0));//TODO: Change to black when finished with testing
        }

        /// <summary>
        /// Updates the locations of the left and right side of the <see cref="TextBox"/>.
        /// </summary>
        private void UpdateSideLocations()
        {
            var halfWidth = Width / 2;

            this.leftSide = (int)Position.X - halfWidth + LEFTMARGIN;
            this.rightSide = (int)Position.X + halfWidth - RIGHTMARGIN;
        }

        /// <summary>
        /// Processess any keyboard input inside of the <see cref="TextBox"/>.
        /// </summary>
        private void ProcessKeys()
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

            // if (!(this.visibleText is null) && !string.IsNullOrEmpty(this.visibleText.Text))
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

            // if (!(this.visibleText is null))
            //        this.visibleText.Text = this.visibleText.Text.Insert(this.characterPosition, letterText);

            // this.characterPosition += 1;
            // }

            // TODO: Need to change how we can check for pressed numbers AND to return the number itself.  Tuples?
            //// If a number was pressed on the keyboard
            // if (this.keyboard.AnyNumbersPressed(out var number))
            // {
            //    var character = this.keyboard.KeyToChar(number).ToString(CultureInfo.InvariantCulture);

            // if (!(this.visibleText is null))
            //        this.visibleText.Text = this.visibleText.Text.Insert(this.characterPosition, character);

            // this.characterPosition += 1;
            // }

            // this.keyboard.UpdatePreviousState();
        }

        /// <summary>
        /// Calculates the starting position of the cursor inside of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns>Result of the calculated start position of the textbox cursor.</returns>
        private Vector2 CalcCursorStart() => new Vector2(this.leftSide + CalcCursorXPos(), Position.Y - (BackgroundTexture.Height / 2) + 3);

        /// <summary>
        /// Calculates the stopping position of the cursor insdie of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns>Result of the calcualted end position of the textbox cursor.</returns>
        private Vector2 CalcCursorStop() => new Vector2(this.leftSide + CalcCursorXPos(), Position.Y + (BackgroundTexture.Height / 2) - 3);

        /// <summary>
        /// Removes the characters using the backspace key.
        /// </summary>
        private void RemoveCharacterUsingBackspace()
        {
            if (this.visibleText is null)
            {
                return;
            }

            Text = Text.Remove(Text.IndexOf(this.visibleText.Text, StringComparison.Ordinal) + this.characterPosition - 1, 1);
        }

        /// <summary>
        /// Calculates the cursor position inside of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns>The calculated X position of the cursor.</returns>
        private int CalcCursorXPos()
        {
            if (this.textRuler is null)
            {
                return 0;
            }

            this.textRuler.Text = string.Empty;

            // Update the text that is from the first letter up to the cursor position
            this.textRuler.Text = Text.Substring(this.charPosDelta, Math.Abs(this.characterPosition - this.charPosDelta));

            var result = this.textRuler.Width;

            this.textRuler.Text = string.Empty;

            return result;
        }

        /// <summary>
        /// The amount of text to be clipped from the text box that is past
        /// the right side of the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="text">The text of the <see cref="TextBox"/>.</param>
        /// <returns>The text that is clipped.</returns>
        private string ClipText(string text)
        {
            if (this.textRuler is null)
            {
                return string.Empty;
            }

            this.textRuler.Text = string.Empty;

            var textAreaWidth = this.rightSide - this.leftSide;

            var startIndex = this.charPosDelta == 0 ?
                0 :
                this.charPosDelta + this.lastDirectionOfTravel;

            for (var i = startIndex; i < text.Length; i++)
            {
                this.textRuler.Text += Text[i].ToString(CultureInfo.InvariantCulture);

                // If the text is currently too wide to fit, remove one character
                if (this.textRuler.Width > textAreaWidth)
                {
                    this.textRuler.Text = this.textRuler.Text[0..^1];
                    break;
                }
            }

            var result = this.textRuler.Text;

            this.textRuler.Text = string.Empty;

            return result;
        }
    }
}
