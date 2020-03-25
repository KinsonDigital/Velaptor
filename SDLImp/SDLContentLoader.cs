using Raptor.Graphics;
using Raptor.Plugins;
using SDLCore;
using System;
using System.IO;

namespace SDLImp
{
    /// <summary>
    /// Loads and unloads content using SDL.
    /// </summary>
    public class SDLContentLoader : IContentLoader
    {
        #region Private Fields
        private SDL _sdl;
        private SDLImage _sdlImage;
        private SDLFonts _sdlFonts;
        #endregion


        #region Constructors
        public SDLContentLoader()
        {
            //TODO: Add code here to load the SDL libraries using a library loader
            ContentRootDirectory = $@"{GamePath}\Content\";
        }
        #endregion


        #region Props
        public string GamePath { get; } = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        public string ContentRootDirectory { get; set; }
        #endregion


        #region Public Methods
        T IContentLoader.LoadTexture<T>(string name)
        {
            var texturePath = $@"{ContentRootDirectory}\Graphics\{name}.png";


            //Load image at specified path
            var loadedSurface = _sdlImage.Load(texturePath);

            //The final optimized image
            IntPtr newTexturePtr;
            if (loadedSurface == IntPtr.Zero)
            {
                throw new Exception($"Unable to load image {texturePath}! \n\nSDL Error: {_sdl.GetError()}");
            }
            else
            {
                //Create texture from surface pixels
                newTexturePtr = _sdl.CreateTextureFromSurface(SDLEngineCore.RendererPointer, loadedSurface);

                if (newTexturePtr == IntPtr.Zero)
                    throw new Exception($"Unable to create texture from {texturePath}! \n\nSDL Error: {_sdl.GetError()}");

                //Get rid of old loaded surface
                _sdl.FreeSurface(loadedSurface);
            }

            ITexture newTexture = new SDLTexture(newTexturePtr);


            return newTexture as T;
        }


        public T GetData<T>(int option) where T : class => throw new NotImplementedException();


        public void InjectData<T>(T data) where T : class => throw new NotImplementedException();


        T IContentLoader.LoadText<T>(string name)
        {
            //TODO: Create a system where the user can create a JSON file with font size and color
            //and default text contained in it that can be created and put in 
            //the same location with the same name as the ttf file.  The 
            //extension should be .ffd (font file data).  If no file with the
            //same name with the .ffd extension exists, throw an exception explaining the issue.
            var font = $@"{ContentRootDirectory}Fonts\{name}.ttf";

            var fontPtr = _sdlFonts.OpenFont(font, 14);

            var newText = new SDLText(fontPtr, "Default Text");


            return newText as T;
        }
        #endregion
    }
}
