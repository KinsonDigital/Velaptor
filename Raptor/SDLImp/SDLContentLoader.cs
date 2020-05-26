using Raptor.Graphics;
using Raptor.Plugins;
using System;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Loads and unloads content using SDL.
    /// </summary>
    internal class SDLContentLoader : IContentLoader
    {
        //#region Private Fields
        //private SDL? _sdl = null;
        //private SDLImage? _sdlImage = null;
        //private SDLFonts? _sdlFonts = null;
        //#endregion


        //#region Constructors
        //public SDLContentLoader()
        //{
        //    //TODO: Add code here to load the SDL libraries using a library loader
        //    ContentRootDirectory = $@"{GamePath}\Content\";
        //}
        //#endregion


        //#region Props
        //public string GamePath
        //{
        //    get
        //    {
        //        var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);


        //        return path is null ? string.Empty : path;
        //    }
        //}

        //public string ContentRootDirectory { get; set; }
        //#endregion


        //#region Public Methods
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "<Pending>")]
        //T? IContentLoader.Load<T>(string name) where T : class
        //{
        //    if (_sdlImage is null || _sdl is null)
        //        return null;

        //    var texturePath = $@"{ContentRootDirectory}\Graphics\{name}.png";


        //    //Load image at specified path
        //    var loadedSurface = _sdlImage.Load(texturePath);

        //    //The final optimized image
        //    IntPtr newTexturePtr;
        //    if (loadedSurface == IntPtr.Zero)
        //    {
        //        throw new Exception($"Unable to load image {texturePath}! \n\nSDL Error: {_sdl.GetError()}");
        //    }
        //    else
        //    {
        //        //Create texture from surface pixels
        //        newTexturePtr = _sdl.CreateTextureFromSurface(SDLGameCore.RendererPointer, loadedSurface);

        //        if (newTexturePtr == IntPtr.Zero)
        //            throw new Exception($"Unable to create texture from {texturePath}! \n\nSDL Error: {_sdl.GetError()}");

        //        //Get rid of old loaded surface
        //        _sdl.FreeSurface(loadedSurface);
        //    }

        //    ITexture newTexture = new SDLTexture(newTexturePtr);


        //    return newTexture as T;
        //}


        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        //public static T GetData<T>(int option) where T : class => throw new NotImplementedException(option.ToString(CultureInfo.InvariantCulture));



        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        //public void InjectData<T>(T data) where T : class => throw new NotImplementedException();

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "<Pending>")]
        //T? IContentLoader.LoadText<T>(string name) where T : class
        //{
        //    if (_sdlFonts is null)
        //        return null;

        //    //TODO: Create a system where the user can create a JSON file with font size and color
        //    //and default text contained in it that can be created and put in 
        //    //the same location with the same name as the ttf file.  The 
        //    //extension should be .ffd (font file data).  If no file with the
        //    //same name with the .ffd extension exists, throw an exception explaining the issue.
        //    var font = $@"{ContentRootDirectory}Fonts\{name}.ttf";

        //    var fontPtr = _sdlFonts.OpenFont(font, 14);

        //    var newText = new SDLText(fontPtr, "Default Text");


        //    return newText as T;
        //}

        //public Texture? LoadTexture(string name)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion
        public string ContentRootDirectory => throw new NotImplementedException();

        public Texture? LoadTexture(string name)
        {
            throw new NotImplementedException();
        }
    }
}
