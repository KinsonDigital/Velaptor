// <copyright file="TextCursor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Carbonate.UniDirectional;
using ExtensionMethods;
using Graphics;
using Input;
using ReactableData;

/// <summary>
/// Manages the cursor for a text box.
/// </summary>
internal class TextCursor : ITextCursor
{
    private readonly Guid textBoxDataEventId = new ("71931561-826B-431B-BCE6-B139034A1FF4");
    private readonly IDisposable unsubscriber;
    private TextBoxStateData preMutateState;
    private TextBoxStateData postMutateState;
    private RectShape cursor;
    private KeyCode lastMovementKey;
    private bool visible = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextCursor"/> class.
    /// </summary>
    /// <param name="textBoxStateReactable">Receives notifications of text box state data.</param>
    public TextCursor(IPushReactable<TextBoxStateData> textBoxStateReactable)
    {
        this.unsubscriber = textBoxStateReactable.Subscribe(new ReceiveReactor<TextBoxStateData>(
            eventId: this.textBoxDataEventId,
            name: "TextBoxStateDataUpdate",
            onReceiveData: data =>
            {
                switch (data.TextMutateType)
                {
                    case MutateType.PreMutate:
                        this.preMutateState = data;
                        break;
                    case MutateType.PostMutate:
                        this.postMutateState = data;

                        if (this.postMutateState.Key.IsMoveCursorKey())
                        {
                            this.lastMovementKey = this.postMutateState.Key;
                        }

                        break;
                    default:
                        const string argName = $"{nameof(TextBoxStateData)}.{nameof(TextBoxStateData.TextMutateType)}";
                        throw new InvalidEnumArgumentException(argName, (int)this.postMutateState.TextMutateType, typeof(MutateType));
                }
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));

        Cursor = new RectShape
        {
            Color = Color.CornflowerBlue,
            Width = 2,
            IsSolid = true,
        };
    }

    /// <inheritdoc/>
    public RectShape Cursor
    {
        get => this.cursor;
        internal set => this.cursor = value;
    }

    /// <inheritdoc/>
    public int Width
    {
        get => (int)this.cursor.Width;
        set => this.cursor.Width = value;
    }

    /// <inheritdoc/>
    public int Height
    {
        get => (int)this.cursor.Height;
        set => this.cursor.Height = value;
    }

    /// <inheritdoc/>
    public Vector2 Position
    {
        get => this.cursor.Position;
        set => this.cursor.Position = value;
    }

    /// <inheritdoc/>
    public Color Color
    {
        get => this.cursor.Color;
        set => this.cursor.Color = value;
    }

    /// <inheritdoc/>
    public bool Visible
    {
        get => this.postMutateState.Width > 0 && this.visible;
        set => this.visible = value;
    }

    /// <inheritdoc/>
    public void Update()
    {
        switch (this.postMutateState.Event)
        {
            case TextBoxEvent.None:
                break;
            case TextBoxEvent.AddingCharacter:
                HandleAddCharEvent();
                break;
            case TextBoxEvent.RemovingSingleChar:
                HandleRemovingSingleCharEvent(this.postMutateState.Key);
                break;
            case TextBoxEvent.RemovingSelectedChars:
                HandleRemovingSelectedCharsEvent();
                break;
            case TextBoxEvent.MovingCursor:
                HandleMovingCursorEvent(this.postMutateState.Key);
                break;
            case TextBoxEvent.DimensionalChange:
                HandleTextBoxResizing();
                break;
            case TextBoxEvent.Movement:
                HandleTextBoxMovement();
                break;
            default:
                const string argName = $"{nameof(TextBoxStateData)}.{nameof(TextBoxStateData.Event)}";
                throw new InvalidEnumArgumentException(argName, (int)this.postMutateState.Event, typeof(TextBoxEvent));
        }
    }

    /// <inheritdoc/>
    public void SnapCursorToLeft() => this.cursor.Position = this.cursor.Position with { X = this.postMutateState.TextLeft };

    /// <inheritdoc/>
    public void SnapCursorToRight() => this.cursor.Position = this.cursor.Position with { X = this.postMutateState.TextRight };

    /// <inheritdoc/>
    public void SnapToFirstVisibleChar() => this.cursor.Position = this.cursor.Position with
    {
        X = this.postMutateState.FirstVisibleCharBounds.Left,
    };

    /// <inheritdoc/>
    public void SnapToLastVisibleChar() => this.cursor.Position = this.cursor.Position with
    {
        X = this.postMutateState.LastVisibleCharBounds.Right,
    };

    /// <summary>
    /// Handles the cursor position related to the <see cref="TextBoxEvent.MovingCursor"/> event.
    /// </summary>
    /// <param name="key">The keyboard key related to the event.</param>
    private void HandleMovingCursorEvent(KeyCode key)
    {
        var charIndexWasAtEnd = this.preMutateState.CharIndex >= this.preMutateState.TextLength - 1;
        var charIndexIsAtEnd = this.postMutateState.CharIndex >= this.postMutateState.TextLength - 1;

        var toTheEndKey = key is KeyCode.End or KeyCode.PageDown;

        if ((key == KeyCode.Right && charIndexWasAtEnd && charIndexIsAtEnd) || toTheEndKey)
        {
            SetCursorToCharRight();
        }
        else
        {
            SetCursorToCharLeft();
        }
    }

    /// <summary>
    /// Handles the cursor position related to the <see cref="TextBoxEvent.AddingCharacter"/> event.
    /// </summary>
    private void HandleAddCharEvent()
    {
        var cursorWasAtRightEndOfText = this.preMutateState.Text.IsEmpty() || Cursor.Right > this.preMutateState.TextRight;
        var charIndexIsAtEnd = this.postMutateState.CharIndex >= this.postMutateState.TextLength - 1;

        if (cursorWasAtRightEndOfText && charIndexIsAtEnd)
        {
            SetCursorToCharRight();
        }
        else
        {
            SetCursorToCharLeft();
        }
    }

    /// <summary>
    /// Handles the cursor position related to the <see cref="TextBoxEvent.RemovingSingleChar"/> event
    /// when removing a character with the <see cref="KeyCode.Backspace"/> key.
    /// </summary>
    private void HandleBackspaceSingleCharEvent()
    {
        // NOTE: Do not use the 'Text' property to get its length on the preMutateState.  It is a reference
        // to the original text and does not reflect the state before the text was mutated.
        var charIndexWasAtEnd = this.preMutateState.CharIndex >= this.preMutateState.TextLength - 1;
        var cursorWasAtRightEndOfText = this.preMutateState.Text.IsEmpty() || Cursor.Right > this.preMutateState.TextRight;
        var cursorWasNotAtRightEndOfText = !cursorWasAtRightEndOfText;

        var charIndexIsAtStart = this.postMutateState.CharIndex <= 0;
        var charIndexIsAtEnd = this.postMutateState.CharIndex >= this.postMutateState.TextLength - 1;
        var charIndexIsInMiddle = charIndexIsAtStart is false && charIndexIsAtEnd is false;

        var charIndexIsStartOrMiddle = charIndexIsAtStart || charIndexIsInMiddle;
        var cursorIsNotAtRightEndOfText = this.postMutateState.Text.IsEmpty() is false && this.cursor.Right <= this.postMutateState.TextRight;

        if ((charIndexWasAtEnd && cursorWasNotAtRightEndOfText && charIndexIsAtEnd) ||
                 (charIndexIsStartOrMiddle && cursorIsNotAtRightEndOfText))
        {
            SetCursorToCharLeft();
        }
        else if (charIndexWasAtEnd && cursorWasAtRightEndOfText && charIndexIsAtEnd)
        {
            SetCursorToCharRight();
        }
    }

    /// <summary>
    /// Handles the cursor position related to the <see cref="TextBoxEvent.RemovingSingleChar"/> event
    /// when removing a character with the <see cref="KeyCode.Delete"/> key.
    /// </summary>
    private void HandleDeleteSingleCharEvent()
    {
        // NOTE: Do not use the 'Text' property to get its length on the preMutateState.  It is a reference
        // to the original text and does not reflect the state before the text was mutated.
        var charIndexWasNotAtStart = this.preMutateState.CharIndex > 0;
        var charIndexWasAtEnd = this.preMutateState.CharIndex >= this.preMutateState.TextLength - 1;
        var charIndexWasInMiddle = charIndexWasNotAtStart && !charIndexWasAtEnd;
        var cursorWasNotAtRightEndOfText = this.preMutateState.Text.IsEmpty() is false && Cursor.Right <= this.preMutateState.TextRight;

        var charIndexIsAtStart = this.postMutateState.CharIndex <= 0;
        var charIndexIsAtEnd = this.postMutateState.CharIndex >= this.postMutateState.TextLength - 1;

        var cursorIsAtRightEndOfText = this.postMutateState.Text.IsEmpty() || this.cursor.Right > this.postMutateState.TextRight;
        var cursorIsNotAtRightEndOfText = !cursorIsAtRightEndOfText;

        if (charIndexWasInMiddle)
        {
            SetCursorToCharLeft();
        }
        else if (charIndexWasAtEnd && charIndexIsAtEnd && cursorWasNotAtRightEndOfText && cursorIsAtRightEndOfText)
        {
            SetCursorToCharRight();
        }
        else if (charIndexIsAtStart && charIndexIsAtEnd && cursorWasNotAtRightEndOfText)
        {
            SetCursorToCharLeft();
        }
        else if (charIndexIsAtEnd && cursorWasNotAtRightEndOfText && cursorIsNotAtRightEndOfText)
        {
            SetCursorToCharRight();
        }
    }

    /// <summary>
    /// Handles the cursor position related to removing characters with the <see cref="KeyCode.Backspace"/>
    /// or <see cref="KeyCode.Delete"/> key.
    /// </summary>
    /// <param name="key">The key used to remove the characters.</param>
    /// <exception cref="InvalidEnumArgumentException">
    ///     Occurs if the <paramref name="key"/> is not the <see cref="KeyCode.Backspace"/> or <see cref="KeyCode.Delete"/> key.
    /// </exception>
    private void HandleRemovingSingleCharEvent(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Backspace:
                HandleBackspaceSingleCharEvent();
                break;
            case KeyCode.Delete:
                HandleDeleteSingleCharEvent();
                break;
            default:
            {
                const string keyArgName = $"{nameof(this.postMutateState)}.{nameof(this.postMutateState.Key)}";
                throw new InvalidEnumArgumentException(keyArgName, (int)this.postMutateState.Key, typeof(KeyCode));
            }
        }
    }

