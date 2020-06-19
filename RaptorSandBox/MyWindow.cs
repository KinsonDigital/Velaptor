using FileIO.File;
using Raptor;
using Raptor.Content;
using Raptor.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private Texture _linkTexture;
        private Texture _bgTexture;
        private readonly ContentLoader _contentLoader;
        private RendererREFONLY _renderer;
        private double _timeElapsed;
        private int _textureX = 200;
        private int _speed = 2;


        public MyWindow()
        {
            _contentLoader = new ContentLoader(new ImageFile());
            _renderer = new RendererREFONLY();

            Width = 1020;
            Height = 800;
        }


        public override void OnLoad()
        {
            base.OnLoad();
        }


        public override void OnUpdate(FrameTime frameTime)
        {
            base.OnUpdate(frameTime);
        }


        public override void OnDraw(FrameTime frameTime)
        {
            base.OnDraw(frameTime);
        }


        public override void OnResize()
        {
            base.OnResize();
        }
    }
}
