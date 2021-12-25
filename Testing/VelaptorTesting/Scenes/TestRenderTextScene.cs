// <copyright file="TestRenderTextScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using VelaptorTesting.Core;

    /// <summary>
    /// Used to test out if text is properly being rendered to the screen.
    /// </summary>
    public class TestRenderTextScene : SceneBase
    {
        private const string TextToRender = "If can you see this text, then text rendering is working correctly.";
        private IFont? font;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRenderTextScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestRenderTextScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
        }

        /// <inheritdoc cref="IScene.LoadContent"/>
        public override void LoadContent()
        {
            ThrowExceptionIfLoadingWhenDisposed();

            if (IsLoaded)
            {
                return;
            }

            this.font = ContentLoader.Load<IFont>("TimesNewRoman");

            base.LoadContent();
        }

        /// <inheritdoc cref="IScene.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            UnloadSceneContent();

            base.UnloadContent();
        }

        /// <inheritdoc cref="IDrawable.Render"/>
        public override void Render(ISpriteBatch spriteBatch)
        {
            var xPos = (int)(MainWindow.WindowWidth / 2f);
            var yPos = (int)MainWindow.WindowHeight / 2;

            spriteBatch.Render(this.font, TextToRender, xPos, yPos, 1f, 0f);

            base.Render(spriteBatch);
        }

        /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !IsLoaded)
            {
                return;
            }

            if (disposing)
            {
                UnloadSceneContent();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Unloads the scenes content.
        /// </summary>
        private void UnloadSceneContent() => this.font = null;
    }
}
