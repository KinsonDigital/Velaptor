// <copyright file="Slider.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Carbonate.NonDirectional;
using ImGuiNET;
using Velaptor.NativeInterop.ImGui;

/// <inheritdoc cref="ISlider"/>
internal sealed class Slider : Control, ISlider
{
    private readonly string id = Guid.NewGuid().ToString();
    private float value;
    private float min;
    private float max;
    private string text = "Slider:";

    /// <summary>
    /// Initializes a new instance of the <see cref="Slider"/> class.
    /// </summary>
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="renderReactable">Manages render notifications.</param>
    public Slider(IImGuiInvoker imGuiInvoker, IPushReactable renderReactable)
        : base(imGuiInvoker, renderReactable) =>
        Width = 100;

    /// <inheritdoc/>
    public event EventHandler<float>? ValueChanged;

    /// <inheritdoc/>
    public string Text
    {
        get => this.text;
        set => this.text = value.EndsWith(':') ? value : $"{value}:";
    }

    /// <inheritdoc/>
    [SuppressMessage(
        "csharpsquid | Make this an auto-implemented property and remove its backing field",
        "S2292",
        Justification = "Backing field required for reference usage for ImGuiInvoker.")]
    public float Value
    {
        get => this.value;
        set => this.value = value;
    }

    /// <inheritdoc/>
    public float Min
    {
        get => this.min;
        set
        {
            this.min = value;
            this.value = this.value < this.min ? this.min : this.value;
        }
    }

    /// <inheritdoc/>
    public float Max
    {
        get => this.max;
        set
        {
            this.max = value;
            this.value = this.value > this.max ? this.max : this.value;
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        this.ValueChanged = null;
        base.Dispose(disposing);
    }

    /// <inheritdoc cref="Control.Render"/>
    protected override void Render()
    {
        if (!Visible)
        {
            return;
        }

        var currentPos = ImGuiInvoker.GetCursorPos();

        // Bump the label down to be vertically centered with the combo box
        ImGuiInvoker.SetCursorPos(currentPos with { Y = currentPos.Y + 5 });

        ImGuiInvoker.PushID(this.id);
        ImGuiInvoker.PushStyleColor(ImGuiCol.Text, Color.White);

        ImGuiInvoker.Text(this.text);

        ImGuiInvoker.PopID();

        var spacing = ImGuiInvoker.GetStyle().ItemSpacing.X;
        ImGuiInvoker.SameLine(0, spacing);

        currentPos = ImGuiInvoker.GetCursorPos();

        ImGuiInvoker.SetNextItemWidth(Width);

        // Bump the combo box up to be vertically centered with the label
        ImGuiInvoker.SetCursorPos(currentPos with { Y = currentPos.Y - 3 });
        if (ImGuiInvoker.SliderFloat($"##Slider{this.id}", ref this.value, this.min, this.max))
        {
            this.ValueChanged?.Invoke(this, this.value);
        }
    }
}
