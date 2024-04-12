// <copyright file="Button.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Content;
using Factories;
using Graphics;
using Graphics.Renderers;
using Guards;
using Input;

/// <summary>
/// A button that can be clicked to execute functionality.
/// </summary>
[Obsolete("This class is deprecated and will be removed in a future release.")]
public sealed class Button : ControlBase
{
    private const int TextLayer = int.MaxValue - 1;
    private const int BorderLayer = int.MaxValue - 2;
    private const int BtnFaceLayer = int.MaxValue - 3;
    private const float HoverBrightness = 0.2f;
    private const float MouseDownBrightness = 0.2f;
    private const uint DefaultButtonWidth = 100;
    private const uint DefaultButtonHeight = 50;
    private const uint HorizontalMargin = 10;
    private readonly IShapeRenderer rectRenderer;
    private IUIControlFactory controlFactory = null!;
    private string cachedText = string.Empty;
    private uint cachedAutoSizeOffWidth;
    private uint cachedAutoSizeOffHeight;
    private bool isMouseDown;
    private bool autoSize = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    public Button()
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="label">The label to display on the face of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(Label? label)
    {
        ArgumentNullException.ThrowIfNull(label);

        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Label = label;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="position">The position of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(Point position)
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Position = position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="width">The width of the button.</param>
    /// <param name="height">The height of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(uint width, uint height)
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="width">The width of the button.</param>
    /// <param name="height">The height of the button.</param>
    /// <param name="label">The label to display on the face of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(uint width, uint height, Label? label)
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Width = width;
        Height = height;
        Label = label;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="position">The position of the button.</param>
    /// <param name="width">The width of the button.</param>
    /// <param name="height">The height of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(Point position, uint width, uint height)
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Position = position;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="position">The position of the button.</param>
    /// <param name="width">The width of the button.</param>
    /// <param name="height">The height of the button.</param>
    /// <param name="label">The label to display on the face of the button.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Button(Point position, uint width, uint height, Label? label)
    {
        this.rectRenderer = RendererFactory.CreateShapeRenderer();

        Init();
        Position = position;
        Width = width;
        Height = height;
        Label = label;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="controlFactory">Creates UI controls.</param>
    /// <param name="keyboard">Manages keyboard input.</param>
    /// <param name="mouse">The system mouse.</param>
    // ReSharper disable UnusedMember.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal Button(
        IUIControlFactory controlFactory,
        IAppInput<KeyboardState> keyboard,
        IAppInput<MouseState> mouse)
        : base(keyboard, mouse)
    {
        ArgumentNullException.ThrowIfNull(controlFactory);

        this.controlFactory = controlFactory;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    // ReSharper restore UnusedMember.Global

    /// <summary>
    /// Gets the <see cref="Label"/> of the <see cref="Button"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public Label? Label { get; private set; }

    /// <inheritdoc cref="IControl"/>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public override Point Position
    {
        get => base.Position;
        set
        {
            base.Position = value;

            if (Label is not null)
            {
                Label.Position = Position;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the size of the <see cref="Button"/> will be
    /// managed automatically based on the size of the <see cref="UI.Label"/>.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSize"/> is <c>false</c>, the user can set the size to anything they
    ///     desire.  If the size is less than the width or height of the text, then only the text characters
    ///     that are still within the bounds of the <see cref="UI.Label"/> will be rendered.
    /// </remarks>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public bool AutoSize
    {
        get => this.autoSize;
        set
        {
            // If auto size is currently turned on and is being turned off,
            // set the width back to the cached width before auto size was turned on
            if (this.autoSize && !value)
            {
                base.Width = this.cachedAutoSizeOffWidth;
                base.Height = this.cachedAutoSizeOffHeight;
            }

            this.autoSize = value;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Width"/> of the <see cref="Button"/>.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSize"/> is <c>true</c>, the <see cref="Width"/> value will be set but ignored
    ///     and the <see cref="Width"/> will be automatic based on the <see cref="Width"/> of the <see cref="Text"/>.
    /// </remarks>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public override uint Width
    {
        get => AutoSize ? Label?.Width + (HorizontalMargin * 2u) ?? HorizontalMargin * 2u : base.Width;
        set
        {
            this.cachedAutoSizeOffWidth = value;
            if (AutoSize)
            {
                return;
            }

            base.Width = value;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Height"/> of the <see cref="Button"/>.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSize"/> is <c>true</c>, the <see cref="Height"/> value will be set but ignored
    ///     and the <see cref="Height"/> will be automatic based on the <see cref="Height"/> of the <see cref="Text"/>.
    /// </remarks>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public override uint Height
    {
        get => AutoSize ? Label?.Height + (HorizontalMargin * 2u) ?? HorizontalMargin * 2u : base.Height;
        set
        {
            const uint marginTotal = HorizontalMargin * 2u;

            base.Height = AutoSize ? Label?.Height + marginTotal ?? marginTotal : value;
            this.cachedAutoSizeOffHeight = value;
        }
    }

    /// <summary>
    /// Gets or sets the text of the button.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public string Text
    {
        get => IsLoaded
            ? Label?.Text ?? string.Empty
            : this.cachedText;
        set
        {
            if (Label is null || !IsLoaded)
            {
                this.cachedText = value;
                return;
            }

            Label.Text = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the border of the <see cref="Button"/> is visible.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public bool BorderVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the color of the <see cref="Button"/> border.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public Color BorderColor { get; set; } = Color.SlateGray;

    /// <summary>
    /// Gets or sets the thickness of the <see cref="Button"/>'s border.
    /// </summary>
    /// <remarks>
    ///     This value uses pixels as unit of measure.
    /// </remarks>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public uint BorderThickness { get; set; } = 2u;

    /// <summary>
    /// Gets or sets the color of the face of the <see cref="Button"/>.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public Color FaceColor { get; set; } = Color.DarkGray;

    /// <summary>
    /// Gets or sets the radius values for each corner of the <see cref="Button"/>
    /// border and face.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public CornerRadius CornerRadius { get; set; } = new (6f);

    /// <summary>
    /// Gets or sets the font size of the text on the face of the button.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    public uint FontSize
    {
        get => Label?.FontSize ?? 0u;
        set
        {
            if (Label is not null)
            {
                Label.FontSize = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the button is enabled.
    /// </summary>
    public override bool Enabled
    {
        get => Label?.Enabled ?? base.Enabled;
        set
        {
            if (Label is not null)
            {
                Label.Enabled = value;
            }

            // Save the value to the base enabled property which will cache the value
            // before the content is loaded.
            base.Enabled = value;
        }
    }

    /// <inheritdoc cref="ControlBase.UnloadContent"/>
    /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        Label ??= this.controlFactory.CreateLabel(this.cachedText);

        Label?.LoadContent();

        /* Update the enabled status of the label to match the button.
         * If the enabled value was set before the content was loaded,
         * it will "cache" the value until the content is loaded
        */
        if (Label is not null)
        {
            Label.Enabled = base.Enabled;
            Label.Position = Position;

            /* Set the text to the cached text value if was set previously
             * This occurs when the text of the button was set before load content was invoked
             */
            Label.Text = string.IsNullOrEmpty(this.cachedText) ? string.Empty : this.cachedText;
        }

        base.LoadContent();
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        Label?.UnloadContent();

        base.UnloadContent();
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        if (!IsLoaded || !Visible)
        {
            return;
        }

        var faceColor = FaceColor;

        if (IsMouseOver)
        {
            faceColor = this.isMouseDown ? MouseDownColor : MouseHoverColor;
        }

        faceColor = Enabled ? faceColor : FaceColor.DecreaseBrightness(0.25f);

        var buttonFace = default(RectShape);
        buttonFace.Position = Position.ToVector2();
        buttonFace.IsSolid = true;
        buttonFace.Color = faceColor;
        buttonFace.Width = Width;
        buttonFace.Height = Height;
        buttonFace.CornerRadius = CornerRadius;

        this.rectRenderer.Render(buttonFace, BtnFaceLayer);

        if (BorderVisible)
        {
            var buttonBorder = default(RectShape);
            buttonBorder.Position = Position.ToVector2();
            buttonBorder.IsSolid = false;
            buttonBorder.BorderThickness = BorderThickness;
            buttonBorder.Color = Enabled ? BorderColor : BorderColor.DecreaseBrightness(0.25f);
            buttonBorder.Width = Width;
            buttonBorder.Height = Height;
            buttonBorder.CornerRadius = CornerRadius;

            this.rectRenderer.Render(buttonBorder, BorderLayer);
        }

        var textHeightNotTooLarge = Label?.Height <= Height;
        var textIsTooWide = Label?.Width > Width;
        var textToRender = Label?.Text ?? string.Empty;

        if (textHeightNotTooLarge && !string.IsNullOrEmpty(textToRender))
        {
            // If the text is wider than the button, figure out which characters are physically past
            // the left and right edges of the button and prevent them from being rendered.
            // The reason we check for the left and right side is because the text is centered.
            if (textIsTooWide && Label is not null)
            {
                var textCharBounds = Label.CharacterBounds.ToArray();

                var charsToRender = (from c in textCharBounds
                    where c.bounds.Left > Left &&
                          c.bounds.Right < Right
                    select c.character).ToArray();

                textToRender = new string(charsToRender);
            }

            Label?.Render(textToRender, TextLayer);
        }

        base.Render();
    }

    /// <summary>
    /// Invoked on mouse down and sets the mouse down state.
    /// </summary>
    protected override void OnMouseDown()
    {
        this.isMouseDown = true;
        base.OnMouseDown();
    }

    /// <summary>
    /// Invoked on mouse up and sets the mouse up state.
    /// </summary>
    protected override void OnMouseUp()
    {
        this.isMouseDown = false;
        base.OnMouseUp();
    }

    /// <summary>
    /// Initializes the <see cref="Button"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    private void Init()
    {
        this.controlFactory = new UIControlFactory();
        Width = DefaultButtonWidth;
        Height = DefaultButtonHeight;
        MouseHoverColor = FaceColor.IncreaseBrightness(HoverBrightness);
        MouseDownColor = FaceColor.DecreaseBrightness(MouseDownBrightness);
    }
}
