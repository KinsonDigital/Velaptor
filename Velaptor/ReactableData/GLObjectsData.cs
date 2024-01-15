// <copyright file="GLObjectsData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using System.Diagnostics.CodeAnalysis;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

/// <summary>
/// Holds data for sending push notifications with OpenGL objects.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Native SILK/OpenGL objects.")]
internal readonly record struct GLObjectsData
{
    /// <summary>
    /// Gets the OpenGL object.
    /// </summary>
    public GL? GL { get; init; }

    /// <summary>
    /// Gets the OpenGL window.
    /// </summary>
    public IWindow Window { get; init; }

    /// <summary>
    /// Gets the input context.
    /// </summary>
    public IInputContext InputContext { get; init; }
}
