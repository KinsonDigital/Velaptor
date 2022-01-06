using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;
using Velaptor.Content.Exceptions;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Services;

namespace Velaptor.Content
{
    internal class TextureCache : IDisposableItemCache<string, ITexture>
    {
        private readonly ConcurrentDictionary<string, ITexture> textures = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IImageService imageService;
        private readonly IPath path;
        private bool isDisposed;
        private readonly ITextureFactory textureFactory;

        public TextureCache(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IImageService imageService,
            ITextureFactory textureFactory,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.imageService = imageService;
            this.textureFactory = textureFactory;
            this.path = path;
        }

        ~TextureCache() => Dispose(false);

        public ITexture GetItem(string filePath)
        {
            if (filePath.IsValidFilePath() is false)
            {
                throw new LoadTextureException($"The texture file path '{filePath}' is not a valid path.");
            }

            return this.textures.GetOrAdd(filePath, textureFilePath =>
            {
                var imageData = this.imageService.Load(textureFilePath);

                var name = this.path.GetFileNameWithoutExtension(textureFilePath);

                return this.textureFactory.Create(name, textureFilePath, imageData, true);
            });
        }

        public void Unload(string filePath)
        {
            this.textures.TryRemove(filePath, out var texture);

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
