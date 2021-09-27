using System;

namespace VelaptorTesting.Core
{
    using System.Collections.Generic;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Velaptor.Graphics;
    using Velaptor.UI;
    
    // TODO: Setup this class to be IDisposable
    public abstract class SceneBase : IScene
    {
        private readonly List<IControl> controls = new ();
        
        public SceneBase(string name)
        {
            Name = name;
            FontLoader = ContentLoaderFactory.CreateFontLoader();
            AtlasDataLoader = ContentLoaderFactory.CreateTextureAtlasLoader();
            TextureLoader = ContentLoaderFactory.CreateTextureLoader();
            IsActive = false;
        }

        protected ILoader<IFont> FontLoader { get; set; }

        protected ILoader<IAtlasData> AtlasDataLoader { get; set; }

        protected ILoader<ITexture> TextureLoader { get; set; }
        
        public virtual void Load()
        {
            if (IsLoaded)
            {
                return;
            }
            
            IsLoaded = true;
        }

        public void AddControl(IControl control)
        {
            controls.Add(control);
        }

        public virtual void Unload()
        {
            IsLoaded = false;
        }

        public virtual void Update(FrameTime frameTime)
        {
            if (IsLoaded is false)
            {
                return;
            }
            
            foreach (var control in this.controls)
            {
                control.Update(frameTime);
            }
        }

        public virtual void Render(ISpriteBatch spriteBatch)
        {
            if (IsLoaded is false)
            {
                return;
            }
            
            foreach (var control in this.controls)
            {
                control.Render(spriteBatch);
            }
        }

        public string Name { get; protected set; } = string.Empty;
        
        public bool IsLoaded { get; private set; }

        public bool IsActive { get; set; }

        public virtual void Dispose()
        {
            FontLoader = null;
            AtlasDataLoader = null;
            TextureLoader = null;

            foreach (var control in this.controls)
            {
                control.Dispose();
            }
            
            this.controls.Clear();
        }
    }
}
