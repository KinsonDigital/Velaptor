using Raptor;
using Raptor.Content;
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

            base.OnLoad();
        }


        public override void OnUpdate(FrameTime frameTime)
        {
            this.currentKeyboardState = Keyboard.GetState();
            this.currentMouseState = Mouse.GetMouseState();

            if (currentKeyboardState.IsKeyUp(KeyCode.Space) && previousKeyboardState.IsKeyDown(KeyCode.Space))
            {
            }
            
            if (currentMouseState.IsLeftButtonUp() && previousMouseState.IsLeftButtonDown())
            {
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
