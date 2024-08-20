// <copyright file="IContentLoadable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides the ability to load content.
/// </summary>
public interface IContentLoadable
{
    /// <summary>
    /// Gets a value indicating whether the content for an object is loaded.
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "UnusedMemberInSuper.Global",
        Justification = "Public API for users.")]
    bool IsLoaded { get; }

    /// <summary>
    /// Loads the content for an object.
    /// </summary>
    void LoadContent();

    /// <summary>
    /// Unloads the content for an object.
    /// </summary>
    [SuppressMessage(
        "ReSharper",
        "UnusedMemberInSuper.Global",
        Justification = "Public API for users.")]
    void UnloadContent();
}
