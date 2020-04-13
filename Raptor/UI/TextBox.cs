using System;
using System.Numerics;
using Raptor.Content;
using Raptor.Graphics;
using Raptor.Input;

namespace Raptor.UI
{
    /// <summary>
    /// Provides the ability to enter text into a box.
    /// </summary>
    public class TextBox : IControl
    {
        #region Private Fields
        private readonly Keyboard _keyboard = new Keyboard();
        private readonly string _leftText = string.Empty;
        private GameText _visibleText;
        private static GameText _textRuler;//Used for measuring text with and height
        private int _visibleTextCharPosition;
        private int _charPosDelta;
        private Vector2 _textPosition;
        private int _characterPosition;
        private int _cursorElapsedMilliseconds;
        private bool _cursorVisible;
        private const int LEFT_MARGIN = 5;
        private const int RIGHT_MARGIN = 5;
        private int _rightSide;
        private int _leftSide;
        private int _lastDirectionOfTravel = 0;
        private readonly DeferredActions _deferredActions = new DeferredActions();
        #endregion
        

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="TextBox"/>.
        /// </summary>
        public TextBox() { }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the position of the text box.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets the width of the <see cref="TextBox"/>.
        /// </summary>
        public int Width => Background.Width;

        /// <summary>
        /// Gets the height of the <see cref="TextBox"/>.
        /// </summary>
        public int Height => Background.Height;

        /// <summary>
        /// Gets the background UI of the <see cref="TextBox"/>.
        /// </summary>
        public Texture Background { get; set; }

