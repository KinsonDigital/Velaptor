// <copyright file="TextSelection.cs" company="KinsonDigital">
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

/// <inheritdoc/>
internal class TextSelection : ITextSelection
{
    private readonly Guid textBoxDataEventId = new ("71931561-826B-431B-BCE6-B139034A1FF4");
    private readonly IDisposable textBoxDataUnsubscriber;
    private TextBoxStateData preMutateState;
    private TextBoxStateData postMutateState;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextSelection"/> class.
    /// </summary>
    /// <param name="textBoxDataReactable">Receives notifications of the state of the text box.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the type of text mutation is invalid.
    /// </exception>
    public TextSelection(IPushReactable<TextBoxStateData> textBoxDataReactable)
    {
        this.textBoxDataUnsubscriber = textBoxDataReactable.Subscribe(new ReceiveReactor<TextBoxStateData>(
            eventId: this.textBoxDataEventId,
            name: "TextBoxStateDataUpdate",
            onReceiveData: textBoxData =>
            {
                switch (textBoxData.TextMutateType)
                {
                    case MutateType.PreMutate:
                        this.preMutateState = textBoxData;
                        break;
                    case MutateType.PostMutate:
                        this.postMutateState = textBoxData;
                        break;
                    default:
                        const string argName = $"{nameof(TextBoxStateData)}.{nameof(TextBoxStateData.TextMutateType)}";
                        throw new InvalidEnumArgumentException(argName, (int)textBoxData.TextMutateType, typeof(MutateType));
                }
            },
            onUnsubscribe: () => this.textBoxDataUnsubscriber?.Dispose()));

        SelectionRect = SelectionRect with { Color = Color.CornflowerBlue };
    }

    /// <inheritdoc/>
    public RectShape SelectionRect { get; private set; }

    /// <inheritdoc/>
    public string SelectedText { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public Color SelectionColor
    {
        get => SelectionRect.Color;
        set => SelectionRect = SelectionRect with { Color = value };
    }

    /// <inheritdoc/>
    public void Update()
    {
        var key = this.postMutateState.Key;

        if (key.IsNotMoveCursorKey() && key.IsNotVisibleKey() && key.IsNotDeletionKey())
        {
            return;
        }

        var startIndex = Math.Min(this.postMutateState.SelectionStartIndex, this.postMutateState.SelectionStopIndex);

        int selectionLen;
        float rectRight;

        var movingToTheRight = key is KeyCode.Right or KeyCode.End or KeyCode.PageDown;

        if (movingToTheRight)
        {
            (selectionLen, rectRight) = HandleMovingToTheRight();
        }
        else
        {
            (selectionLen, rectRight) = HandleMovingToTheLeft();
        }

        if (this.postMutateState.InSelectionMode is false)
        {
            return;
        }

        var rectLeft = this.postMutateState.Text.IsEmpty()
            ? 0
            : Math.Min(this.postMutateState.SelStartCharBounds.Left, this.postMutateState.SelStopCharBounds.Left);

        rectLeft = rectLeft < this.postMutateState.TextView.Left
            ? this.postMutateState.TextView.Left
            : rectLeft;

        rectRight = rectRight > this.postMutateState.TextView.Right
            ? this.postMutateState.TextView.Right
            : rectRight;

        SelectedText = selectionLen < 0
            ? string.Empty
            : this.postMutateState.Text.Substring((uint)startIndex, (uint)selectionLen);

        var rectWidth = Math.Abs(rectRight - rectLeft);
        var smallestX = Math.Min(rectLeft, rectRight);

        SelectionRect = SelectionRect with
        {
            IsSolid = true,
            Position = new Vector2(SelectionRect.Position.X, this.postMutateState.Position.Y),
            Width = rectWidth,
            Height = this.postMutateState.SelectionHeight,
            Left = smallestX,
        };
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.preMutateState = default;
        this.postMutateState = default;
        SelectedText = string.Empty;
    }

    /// <summary>
    /// Handles the selection of the text when moving from the left to the right.
    /// </summary>
    /// <returns>The selection length and the right end of the selection rectangle.</returns>
    private (int selLen, float rectRight) HandleMovingToTheRight()
    {
        var charIndexWasAtEnd = this.preMutateState.CharIndex >= this.preMutateState.TextLength - 1;
        var charIndexIsAtEnd = this.postMutateState.CharIndex >= this.postMutateState.TextLength - 1;
        var selectionIsAtRightEnd = this.postMutateState.SelectionAtRightEnd;
        var startIndex = Math.Min(this.postMutateState.SelectionStartIndex, this.postMutateState.SelectionStopIndex);
        var endIndex = Math.Max(this.postMutateState.SelectionStartIndex, this.postMutateState.SelectionStopIndex);

        // Limit range of the end value
        var max = this.postMutateState.Text.IsEmpty() ? 0 : this.postMutateState.Text.LastCharIndex();
        endIndex = Math.Clamp(endIndex, 0, max);
        var indexDelta = Math.Abs(startIndex - endIndex);

        var endOrPageDownKey = this.postMutateState.Key is KeyCode.End or KeyCode.PageDown;

        var selectionLen = selectionIsAtRightEnd || endOrPageDownKey || (charIndexWasAtEnd && charIndexIsAtEnd)
            ? indexDelta + 1
            : indexDelta;

        var rectRight = selectionIsAtRightEnd || endOrPageDownKey || (charIndexWasAtEnd && charIndexIsAtEnd)
            ? Math.Max(this.postMutateState.SelStartCharBounds.Right, this.postMutateState.SelStopCharBounds.Right)
            : Math.Max(this.postMutateState.SelStartCharBounds.Left, this.postMutateState.SelStopCharBounds.Left);

        return (selectionLen, rectRight);
    }

    /// <summary>
    /// Handles the selection of the text when moving from the right to the left.
    /// </summary>
    /// <returns>The selection length and the left end of the selection rectangle.</returns>
    private (int selLen, float rectLeft) HandleMovingToTheLeft()
    {
        // Limit range of the end value
        var max = this.postMutateState.Text.IsEmpty() ? 0 : this.postMutateState.Text.LastCharIndex();

        var startIndex = Math.Min(this.postMutateState.SelectionStartIndex, this.postMutateState.SelectionStopIndex);
        var endIndex = Math.Max(this.postMutateState.SelectionStartIndex, this.postMutateState.SelectionStopIndex);
        endIndex = Math.Clamp(endIndex, 0, max);

        var indexDelta = Math.Abs(startIndex - endIndex);
        var selectionIsAtRightEnd = this.postMutateState.SelectionAtRightEnd;

        var selectionLen = selectionIsAtRightEnd
            ? indexDelta + 1
            : indexDelta;

        var rectRight = selectionIsAtRightEnd
            ? this.postMutateState.SelStartCharBounds.Right
            : Math.Max(this.postMutateState.SelStartCharBounds.Left, this.postMutateState.SelStopCharBounds.Left);

        return (selectionLen, rectRight);
    }
}
