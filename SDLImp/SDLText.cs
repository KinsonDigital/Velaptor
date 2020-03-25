using RaptorCore;
using RaptorCore.Graphics;
using SDLCore;
using SDLCore.Structs;
using System;

namespace SDLScorpPlugin
{
    /// <summary>
    /// SDL text that can be rendered to the graphics surface.
    /// </summary>
    public class SDLText : IText
    {
        #region Private Fields
        private SDL _sdl;
        private SDLFonts _sdlFonts;
        private readonly IntPtr _fontPtr;
        private IntPtr _texturePointer;
        private string _text;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SDLText"/>.
        /// </summary>
        /// <param name="fontPtr">The pointer to the SDL font.</param>
        /// <param name="text">The text to render to the surface.</param>
        public SDLText(IntPtr fontPtr, string text)
        {
            //TODO: Load the SDL libraries using a library loader
            _fontPtr = fontPtr;
            Color = new GameColor(255, 255, 255, 255);
            Text = text;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the text to be rendered to the graphics surface.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;

                var color = new Color()
                {
                    r = Color.Red,
                    g = Color.Green,
                    b = Color.Blue,
                    a = Color.Alpha,
                };

                //Create a surface for which to render the text to
                var surfacePtr = _sdlFonts.RenderTextSolid(_fontPtr, value, color);

                //Remove the old texture pointer before creating a new one to prevent a memory leak
                if (_texturePointer != IntPtr.Zero)
                    _sdl.DestroyTexture(_texturePointer);

                //Create a texture from the surface
                _texturePointer = _sdl.CreateTextureFromSurface(SDLEngineCore.RendererPointer, surfacePtr);

                _sdl.FreeSurface(surfacePtr);
            }
        }

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        public int Width
        {
            get
            {
                _sdl.QueryTexture(_texturePointer, out var _, out var _, out var width, out var _);


                return width;
            }
        }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        public int Height
        {
            get
            {
                _sdl.QueryTexture(_texturePointer, out var _, out var _, out var _, out var height);


                return height;
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public GameColor Color { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        public void InjectData<T>(T data) where T : class => throw new NotImplementedException();


        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Use the value of '1' if the data to return is of type <see cref="PointerContainer"/>.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <exception cref="Exception">Thrown if the <paramref name="option"/> value is not the value of
        /// type '1' for the type <see cref="PointerContainer"/>.</exception>
        public T GetData<T>(int option) where T : class
        {
            if (option == 1)
            {
                var ptrContainer = new PointerContainer();

                ptrContainer.PackPointer(_texturePointer);


                return ptrContainer as T;
            }


            //TODO: Create a custom InvalidGetDataOptionException class.  Implement this for all GetData<T> implementations
            throw new Exception($"The option '{option}' is not valid. \n\nValid options are 1.");
        }
        #endregion
    }
}