        /// <summary>
        /// Gets or sets the font name of the <see cref="TextBox"/>.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the text in the <see cref="TextBox"/>.
        /// </summary>
        public string Text { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Initializes the <see cref="TextBox"/>.
        /// </summary>
        public void Initialize() { }


        /// <summary>
        /// Loads the content of the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="contentLoader">The content loader used to load the content.</param>
        public void LoadContent(ContentLoader contentLoader)
        {
            _visibleText = contentLoader.LoadText(FontName);
            _textRuler = contentLoader.LoadText(FontName);

            _deferredActions.ExecuteAll();
        }


        /// <summary>
        /// Updates the texbox.
        /// </summary>
        /// <param name="engineTime">The game engine time.</param>
        public void Update(EngineTime engineTime)
        {
            UpdateSideLocations();

            ProcessKeys();

            _cursorElapsedMilliseconds += engineTime.ElapsedEngineTime.Milliseconds;

            if (_cursorElapsedMilliseconds >= 500)
            {
                _cursorElapsedMilliseconds = 0;
                _cursorVisible = !_cursorVisible;
            }
        }


        /// <summary>
        /// Renders the <see cref="TextBox"/> to the graphics surface.
        /// </summary>
        /// <param name="renderer">The renderer used to render the <see cref="TextBox"/>.</param>
        public void Render(Renderer renderer)
        {
            renderer.Render(Background, Position);

            //Update the X position of the text
            _textPosition = new Vector2(_leftSide, Position.Y - _visibleText.Height / 2f);

            //Render the text inside of the <see cref="TextBox"/>
            _visibleText.Text = ClipText(Text);

            renderer.Render(_visibleText, _textPosition, new GameColor(255, 0, 0, 0));

            //Render the end to cover any text that has passed the end of the render area
            var topLeftCorner = new Vector2(Position.X - Width / 2, Position.Y - Height / 2);

            var areaWidth = Width - (_rightSide - topLeftCorner.X);

            var coverArea = new Rect(Width - areaWidth, 0, areaWidth, Height);
            var coverPosition = new Vector2(454, 250);// new Vector2(_rightSide, Position.Y);

            renderer.RenderTextureArea(Background, coverArea, coverPosition);

            //DEBUGGING
            //Render the dot at the right side line
            renderer.FillCircle(new Vector2(_rightSide, Position.Y - Height / 2), 5, new GameColor(255, 125, 125, 0));

            //Render the margins for visual debugging
            var leftMarginStart = new Vector2(_leftSide, Position.Y - 50);
            var leftMarginStop = new Vector2(_leftSide, Position.Y + 50);
            renderer.Line(leftMarginStart, leftMarginStop, new GameColor(255, 0, 255, 0));

            var rightMarginStart = new Vector2(_rightSide, Position.Y - 50);
            var rightMarginStop = new Vector2(_rightSide, Position.Y + 50);
            renderer.Line(rightMarginStart, rightMarginStop, new GameColor(255, 0, 255, 0));
            ///////////

            //Render the blinking cursor
            var lineStart = CalcCursorStart();// new Vector2(cursorPositionX, Position.Y - (Background.Height / 2) + 3);
            var lineStop = CalcCursorStop();// new Vector2(cursorPositionX, Position.Y + (Background.Height / 2) - 3);

            lineStart.X = lineStart.X > _rightSide ? _rightSide : lineStart.X;
            lineStop.X = lineStop.X > _rightSide ? _rightSide : lineStop.X;

            if (_cursorVisible)
                renderer.Line(lineStart, lineStop, new GameColor(255, 255, 0, 0));//TODO: Change to black when finished with testing
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Updates the locations of the left and right side of the <see cref="TextBox"/>.
        /// </summary>
        private void UpdateSideLocations()
        {
            var halfWidth = Width / 2;

            _leftSide = (int)Position.X - halfWidth + LEFT_MARGIN;
            _rightSide = (int)Position.X + halfWidth - RIGHT_MARGIN;
        }


        /// <summary>
        /// Processess any keyboard input inside of the <see cref="TextBox"/>.
        /// </summary>
        private void ProcessKeys()
        {
            _keyboard.UpdateCurrentState();

            if (_keyboard.IsKeyPressed(KeyCodes.Right))
            {
                _lastDirectionOfTravel = 1;

                _characterPosition = _characterPosition > Text.Length - 1 ?
                    _characterPosition :
                    _characterPosition + 1;

                _visibleTextCharPosition = _visibleTextCharPosition > _visibleText.Text.Length - 1 ?
                    _visibleTextCharPosition :
                    _visibleTextCharPosition + 1;

                _charPosDelta = Math.Abs(_characterPosition - _visibleTextCharPosition);

                _keyboard.UpdatePreviousState();
                return;
            }

            if (_keyboard.IsKeyPressed(KeyCodes.Left))
            {
                _lastDirectionOfTravel = -1;

                _characterPosition = _characterPosition <= 0 ?
                    _characterPosition :
                    _characterPosition - 1;

                _visibleTextCharPosition = _visibleTextCharPosition == 0 ?
                    _visibleTextCharPosition :
                    _visibleTextCharPosition - 1;

                _charPosDelta = Math.Abs(_characterPosition - _visibleTextCharPosition);

                _keyboard.UpdatePreviousState();
                return;
            }

            var isShiftDown = _keyboard.IsKeyDown(KeyCodes.LeftShift) || _keyboard.IsKeyDown(KeyCodes.RightShift);

            if (!string.IsNullOrEmpty(_visibleText.Text))
            {
                //The delete keys. This is the standard one and the numpad one
                if(_keyboard.IsDeleteKeyPressed())
                {
                    _visibleText.Text = _visibleText.Text.Remove(_characterPosition, 1);
                }

                if (_keyboard.IsKeyPressed(KeyCodes.Back) && _characterPosition > 0)
                {
                    RemoveCharacterUsingBackspace();
                }
            }

            //If a letter is pressed, add it to the <see cref="TextBox"/>
            if (_keyboard.AnyLettersPressed(out KeyCodes letter))
            {
                var letterText = "";

                if (letter == KeyCodes.Space)
                {
                    letterText = " ";
                }
                else
                {
                    letterText = isShiftDown || _keyboard.CapsLockOn ?
                        letter.ToString() :
                        letter.ToString().ToLower();
                }

                _visibleText.Text = _visibleText.Text.Insert(_characterPosition, letterText);
                _characterPosition += 1;
            }

            //If a number was pressed on the keyboard
            if (_keyboard.AnyNumbersPressed(out KeyCodes number))
            {
                var character = _keyboard.KeyToChar(number).ToString();

                _visibleText.Text = _visibleText.Text.Insert(_characterPosition, character);

                _characterPosition += 1;
            }


            _keyboard.UpdatePreviousState();
        }


        /// <summary>
        /// Calculates the starting position of the cursor inside of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns></returns>
        private Vector2 CalcCursorStart() => new Vector2(_leftSide + CalcCursorXPos(), Position.Y - (Background.Height / 2) + 3);


        /// <summary>
        /// Calculates the stopping position of the cursor insdie of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns></returns>
        private Vector2 CalcCursorStop() => new Vector2(_leftSide + CalcCursorXPos(), Position.Y + (Background.Height / 2) - 3);


        /// <summary>
        /// Removes the characters using the backspace key.
        /// </summary>
        private void RemoveCharacterUsingBackspace() => Text = Text.Remove(Text.IndexOf(_visibleText.Text) + _characterPosition - 1, 1);


        /// <summary>
        /// Calculates the cursor position inside of the <see cref="TextBox"/>.
        /// </summary>
        /// <returns></returns>
        private int CalcCursorXPos()
        {
            _textRuler.Text = string.Empty;

            //Update the text that is from the first letter up to the cursor position
            _textRuler.Text = Text.Substring(_charPosDelta, Math.Abs(_characterPosition - _charPosDelta));

            var result = _textRuler.Width;

            _textRuler.Text = string.Empty;

            return result;
        }


        /// <summary>
        /// The amount of text to be clipped from the text box that is past
        /// the right side of the <see cref="TextBox"/>.
        /// </summary>
        /// <param name="text">The text of the <see cref="TextBox"/>.</param>
        /// <returns></returns>
        private string ClipText(string text)
        {
            _textRuler.Text = string.Empty;

            var textAreaWidth = _rightSide - _leftSide;

            var startIndex = _charPosDelta == 0 ?
                0 :
                _charPosDelta + _lastDirectionOfTravel;

            for (int i = startIndex; i < text.Length; i++)
            {
                _textRuler.Text += Text[i].ToString();

                //If the text is currently too wide to fit, remove one character
                if (_textRuler.Width > textAreaWidth)
                {
                    _textRuler.Text = _textRuler.Text.Substring(0, _textRuler.Text.Length - 1);
                    break;
                }
            }


            var result = _textRuler.Text;

            _textRuler.Text = string.Empty;


            return result;
        }
        #endregion
    }
}
