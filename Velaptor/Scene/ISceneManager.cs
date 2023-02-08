// <copyright file="ISceneManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Manages scenes by loading and unloading content, updating, and rendering scenes.
/// </summary>
public interface ISceneManager : IUpdatable, IDrawable, IDisposable
{
    /// <summary>
    /// Gets the current scene.
    /// </summary>
    public IScene? CurrentScene { get; }

    /// <summary>
    /// Gets a list of all the <see cref="Guid"/>'s for the scenes that are inactive.
    /// </summary>
    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global", Justification = "Intended to be readonly.")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    IReadOnlyCollection<Guid> InActiveScenes { get; }

    /// <summary>
    /// Adds the given scene.
    /// </summary>
    /// <param name="scene">The scene to add.</param>
    /// <remarks>The scene will not be activated when added using this method.</remarks>
    public void AddScene(IScene scene);

    /// <summary>
    /// Adds the given scene and sets it as active or inactive.
    /// </summary>
    /// <param name="scene">The scene to add.</param>
    /// <param name="setToActive">
    ///     When set to <c>true</c>, the scene being added will be set to active and
    ///     the all other scenes will bet set to inactive.
    /// </param>
    public void AddScene(IScene scene, bool setToActive);


    /// <summary>
    /// Removes the scene that matches the given <see cref="sceneId"/>.
    /// </summary>
    /// <param name="sceneId">The ID of the scene to remove.</param>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used for library users.")]
    public void RemoveScene(Guid sceneId);

    /// <summary>
    /// Moves to the next scene.
    /// </summary>
    public void NextScene();

    /// <summary>
    /// Moves to the previous scene.
    /// </summary>
    public void PreviousScene();

    /// <summary>
    /// Sets a scene that matches the given <paramref name="id"/> to be the active scene.
    /// </summary>
    /// <param name="id">The ID of the scene.</param>
    /// <remarks>
    ///     This will set all of the other scenes to inactive.
    /// </remarks>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    void SetSceneAsActive(Guid id);

    /// <summary>
    /// Loads the content for the manager and the current scene.
    /// </summary>
    public void LoadContent();

    /// <summary>
    /// Unloads the scene manager content and added scenes.
    /// </summary>
    public void UnloadContent();

    /// <summary>
    /// Returns a value indicating whether or not a scene exists that matches the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the scene.</param>
    /// <returns>
    ///     <c>true</c> if a scene exists with the given ID.
    /// </returns>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    bool SceneExists(Guid id);
}
