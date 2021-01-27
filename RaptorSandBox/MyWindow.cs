using Newtonsoft.Json;
using Raptor;
using Raptor.Audio;
using Raptor.Content;
using Raptor.Desktop;
using Raptor.Factories;
using Raptor.Graphics;
using Raptor.Input;
using System;
using System.Drawing;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private IAtlasData? subTextureAtlas;
        private readonly AtlasRegionRectangle[] atlasData;
        private ISpriteBatch? spriteBatch;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private ISound? zapSound;
        private ISound? deadShipsMusic;
        private ISound? quietPlaceMusic;
        private IKeyboard keyboard;
        private readonly Mouse mouse;
        private float timeElapsed;
        private int linkTexturePosX;
        private bool isDisposed;
        private int currentFrameIndex;
        private Rectangle currentFrame;

        public MyWindow(IWindow window)
            : base(window)
        {
            this.linkTexturePosX = 400;
            this.keyboard = new Keyboard();
            this.mouse = new Mouse();
        }

        public override void OnLoad()
        {
            if (ContentLoader is null)
            {
                throw new NullReferenceException($"The ContentLoader must not be null.");
            }

            this.spriteBatch = SpriteBatchFactory.CreateSpriteBatch(Width, Height);

            this.subTextureAtlas = ContentLoader.Load<IAtlasData>("Main-Atlas");

            this.quietPlaceMusic = ContentLoader.Load<ISound>("deadships.ogg");
            this.quietPlaceMusic.SetTimePosition(50);


            base.OnLoad();
        }

        private int elapsedFrameTime = 0;

        public override void OnUpdate(FrameTime frameTime)
        {
            ProcessAnimation(frameTime);
            this.currentKeyboardState = this.keyboard.GetState();
            this.currentMouseState = this.mouse.GetMouseState();

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Right))
            {
                this.linkTexturePosX += 1;
            }

            if (this.currentMouseState.IsLeftButtonUp() && this.previousMouseState.IsLeftButtonDown())
            {
                this.quietPlaceMusic.PlaySound();
            }

            if (this.currentMouseState.IsRightButtonUp() && this.previousMouseState.IsRightButtonDown())
            {
                this.quietPlaceMusic.PauseSound();
            }

            if (this.timeElapsed < 1000)
            {
                this.timeElapsed += (float)frameTime.ElapsedTime.TotalMilliseconds;
            }
            else
            {
                Title = $"Time: {this.quietPlaceMusic?.TimePositionSeconds ?? 0}";
                this.timeElapsed = 0;
            }


            this.previousKeyboardState = this.currentKeyboardState;
            this.previousMouseState = this.currentMouseState;

            base.OnUpdate(frameTime);
        }

        private void ProcessAnimation(FrameTime frameTime)
        {
            if (this.elapsedFrameTime >= 62)
            {
                this.elapsedFrameTime = 0;

                this.currentFrameIndex += 1;
                this.currentFrameIndex = this.currentFrameIndex > 5 ? 0 : this.currentFrameIndex;
            }
            else
            {
                this.elapsedFrameTime += frameTime.ElapsedTime.Milliseconds;
            }
        }

        public override void OnDraw(FrameTime frameTime)
        {
            this.spriteBatch?.BeginBatch();

            var subTexture = this.subTextureAtlas[this.currentFrameIndex];

            this.spriteBatch?.Render(this.subTextureAtlas.Texture, subTexture.Bounds, new Rectangle(100, 100, 500, 100), 1, 0, Color.White);

            this.spriteBatch?.EndBatch();

            base.OnDraw(frameTime);
        }

        public override void OnResize() => base.OnResize();

        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.subTextureAtlas?.Dispose();
                    this.spriteBatch?.Dispose();
                    this.quietPlaceMusic?.Dispose();
                }

                this.isDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
