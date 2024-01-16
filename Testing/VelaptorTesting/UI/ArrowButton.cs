// <copyright file="ArrowButton.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Drawing;
using Carbonate.NonDirectional;
using ImGuiNET;
using Velaptor.NativeInterop.ImGui;

/// <inheritdoc cref="IArrowButton"/>
internal sealed class ArrowButton : Control, IArrowButton
{
    private readonly string id = $"##arrow-btn-{Guid.NewGuid().ToString()}";
    private bool isMousePressedInvoked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrowButton"/> class.
    /// </summary>
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="renderReactable">Manages render notifications.</param>
    public ArrowButton(IImGuiInvoker imGuiInvoker, IPushReactable renderReactable)
        : base(imGuiInvoker, renderReactable)
    {
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? Click;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? MousePressed;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? MouseReleased;

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        this.Click = null;
        this.MousePressed = null;
        this.MouseReleased = null;
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void Render()
    {
        if (!Visible)
        {
            return;
        }

        ImGuiInvoker.PushID(this.id);

        ImGuiInvoker.PushStyleColor(ImGuiCol.Button, Color.IndianRed);

        ImGuiInvoker.ArrowButton(this.id, ImGuiDir.Down);

        ImGuiInvoker.PopStyleColor(1);
        ImGuiInvoker.PopID();

        ProcessMouseEvents();
    }

    /// <summary>
    /// Processes the mouse events.
    /// </summary>
    private void ProcessMouseEvents()
    {
        if (!ImGuiInvoker.IsItemHovered())
        {
            return;
        }

        if (ImGuiInvoker.IsMouseDown(ImGuiMouseButton.Left) && !this.isMousePressedInvoked)
        {
            this.MousePressed?.Invoke(this, EventArgs.Empty);
            this.isMousePressedInvoked = true;
        }

        if (!ImGuiInvoker.IsMouseReleased(ImGuiMouseButton.Left))
        {
            return;
        }

        this.isMousePressedInvoked = false;

        this.MouseReleased?.Invoke(this, EventArgs.Empty);
        this.Click?.Invoke(this, EventArgs.Empty);
    }
}
