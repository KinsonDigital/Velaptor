// <copyright file="SceneManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Collections.Generic;
using System.Linq;
using Exceptions;

/// <summary>
/// Manages scenes by loading and unloading content, updating, and rendering scenes.
/// </summary>
internal sealed class SceneManager : ISceneManager
{
    private readonly List<(IScene scene, bool isActive)> scenes = [];
    private bool isDisposed;

    /// <inheritdoc/>
    public IScene? CurrentScene
    {
        get
        {
            var foundScene = this.scenes.IndexOf(s => s.isActive);

            return foundScene == -1 ? null : this.scenes[foundScene].scene;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<Guid> InActiveScenes =>
        this.scenes.Where(s => !s.isActive)
            .Select(s => s.scene.Id).ToArray().AsReadOnly();

    /// <inheritdoc/>
    public bool IsLoaded { get; private set; }

    /// <inheritdoc/>
    public int TotalScenes => this.scenes.Count;

    /// <inheritdoc/>
    public int CurrentSceneIndex { get; private set; }

    /// <inheritdoc/>
    public bool UsesNavigationWrapping { get; set; } = true;

    /// <inheritdoc/>
    public void AddScene(IScene scene) => AddScene(scene, false);

    /// <inheritdoc/>
    /// <remarks>
    ///     If no scenes exist in the manager, the scene will be active even if the <paramref name="setToActive"/>
    ///     parameter is set to <c>false</c>.
    ///     <br/>
    ///     This is because there has to be one active scene at all times.
    /// </remarks>
    /// <exception cref="Exception">
    ///     Thrown if a scene with the given <paramref name="scene"/>'s ID already exists.
    /// </exception>
    public void AddScene(IScene scene, bool setToActive)
    {
        if (SceneExists(scene.Id))
        {
            throw new SceneAlreadyExistsException(scene.Name, scene.Id);
        }

        // If the scene is to be set to active, set all the other scenes to false first.
        // Only one scene can be active at a time.
        if (setToActive)
        {
            for (var i = 0; i < this.scenes.Count; i++)
            {
                this.scenes[i] = (this.scenes[i].scene, false);
            }
        }

        // If no scene exists, set the scene as active regardless of the setToActive value
        // There has to be a single active scene.
        this.scenes.Add((scene, this.scenes.Count <= 0 || setToActive));

        if (this.scenes.Count <= 0 || setToActive)
        {
            CurrentSceneIndex = this.scenes.Count - 1;
        }
    }

    /// <inheritdoc/>
    public void RemoveScene(Guid sceneId)
    {
        if (this.scenes.Count <= 0)
        {
            return;
        }

        var sceneToRemove = this.scenes.Find(s => s.scene.Id == sceneId);
        var sceneBeingRemovedIndex = this.scenes.IndexOf(sceneToRemove);

        if (sceneBeingRemovedIndex < 0)
        {
            return;
        }

        if (sceneToRemove.isActive)
        {
            var lastIndex = this.scenes.Count - 1;

            var sceneToActivateIndex = sceneBeingRemovedIndex == 0
                ? lastIndex
                : sceneBeingRemovedIndex - 1;

            // Make the previous item the active scene
            this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        }

        this.scenes[sceneBeingRemovedIndex].scene.UnloadContent();
        this.scenes.Remove(sceneToRemove);
    }

    /// <inheritdoc/>
    public void NextScene()
    {
        if (this.scenes.Count <= 1)
        {
            return;
        }

        var isAtLastScene = CurrentSceneIndex == this.scenes.Count - 1;

        var nextSceneIndex = UsesNavigationWrapping ? 0 : CurrentSceneIndex;
        var incrementAmount = UsesNavigationWrapping && isAtLastScene ? 0 : 1;

        var sceneToDeactivateIndex = this.scenes.IndexOf(s => s.isActive);
        var sceneToDeactivateIsLastItem = sceneToDeactivateIndex >= this.scenes.Count - 1;

        var sceneToActivateIndex = sceneToDeactivateIsLastItem
            ? nextSceneIndex
            : sceneToDeactivateIndex + incrementAmount;

        CurrentSceneIndex = sceneToActivateIndex;

        if (!UsesNavigationWrapping && isAtLastScene)
        {
            return;
        }

        this.scenes[sceneToDeactivateIndex] = (this.scenes[sceneToDeactivateIndex].scene, false);
        this.scenes[sceneToDeactivateIndex].scene.UnloadContent();

        this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        this.scenes[sceneToActivateIndex].scene.LoadContent();
    }

    /// <inheritdoc/>
    public void PreviousScene()
    {
        if (this.scenes.Count <= 1)
        {
            return;
        }

        var isAtFirstScene = CurrentSceneIndex == 0;
        var nextSceneIndex = UsesNavigationWrapping ? this.scenes.Count - 1 : CurrentSceneIndex;
        var decrementAmount = !UsesNavigationWrapping && isAtFirstScene ? 0 : 1;

        var sceneToDeactivateIndex = this.scenes.IndexOf(s => s.isActive);
        var sceneToDeactivateIsFirstItem = sceneToDeactivateIndex <= 0;

        var sceneToActivateIndex = sceneToDeactivateIsFirstItem
            ? nextSceneIndex
            : sceneToDeactivateIndex - decrementAmount;

        CurrentSceneIndex = sceneToActivateIndex;

        if (!UsesNavigationWrapping && isAtFirstScene)
        {
            return;
        }

        this.scenes[sceneToDeactivateIndex] = (this.scenes[sceneToDeactivateIndex].scene, false);
        this.scenes[sceneToDeactivateIndex].scene.UnloadContent();

        this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        this.scenes[sceneToActivateIndex].scene.LoadContent();
    }

    /// <inheritdoc/>
    public void SetSceneAsActive(Guid id)
    {
        var sceneToSetIndex = this.scenes.IndexOf(s => s.scene.Id == id);

        if (sceneToSetIndex == -1)
        {
            throw new SceneDoesNotExistException(id);
        }

        for (var i = 0; i < this.scenes.Count; i++)
        {
            var sceneData = this.scenes[i];
            sceneData.isActive = false;

            this.scenes[i] = sceneData;
        }

        this.scenes[sceneToSetIndex] = (this.scenes[sceneToSetIndex].scene, true);
    }

    /// <inheritdoc/>
    public void LoadContent()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(SceneManager), "Cannot load a scene manager that has been disposed.");
        }

