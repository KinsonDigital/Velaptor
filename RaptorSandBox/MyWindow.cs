using Raptor;
using Raptor.Audio;
using Raptor.Content;
using Raptor.Desktop;
using Raptor.Factories;
using Raptor.Graphics;
using Raptor.Input;
using System;

namespace RaptorSandBox
{
    public class MyWindow : Window
    {
        private ITexture? linkTexture;
        private ITexture? dungeonTexture;
        private readonly AtlasRegionRectangle[] atlasData;
        private ISpriteBatch? spriteBatch;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private ISound? zapSound;
        private ISound? deadShipsMusic;
        private ISound? quietPlaceMusic;
        private float timeElapsed;
        private int linkTexturePosX;

        public MyWindow(IWindow window)
            : base(window)
        {
            this.linkTexturePosX = 400;
        }

        public override void OnLoad()
        {
            if (ContentLoader is null)
                throw new NullReferenceException($"The ContentLoader must not be null.");

            this.spriteBatch = SpriteBatchFactory.CreateSpriteBatch(Width, Height);

            this.dungeonTexture = ContentLoader.Load<ITexture>("dungeon.png");
            this.linkTexture = ContentLoader.Load<ITexture>("Link.png");
            //this.zapSound = ContentLoader.LoadSound("zap.ogg");
            //this.deadShipsMusic = ContentLoader.LoadSound("deadships.ogg");
            //this.deadShipsMusic.SetTimePosition(60);

            this.quietPlaceMusic = ContentLoader.Load<ISound>("deadships.ogg");
            this.quietPlaceMusic.SetTimePosition(50);

            base.OnLoad();
        }

        public override void OnUpdate(FrameTime frameTime)
        {
            this.currentKeyboardState = Keyboard.GetState();
            this.currentMouseState = Mouse.GetMouseState();

            if (this.currentKeyboardState.IsKeyDown(KeyCode.Right))
            {
                this.linkTexturePosX += 1;
            }

            if (this.currentKeyboardState.IsKeyUp(KeyCode.Space) && this.previousKeyboardState.IsKeyDown(KeyCode.Space))
            {
                this.WindowState = StateOfWindow.Minimized;
                //var headPhones = AudioDevice.DeviceNames.Where(n => n.Contains("WH-1000XM3 Hands-Free AG Audio")).ToArray().FirstOrDefault();
                //AudioDevice.ChangeDevice(headPhones);
            }

            if (this.currentKeyboardState.IsKeyUp(KeyCode.P) && this.previousKeyboardState.IsKeyDown(KeyCode.P))
            {
                this.quietPlaceMusic.PlaySound();
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


            this.previousKeyboardState =  this.currentKeyboardState;
            this.previousMouseState = this.currentMouseState;

            base.OnUpdate(frameTime);
        }

        public override void OnDraw(FrameTime frameTime)
        {
            if (this.dungeonTexture is null || this.linkTexture is null)
            {
                return;
            }

            this.spriteBatch?.BeginBatch();

            this.spriteBatch?.Render(this.dungeonTexture, 0, 0);
            this.spriteBatch?.Render(this.linkTexture, this.linkTexturePosX, 400);

            this.spriteBatch?.EndBatch();

            base.OnDraw(frameTime);
        }


        public override void OnResize() => base.OnResize();
    }
}
