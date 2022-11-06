// <copyright file="IContentUnloadable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Provides the ability to unload content.
/// </summary>
public interface IContentUnloadable
{
    /// <summary>
    /// Unloads the content using the given <paramref name="contentLoader"/>.
    /// </summary>
    /// <param name="contentLoader">Used to unload content.</param>
    void UnloadContent(IContentLoader contentLoader);
}
