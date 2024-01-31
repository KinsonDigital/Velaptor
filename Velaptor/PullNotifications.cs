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
    /// Gets the unique <see cref="Guid"/> for pull notifications of when requesting the size of the window.
    /// </summary>
    public static Guid GetWindowSizeId { get; } = new ("76bef3ba-a4c8-49c8-9842-9fa4e3bd7692");
}
