// <copyright file="NotificationIds.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using Carbonate;

/// <summary>
/// Contains unique event GUIDs for the <see cref="Reactable"/> push notification system.
/// </summary>
internal static class NotificationIds
{
    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for the OpenGL context.
    /// </summary>
    public static Guid GLContextId { get; } = new ("c44ff8ef-d7fe-4ede-8f72-f4d0d57a721c");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for batch sizes.
    /// </summary>
    public static Guid BatchSizeId { get; } = new ("7819e8f0-9797-4bfd-b9ed-6505c8a6ca89");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for the push notifications of the mouse position.
    /// </summary>
    public static Guid MouseId { get; } = new ("b63c2dcd-3ce4-475e-b574-8951413ff381");
}
