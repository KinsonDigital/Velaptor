// <copyright file="TestNonAnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Factories;
    using Velaptor.Graphics;
    using Velaptor.Input;
    using VelaptorTesting.Core;

    /// <summary>
    /// Tests that graphics properly render to the screen.
    /// </summary>
    public class TestNonAnimatedGraphicsScene : SceneBase
    {
        private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
        private readonly IAppInput<KeyboardState> keyboard;
        private readonly int windowHalfWidth;
        private readonly int windowHalfHeight;
        private IAtlasData? mainAtlas;
        private AtlasSubTextureData? octagonData;
        private IFont? font;
        private KeyboardState currentKeyState;
        private KeyboardState prevKeyState;
        private RenderEffects renderEffects = RenderEffects.None;
        private string instructions = string.Empty;
        private SizeF textSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestNonAnimatedGraphicsScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestNonAnimatedGraphicsScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
            this.keyboard = AppInputFactory.CreateKeyboard();
            this.windowHalfWidth = (int)MainWindow.WindowWidth / 2;
            this.windowHalfHeight = (int)MainWindow.WindowHeight / 2;
        }

        /// <inheritdoc cref="IScene.LoadContent"/>
        public override void LoadContent()
        {
            if (IsLoaded)
            {
                return;
            }

            this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);
            var textLines = new List<string>
            {
                    "Use arrow keys to flip the texture horizontally and vertically.",
                    $"{Environment.NewLine}Left: Flip Horizontally",
                    "Right: Flip Horizontally",
                    "Up: Flip Vertically",
                    "Down: Flip Vertically",
            };
            this.instructions = string.Join(Environment.NewLine, textLines);

            this.textSize = this.font.Measure(this.instructions);

            this.mainAtlas = ContentLoader.LoadAtlas("Main-Atlas");
            this.octagonData = this.mainAtlas.GetFrame("octagon-flip");

            base.LoadContent();
        }

        /// <inheritdoc cref="IScene.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            ContentLoader.UnloadAtlas(this.mainAtlas);
            ContentLoader.UnloadFont(this.font);

            this.mainAtlas = null;

            base.UnloadContent();
        }

        /// <inheritdoc cref="IUpdatable.Update"/>
        public override void Update(FrameTime frameTime)
        {
            this.currentKeyState = this.keyboard.GetState();

            if (this.currentKeyState.IsKeyUp(KeyCode.Right) && this.prevKeyState.IsKeyDown(KeyCode.Right))
            {
                this.renderEffects = this.renderEffects switch
                {
                    RenderEffects.FlipHorizontally => RenderEffects.None,
                    RenderEffects.FlipBothDirections => RenderEffects.FlipVertically,
                    _ => this.renderEffects
                };
            }

            if (this.currentKeyState.IsKeyUp(KeyCode.Left) && this.prevKeyState.IsKeyDown(KeyCode.Left))
            {
                this.renderEffects = this.renderEffects switch
                {
                    RenderEffects.None => RenderEffects.FlipHorizontally,
                    RenderEffects.FlipVertically => RenderEffects.FlipBothDirections,
                    _ => this.renderEffects
                };
            }

            if (this.currentKeyState.IsKeyUp(KeyCode.Down) && this.prevKeyState.IsKeyDown(KeyCode.Down))
            {
                this.renderEffects = this.renderEffects switch
                {
                    RenderEffects.None => RenderEffects.FlipVertically,
                    RenderEffects.FlipHorizontally => RenderEffects.FlipBothDirections,
                    _ => this.renderEffects
                };
            }

            if (this.currentKeyState.IsKeyUp(KeyCode.Up) && this.prevKeyState.IsKeyDown(KeyCode.Up))
            {
                this.renderEffects = this.renderEffects switch
                {
                    RenderEffects.FlipVertically => RenderEffects.None,
                    RenderEffects.FlipBothDirections => RenderEffects.FlipHorizontally,
                    _ => this.renderEffects
                };
            }

            this.prevKeyState = this.currentKeyState;
        }

        /// <inheritdoc cref="IDrawable.Render"/>
        public override void Render(IRenderer renderer)
        {
            var posX = this.windowHalfWidth - (this.octagonData.Bounds.Width / 2);
            var posY = this.windowHalfHeight - (this.octagonData.Bounds.Height / 2);

            var instructionsX = (int)(this.textSize.Width / 2) + 25;
            var instructionsY = (int)(this.textSize.Height / 2) + 25;

            renderer.Render(this.font, this.instructions, instructionsX, instructionsY);

            renderer.Render(
                this.mainAtlas.Texture,
                this.octagonData.Bounds,
                new Rectangle(posX, posY, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
                1f,
                0f,
                Color.White,
                this.renderEffects);

            base.Render(renderer);
        }

        /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !IsLoaded)
            {
                return;
            }

            base.Dispose(disposing);
        }
    }
}
