using FileIO.File;
using Raptor;
using Raptor.Content;
using Raptor.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private ITexture? linkTexture;
        private SpriteBatch? spriteBatch;


        public MyWindow()
        {
            Width = 1020;
            Height = 800;
        }


        public override void OnLoad()
        {
            if (ContentLoader is null)
                throw new NullReferenceException($"The ContentLoader must not be null.");

            this.linkTexture = ContentLoader.LoadTexture("Link.png");
            this.spriteBatch = new SpriteBatch(Width, Height);
            base.OnLoad();
        }


        public override void OnUpdate(FrameTime frameTime)
        {
            base.OnUpdate(frameTime);
        }


        public override void OnDraw(FrameTime frameTime)
        {
            this.spriteBatch?.Begin();

            this.spriteBatch?.Render(this.linkTexture, 400, 400);

            this.spriteBatch?.End();

            base.OnDraw(frameTime);
        }


        public override void OnResize()
        {
            base.OnResize();
        }
    }
}