    /// <summary>
    /// Handles the cursor position related to removing characters with the <see cref="KeyCode.Backspace"/>
    /// or <see cref="KeyCode.Delete"/> key.
    /// </summary>
    private void HandleRemovingSelectedCharsEvent()
    {
        var selectionLeftToRight = this.preMutateState.SelectionStartIndex < this.preMutateState.SelectionStopIndex;
        var selectionRightToLeft = this.preMutateState.SelectionStartIndex > this.preMutateState.SelectionStopIndex;

        var prevTextWidth = Math.Abs(this.preMutateState.TextLeft - this.preMutateState.TextRight);
        var currTextWidth = Math.Abs(this.postMutateState.TextLeft - this.postMutateState.TextRight);
        var displacement = Math.Abs(prevTextWidth - currTextWidth);

        var prevTextViewWidth = Math.Abs(this.preMutateState.TextView.Left - this.preMutateState.TextView.Right);
        var currTextViewWidth = Math.Abs(this.postMutateState.TextView.Left - this.postMutateState.TextView.Right);

        var prevTextIsLargerThanView = prevTextWidth >= prevTextViewWidth;
        var currTextIsLargerThanView = currTextWidth >= currTextViewWidth;
        var firstCharVisible = this.postMutateState.TextLeft >= this.postMutateState.TextView.Left;

        var lastCharVisible = this.postMutateState.TextRight <= this.postMutateState.TextView.Right;

        if (currTextIsLargerThanView)
        {
            if (firstCharVisible && selectionLeftToRight)
            {
                this.cursor.Left -= displacement;
            }
            else if (lastCharVisible)
            {
                if (selectionLeftToRight)
                {
                    displacement = Math.Abs(this.preMutateState.TextRight - this.postMutateState.TextRight);
                    this.cursor.Left -= displacement;
                }
                else if (selectionRightToLeft)
                {
                    displacement = lastCharVisible
                        ? Math.Abs(this.preMutateState.TextLeft - this.postMutateState.TextLeft)
                        : displacement;
                    this.cursor.Left += displacement;
                }
            }
        }
        else
        {
            if (this.postMutateState.Text.IsEmpty())
            {
                this.cursor.Left = this.postMutateState.TextView.Left;
            }
            else if (selectionLeftToRight)
            {
                this.cursor.Left -= displacement;
            }
            else if (selectionLeftToRight is false && selectionRightToLeft is false)
            {
                this.cursor.Left = this.postMutateState.TextRight;
            }
            else if (prevTextIsLargerThanView && !currTextIsLargerThanView)
            {
                displacement = Math.Abs(this.preMutateState.TextLeft - this.postMutateState.TextLeft);

                if (selectionRightToLeft)
                {
                    this.cursor.Left += displacement;
                }
            }
        }
    }

