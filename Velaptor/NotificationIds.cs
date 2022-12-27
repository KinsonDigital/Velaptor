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
    /// Gets the unique event <see cref="Guid"/> for push notifications of when the OpenGL context is created.
    /// </summary>
    public static Guid GLContextCreatedId { get; } = new ("c44ff8ef-d7fe-4ede-8f72-f4d0d57a721c");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when OpenGL has been initialized.
    /// </summary>
    public static Guid GLInitializedId { get; } = new ("2ef5c76f-c7ec-4f8b-b73e-c114b7cfbe2b");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when the batch size is set.
    /// </summary>
    public static Guid BatchSizeSetId { get; } = new ("7819e8f0-9797-4bfd-b9ed-6505c8a6ca89");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for the push notifications for when the mouse state has changed.
    /// </summary>
    public static Guid MouseStateChangedId { get; } = new ("b63c2dcd-3ce4-475e-b574-8951413ff381");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for the push notifications for when the keyboard state has changed.
    /// </summary>
    public static Guid KeyboardStateChangedId { get; } = new ("a18686c8-10c9-4ba9-afaf-ceea77c130e2");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when a texture is disposed.
    /// </summary>
    public static Guid TextureDisposedId { get; } = new ("953d4a76-6c3e-49b2-a609-e73b2add942a");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when a sound is disposed.
    /// </summary>
    public static Guid SoundDisposedId { get; } = new ("863983d2-6657-4c8e-8e9a-f3cbd688abe1");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when the system is shutting down.
    /// </summary>
    public static Guid SystemShuttingDownId { get; } = new ("17b9fd1c-67ef-4f36-8973-45e32b0ee85b");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when the window size changes.
    /// </summary>
    public static Guid WindowSizeChangedId { get; } = new ("d1095c6e-cf1f-4719-b7e2-aeeb285d1d02");

    /// <summary>
    /// Gets the unique event <see cref="Guid"/> for push notifications for when the viewport size changes.
    /// </summary>
    public static Guid ViewPortSizeChangedId { get; } = new ("430e7d43-ffd5-4f81-90b9-039e05ed490e");
}
