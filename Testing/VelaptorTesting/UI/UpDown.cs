// <copyright file="UpDown.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Globalization;
using Carbonate.NonDirectional;
using ImGuiNET;
using Velaptor.NativeInterop.ImGui;

/// <summary>
/// <inheritdoc cref="IUpDown"/>
/// </summary>
internal sealed class UpDown : Control, IUpDown
{
    private readonly string upId = Guid.NewGuid().ToString();
    private readonly string downId = Guid.NewGuid().ToString();
    private string text = "ButtonRepeat";

    /// <summary>
    /// Initializes a new instance of the <see cref="UpDown"/> class.
    /// </summary>
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="renderReactable">Manages render notifications.</param>
    public UpDown(IImGuiInvoker imGuiInvoker, IPushReactable renderReactable)
        : base(imGuiInvoker, renderReactable)
    {
    }

    public event EventHandler<float>? ValueChanged;

    /// <inheritdoc/>
    public float Value { get; set; }

    /// <inheritdoc/>
    public float Increment { get; set; } = 1f;

    /// <inheritdoc/>
    public float Decrement { get; set; } = 1f;

    /// <inheritdoc/>
    public float Min { get; set; } = 0;

    /// <inheritdoc/>
    public float Max { get; set; } = 100;

    /// <inheritdoc/>
    public string Text
    {
        get => this.text;
        set => this.text = value.EndsWith(':') ? value : $"{value}:";
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        this.ValueChanged = null;
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void Render()
    {
        ImGuiInvoker.Text(Text);
        var textSize = ImGuiInvoker.CalcTextSize(Text);

        var spacing = ImGuiInvoker.GetStyle().ItemInnerSpacing.X;
        var totalSpacing = spacing * 3;
        ImGuiInvoker.SameLine(0.0f, spacing);

        // Set the repeat button to be able to repeat when held down
        ImGuiInvoker.PushButtonRepeat(true);

        var upBtnHeight = (uint)ImGuiInvoker.GetFrameHeightWithSpacing();
        if (ImGuiInvoker.ArrowButton(this.upId, ImGuiDir.Up))
        {
            Value += Increment;
            this.ValueChanged?.Invoke(this, Value);
        }

        ImGuiInvoker.SameLine(0.0f, spacing);

        var downBtnHeight = (int)ImGuiInvoker.GetFrameHeightWithSpacing();
        if (ImGuiInvoker.ArrowButton(this.downId, ImGuiDir.Down))
        {
            Value -= Decrement;
            this.ValueChanged?.Invoke(this, Value);
        }

        Value = Value < Min ? Min : Value;
        Value = Value > Max ? Max : Value;

        ImGuiInvoker.PopButtonRepeat();
        ImGuiInvoker.SameLine();
        var valueText = Value.ToString(CultureInfo.InvariantCulture);
        ImGuiInvoker.Text(valueText);
        var valueTextSize = ImGuiInvoker.CalcTextSize(valueText);

        Width = (int)(textSize.X + totalSpacing + valueTextSize.X);
        Height = (int)Math.Max(Math.Max(textSize.Y, upBtnHeight), Math.Max(downBtnHeight, valueTextSize.Y));
    }
}
