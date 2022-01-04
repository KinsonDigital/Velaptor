using System;
using System.Collections.Concurrent;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Services;

namespace Velaptor.Content
{
    internal class TextureCache : IDisposableItemCache<(string name, string filePath), ITexture>
    {
        private readonly ConcurrentDictionary<(string name, string filePath), ITexture> textures = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IImageService imageService;
        private bool isDisposed;

        public TextureCache(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IImageService imageService)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.imageService = imageService;
        }

        ~TextureCache() => Dispose(false);

        public ITexture GetItem((string name, string filePath) pathAndSize)
            => this.textures.GetOrAdd(pathAndSize, nameAndPathValue =>
            {
                var (name, textureFilePath) = nameAndPathValue;

                var imageData = this.imageService.Load(textureFilePath);

                return new Texture(this.gl, this.glExtensions, name, textureFilePath, imageData) { IsPooled = true };
            });

        public void Unload((string name, string filePath) key)
        {
            this.textures.TryRemove(key, out var texture);

            if (texture is null)
            {
                return;
            }

            texture.IsPooled = false;
            texture.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var texture in this.textures.Values)
                {
                    texture.IsPooled = false;
                    texture.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}
