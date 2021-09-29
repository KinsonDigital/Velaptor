// <copyright file="TestKeyboardScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System.Drawing;
    using System.Text;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.Input;
    using Velaptor.UI;
    using Core;

    /// <summary>
    /// Used to test out if the keyboard works correctly.
    /// </summary>
    public class TestKeyboardScene : SceneBase
    {
        private const int TopMargin = 40;
        private const int LeftMargin = 5;
        private readonly Keyboard keyboard;
        private readonly Label instructions;
        private readonly Label downKeys;
        private KeyboardState currentKeyboardState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestKeyboardScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestKeyboardScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
            this.keyboard = new Keyboard();

            this.instructions = new Label(ContentLoader)
            {
                Name = "Instructions",
                Color = Color.White,
            };

            this.downKeys = new Label(ContentLoader)
            {
                Name = "DownKeys",
                Color = Color.White,
            };

            this.instructions.Text = "Hit a key on the keyboard to see if it is correct.";
        }

        public override void Load()
        {
            this.instructions.LoadContent();
            this.downKeys.LoadContent();

            this.instructions.Position = new Point(LeftMargin, TopMargin);

            base.Load();
        }

        public override void Update(FrameTime frameTime)
        {
            this.currentKeyboardState = this.keyboard.GetState();

            if (this.currentKeyboardState.GetDownKeys().Length > 0)
            {
                var downKeyText = new StringBuilder();

                foreach (var key in this.currentKeyboardState.GetDownKeys())
                {
                    downKeyText.Append(key);
                    downKeyText.Append(", ");
                }

                this.downKeys.Text = downKeyText.ToString().TrimEnd(' ').TrimEnd(',');
            }
            else
            {
                this.downKeys.Text = "No Keys Pressed";
            }

            var posX = (int)((MainWindow.WindowWidth / 2f) - (this.downKeys.Width / 2f));
            var posY = (int)((MainWindow.WindowHeight / 2f) - (this.downKeys.Height / 2f));

            this.downKeys.Position = new Point(posX, posY); // KEEP

            this.instructions.Update(frameTime);
            this.downKeys.Update(frameTime);

            base.Update(frameTime);
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="spriteBatch">Renders graphics to the screen.</param>
        public override void Render(ISpriteBatch spriteBatch)
        {
            this.instructions.Render(spriteBatch);
            this.downKeys.Render(spriteBatch);

            base.Render(spriteBatch);
        }
    }
}
