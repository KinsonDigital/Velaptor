// <copyright file="TestSoundsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Drawing;
using Velaptor;
using Velaptor.Content;
using Velaptor.UI;

namespace VelaptorTesting.Scenes
{
    using VelaptorTesting.Core;

    public class TestSoundsScene : SceneBase
    {
        private Label? description;

        public TestSoundsScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
        }

        public override void LoadContent()
        {
            this.description = new Label
            {
                Text = "Use the buttons below to manipulate the sound.",
                Position = new Point((int)(MainWindow.WindowWidth / 2), 50),
                Color = Color.White,
            };

            AddControl(this.description);
            base.LoadContent();
        }

        public override void Update(FrameTime frameTime)
        {
            base.Update(frameTime);
        }
    }
}
