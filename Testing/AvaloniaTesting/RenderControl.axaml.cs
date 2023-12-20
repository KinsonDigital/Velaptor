// <copyright file="RenderControl.axaml.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AvaloniaTesting;

using Avalonia.Controls;
using System;
using System.Linq;
using System.Numerics;
using Avalonia.VisualTree;

/// <summary>
/// Used for rendering OpenGL.
/// </summary>
public partial class RenderControl : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderControl"/> class.
    /// </summary>
    public RenderControl()
    {
        InitializeComponent();

        this.Overlay.PointerReleased += (_, args) =>
        {
            var controlOverlay = this.GetVisualAncestors().OfType<UserControl>().FirstOrDefault();

            var x = (float)args.GetCurrentPoint(controlOverlay).Position.X;
            var y = (float)args.GetCurrentPoint(controlOverlay).Position.Y;

            this.GlControl.OnMouseReleased(new Vector2(x, y));
        };

        this.Overlay.PointerMoved += (_, args) =>
        {
            var controlOverlay = this.GetVisualAncestors().OfType<UserControl>().FirstOrDefault();

            var window = this.GetVisualAncestors().OfType<Window>().FirstOrDefault();

            var x = (int)Math.Round((float)args.GetCurrentPoint(controlOverlay).Position.X, 0);
            var y = (int)Math.Round((float)args.GetCurrentPoint(controlOverlay).Position.Y, 0);

            if (window is not null)
            {
                window.Title = $"X: {x}, Y: {y}";
            }
        };

        this.Overlay.SizeChanged += (_, args) =>
        {
            this.GlControl.RenderPosition = new Vector2((float)args.NewSize.Width / 2f, (float)args.NewSize.Height / 2f);
        };
    }
}
