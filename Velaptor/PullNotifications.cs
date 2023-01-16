// <copyright file="PullNotifications.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;

/// <summary>
/// Contains unique GUIDs for the pull notification system.
/// </summary>
internal static class PullNotifications
{
    /// <summary>
    /// Gets the unique <see cref="Guid"/> for pull notifications of when to pull texture render items for rendering.
    /// </summary>
    public static Guid GetTextureRenderItemsId { get; } = new Guid("984ce7d0-5662-497d-844b-017e39bd1801");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for pull notifications of when to pull font render items for rendering.
    /// </summary>
    public static Guid GetFontRenderItemsId { get; } = new Guid("20ea7a83-639f-4064-b91b-c8fd99518ab8");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for pull notifications of when to pull rectangle render items for rendering.
    /// </summary>
    public static Guid GetRectRenderItemsId { get; } = new Guid("7519364f-9dc0-41d7-afde-18a5aa859422");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for pull notifications of when to pull line render items for rendering.
    /// </summary>
    public static Guid GetLineRenderItemsId { get; } = new Guid("a8a2bf09-8e8d-4dc5-a487-d4c330257729");
}
