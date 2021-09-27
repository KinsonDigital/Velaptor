using System;
using Velaptor;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.UI;

namespace VelaptorTesting.Core
{
    // TODO: Setup this class to be IDisposable
    public interface IScene: IUpdatable, IDrawable, IDisposable
    {
        string Name { get; }

        bool IsLoaded { get; }
        
        bool IsActive { get; set; }
        
        void Load();

        void AddControl(IControl control);

        void Unload();
    }
}
