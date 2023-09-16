// <copyright file="Label.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Content;
using Content.Exceptions;
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
    private readonly Color disabledColor = Color.DarkGray;
    private IContentLoader contentLoader = null!;
    private IFontRenderer fontRenderer = null!;
    private CachedValue<uint> cachedWidth = null!;
    private CachedValue<uint> cachedHeight = null!;
    private CachedValue<FontStyle> cachedFontStyle = null!;
    private CachedValue<uint> cachedFontSize = null!;
    private string labelText = string.Empty;
    private (char character, RectangleF bounds)[]? textCharBounds;
    private IFont? font;

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    public Label()
    {
        Keyboard = IoC.Container.GetInstance<IAppInput<KeyboardState>>();

        Init(ContentLoaderFactory.CreateContentLoader(), IoC.Container.GetInstance<IRendererFactory>().CreateFontRenderer());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="text">The text of the label.</param>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Label(string text)
    {
        Keyboard = IoC.Container.GetInstance<IAppInput<KeyboardState>>();
        Text = text;

        Init(ContentLoaderFactory.CreateContentLoader(), IoC.Container.GetInstance<IRendererFactory>().CreateFontRenderer());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads various kinds of content.</param>
    /// <param name="keyboard">Manages keyboard input.</param>
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
        IAppInput<KeyboardState> keyboard,
        IAppInput<MouseState> mouse,
        IRendererFactory rendererFactory)
            : base(keyboard, mouse)
    {
        EnsureThat.ParamIsNotNull(contentLoader);
        EnsureThat.ParamIsNotNull(rendererFactory);

        Init(contentLoader, rendererFactory.CreateFontRenderer());
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
    public IReadOnlyCollection<(char character, RectangleF bounds)> CharacterBounds =>
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
        get => this.cachedWidth.GetValue();
        set => this.cachedWidth.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="Label"/>.
    /// </summary>
    public override uint Height
    {
        get => this.cachedHeight.GetValue();
        set => this.cachedHeight.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the font style of the text.
    /// </summary>
    public FontStyle Style
    {
        get => this.cachedFontStyle.GetValue();
        set => this.cachedFontStyle.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public uint FontSize
    {
        get => this.cachedFontSize.GetValue();
        set => this.cachedFontSize.SetValue(value);
    }

    /// <summary>
    /// Gets the font family name.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
    public string FontFamilyName => this.font?.FamilyName ?? string.Empty;

    /// <summary>
    /// Gets or sets the color of the text.
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <inheritdoc cref="ControlBase.UnloadContent"/>
    /// <exception cref="Exception">Thrown if the control has been disposed.</exception>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        var loadedFont = this.contentLoader.LoadFont(DefaultRegularFont, DefaultFontSize);

        this.font = loadedFont ?? throw new LoadFontException("Failed to load the default font for the label.");

        base.LoadContent();

        CalcTextCharacterBounds();

        this.cachedWidth.IsCaching = false;
        this.cachedHeight.IsCaching = false;
    }

    /// <inheritdoc cref="ControlBase.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        if (this.font is not null)
        {
            this.contentLoader.UnloadFont(this.font);
        }

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
    /// <param name="layer">The layer to render the label.</param>
    internal void Render(string text, int layer)
    {
        if (!IsLoaded || !Visible)
        {
            return;
        }

        if (!string.IsNullOrEmpty(text) && this.font is not null)
        {
            this.fontRenderer.Render(
                this.font,
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
    /// Initializes the <see cref="Label"/> class.
    /// </summary>
    /// <param name="newContentLoader">Loads various kinds of content.</param>
    /// <param name="newFontRenderer">Renders font.</param>
    private void Init(IContentLoader newContentLoader, IFontRenderer newFontRenderer)
    {
        this.contentLoader = newContentLoader;
        this.fontRenderer = newFontRenderer;

        this.cachedWidth = new (
            defaultValue: base.Width,
            getterWhenNotCaching: () =>
            {
                if (!AutoSize)
                {
                    return base.Width;
                }

                var textLines = this.labelText.Split("\n");

                var textSize = textLines.Max(l => this.font?.Measure(l).Width);

                return textSize is null ? 0u : (uint)textSize;
            },
            setterWhenNotCaching: (value) =>
            {
                base.Width = value;
            });

        this.cachedHeight = new (
            defaultValue: base.Width,
            getterWhenNotCaching: () =>
            {
                if (!AutoSize)
                {
                    return base.Height;
                }

                var textSize = this.font?.Measure(this.labelText) ?? SizeF.Empty;
                return (uint)textSize.Height;
            },
            setterWhenNotCaching: (value) =>
            {
                base.Height = value;
            });

        this.cachedFontStyle = new (
            defaultValue: this.font?.Style ?? FontStyle.Regular,
            getterWhenNotCaching: () => this.font?.Style ?? FontStyle.Regular,
            setterWhenNotCaching: (value) =>
            {
                if (this.font is null)
                {
                    return;
                }

                this.font.Style = value;
            });

        this.cachedFontSize = new (
            defaultValue: DefaultFontSize,
            getterWhenNotCaching: () => this.font?.Size ?? 0u,
            setterWhenNotCaching: (value) =>
            {
                if (this.font is null)
                {
                    return;
                }

                this.font.Size = value;
            });
    }

    /// <summary>
    /// Calculates the bounds of each character of the labels <see cref="Text"/>.
    /// </summary>
    private void CalcTextCharacterBounds()
    {
        if (!IsLoaded)
        {
            return;
        }

        // Offset the position by the half width for centering purposes
        var textPos = Position.ToVector2();
        textPos.X -= Width / 2f;

        var charBounds = this.font?.GetCharacterBounds(this.labelText, textPos);

        this.textCharBounds = charBounds?.ToArray();
    }
}
