using OpenToolkit.Audio.OpenAL;
using Raptor;
using Raptor.Audio;
using Raptor.Content;
using Raptor.Factories;
using Raptor.Graphics;
using Raptor.Input;
using System;
using System.Linq;

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
        private ISound zapSound;
        private ISound deadShipsMusic;
        private ISound quietPlaceMusic;

        public MyWindow(IWindow window, IContentLoader? contentLoader) : base(window, contentLoader)
        {
        }

        public override void OnLoad()
        {
            if (ContentLoader is null)
                throw new NullReferenceException($"The ContentLoader must not be null.");

            this.spriteBatch = SpriteBatchFactory.CreateSpriteBatch(Width, Height);

            this.dungeonTexture = ContentLoader.LoadTexture("dungeon.png");
            this.linkTexture = ContentLoader.LoadTexture("Link.png");
            //this.zapSound = ContentLoader.LoadSound("zap.ogg");
            this.deadShipsMusic = ContentLoader.LoadSound("deadships.ogg");
            this.deadShipsMusic.SetTimePosition(60);

            this.quietPlaceMusic = ContentLoader.LoadSound("thequietplace.ogg");
            this.quietPlaceMusic.SetTimePosition(50);
            this.quietPlaceMusic.Play();

            base.OnLoad();
        }


        public override void OnUpdate(FrameTime frameTime)
        {
            this.currentKeyboardState = Keyboard.GetState();
            this.currentMouseState = Mouse.GetMouseState();

            if (currentKeyboardState.IsKeyUp(KeyCode.Space) && previousKeyboardState.IsKeyDown(KeyCode.Space))
            {
                //var headPhones = audioManager.DeviceNames.Where(n => n.Contains("WH-1000XM3 Hands-Free AG Audio")).ToArray().FirstOrDefault();
                //this.audioManager.ChangeDevice(headPhones);
                this.quietPlaceMusic.Dispose();
            }

            if (currentKeyboardState.IsKeyUp(KeyCode.P) && previousKeyboardState.IsKeyDown(KeyCode.P))
            {
                this.quietPlaceMusic.Play();
            }


            this.previousKeyboardState =  this.currentKeyboardState;
            this.previousMouseState = this.currentMouseState;

            base.OnUpdate(frameTime);
        }


        public override void OnDraw(FrameTime frameTime)
        {
            this.spriteBatch?.BeginBatch();

            this.spriteBatch?.Render(this.dungeonTexture, 0, 0);
            this.spriteBatch?.Render(this.linkTexture, 400, 400);

            this.spriteBatch?.EndBatch();

            base.OnDraw(frameTime);
        }


        public override void OnResize()
        {
            base.OnResize();
        }
    }
}
