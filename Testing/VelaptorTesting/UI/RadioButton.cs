// <copyright file="RadioButton.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using Carbonate.NonDirectional;
using ImGuiNET;
using Velaptor.NativeInterop.ImGui;

/// <inheritdoc cref="IRadioButton"/>
internal class RadioButton : Control, IRadioButton
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RadioButton"/> class.
    /// </summary>
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="renderReactable">Manages render notifications.</param>
    public RadioButton(IImGuiInvoker imGuiInvoker, IPushReactable renderReactable)
        : base(imGuiInvoker, renderReactable)
    {
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? Selected;

    /// <inheritdoc/>
    public string Text { get; set; } = "Radio Button";

    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <inheritdoc/>
    protected override void Render()
    {
        if (ImGui.RadioButton(Text, IsSelected))
        {
            IsSelected = true;
            this.Selected?.Invoke(this, EventArgs.Empty);
        }
    }
}