        if (IsLoaded)
        {
            return;
        }

        CurrentScene?.LoadContent();

        IsLoaded = true;
    }

    /// <inheritdoc/>
    public void UnloadContent()
    {
        if (!IsLoaded || this.isDisposed)
        {
            return;
        }

        UnloadAllSceneContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public void Update(FrameTime frameTime) => CurrentScene?.Update(frameTime);

    /// <inheritdoc cref="IDrawable.Render"/>
    public void Render() => CurrentScene?.Render();

    /// <summary>
    /// Returns a value indicating whether a scene with the given ID already exists.
    /// </summary>
    /// <param name="id">The ID of the scene to check for.</param>
    /// <returns>True if the scene exists.</returns>
    public bool SceneExists(Guid id) => this.scenes.Exists(s => s.scene.Id == id);

    /// <inheritdoc />
    public void Resize(SizeU size)
    {
        foreach ((IScene? scene, _) in this.scenes)
        {
            scene.Resize(size);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        UnloadAllSceneContent();
        this.scenes.Clear();

        this.isDisposed = true;
    }

    /// <summary>
    /// Unloads the content from all the scenes.
    /// </summary>
    private void UnloadAllSceneContent()
    {
        foreach ((IScene scene, _) in this.scenes)
        {
            scene.UnloadContent();
        }
    }
}