    /// <summary>
    /// Handles the cursor position when the text box has resized.
    /// </summary>
    private void HandleTextBoxResizing()
    {
        var postTextWidth = Math.Abs(this.postMutateState.TextLeft - this.postMutateState.TextRight);

        var preTextViewWidth = this.preMutateState.TextView.Width;
        var postTextViewWidth = this.postMutateState.TextView.Width;

        var postTextViewHeight = this.postMutateState.TextView.Height;

        var textWidthSmallerThanView = postTextWidth < postTextViewWidth;
        var widthHasGrown = postTextViewWidth > preTextViewWidth;

        if (textWidthSmallerThanView)
        {
            var horizontalDisplacement = Math.Abs(preTextViewWidth - postTextViewWidth).Half();

            if (widthHasGrown)
            {
                // Move cursor to the left
                this.cursor.Position = this.cursor.Position with { X = this.cursor.Position.X - horizontalDisplacement };
            }
            else
            {
                // Move cursor to the right
                this.cursor.Position = this.cursor.Position with { X = this.cursor.Position.X + horizontalDisplacement };
            }
        }
        else
        {
            var firstCharVisible = this.postMutateState.TextLeft >= this.postMutateState.TextView.Left;
            var lastCharVisible = this.postMutateState.TextRight <= this.postMutateState.TextView.Right;

            if (firstCharVisible)
            {
                SnapCursorToLeft();
            }

            if (lastCharVisible)
            {
                SnapCursorToRight();
            }

            if (firstCharVisible is false && lastCharVisible is false)
            {
                var cursorPastRight = this.cursor.Position.X > this.postMutateState.TextView.Right;
                var cursorPastLeft = this.cursor.Position.X < this.postMutateState.TextView.Left;

                if (cursorPastRight)
                {
                    SnapToLastVisibleChar();
                }
                else if (cursorPastLeft)
                {
                    SnapToFirstVisibleChar();
                }
            }
        }

        this.cursor.Height = postTextViewHeight - 12;
    }

    private void HandleTextBoxMovement()
    {
        var deltaX = this.postMutateState.Position.X - this.preMutateState.Position.X;
        var deltaY = this.postMutateState.Position.Y - this.preMutateState.Position.Y;

        this.cursor.Position = new Vector2(this.cursor.Position.X + deltaX, this.cursor.Position.Y + deltaY);
    }

    /// <summary>
    /// Sets the position of the cursor to the left side of the character.
    /// </summary>
    private void SetCursorToCharLeft() =>
        this.cursor.Left = this.postMutateState.Text.IsEmpty()
            ? this.postMutateState.TextLeft
            : this.postMutateState.CurrentCharLeft;

    /// <summary>
    /// Sets the position of the cursor to the right side of the character.
    /// </summary>
    private void SetCursorToCharRight() =>
        this.cursor.Left = this.postMutateState.Text.IsEmpty()
            ? this.postMutateState.TextView.Left
            : this.postMutateState.CurrentCharRight;
}
