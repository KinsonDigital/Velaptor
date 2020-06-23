namespace Raptor.Content
{
    using System.IO;
    using FileIO.Core;
    using Raptor.Graphics;

    public class TextureLoader : ILoader<ITexture>
    {
        private IImageFile imageFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageFile">Loads an image file.</param>
        public TextureLoader(IImageFile imageFile) => this.imageFile = imageFile;

        /// <inheritdoc/>
        public ITexture Load(string filePath)
        {
            var (pixels, width, height) = this.imageFile.Load(filePath);

            return new Texture(pixels, Path.GetFileNameWithoutExtension(filePath), width, height);
        }
    }
}
