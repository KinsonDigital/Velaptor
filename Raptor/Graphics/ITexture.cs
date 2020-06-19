using System;

namespace Raptor.Graphics
{
    public interface ITexture : IDisposable
    {
        int ID { get; }

        string Name { get; }

        int Width { get; }

        int Height { get; }
    }
}