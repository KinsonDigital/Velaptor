using FileIO.File;
using Raptor;
using Raptor.Content;
using Raptor.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private Texture _linkTexture;
        private Texture _bgTexture;
        private readonly ContentLoader _contentLoader;
        private Renderer _renderer;
        private double _timeElapsed;
        private int _textureX = 200;
        private int _speed = 2;


        public MyWindow()
        {
            _contentLoader = new ContentLoader(new ImageFile());
            _renderer = new Renderer();

            Width = 1020;
            Height = 800;
        }


        public override void OnLoad()
        {
            _bgTexture = _contentLoader.LoadTexture("Dungeon.png");
            _linkTexture = _contentLoader.LoadTexture("Link.png");


            _linkTexture.Layer = 1;
            
            base.OnLoad();
        }


        public override void OnUpdate(FrameTime frameTime)
        {
            _timeElapsed += frameTime.ElapsedTime.TotalMilliseconds;

            if (_timeElapsed >= 4000)
            {
                _timeElapsed = 0;
                _speed *= -1;
            }


            _textureX += _speed;


            base.OnUpdate(frameTime);
        }


        List<double> _perfTimes = new List<double>();

        public override void OnDraw(FrameTime frameTime)
        {
            var timer = new Stopwatch();

            timer.Start();
            //------TEST 1------
            _renderer.Begin();
            _renderer.Render(_linkTexture, _textureX, 300, 0, 1f, new GameColor(255, 255, 255, 255));
            _renderer.Render(_bgTexture, Width / 2, Height / 2, 0, 1, new GameColor(255, 255, 255, 255));
            _renderer.End();
            /*Results:
                0.0416
                0.0338
            */
            //------------------



            //------TEST 2------
            //_renderer.Render_OLD(_bgTexture, Width / 2, Height / 2, 0, 1, new GameColor(255, 255, 255, 255));
            //_renderer.Render_OLD(_linkTexture, _textureX, 300, 0, 1f, new GameColor(255, 255, 255, 255));
            //Result: 0.0291
            //------------------
            timer.Stop();

            _perfTimes.Add(timer.Elapsed.TotalMilliseconds);


            if (_perfTimes.Count >= 500)
            {
                var perfResult = _perfTimes.Average();

                Debugger.Break();
            }


            base.OnDraw(frameTime);
        }


        public override void OnResize()
        {
            _renderer.ViewportWidth = Width;
            _renderer.ViewportHeight = Height;
            base.OnResize();
        }
    }
}
