// <copyright file="SceneManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Collections.Generic;
using System.Linq;
using Exceptions;
using Velaptor;

/// <summary>
/// Manages scenes by loading and unloading content, updating, and rendering scenes.
/// </summary>
public sealed class SceneManager : ISceneManager
{
    private readonly List<(IScene? scene, bool isActive)> scenes = new ();
    private bool isDisposed;
    private bool isLoaded;

    /// <inheritdoc/>
    public IScene? CurrentScene => this.scenes.FirstOrDefault(s => s.isActive).scene;

    /// <inheritdoc/>
    public IReadOnlyCollection<Guid> InActiveScenes =>
        this.scenes.Where(s => s.scene is not null && s.isActive is false)
            .Select(s => s.scene?.Id ?? Guid.Empty).ToArray().AsReadOnly();

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
    ///     Thrown if a scene with with the given <paramref name="scene"/>'s ID already exists.
    /// </exception>
    public void AddScene(IScene scene, bool setToActive)
    {
        if (SceneExists(scene.Id))
        {
            throw new SceneAlreadyExistsException(scene.Name, scene.Id);
        }

        // If the scene is to be set to active, set all of the other scenes to false first.
        // Only one scene can be active at a time.
        if (setToActive)
        {
            for (var i = 0; i < this.scenes.Count; i++)
            {
                this.scenes[i] = (this.scenes[i].scene, false);
            }
        }

        // If no scenes exists, set the scene as active regardless of the setToActive value
        // There has to be a single active scene.
        this.scenes.Add((scene, this.scenes.Count <= 0 || setToActive));
    }

    /// <inheritdoc/>
    public void RemoveScene(Guid sceneId)
    {
        if (this.scenes.Count <= 0)
        {
            return;
        }

        var sceneToRemove = this.scenes.FirstOrDefault(s => s.scene?.Id == sceneId);

        if (sceneToRemove.scene is null)
        {
            return;
        }

        var sceneBeingRemovedIndex = this.scenes.IndexOf(sceneToRemove);

        if (sceneToRemove.isActive)
        {
            var lastIndex = this.scenes.Count - 1;

            var sceneToActivateIndex = sceneBeingRemovedIndex == 0
                ? lastIndex
                : sceneBeingRemovedIndex - 1;

            // Make the previous item the active scene
            this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        }

        this.scenes[sceneBeingRemovedIndex].scene?.UnloadContent();
        this.scenes.Remove(sceneToRemove);
    }

    /// <inheritdoc/>
    public void NextScene()
    {
        if (this.scenes.Count <= 1)
        {
            return;
        }

        var sceneToDeactivateIndex = this.scenes.IndexOf(s => s.isActive);
        var sceneToActivateIndex = sceneToDeactivateIndex >= this.scenes.Count - 1
            ? 0
            : sceneToDeactivateIndex + 1;

        this.scenes[sceneToDeactivateIndex] = (this.scenes[sceneToDeactivateIndex].scene, false);
        this.scenes[sceneToDeactivateIndex].scene?.UnloadContent();

        this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        this.scenes[sceneToActivateIndex].scene?.LoadContent();
    }

    /// <inheritdoc/>
    public void PreviousScene()
    {
        if (this.scenes.Count <= 1)
        {
            return;
        }

        var sceneToDeactivateIndex = this.scenes.IndexOf(s => s.isActive);
        var sceneToActivateIndex = sceneToDeactivateIndex <= 0
            ? this.scenes.Count - 1
            : sceneToDeactivateIndex - 1;

        this.scenes[sceneToDeactivateIndex] = (this.scenes[sceneToDeactivateIndex].scene, false);
        this.scenes[sceneToDeactivateIndex].scene?.UnloadContent();

        this.scenes[sceneToActivateIndex] = (this.scenes[sceneToActivateIndex].scene, true);
        this.scenes[sceneToActivateIndex].scene?.LoadContent();
    }

    /// <inheritdoc/>
    public void SetSceneAsActive(Guid id)
    {
        var sceneToSetIndex = this.scenes.IndexOf(s => s.scene?.Id == id);

        if (sceneToSetIndex == -1)
        {
            return;
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

        if (this.isLoaded)
        {
            return;
        }

        CurrentScene?.LoadContent();

        this.isLoaded = true;
    }

    /// <inheritdoc/>
    public void UnloadContent()
    {
        if (!this.isLoaded || this.isDisposed)
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
    /// Returns a value indicating whether or not a scene with the given ID already exists.
    /// </summary>
    /// <param name="id">The ID of the scene to check for.</param>
    /// <returns>True if the scene exists.</returns>
    public bool SceneExists(Guid id) => this.scenes.Any(s => s.scene?.Id == id);

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
    /// Unloads the content from all of the scenes.
    /// </summary>
    private void UnloadAllSceneContent()
    {
        foreach ((IScene? scene, _) in this.scenes)
        {
            scene?.UnloadContent();
        }
    }
}
