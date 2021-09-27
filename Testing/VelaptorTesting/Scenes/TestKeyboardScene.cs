using System.Drawing;
using System.Text;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.Input;
using Velaptor.UI;
using VelaptorTesting.Core;

namespace VelaptorTesting.Scenes
{
    public class TestKeyboardScene: SceneBase
    {
        private readonly Keyboard keyboard;
        private readonly Label instructions;
        private readonly Label downKeys;
        private IFont font;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private const int LeftMargin = 5;
        private const int TopMargin = 40;

        public TestKeyboardScene() : base("TestKeyboardScene")
        {
            this.keyboard = new Keyboard();
            
            this.instructions = new Label()
            {
                Name = "Instructions",
                Color = Color.White
            };
            
            this.downKeys = new Label()
            {
                Name = "DownKeys",
                Color = Color.White,
            };
            
            this.instructions.Text = "Hit a key on the keyboard to see if it is correct.";
        }

        public override void Load()
        {
            this.font = FontLoader.Load("TimesNewRoman");
            this.instructions.LoadContent();
            this.downKeys.LoadContent();
            
            this.instructions.Position = new Point(LeftMargin, TopMargin);
            
            base.Load();
        }

        public override void Update(FrameTime frameTime)
        {
            this.currentKeyboardState = this.keyboard.GetState();

            if (this.currentKeyboardState.GetDownKeys().Length > 0)
            {
                var downKeyText = new StringBuilder();

                foreach (var key in this.currentKeyboardState.GetDownKeys())
                {
                    downKeyText.Append(key);
                    downKeyText.Append(", ");
                }

                this.downKeys.Text = downKeyText.ToString().TrimEnd(' ').TrimEnd(',');
            }
            else
            {
                this.downKeys.Text = "No Keys Pressed";
            }

            var posX = (int)((MainWindow.WindowWidth / 2f) - (this.downKeys.Width / 2f));
            var posY = (int)((MainWindow.WindowHeight / 2f) - (this.downKeys.Height / 2f));
            
            this.downKeys.Position = new Point(posX, posY); // KEEP
            
            this.instructions.Update(frameTime);
            this.downKeys.Update(frameTime);

            this.previousKeyboardState = this.currentKeyboardState;
            base.Update(frameTime);
        }

        public override void Render(ISpriteBatch spriteBatch)
        {
            this.instructions.Render(spriteBatch);
            this.downKeys.Render(spriteBatch);
            
            base.Render(spriteBatch);
        }
    }
}
