// <copyright file="TestRenderGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System.Drawing;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using VelaptorTesting.Core;

    /// <summary>
    /// Tests that graphics properly render to the screen.
    /// </summary>
    public class TestRenderGraphicsScene : SceneBase
    {
        private IAtlasData? mainAtlas;
        private AtlasSubTextureData[]? frames;
        private int elapsedTime;
        private int currentFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRenderGraphicsScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestRenderGraphicsScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
        }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        public override void Load()
        {
            this.mainAtlas = ContentLoader.Load<IAtlasData>("Main-Atlas");
            this.frames = this.mainAtlas.GetFrames("square");

            base.Load();
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="frameTime">The amount of time passed for the current frame.</param>
        public override void Update(FrameTime frameTime)
        {
            if (this.elapsedTime >= 32)
            {
                this.elapsedTime = 0;

                this.currentFrame = this.currentFrame >= this.frames.Length - 1
                    ? 0
                    : this.currentFrame + 1;
            }
            else
            {
                this.elapsedTime += frameTime.ElapsedTime.Milliseconds;
            }
        }

        /// <summary>
        /// Renders graphics to the scene.
        /// </summary>
        /// <param name="spriteBatch">Renders graphics to the screen.</param>
        public override void Render(ISpriteBatch spriteBatch)
        {
            var sqrPosX = (MainWindow.WindowWidth / 2) - (this.frames[this.currentFrame].Bounds.Width / 2);
            var sqrPosY = (MainWindow.WindowHeight / 2) - (this.frames[this.currentFrame].Bounds.Height / 2);

            spriteBatch.Render(
                this.mainAtlas.Texture,
                this.frames[this.currentFrame].Bounds,
                new Rectangle(sqrPosX, sqrPosY, this.mainAtlas.Width, this.mainAtlas.Height),
                1f,
                0f,
                Color.White,
                RenderEffects.None);
            base.Render(spriteBatch);
        }
    }
}
