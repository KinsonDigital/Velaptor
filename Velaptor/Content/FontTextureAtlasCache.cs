using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Services;

namespace Velaptor.Content
{
    internal class FontTextureAtlasCache : IDisposableItemCache<(string filePath, uint size), ITexture>
    {
        private readonly ConcurrentDictionary<(string filePath, uint size), ITexture> textures = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IItemCache<(string, uint), (ImageData, GlyphMetrics[])> fontAtlasDataCache;
        private readonly IImageService imageService;
        private readonly IPath path;
        private bool isDisposed;

        public FontTextureAtlasCache(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IImageService imageService,
            IItemCache<(string, uint), (ImageData, GlyphMetrics[])> fontAtlasDataCache,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.imageService = imageService;
            this.fontAtlasDataCache = fontAtlasDataCache;
            this.path = path;
        }

        ~FontTextureAtlasCache() => Dispose(false);

        public ITexture GetItem((string filePath, uint size) pathAndSize) =>
            this.textures.GetOrAdd(pathAndSize, nameAndPathValue =>
            {
                var (fontFilePath, _) = nameAndPathValue;

                var (imageData, _) = this.fontAtlasDataCache.GetItem(pathAndSize);

                var name = this.path.GetFileNameWithoutExtension(fontFilePath);

                return new Texture(this.gl, this.glExtensions, name, fontFilePath, imageData) { IsPooled = true };
            });

        public void Unload((string filePath, uint size) key)
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
