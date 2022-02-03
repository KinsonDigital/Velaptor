// <copyright file="SceneManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Graphics;
    using Velaptor.UI;

    /// <summary>
    /// Manages scenes by loading and unloading content, updating, and rendering scenes.
    /// </summary>
    public sealed class SceneManager : IUpdatable, IDisposable
    {
        private readonly List<IScene> scenes = new ();
        private readonly Button nextButton;
        private readonly Button previousButton;
        private ISpriteBatch spriteBatch;
        private int currentSceneIndex;
        private bool isDisposed;
        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="spriteBatch">Renders all of the scenes.</param>
        public SceneManager(ISpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            this.nextButton = new Button { Text = "-->", FaceTextureName = "button-next-prev-scene" };
            this.nextButton.Click += (_, _) => NextScene();

            this.previousButton = new Button { Text = "<--", FaceTextureName = "button-next-prev-scene" };
            this.previousButton.Click += (_, _) => PreviousScene();
        }

        /// <summary>
        /// Gets the current scene.
        /// </summary>
        public IScene? CurrentScene => this.scenes.Count <= 0 ? null : this.scenes[this.currentSceneIndex];

        /// <summary>
        /// Adds the given scene.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        /// <param name="setToActive">
        ///     When set to <c>true</c>, the scene being added will be set to active and
        ///     the all other scenes will bet set to inactive.
        /// </param>
        /// <exception cref="Exception">
        ///     Thrown if a scene with with the given <paramref name="scene"/>'s ID already exists.
        /// </exception>
        public void AddScene(IScene scene, bool setToActive = false)
        {
            if (SceneExists(scene.Id))
            {
                throw new Exception($"The sceneBase '{scene.Name}' already exists.");
            }

            // If the scene is to be set to active, set all of the other scenes to false first.
            // Only one scene can be active at a time.
            if (setToActive)
            {
                foreach (var currentScene in this.scenes)
                {
                    currentScene.IsActive = false;
                }
            }

            scene.IsActive = setToActive;
            this.scenes.Add(scene);
        }

        /// <summary>
        /// Removes the scene that matches the given <see cref="sceneId"/>.
        /// </summary>
        /// <param name="sceneId">The ID of the scene to remove.</param>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used for library users.")]
        public void RemoveScene(Guid sceneId)
        {
            if (SceneExists(sceneId) is false)
            {
                return;
            }

            this.scenes.Remove(this.scenes.FirstOrDefault(s => s.Id == sceneId));
        }

        /// <summary>
        /// Removes a scene that matches the given scene ID.
        /// </summary>
        /// <param name="scene">The scene to remove.</param>
        public void RemoveScene(IScene scene) => RemoveScene(scene.Id);

        /// <summary>
        /// Moves to the next scene.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used for library users.")]
        public void NextScene()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }

            var previousScene = this.currentSceneIndex;
            this.currentSceneIndex = this.currentSceneIndex >= this.scenes.Count - 1
                ? 0 : this.currentSceneIndex + 1;

            this.scenes[previousScene].IsActive = false;
            this.scenes[previousScene].UnloadContent();

            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].LoadContent();
        }

        /// <summary>
        /// Moves to the previous scene.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Need for future use.")]
        public void PreviousScene()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }

            var previousScene = this.currentSceneIndex;
            this.currentSceneIndex = this.currentSceneIndex <= 0
                ? this.scenes.Count - 1 : this.currentSceneIndex - 1;

            this.scenes[previousScene].IsActive = false;
            this.scenes[previousScene].UnloadContent();

            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].LoadContent();
        }

        /// <summary>
        /// Loads the content for the manager and the current scene.
        /// </summary>
        public void LoadContent()
        {
            if (this.isDisposed)
            {
                throw new Exception("Cannot load a scene manager that has been disposed.");
            }

            if (this.isLoaded)
            {
                return;
            }

            this.scenes[this.currentSceneIndex].LoadContent();
            this.nextButton.LoadContent();
            this.previousButton.LoadContent();

            const int buttonSpacing = 15;
            const int rightMargin = 15;

            var buttonTops = (int)(MainWindow.WindowHeight - (new[] { this.nextButton.Height, this.previousButton.Height }.Max() + 20));
            var buttonGroupLeft = (int)(MainWindow.WindowWidth - (this.nextButton.Width + this.previousButton.Width + buttonSpacing + rightMargin));
            this.previousButton.Position = new Point(buttonGroupLeft, buttonTops);
            this.nextButton.Position = new Point(this.previousButton.Position.X + (int)this.previousButton.Width + buttonSpacing, buttonTops);

            this.isLoaded = true;
        }

        /// <summary>
        /// Unloads the scene manager content and added scenes.
        /// </summary>
        public void UnloadContent()
        {
            if (!this.isLoaded || this.isDisposed)
            {
                return;
            }

            DisposeOrUnloadContent();
        }

        /// <summary>
        /// Updates the active scenes.
        /// </summary>
        /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
        public void Update(FrameTime frameTime)
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }

            this.nextButton.Update(frameTime);
            this.previousButton.Update(frameTime);

            this.scenes[this.currentSceneIndex].Update(frameTime);
        }

        /// <summary>
        /// Renders the active scenes.
        /// </summary>
        public void Render()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }

            this.spriteBatch.Clear();
            this.spriteBatch.BeginBatch();

            this.scenes[this.currentSceneIndex].Render(this.spriteBatch);

            // Render the scene manager UI on top of all other textures
            this.nextButton.Render(this.spriteBatch);
            this.previousButton.Render(this.spriteBatch);

            this.spriteBatch.EndBatch();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            DisposeOrUnloadContent();

            this.isDisposed = true;
        }

        /// <summary>
        /// Returns a value indicating whether or not a scene with the given ID already exists.
        /// </summary>
        /// <param name="id">The ID of the scene to check for.</param>
        /// <returns>True if the scene exists.</returns>
        private bool SceneExists(Guid id) => this.scenes.Any(s => s.Id == id);

        /// <summary>
        /// Disposes or unloads all of the scene content.
        /// </summary>
        private void DisposeOrUnloadContent()
        {
            foreach (var scene in this.scenes)
            {
                scene.UnloadContent();
            }

            this.scenes.Clear();

            this.spriteBatch = null;
            this.previousButton.UnloadContent();
            this.nextButton.UnloadContent();
        }
    }
}
