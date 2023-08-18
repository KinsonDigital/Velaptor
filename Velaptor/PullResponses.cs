// <copyright file="PullResponses.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;

/// <summary>
/// Contains unique GUIDs for the request notification system.
/// </summary>
internal static class PullResponses
{
    /// <summary>
    /// Gets the unique <see cref="Guid"/> for request notifications for returning texture batch items.
    /// </summary>
    public static Guid GetTextureItemsId { get; } = new ("de7a986e-2605-4f16-98f3-749fb39ada40");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for request notifications for returning font batch items.
    /// </summary>
    public static Guid GetFontItemsId { get; } = new ("5e0ca7c0-f1de-4468-b488-65b71fa1bb3f");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for request notifications for returning rect batch items.
    /// </summary>
    public static Guid GetShapeItemsId { get; } = new ("bb5b2b6e-05f0-4404-acf2-7d19fca83fa1");

    /// <summary>
    /// Gets the unique <see cref="Guid"/> for request notifications for returning line batch items.
    /// </summary>
    public static Guid GetLineItemsId { get; } = new ("9c76e376-b612-44a5-adfd-7af921c5756b");
}
