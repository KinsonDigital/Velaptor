using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;
using Velaptor.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace VelaptorTesting
{
    public class MainWindow : Window
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
        private IKeyboardInput<KeyCode, KeyboardState> keyboard;
        private readonly IMouseInput<MouseButton, MouseState> mouse;
        private float timeElapsed;
        private int subPositionX;
        private int subPositionY = 200;
        private bool isDisposed;
        private int currentFrameIndex;
        private Rectangle currentFrame;
        private AtlasSubTextureData[] subFrames;
        private AtlasSubTextureData bubbleFrame;
        private List<Rectangle> bubbles = new();
        private ITexture linkTexture;
        private int elapsedFrameTime = 0;
        private IFont myFont;

        public MainWindow(IWindow window)
            : base(window)
        {
            this.subPositionX = 400;
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
            this.quietPlaceMusic.Volume = 10;

            this.myFont = ContentLoader.Load<IFont>("TimesNewRoman");

            base.OnLoad();
        }

        public override void OnUpdate(FrameTime frameTime)
        {
            ProcessAnimation(frameTime);
            this.currentKeyboardState = this.keyboard.GetState();
            this.currentMouseState = this.mouse.GetState();

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Right))
            {
                this.subPositionX += 1;
            }

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Left))
            {
                this.subPositionX -= 1;
            }

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Down))
            {
                this.subPositionY += 1;
            }

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Up))
            {
                this.subPositionY -= 1;
            }

            if (this.currentMouseState.IsLeftButtonUp() && this.previousMouseState.IsLeftButtonDown())
            {
                this.quietPlaceMusic.Play();
            }

            if (this.currentMouseState.IsRightButtonUp() && this.previousMouseState.IsRightButtonDown())
            {
                this.quietPlaceMusic.Pause();
            }

            for (var i = 0; i < this.bubbles.Count; i++)
            {
                var bubble = this.bubbles[i];

                bubble.Y -= 1;
                bubble.Y = bubble.Y < 0 ? this.Height : bubble.Y;

                this.bubbles[i] = bubble;
            }

            if (this.timeElapsed < 1000)
            {
                this.timeElapsed += (float)frameTime.ElapsedTime.TotalMilliseconds;
            }
            else
            {
                Title = $"Time: {this.quietPlaceMusic?.Position.Seconds ?? 0}";
                this.timeElapsed = 0;
            }


            this.previousKeyboardState = this.currentKeyboardState;
            this.previousMouseState = this.currentMouseState;

            base.OnUpdate(frameTime);
        }

        public override void OnDraw(FrameTime frameTime)
        {
            this.spriteBatch?.BeginBatch();

            var subTexture = this.subFrames[this.currentFrameIndex];

            this.spriteBatch?.Render(
                this.mainAtlas.Texture,
                subTexture.Bounds,
                new Rectangle(this.subPositionX, this.subPositionY, 500, 100),
                1,
                0,
                Color.White,
                RenderEffects.None);

            foreach (var bubble in this.bubbles)
            {
                this.spriteBatch?.Render(
                    this.mainAtlas.Texture,
                    this.bubbleFrame.Bounds,
                    bubble,
                    1,
                    0,
                    Color.White,
                    RenderEffects.None);
            }

            //this.spriteBatch?.Render(this.linkTexture, 400, 500);

            var lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
            var upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "1234567890";
            var numSymbols = "~!@#$%^&*()_+ⱠⱣⱦⱧ`-=[]\\;',./{}|:\"<>?";

            this.spriteBatch?.Render(this.myFont, lowerCaseLetters, 50, 200, Color.AliceBlue);
            this.spriteBatch?.Render(this.myFont, upperCaseLetters, 50, 300, Color.Crimson);
            this.spriteBatch?.Render(this.myFont, numbers, 50, 400, Color.MediumPurple);
            this.spriteBatch?.Render(this.myFont, numSymbols, 50, 500, Color.IndianRed);

            // Print mouse location
            this.spriteBatch?.Render(this.myFont, $"X: {this.currentMouseState.GetX()}, Y: {this.currentMouseState.GetY()}", 10, 35);

            this.spriteBatch?.EndBatch();

            base.OnDraw(frameTime);
        }

        public override void OnResize() => base.OnResize();

        public override void OnUnload()
        {
            this.mainAtlas?.Dispose();
            this.spriteBatch?.Dispose();
            this.quietPlaceMusic?.Dispose();

            base.OnUnload();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
            }

            base.Dispose(disposing);
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
    }
}
