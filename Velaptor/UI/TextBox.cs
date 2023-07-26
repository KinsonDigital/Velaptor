// <copyright file="TextBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Carbonate.UniDirectional;
using Content.Fonts;
using ExtensionMethods;
using Factories;
using Graphics;
using Graphics.Renderers;
using Input;
using ReactableData;
using Color = System.Drawing.Color;

/// <summary>
/// Provides the ability to enter text into a box.
/// </summary>
public sealed class TextBox : ControlBase
{
    private const uint CursorBlinkRate = 500;
    private const int MarginTop = 12;
    private const int MarginLeft = 2;
    private const int MarginRight = 2;
    private const int BorderThickness = 2;
    private const bool RenderAllText = false;
    private readonly Guid textBoxDataEventId = new ("71931561-826B-431B-BCE6-B139034A1FF4");
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly ITextSelection textSelection;
    private readonly ITextCursor textCursor;
    private readonly StringBuilder text = new ();
    private readonly List<(char character, RectangleF bounds)> charBounds = new ();
    private readonly IPushReactable<TextBoxStateData> textBoxDataReactable;
    private IFont? font;
    private Vector2 textPos;
    private RectShape ctrlBorder;
    private RectShape textAreaBounds;
    private KeyboardState currKeyState;
    private SizeF textSize;
    private uint height = 42;
    private float cursorBlinkTimeElapsed;
    private bool prevInSelectionMode;
    private float maxWidth;
    private uint width = 100;
    private int currentCharIndex;
    private TextBoxStateData preTextBoxState;
    private TextBoxStateData postTextBoxState;
    private bool inSelectionMode;
    private int selectionStartIndex;
    private bool selectionAtRightEnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox"/> class.
    /// </summary>
    public TextBox()
    {
        this.textBoxDataReactable = IoC.Container.GetInstance<IPushReactable<TextBoxStateData>>();
        this.textCursor = new TextCursor(this.textBoxDataReactable);
        this.textSelection = new TextSelection(this.textBoxDataReactable);

        var rendererFactory = new RendererFactory();
        this.shapeRenderer = rendererFactory.CreateShapeRenderer();
        this.fontRenderer = rendererFactory.CreateFontRenderer();
        Keyboard = IoC.Container.GetInstance<IAppInput<KeyboardState>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox"/> class.
    /// </summary>
    /// <param name="textBoxDataReactable">Manages notifications to the text box cursor and selection objects.</param>
    /// <param name="rendererFactory">Creates different types of renderers.</param>
    /// <param name="keyboard">Manages keyboard input.</param>
    /// <param name="uiDependencyFactory">Creates UI dependencies.</param>
    internal TextBox(
        IPushReactable<TextBoxStateData> textBoxDataReactable,
        IRendererFactory rendererFactory,
        IAppInput<KeyboardState> keyboard,
        IUIDependencyFactory uiDependencyFactory)
    {
        this.textBoxDataReactable = textBoxDataReactable;

        this.shapeRenderer = rendererFactory.CreateShapeRenderer();
        this.fontRenderer = rendererFactory.CreateFontRenderer();

        this.textCursor = uiDependencyFactory.CreateTextCursor(this.textBoxDataReactable);
        this.textSelection = uiDependencyFactory.CreateTextSelection(this.textBoxDataReactable);

        Keyboard = keyboard;
    }

    /// <summary>
    /// Gets or sets the text in the <see cref="TextBox"/>.
    /// </summary>
    public string Text
    {
        get => this.text.ToString();
        set
        {
            this.text.Clear();
            this.text.Append(value);
        }
    }

    /// <summary>
    /// Gets the selected text.
    /// </summary>
    public string SelectedText => this.textSelection.SelectedText;

    /// <summary>
    /// Gets or sets the color of the text.
    /// </summary>
    public Color TextColor { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the color of the cursor.
    /// </summary>
    public Color CursorColor
    {
        get => this.textCursor.Color;
        set => this.textCursor.Color = value;
    }

    /// <summary>
    /// Gets or sets the width of the cursor.
    /// </summary>
    public int CursorWidth
    {
        get => this.textCursor.Width;
        set => this.textCursor.Width = value;
    }

    /// <summary>
    /// Gets or sets the size of the font of the text box.
    /// </summary>
    public uint FontSize
    {
        get => this.font?.Size ?? 0u;
        set
        {
            if (this.font is null)
            {
                return;
            }

            var textRight = this.charBounds.TextRight();
            var hasBounds = this.charBounds.Count > 0;

            UpdateState(MutateType.PreMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.DimensionalChange);

            var beforeTextSize = this.font.Measure(this.text.ToString());

            this.font.Size = value;

            var afterTextSize = this.font.Measure(this.text.ToString());

            var textWidthDelta = afterTextSize.Width - beforeTextSize.Width;
            var textHeightDelta = afterTextSize.Height - beforeTextSize.Height;

            this.height += (uint)textHeightDelta;
            this.textAreaBounds.Width += textWidthDelta;
            this.textAreaBounds.Height += textHeightDelta;
            this.textAreaBounds.Left += textWidthDelta.Half();
            this.textAreaBounds.Top += textHeightDelta.Half();

            UpdateBounds(KeyCode.Unknown);
            UpdateTextPos();

            if (textHeightDelta > 0)
            {
                this.charBounds.BumpAllDown(textHeightDelta);
            }

            textRight = this.charBounds.TextRight();
            hasBounds = this.charBounds.Count > 0;

            UpdateState(MutateType.PostMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.DimensionalChange);

            this.textCursor.Update();
        }
    }

    /// <summary>
    /// Gets or sets the color of the text selection rectangle.
    /// </summary>
    public Color SelectionColor
    {
        get => this.textSelection.SelectionColor;
        set => this.textSelection.SelectionColor = value;
    }

    /// <summary>
    /// Gets or sets the color of the selected text.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for library users.")]
    public Color SelectedTextColor { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the width of the <see cref="TextBox"/>.
    /// </summary>
    public override uint Width
    {
        get => this.width;
        set
        {
            var prevWidth = this.width;
            this.width = value;

            this.width = value > 1_000_000u ? 0 : value;

            this.ctrlBorder = this.ctrlBorder with { Width = value };

            var textRight = this.charBounds.TextRight();
            var hasBounds = this.charBounds.Count > 0;
            UpdateState(MutateType.PreMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.DimensionalChange);

            this.textAreaBounds = this.textAreaBounds with
            {
                Width = this.ctrlBorder.Width - (MarginLeft + MarginRight + (BorderThickness * 2)),
            };

            var textSmallerThanView = this.charBounds.TextWidth() < this.textAreaBounds.Width;

            if (textSmallerThanView)
            {
                SnapBoundsToLeft();
            }
            else
            {
                var firstCharVisible = !this.charBounds.IsEmpty() && this.charBounds.TextLeft() >= this.textAreaBounds.Left;
                var lastCharVisible = !this.charBounds.IsEmpty() && this.charBounds.TextRight() <= this.textAreaBounds.Right;

                if (firstCharVisible)
                {
                    SnapBoundsToLeft();
                }

                if (lastCharVisible)
                {
                    SnapBoundsToRight();
                }
            }

            UpdateTextPos();

            var hasShrunk = this.width < prevWidth;
            var cursorPastRightEnd = this.textCursor.Position.X > this.textAreaBounds.Right;

            if (hasShrunk && cursorPastRightEnd)
            {
                var lastVisibleCharBounds = this.charBounds.IsEmpty()
                    ? default
                    : this.charBounds.LastOrDefault(i => i.bounds.Right < this.textAreaBounds.Right);

                var newIndex = this.charBounds.LastIndexOf(lastVisibleCharBounds);

                this.currentCharIndex = newIndex + 1;
            }

            textRight = this.charBounds.TextRight();
            hasBounds = this.charBounds.Count > 0;
            UpdateState(MutateType.PostMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.DimensionalChange);

            this.textCursor.Update();
        }
    }

    /// <summary>
    /// Gets the height of the <see cref="TextBox"/>.
    /// </summary>
    public override uint Height => this.height;

    /// <summary>
    /// Gets or sets the position of the <see cref="TextBox"/> on the screen.
    /// </summary>
    public override Point Position
    {
        get => base.Position;
        set
        {
            if (base.Position == value)
            {
                return;
            }

            var hasChangedX = value.X != base.Position.X;
            var movingLeft = hasChangedX && value.X < base.Position.X;
            var movingRight = hasChangedX && value.X > base.Position.X;

            var hasChangedY = value.Y != base.Position.Y;
            var movingUp = hasChangedY && value.Y < base.Position.Y;
            var deltaX = Math.Abs(value.X - base.Position.X);
            var deltaY = Math.Abs(value.Y - base.Position.Y);

            var textRight = this.charBounds.TextRight();
            var hasBounds = this.charBounds.Count > 0;

            UpdateState(MutateType.PreMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.Movement);

            base.Position = value;

            if (movingLeft)
            {
                this.charBounds.BumpAllToLeft(deltaX);
                this.textAreaBounds.Position = this.textAreaBounds.Position with { X = this.textAreaBounds.Position.X - deltaX, };
            }
            else if (movingRight)
            {
                this.charBounds.BumpAllToRight(deltaX);
                this.textAreaBounds.Position = this.textAreaBounds.Position with { X = this.textAreaBounds.Position.X + deltaX, };
            }

            UpdateBounds(KeyCode.Unknown);

            if (hasChangedY)
            {
                this.charBounds.BumpAllUp(deltaY);

                this.textPos.Y = movingUp
                    ? this.textPos.Y - deltaY
                    : this.textPos.Y + deltaY;
            }

            UpdateTextPos();

            this.textAreaBounds.Position = this.textAreaBounds.Position with { Y = Position.Y };

            textRight = (int)(this.charBounds.IsEmpty() ? this.textAreaBounds.Right : this.charBounds.TextRight());
            hasBounds = this.charBounds.Count > 0;
            UpdateState(MutateType.PostMutate, hasBounds, textRight, KeyCode.Unknown, TextBoxEvent.Movement);
            this.textCursor.Update();
        }
    }

    /// <summary>
    /// Loads the content of the <see cref="TextBox"/>.
    /// </summary>
    public override void LoadContent()
    {
        var contentLoader = ContentLoaderFactory.CreateContentLoader();

        // the font when the font type or size changes.
        this.font = contentLoader.LoadFont("TimesNewRoman-Regular", 12);

        this.maxWidth = this.font.Metrics.Max(m => m.GlyphWidth);

        UpdateBorder();

        var areaBoundsWidth = this.ctrlBorder.Right - this.ctrlBorder.Left;
        areaBoundsWidth -= MarginLeft + MarginRight + (BorderThickness * 2);

        this.textAreaBounds = new RectShape
        {
            Position = Position.ToVector2(),
            Width = areaBoundsWidth,
            Height = Height,
            Color = Color.Orange,
            IsSolid = false,
        };

        this.textCursor.Height = (int)Height - MarginTop;
        this.textCursor.Position = new Vector2(this.textAreaBounds.Left, Position.Y);

        this.textPos = new Vector2(this.textAreaBounds.Left, Position.Y);

        UpdateState(MutateType.PreMutate, false, 0, KeyCode.Unknown, TextBoxEvent.None);
        UpdateState(MutateType.PostMutate, false, 0, KeyCode.Unknown, TextBoxEvent.None);

        base.LoadContent();
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

        this.cursorBlinkTimeElapsed += (float)frameTime.ElapsedTime.TotalMilliseconds;

        if (this.cursorBlinkTimeElapsed >= CursorBlinkRate)
        {
            this.textCursor.Visible = !this.textCursor.Visible;
            this.cursorBlinkTimeElapsed -= CursorBlinkRate;
        }

        this.currKeyState = Keyboard.GetState();

        UpdateBorder();

        base.Update(frameTime);
    }

    /// <inheritdoc/>
    public override void Render()
    {
        if (IsLoaded is false || Visible is false)
        {
            return;
        }

        this.shapeRenderer.Render(this.ctrlBorder, layer: 0);

        if (this.textCursor.Visible)
        {
            this.shapeRenderer.Render(this.textCursor.Cursor, layer: 0);
        }

        // When unselecting text to the left, we cannot see any of the selection rect.
        if (this.inSelectionMode)
        {
            var textLeft = this.postTextBoxState.TextLeft;
            var textRight = this.postTextBoxState.TextRight;
            var leftEndPastView = this.textSelection.SelectionRect.Left < textLeft;
            var rightEndPastView = this.textSelection.SelectionRect.Right > textRight;

            RectShape selectionRect;

            if (rightEndPastView && !leftEndPastView)
            {
                var amountPastEnd = this.textSelection.SelectionRect.Right - textRight;

                selectionRect = this.textSelection.SelectionRect with
                {
                    Width = this.textSelection.SelectionRect.Width - amountPastEnd,
                };

                selectionRect.Left -= amountPastEnd.Half();
            }
            else if (leftEndPastView && !rightEndPastView)
            {
                var amountPastEnd = textLeft - this.textSelection.SelectionRect.Left;

                selectionRect = this.textSelection.SelectionRect with
                {
                    Width = this.textSelection.SelectionRect.Width - amountPastEnd,
                };

                selectionRect.Left += amountPastEnd.Half();
            }
            else
            {
                selectionRect = this.textSelection.SelectionRect;
            }

            this.shapeRenderer.Render(selectionRect, layer: 10);
        }

        RenderText();
    }

    /// <summary>
    /// Processes the keyboard key that has been lifted to apply behaviors to the <see cref="TextBox"/>
    /// such as adding text, deleting text, moving the cursor, etc.
    /// </summary>
    /// <param name="key">The key that was lifted.</param>
    protected override void OnKeyUp(KeyCode key)
    {
        var anyMoveCursorKeysDown = key.IsArrowKey() ||
                                    key == KeyCode.Home || key == KeyCode.End ||
                                    key == KeyCode.PageUp || key == KeyCode.PageDown;

        if (anyMoveCursorKeysDown)
        {
            this.inSelectionMode = this.currKeyState.AnyShiftKeysDown();
        }

        StartSelection();

        var textBoxEvent = GetTextBoxEvent(key);

        var textRight = this.charBounds.TextRight();
        var hasBounds = this.charBounds.Count > 0;

        UpdateState(MutateType.PreMutate, hasBounds, textRight, key, textBoxEvent);

        if (key.IsVisibleKey())
        {
            Add(key);
        }
        else if (KeyboardKeyGroups.DeletionKeys.Contains(key))
        {
            Remove(key);
        }
        else if (KeyboardKeyGroups.CursorMovementKeys.Contains(key))
        {
            this.textCursor.Visible = true;
            SetCursorIndex(key);
        }

        ClearSelection(key);

        textRight = (int)(this.charBounds.IsEmpty() ? this.textAreaBounds.Right : this.charBounds.TextRight());
        hasBounds = this.charBounds.Count > 0;

        UpdateState(MutateType.PostMutate, hasBounds, textRight, key, textBoxEvent);
        this.textCursor.Update();
        this.textSelection.Update();

        UpdateTextPos();

        this.prevInSelectionMode = this.inSelectionMode;
        base.OnKeyUp(key);
    }

    /// <summary>
    /// Creates a <see cref="TextBoxEvent"/> based on the given keyboard <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>The text box event that has occurred.</returns>
    private TextBoxEvent GetTextBoxEvent(KeyCode key)
    {
        if (key.IsVisibleKey())
        {
            return TextBoxEvent.AddingCharacter;
        }

        if (KeyboardKeyGroups.DeletionKeys.Contains(key))
        {
            return string.IsNullOrEmpty(this.textSelection.SelectedText) switch
            {
                true => TextBoxEvent.RemovingSingleChar,
                false => TextBoxEvent.RemovingSelectedChars,
            };
        }

        return KeyboardKeyGroups.CursorMovementKeys.Contains(key) ? TextBoxEvent.MovingCursor : TextBoxEvent.None;
    }

    /// <summary>
    /// Updates the position of the text represented by the character bounds.
    /// </summary>
    private void UpdateTextPos()
    {
        // Update the text rendering position
        var charBoundsCenterX = this.charBounds.CenterPositionX();
        this.textPos = this.textPos with { X = charBoundsCenterX };
    }

    /// <summary>
    /// Starts the text selection process.
    /// </summary>
    private void StartSelection()
    {
        var movingIntoSelectionMode = this.inSelectionMode && this.prevInSelectionMode is false;

        // When moving into selection mode
        if (!movingIntoSelectionMode)
        {
            return;
        }

        this.selectionStartIndex = this.currentCharIndex;
        this.selectionAtRightEnd = this.textCursor.Cursor.Right > this.charBounds.TextRight();
    }

    /// <summary>
    /// Clears the selected text.
    /// </summary>
    /// <param name="key">The key that has been pressed.</param>
    private void ClearSelection(KeyCode key)
    {
        if (KeyboardKeyGroups.DeletionKeys.Contains(key))
        {
            if (!this.inSelectionMode)
            {
                return;
            }

            this.inSelectionMode = false;
            this.textSelection.Clear();
        }
        else if (KeyboardKeyGroups.CursorMovementKeys.Contains(key))
        {
            // If moving out of selection mode
            if (this.inSelectionMode || !this.prevInSelectionMode)
            {
                return;
            }

            this.inSelectionMode = false;
            this.textSelection.Clear();
        }
    }

    /// <summary>
    /// Renders the text of the text box.
    /// </summary>
    private void RenderText()
    {
        if (this.font is null)
        {
            return;
        }

        var glyphsToRender = this.charBounds.Select((cb, i) =>
        {
            var inView = this.textAreaBounds.Contains(new Vector2(cb.bounds.Left, Position.Y)) &&
                         this.textAreaBounds.Contains(new Vector2(cb.bounds.Right, Position.Y));

            var glyphMetrics = this.font.Metrics.First(m => m.Glyph == cb.character);

            var minIndex = Math.Min(this.selectionStartIndex, this.currentCharIndex);
            var maxIndex = Math.Max(this.selectionStartIndex, this.currentCharIndex);

            var lastCharSelected = this.textSelection.SelectionRect.Right >= this.charBounds.TextRight();

            var inSelectedRange = lastCharSelected switch
            {
                true => this.inSelectionMode && i >= minIndex && i <= maxIndex,
                false => this.inSelectionMode && i >= minIndex && i < maxIndex,
            };

            var clr = inSelectedRange
                ? SelectedTextColor
                : TextColor;

            if (RenderAllText || inView)
            {
                return (glyphMetrics, clr);
            }

            glyphMetrics.GlyphBounds = glyphMetrics.GlyphBounds with { Width = 0 };

            return (glyphMetrics, clr);
        }).ToArray().AsSpan();

        this.fontRenderer.Render(
            this.font,
            glyphsToRender,
            (int)this.textPos.X,
            (int)this.textPos.Y,
            1f,
            0f,
            layer: 20);
    }

    /// <summary>
    /// Adds a character to the text box.
    /// </summary>
    /// <param name="charKey">The character to add.</param>
    private void Add(KeyCode charKey)
    {
        if (this.font is null)
        {
            return;
        }

        var character = charKey == KeyCode.Space ? " " : charKey.ToChar(this.currKeyState.AnyShiftKeysDown()).ToString();
        var charIndexAtEnd = this.currentCharIndex >= this.text.LastCharIndex();
        var cursorAtEnd = this.preTextBoxState.CursorAtEnd;

        var insertIndex = charIndexAtEnd && cursorAtEnd
            ? this.text.Length
            : this.currentCharIndex;

        var prevHeight = this.textSize.Height;

        this.text.Insert(insertIndex, character);
        this.textSize = this.font.Measure(this.text.ToString());

        UpdateBounds(charKey); // NEW
        CalcYPositions(prevHeight, this.textSize.Height);

        this.currentCharIndex = charIndexAtEnd
            ? this.text.LastCharIndex()
            : this.currentCharIndex + 1;
    }

    /// <summary>
    /// Removes a character from the text box.
    /// </summary>
    /// <param name="removeKey">The type of remove key.</param>
    private void Remove(KeyCode removeKey)
    {
        if (this.text.IsEmpty())
        {
            return;
        }

        switch (removeKey)
        {
            case KeyCode.Backspace when string.IsNullOrEmpty(this.textSelection.SelectedText):
                BackspaceSingleChar();
                break;
            case KeyCode.Delete when string.IsNullOrEmpty(this.textSelection.SelectedText):
                DeleteCurrentChar();
                break;
            case KeyCode.Backspace or KeyCode.Delete when !string.IsNullOrEmpty(this.textSelection.SelectedText):
                RemoveSelectedChars();
                break;
        }
    }

    /// <summary>
    /// Backspaces a single character.
    /// </summary>
    private void BackspaceSingleChar()
    {
        var charIndexAtStart = this.currentCharIndex <= 0;
        var cursorAtRightEndOfText = this.textCursor.Cursor.Right > this.charBounds.TextRight();
        var cursorNotAtRightEndOfText = !cursorAtRightEndOfText;

        if ((charIndexAtStart && cursorNotAtRightEndOfText) || this.font is null)
        {
            return;
        }

        var prevHeight = this.textSize.Height;

        if (cursorAtRightEndOfText)
        {
            this.text.RemoveLastChar();
        }
        else
        {
            this.text.RemoveChar((uint)(this.currentCharIndex - 1));
        }

        this.textSize = this.font.Measure(this.text.ToString());

        UpdateBounds(this.preTextBoxState.Key); // NEW
        CalcYPositions(prevHeight, this.textSize.Height);
    }

    /// <summary>
    /// Deletes the current character.
    /// </summary>
    private void DeleteCurrentChar()
    {
        var cursorWasAtRightEndOfText = this.preTextBoxState.CursorAtEnd;
        if (cursorWasAtRightEndOfText || this.font is null)
        {
            return;
        }

        var prevHeight = this.textSize.Height;

        this.text.RemoveChar((uint)this.currentCharIndex);
        this.textSize = this.font.Measure(this.text.ToString());

        UpdateBounds(this.preTextBoxState.Key); // NEW
        CalcYPositions(prevHeight, this.textSize.Height);
    }

    /// <summary>
    /// Removes the selected characters.
    /// </summary>
    private void RemoveSelectedChars()
    {
        if (!this.inSelectionMode || this.font is null)
        {
            return;
        }

        var selectionToEndOfText = this.textSelection.SelectionRect.Right >= this.charBounds.TextRight();
        var minCharIndex = Math.Min(this.selectionStartIndex, this.currentCharIndex);
        var maxCharIndex = Math.Max(this.selectionStartIndex, this.currentCharIndex);
        var stopCharIndex = selectionToEndOfText ? maxCharIndex + 1 : maxCharIndex;

        var prevHeight = this.textSize.Height;
        var removeLength = Math.Abs(minCharIndex - stopCharIndex);

        this.text.Remove(minCharIndex, removeLength);

        this.textSize = this.font.Measure(this.text.ToString());

        UpdateBounds(this.preTextBoxState.Key); // NEW
        CalcYPositions(prevHeight, this.textSize.Height);
    }

    /// <summary>
    /// Updates the pre and post text box states.
    /// </summary>
    /// <param name="mutateType">The type text mutation.</param>
    /// <param name="boundsExist">True if the any character bounds exist.</param>
    /// <param name="textRight">The right end of the text.</param>
    /// <param name="key">The key that has been pressed.</param>
    /// <param name="textBoxEvent">The type of text box event that has occured.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="mutateType"/> is an invalid value.
    /// </exception>
    private void UpdateState(MutateType mutateType, bool boundsExist, int textRight, KeyCode key, TextBoxEvent textBoxEvent)
    {
        RectangleF selStartCharBounds = default;
        RectangleF selStopCharBounds = default;

        var currCharIndex = this.currentCharIndex < 0 ? 0 : this.currentCharIndex;

        if (boundsExist)
        {
            selStartCharBounds = this.inSelectionMode ? this.charBounds[this.selectionStartIndex].bounds : default;
            selStopCharBounds = this.inSelectionMode ? this.charBounds[currCharIndex].bounds : default;
        }

        var requestedStateKey = mutateType switch
        {
            MutateType.PreMutate => this.preTextBoxState.Key,
            MutateType.PostMutate => this.postTextBoxState.Key,
            _ => throw new ArgumentOutOfRangeException(nameof(mutateType), mutateType, null)
        };

        var firstVisibleCharBounds = this.charBounds.IsEmpty()
            ? default
            : this.charBounds.Find(i => i.bounds.Left > this.textAreaBounds.Left).bounds;

        var lastVisibleCharBounds = this.charBounds.IsEmpty()
            ? default
            : this.charBounds.LastOrDefault(i => i.bounds.Right < this.textAreaBounds.Right).bounds;

        var newState = new TextBoxStateData
        {
            Text = this.text,
            TextLength = this.text.Length,
            TextLeft = (int)(this.charBounds.IsEmpty() ? this.textAreaBounds.Left : this.charBounds.TextLeft()),
            TextRight = this.charBounds.IsEmpty() ? (int)this.textAreaBounds.Left : textRight,
            TextView = this.textAreaBounds,
            Key = key == KeyCode.Unknown ? requestedStateKey : key,
            CharIndex = currCharIndex,
            CurrentCharLeft = boundsExist ? (int)this.charBounds[currCharIndex].bounds.Left : 0,
            CurrentCharRight = boundsExist ? (int)this.charBounds[currCharIndex].bounds.Right : 0,
            InSelectionMode = this.inSelectionMode,
            SelStartCharBounds = selStartCharBounds,
            SelStopCharBounds = selStopCharBounds,
            FirstVisibleCharBounds = firstVisibleCharBounds,
            LastVisibleCharBounds = lastVisibleCharBounds,
            SelectionStartIndex = this.selectionStartIndex,
            SelectionStopIndex = currCharIndex,
            SelectionHeight = (int)(Height - MarginTop),
            SelectionAtRightEnd = this.selectionAtRightEnd,
            Position = Position.ToVector2(),
            CursorAtEnd = this.text.IsEmpty() || this.textCursor.Cursor.Right > textRight,
            TextMutateType = mutateType,
            Event = textBoxEvent,
            Width = (int)Width,
        };

        switch (mutateType)
        {
            case MutateType.PreMutate:
                this.preTextBoxState = newState;
                this.textBoxDataReactable.Push(newState, this.textBoxDataEventId);
                break;
            case MutateType.PostMutate:
                this.postTextBoxState = newState;
                this.textBoxDataReactable.Push(newState, this.textBoxDataEventId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mutateType), mutateType, null);
        }
    }

    /// <summary>
    /// Adjusts the character bounds after moving the cursor left.
    /// </summary>
    private void AdjustBoundsAfterMovingLeft()
    {
        var charIndexAtStart = this.currentCharIndex <= 0;

        // If currently at the first char index
        if (charIndexAtStart)
        {
            SnapBoundsToLeft();
        }
        else
        {
            var currentCharLeft = this.charBounds.CharLeft(this.currentCharIndex);
            var anyTextLeftOfTextArea = currentCharLeft < this.textAreaBounds.Left;

            if (anyTextLeftOfTextArea)
            {
                var overlapAmount = this.textAreaBounds.Left - currentCharLeft;
                this.charBounds.BumpAllToRight(overlapAmount);
            }

            // If the cursor is at the beginning of the text
            if (this.textCursor.Cursor.Left <= this.charBounds[0].bounds.Left)
            {
                this.charBounds.BumpAllToRight(this.maxWidth);
            }
        }
    }

    /// <summary>
    /// Adjusts the character bounds after moving the cursor right.
    /// </summary>
    private void AdjustBoundsAfterMovingRight()
    {
        var textNotLargerThanView = this.charBounds.TextWidth() <= this.textAreaBounds.Width;

        if (textNotLargerThanView)
        {
            return;
        }

        var currentCharRight = this.charBounds.CharRight(this.currentCharIndex);
        var charIsNotPartiallyHidden = currentCharRight < this.textAreaBounds.Right;

        if (charIsNotPartiallyHidden)
        {
            return;
        }

        var overlapAmount = currentCharRight - this.textAreaBounds.Right;
        this.charBounds.BumpAllToLeft(overlapAmount);

        if (this.charBounds.GapAtRightEnd(this.textAreaBounds.Right))
        {
            SnapBoundsToRight();
        }
    }

    /// <summary>
    /// Sets the index of the cursor based on the given keyboard <paramref name="key"/>
    /// that was pressed.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    private void SetCursorIndex(KeyCode key)
    {
        if (this.text.IsEmpty())
        {
            return;
        }

        this.currentCharIndex = CalcNextCharIndex(key);
        UpdateBounds(key);
    }

    /// <summary>
    /// Updates the character bounds of the text based on the given keyboard <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    private void UpdateBounds(KeyCode key)
    {
        /* NOTE:
         * This is always after the text mutation
         */

        RebuildBounds();

        var isMoveToLeftEndKey = key is KeyCode.Home or KeyCode.PageUp;
        var isMoveLeftKey = key is KeyCode.Left;
        var isMoveToRightEndKey = key is KeyCode.End or KeyCode.PageDown;
        var isMoveRightKey = key is KeyCode.Right;
        var isMovementKey = isMoveToLeftEndKey || isMoveLeftKey || isMoveToRightEndKey || isMoveRightKey;

        var textLargerThanView = this.charBounds.TextWidth() > this.textAreaBounds.Width;
        var charIndexAtEnd = this.preTextBoxState.CharIndex >= this.preTextBoxState.TextLength - 1;

        var cursorAtRightEndOfText = this.textCursor.Cursor.Right > this.charBounds.TextRight();

        var gapAtRightEnd = this.charBounds.GapAtRightEnd(this.textAreaBounds.Right);

        if (textLargerThanView && isMovementKey)
        {
            if (isMoveLeftKey)
            {
                AdjustBoundsAfterMovingLeft();
            }
            else if (isMoveRightKey)
            {
                AdjustBoundsAfterMovingRight();
            }
        }

        if (textLargerThanView)
        {
            if (isMovementKey)
            {
                if (isMoveToLeftEndKey)
                {
                    SnapBoundsToLeft();
                }
                else if (isMoveToRightEndKey)
                {
                    SnapBoundsToRight();
                }
                else if (isMoveLeftKey)
                {
                    AdjustBoundsAfterMovingLeft();
                }
                else if (isMoveRightKey)
                {
                    AdjustBoundsAfterMovingRight();
                }
            }
            else if (key.IsVisibleKey() && charIndexAtEnd)
            {
                SnapBoundsToRight();
            }
            else if (KeyboardKeyGroups.DeletionKeys.Contains(key) && gapAtRightEnd)
            {
                SnapBoundsToRight();
            }
        }
        else
        {
            SnapBoundsToLeft();
        }

        // Update the char index
        switch (key)
        {
            case KeyCode.Delete:
                if (this.inSelectionMode)
                {
                    var minCharIndex = Math.Min(this.selectionStartIndex, this.currentCharIndex);
                    this.currentCharIndex = minCharIndex >= this.text.LastCharIndex()
                        ? this.text.LastCharIndex()
                        : minCharIndex;
                }
                else
                {
                    this.currentCharIndex = charIndexAtEnd
                        ? this.currentCharIndex - 1
                        : this.currentCharIndex;
                }

                break;
            case KeyCode.Backspace:
                this.currentCharIndex = cursorAtRightEndOfText
                    ? Math.Clamp(this.text.LastCharIndex(), 0, int.MaxValue)
                    : this.currentCharIndex - 1;

                break;
        }
    }

    /// <summary>
    /// Calculates the new text position as a whole as well as the individual character bounds
    /// based on the height of the text before and after it was mutated.
    /// This helps keep all of the text vertically centered no matter the height of newly added
    /// or removed characters.
    /// </summary>
    /// <param name="prevHeight">The height of the text before it was mutated.</param>
    /// <param name="currentHeight">The height of the text after it was mutated.</param>
    private void CalcYPositions(float prevHeight, float currentHeight)
    {
        // Calculate the Y position for the text rendering location
        this.textPos = prevHeight == 0
            ? new Vector2(this.textPos.X, Position.Y - currentHeight.Half() + currentHeight.Half())
            : new Vector2(this.textPos.X, this.textPos.Y + (prevHeight - currentHeight).Half());

        // Update all of the character Y positions to have each character centered vertically
        // over the same Y position as the text render location
        for (var i = 0; i < this.charBounds.Count; i++)
        {
            var currItem = this.charBounds[i];

            currItem = currItem with { bounds = currItem.bounds with { Y = this.textPos.Y - currItem.bounds.Height.Half() } };

            this.charBounds[i] = currItem;
        }
    }

    /// <summary>
    /// Calculates the index of the next character based on the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    /// <returns>The new character bounds index.</returns>
    private int CalcNextCharIndex(KeyCode key)
    {
        var lastCharIndex = this.text.LastCharIndex();
        var cursorAtRightEndOfText = this.text.IsEmpty() || this.textCursor.Cursor.Right > this.charBounds.TextRight();
        var isAtLastCharIndex = this.currentCharIndex >= lastCharIndex;

        var newIndex = key switch
        {
            KeyCode.Left => isAtLastCharIndex && cursorAtRightEndOfText
                ? this.currentCharIndex
                : this.currentCharIndex - 1,
            KeyCode.Right => isAtLastCharIndex
                ? lastCharIndex
                : this.currentCharIndex + 1,
            KeyCode.Home => 0,
            KeyCode.PageUp => 0,
            KeyCode.End => lastCharIndex,
            KeyCode.PageDown => lastCharIndex,
            _ => this.currentCharIndex,
        };

        return newIndex < 0 ? 0 : newIndex;
    }

    /// <summary>
    /// Updates the text box border.
    /// </summary>
    private void UpdateBorder() =>
        this.ctrlBorder = new RectShape
        {
            Position = Position.ToVector2(),
            Color = Color.LightGray,
            Width = Width,
            Height = Height,
            BorderThickness = BorderThickness,
            IsSolid = false,
            CornerRadius = new CornerRadius(5),
        };

    /// <summary>
    /// Rebuilds the bounds for each character in the text.
    /// </summary>
    private void RebuildBounds()
    {
        var currentX = this.charBounds.IsEmpty()
            ? this.textAreaBounds.Left
            : this.charBounds.Min(cb => cb.bounds.Left);

        var newBounds = this.font
            ?.GetCharacterBounds(this.text.ToString(), new Vector2(currentX, Position.Y))
            .ToArray() ?? Array.Empty<(char, RectangleF)>();
        this.charBounds.Clear();
        this.charBounds.AddRange(newBounds);
    }

    /// <summary>
    /// Snaps the bounds to the left side of the text view area.
    /// </summary>
    private void SnapBoundsToLeft()
    {
        if (this.charBounds.IsEmpty())
        {
            return;
        }

        var currentY = this.charBounds[0].bounds.Y;
        var newBounds = this.font
            ?.GetCharacterBounds(this.text.ToString(), new Vector2(this.textAreaBounds.Left, currentY))
            .ToArray() ?? Array.Empty<(char, RectangleF)>();
        this.charBounds.Clear();
        this.charBounds.AddRange(newBounds);
    }

    /// <summary>
    /// Snaps the bounds to the right side of the text view area.
    /// </summary>
    private void SnapBoundsToRight()
    {
        if (this.charBounds.IsEmpty())
        {
            return;
        }

        var newX = this.textAreaBounds.Right - this.textSize.Width;
        var currentY = this.charBounds[0].bounds.Y;

        var newBounds = this.font
            ?.GetCharacterBounds(this.text.ToString(), new Vector2(newX, currentY))
            .ToArray() ?? Array.Empty<(char, RectangleF)>();
        this.charBounds.Clear();
        this.charBounds.AddRange(newBounds);

        var textPastRightEnd = this.charBounds.TextRight() > this.textAreaBounds.Right;

        if (!textPastRightEnd)
        {
            return;
        }

        this.charBounds.BumpAllToLeft(this.charBounds.TextRight() - this.textAreaBounds.Right);
    }
}
