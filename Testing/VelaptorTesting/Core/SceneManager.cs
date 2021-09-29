// <copyright file="SceneManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Core
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.UI;

    // TODO: Setup this class to be IDisposable
    public class SceneManager : IUpdatable, IDisposable
    {
        private readonly ISpriteBatch spriteBatch;
        private readonly List<IScene> scenes = new ();
        private readonly Button nextButton;
        private readonly Button previousButton;
        private int currentSceneIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="contentLoader">The loads all of the content for the scenes.</param>
        /// <param name="spriteBatch">The renders all of the scenes.</param>
        public SceneManager(IContentLoader contentLoader, ISpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            // TODO: Improve this by creating a control factory
            var nextButtonLabel = new Label(contentLoader) { Text = "-->" };
            this.nextButton = new Button(contentLoader, nextButtonLabel);
            this.nextButton.Click += (_, e) => NextScene();

            var previousButtonLabel = new Label(contentLoader) { Text = "<--" };
            this.previousButton = new Button(contentLoader, previousButtonLabel);
            this.previousButton.Click += (_, e) =>
            {
                this.previousButton.Position =
                    new Point(this.previousButton.Position.X + 10, this.previousButton.Position.Y);
                PreviousScene();
            };
        }

        /// <summary>
        /// Gets the current scene.
        /// </summary>
        public IScene? CurrentScene => this.scenes.Count <= 0 ? null : this.scenes[this.currentSceneIndex];

        /// <summary>
        /// Adds the given scene.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        /// <exception cref="Exception">
        ///     Thrown if a scene with with the given <paramref name="scene"/>'s ID already exists.
        /// </exception>
        public void AddScene(IScene scene)
        {
            if (SceneExists(scene.ID))
            {
                throw new Exception($"The sceneBase '{scene.Name}' already exists.");
            }

            this.scenes.Add(scene);
        }

        /// <summary>
        /// Removes a scene that matches the given scene ID.
        /// </summary>
        /// <param name="sceneId">The ID of the scene to remove.</param>
        public void RemoveScene(Guid sceneId)
        {
            if (SceneExists(sceneId) is false)
            {
                return;
            }

            this.scenes.Remove(this.scenes.FirstOrDefault(s => s.ID == sceneId));
        }

        /// <summary>
        /// Removes a scene that matches the given scene ID.
        /// </summary>
        /// <param name="scene">The scene to remove.</param>
        public void RemoveScene(IScene scene) => RemoveScene(scene.ID);

        /// <summary>
        /// Moves to the next scene.
        /// </summary>
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
            this.scenes[previousScene].Unload();

            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].Load();
        }

        /// <summary>
        /// Moves to the previous scene.
        /// </summary>
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
            this.scenes[previousScene].Unload();

            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].Load();
        }

        /// <summary>
        /// Loads the content for the manager and the current scene.
        /// </summary>
        public void Load()
        {
            this.scenes[this.currentSceneIndex].Load();
            this.nextButton.LoadContent();
            this.previousButton.LoadContent();

            const int buttonSpacing = 15;
            const int rightMargin = 15;

            var buttonTops = MainWindow.WindowHeight - (new[] { this.nextButton.Height, this.previousButton.Height }.Max() + 20);
            var buttonGroupLeft = MainWindow.WindowWidth - (this.nextButton.Width + this.previousButton.Width + buttonSpacing + rightMargin);
            this.previousButton.Position = new Point(buttonGroupLeft, buttonTops);
            this.nextButton.Position = new Point(this.previousButton.Position.X + this.previousButton.Width + buttonSpacing, buttonTops);
        }

        /// <summary>
        /// Updates the active scenes.
        /// </summary>
        /// <param name="frameTime">The amount of time passed for the current frame.</param>
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating whether a scene with the given ID already exists.
        /// </summary>
        /// <param name="id">The ID of the scene to check for.</param>
        /// <returns>True if the scene exists.</returns>
        private bool SceneExists(Guid id) => this.scenes.Any(s => s.ID == id);
    }
}
