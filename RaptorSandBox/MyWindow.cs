using Raptor;
using Raptor.Audio;
using Raptor.Factories;
using Raptor.Graphics;
using Raptor.Input;
using Raptor.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private IAtlasData? mainAtlas;
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
        private AtlasSubTextureData[] subFrames;
        private AtlasSubTextureData bubbleFrame;
        private List<Rectangle> bubbles = new List<Rectangle>();
        private ITexture linkTexture;

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

            this.mainAtlas = ContentLoader.Load<IAtlasData>("Main-Atlas");

            this.subFrames = this.mainAtlas.GetFrames("sub");

            this.bubbleFrame = this.mainAtlas.GetFrame("bubble");

            var random = new Random();

            for (var i = 0; i < 10; i++)
            {
                this.bubbles.Add(new Rectangle()
                {
                    X = random.Next(0, Width - this.bubbleFrame.Bounds.Width),
                    Y = Height,
                    Width = 500,
                    Height = 100
                });
            }

            this.linkTexture = ContentLoader.Load<ITexture>("Link");

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

            for (var i = 0; i < this.bubbles.Count; i++)
            {
                var bubble = this.bubbles[i];

                bubble.Y -= 1;

                this.bubbles[i] = bubble;
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
                this.currentFrameIndex = this.currentFrameIndex >= this.subFrames.Length - 1 ? 0 : this.currentFrameIndex + 1;
            }
            else
            {
                this.elapsedFrameTime += frameTime.ElapsedTime.Milliseconds;
            }
        }

        public override void OnDraw(FrameTime frameTime)
        {
            this.spriteBatch?.BeginBatch();

            var subTexture = this.subFrames[this.currentFrameIndex];

            this.spriteBatch?.Render(this.mainAtlas.Texture, subTexture.Bounds, new Rectangle(100, 100, 500, 100), 1, 0, Color.White);

            foreach (var bubble in this.bubbles)
            {
                this.spriteBatch?.Render(this.mainAtlas.Texture, this.bubbleFrame.Bounds, bubble, 1, 0, Color.White);
            }

            //this.spriteBatch?.Render(this.linkTexture, 400, 500);

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
                    this.mainAtlas?.Dispose();
                    this.spriteBatch?.Dispose();
                    this.quietPlaceMusic?.Dispose();
                }

                this.isDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
