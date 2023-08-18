// <copyright file="PushNotifications.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;

/// <summary>
/// Contains unique GUIDs for the push notification system.
/// </summary>
internal static class PushNotifications
{
    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications of when the OpenGL context is created.
    /// </summary>
    public static Guid GLContextCreatedId { get; } = new ("c44ff8ef-d7fe-4ede-8f72-f4d0d57a721c");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when OpenGL has been initialized.
    /// </summary>
    public static Guid GLInitializedId { get; } = new ("2ef5c76f-c7ec-4f8b-b73e-c114b7cfbe2b");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the batch size is set.
    /// </summary>
    public static Guid BatchSizeChangedId { get; } = new ("7819e8f0-9797-4bfd-b9ed-6505c8a6ca89");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for the push notifications for when the mouse state has changed.
    /// </summary>
    public static Guid MouseStateChangedId { get; } = new ("b63c2dcd-3ce4-475e-b574-8951413ff381");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for the push notifications for when the keyboard state has changed.
    /// </summary>
    public static Guid KeyboardStateChangedId { get; } = new ("a18686c8-10c9-4ba9-afaf-ceea77c130e2");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when a texture is disposed.
    /// </summary>
    public static Guid TextureDisposedId { get; } = new ("953d4a76-6c3e-49b2-a609-e73b2add942a");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when a sound is disposed.
    /// </summary>
    public static Guid SoundDisposedId { get; } = new ("863983d2-6657-4c8e-8e9a-f3cbd688abe1");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the system is shutting down.
    /// </summary>
    public static Guid SystemShuttingDownId { get; } = new ("17b9fd1c-67ef-4f36-8973-45e32b0ee85b");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the window size changes.
    /// </summary>
    public static Guid WindowSizeChangedId { get; } = new ("d1095c6e-cf1f-4719-b7e2-aeeb285d1d02");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the viewport size changes.
    /// </summary>
    public static Guid ViewPortSizeChangedId { get; } = new ("430e7d43-ffd5-4f81-90b9-039e05ed490e");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the render batch has started.
    /// </summary>
    public static Guid BatchHasBegunId { get; } = new ("845e89b2-5a9d-4091-8689-d56f5c3060f3");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the render batch has been ended.
    /// </summary>
    public static Guid BatchHasEndedId { get; } = new ("ea261a35-58f9-4ddd-8301-004564311002");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications to empty the batch.
    /// </summary>
    public static Guid EmptyBatchId { get; } = new ("e2d7eb6a-53f0-4d10-9941-ad1c64706764");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the textures need to be rendered.
    /// </summary>
    public static Guid RenderTexturesId { get; } = new ("0b2e75f4-cc15-4cdd-b14c-f5732644e818");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the fonts need to be rendered.
    /// </summary>
    public static Guid RenderFontsId { get; } = new ("95cb1356-ae02-46f4-900c-651d50bc0de4");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the rectangles need to be rendered.
    /// </summary>
    public static Guid RenderShapesId { get; } = new ("27c20138-52d3-4b5d-936d-3b62e3b7db4d");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for push notifications for when the lines need to be rendered.
    /// </summary>
    public static Guid RenderLinesId { get; } = new ("3fb13cdb-db24-4d28-b117-b9604722277f");
}
