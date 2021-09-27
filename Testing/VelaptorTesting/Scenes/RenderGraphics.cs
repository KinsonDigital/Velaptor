using System.Drawing;
using Silk.NET.Vulkan.Extensions.KHR;
using Velaptor;
using Velaptor.Graphics;
using VelaptorTesting.Core;

namespace VelaptorTesting.Scenes
{
    public class RenderGraphics : SceneBase
    {
        private IAtlasData mainAtlas;
        private AtlasSubTextureData squareAtlasData;

        public RenderGraphics() : base("RenderGraphicsScene")
        {
        }

        public override void Load()
        {
            this.mainAtlas = AtlasDataLoader.Load("Main-Atlas");
            this.squareAtlasData = this.mainAtlas.GetFrame("square");
            
            base.Load();
        }

        public override void Update(FrameTime frameTime)
        {

        }

        public override void Render(ISpriteBatch spriteBatch)
        {
            var sqrPosX = (MainWindow.WindowWidth / 2) - (this.squareAtlasData.Bounds.Width / 2);
            var sqrPosY = (MainWindow.WindowHeight / 2) - (this.squareAtlasData.Bounds.Height / 2);
            
            spriteBatch.Render(
                this.mainAtlas.Texture,
                this.squareAtlasData.Bounds,
                new Rectangle(sqrPosX, sqrPosY, this.mainAtlas.Width, this.mainAtlas.Height),
                1f,
                0f,
                Color.White,
                RenderEffects.None);
            base.Render(spriteBatch);
        }
    }
}
