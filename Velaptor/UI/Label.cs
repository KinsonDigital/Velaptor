// <copyright file="Label.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Content;
using Content.Fonts;
using Factories;
using Graphics.Renderers;
using Guards;
using Input;

/// <summary>
/// A label that renders text on the screen.
/// </summary>
public class Label : ControlBase
{
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const uint DefaultFontSize = 12;
    private readonly IContentLoader contentLoader;
    private readonly Color disabledColor = Color.DarkGray;
    private readonly IFontRenderer fontRenderer;
    private string labelText = string.Empty;
    private (char character, RectangleF bounds)[]? textCharBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    public Label()
    {
        var renderFactory = IoC.Container.GetInstance<IRendererFactory>();
        this.fontRenderer = renderFactory.CreateFontRenderer();

        this.contentLoader = ContentLoaderFactory.CreateContentLoader();
        Font = this.contentLoader.LoadFont(DefaultRegularFont, 12);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads various kinds of content.</param>
    /// <param name="mouse">Used to get the state of the mouse.</param>
    /// <param name="rendererFactory">Creates different types of renderers.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the any of the parameters below are null:
    ///     <list type="bullet">
    ///         <item><paramref name="contentLoader"/></item>
    ///     </list>
    /// </exception>
    internal Label(
        IContentLoader contentLoader,
        IAppInput<MouseState> mouse,
        IRendererFactory rendererFactory)
            : base(mouse)
    {
        EnsureThat.ParamIsNotNull(contentLoader);
        EnsureThat.ParamIsNotNull(rendererFactory);

        this.contentLoader = contentLoader;
        Font = this.contentLoader.LoadFont(DefaultRegularFont, DefaultFontSize);
        this.fontRenderer = rendererFactory.CreateFontRenderer();
    }

    /// <summary>
    /// Gets or sets the labelText of the label.
    /// </summary>
    public string Text
    {
        get => this.labelText;
        set
        {
            this.labelText = value;

            CalcTextCharacterBounds();
        }
    }

    /// <summary>
    /// Gets a list of all the bounds for each character of the <see cref="Label"/>.<see cref="Text"/>.
    /// </summary>
    public ReadOnlyCollection<(char character, RectangleF bounds)> CharacterBounds =>
        this.textCharBounds?.AsReadOnly() ?? Array.Empty<(char, RectangleF)>().AsReadOnly();

    /// <inheritdoc/>
    public override Point Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            CalcTextCharacterBounds();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the size of the <see cref="Label"/> will be
    /// managed automatically based on the size of the text.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSize"/> is <c>false</c>, it means that the user can set the size to what they
    ///     want.  If the size is less than the width or height of the text, then only the text characters
    ///     that are still within the bounds of the <see cref="Label"/> will be rendered.
    /// </remarks>
    public bool AutoSize { get; set; } = true;

    /// <summary>
    /// Gets or sets the width of the <see cref="Label"/>.
    /// </summary>
    public override uint Width
    {
        get
        {
            if (!AutoSize)
            {
                return base.Width;
            }

            var textSize = Font.Measure(this.labelText);
            base.Width = (uint)textSize.Width;
            return base.Width;
        }
        set => base.Width = value;
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="Label"/>.
    /// </summary>
    public override uint Height
    {
        get
        {
            var result = base.Height;

            if (!AutoSize)
            {
                return result;
            }

            var textSize = Font.Measure(this.labelText);
            return (uint)textSize.Height;
        }
        set => base.Height = value;
    }

    /// <summary>
    /// Gets or sets the font style of the text.
    /// </summary>
    public FontStyle Style
    {
        get => Font.Style;
        set => Font.Style = value;
    }

    /// <summary>
    /// Gets or sets the color of the text.
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// Gets the font for the label.
    /// </summary>
    public IFont Font { get; }

    /// <inheritdoc cref="ControlBase.UnloadContent"/>
    /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        base.LoadContent();

        CalcTextCharacterBounds();
    }

    /// <inheritdoc cref="ControlBase.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.contentLoader.UnloadFont(Font);

        base.UnloadContent();
    }

    /// <summary>
    /// Renders the <see cref="Label"/>.
    /// </summary>
    public override void Render() => Render(this.labelText, int.MaxValue - 1);

    /// <summary>
    /// Renders the <see cref="Label"/>.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="layer">The layer to render the label on.</param>
    internal void Render(string text, int layer)
    {
        if (IsLoaded is false || Visible is false)
        {
            return;
        }

        if (string.IsNullOrEmpty(text) is false)
        {
            this.fontRenderer.Render(
                Font,
                text,
                Position.X,
                Position.Y,
                1f,
                0f,
                Enabled ? Color : this.disabledColor,
                layer);
        }

        base.Render();
    }

    /// <summary>
    /// Calculates the bounds of each character of the labels <see cref="Text"/>.
    /// </summary>
    private void CalcTextCharacterBounds()
    {
        if (IsLoaded is false)
        {
            return;
        }

        // Offset the position by the half width for centering purposes
        var textPos = Position.ToVector2();
        textPos.X -= Width / 2f;

        var charBounds = Font.GetCharacterBounds(this.labelText, textPos);

        this.textCharBounds = charBounds.ToArray();
    }
}
