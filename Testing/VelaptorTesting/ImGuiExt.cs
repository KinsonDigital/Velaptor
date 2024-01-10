// <copyright file="ImGuiExt.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Velaptor;
using Velaptor.NativeInterop.ImGui;

/// <summary>
/// Provides extension methods for the <see cref="ImGui"/> API.
/// </summary>
internal static class ImGuiExt
{
    /// <summary>
    /// Pushes the given <paramref name="color"/> onto a style for the given <paramref name="idx"/>.
    /// </summary>
    /// <param name="invoker">The ImGui function invoker.</param>
    /// <param name="idx">The UI element to push to color to.</param>
    /// <param name="color">The color to push.</param>
    public static void PushStyleColor(this IImGuiInvoker invoker, ImGuiCol idx, Color color) => invoker.PushStyleColor(idx, color.ToImGuiColor());

    /// <summary>
    /// Converts the given <see cref="Color"/> to an <see cref="ImGui"/> compatible color represented by a <see cref="Vector4"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted color.</returns>
    private static Vector4 ToImGuiColor(this Color value)
    {
        var r = value.R.MapValue(0f, 255f, 0f, 1f);
        var g = value.G.MapValue(0f, 255f, 0f, 1f);
        var b = value.B.MapValue(0f, 255f, 0f, 1f);
        var a = value.A.MapValue(0f, 255f, 0f, 1f);

        return new Vector4(r, g, b, a);
    }
}
