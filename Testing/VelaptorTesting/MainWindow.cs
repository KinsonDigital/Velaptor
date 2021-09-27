using System.Drawing;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.UI;
using VelaptorTesting.Core;
using VelaptorTesting.Scenes;

namespace VelaptorTesting
{
    public class MainWindow : Window
    {
        private SceneManager sceneManager;
        private readonly ISpriteBatch spriteBatch;

        public MainWindow(IWindow window)
            : base(window)
        {
            WindowWidth = Width;
            WindowHeight = Height;
            this.spriteBatch = SpriteBatchFactory.CreateSpriteBatch(Width, Height);
            this.spriteBatch.ClearColor = Color.FromArgb(255, 42, 42, 46);
            this.sceneManager = new SceneManager(this.spriteBatch);

            var testMouseScene = new TestMouseScene();
            var testKeyboardScene = new TestKeyboardScene();
            var renderGraphicsScene = new RenderGraphics();
            
            this.sceneManager.AddScene(testMouseScene);
            this.sceneManager.AddScene(testKeyboardScene);
            this.sceneManager.AddScene(renderGraphicsScene);
            
            // TODO: Update the static window width and height when the size of the window changes
        }

        public static int WindowWidth { get; private set; }
        
        public static int WindowHeight { get; private set; }
        
        public override void OnLoad()
        {
            this.sceneManager.Load();
            base.OnLoad();
        }

        public override void OnUpdate(FrameTime frameTime)
        {
            this.sceneManager.Update(frameTime);

            Title = $"Scene: {this.sceneManager.CurrentScene?.Name ?? "No Scene Loaded"}";
            
            base.OnUpdate(frameTime);
        }

        public override void OnDraw(FrameTime frameTime)
        {
            this.sceneManager.Render();
            base.OnDraw(frameTime);
        }

        public override void OnResize()
        {
            MainWindow.WindowWidth = this.Width;
            MainWindow.WindowHeight = this.Height;
            
            base.OnResize();
        }

        // TODO: Call the unload method of the scene manager
        public override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
