using Raptor;
using Raptor.Graphics;
using Raptor.Plugins;
using SDLCore;
using SDLCore.Structs;
using System;
using SDLRect = SDLCore.Structs.Rect;
using CoreRect = Raptor.Rect;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Provides methods for rendering SDL textures, text and primitives to the screen.
    /// </summary>
    public class SDLRenderer : IRenderer
    {
        #region Private Fields
        private SDL _sdl;
        private bool _beginInvoked = false;
        private IntPtr _rendererPtr;
        #endregion


        #region Public Methods
        /// <summary>
        /// Starts the process of rendering a batch of <see cref="Texture"/>s, <see cref="GameText"/> items
        /// or primitives.  This method must be invoked before rendering.
        /// </summary>
        public void Begin()
        {
            CheckRenderPointer();
            _beginInvoked = true;
        }


        /// <summary>
        /// Stops the batching process and renders all of the batched textures to the screen.
        /// </summary>
        public void End()
        {
            CheckRenderPointer();

            if (!_beginInvoked)
                throw new Exception($"The '{nameof(Begin)}' method must be invoked first before the '{nameof(End)}' method.");

            _sdl.RenderPresent(_rendererPtr);

            _beginInvoked = false;
        }


        /// <summary>
        /// Clears the screen to the given color.
        /// </summary>
        /// <param name="color">The color to clear the screen to.</param>
        public void Clear(GameColor color)
        {
            CheckRenderPointer();

            _sdl.SetRenderDrawColor(_rendererPtr, color.Red, color.Green, color.Blue, color.Alpha);
            _sdl.RenderClear(_rendererPtr);
        }


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        public void Render(ITexture texture, float x, float y) => Render(texture, x, y, 0f, 1f, new GameColor(255, 255, 255, 255));


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        public void Render(ITexture texture, float x, float y, float angle) => Render(texture, x, y, angle, 1f, new GameColor(255, 255, 255, 255));


        /// <summary>
        /// Renders the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location and rotates the texture to the given
        /// <paramref name="angle"/> in degrees.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        /// <param name="angle">The angle in degrees to rotate the texture to.</param>
        /// <param name="color">The color to apply to the texture.</param>
        public void Render(ITexture texture, float x, float y, float angle, float size, GameColor color)
        {
            CheckRenderPointer();

            //NOTE: SDL takes the angle in degrees, not radians.
            var textureOrigin = new Point()
            {
                x = (int)((texture.Width * size) / 2f),
                y = (int)((texture.Height * size) / 2f)
            };

            var srcRect = new SDLRect()
            {
                x = 0,
                y = 0,
                w = texture.Width,
                h = texture.Height
            };

            var destRect = new SDLRect()
            {
                x = (int)(x - ((texture.Width * size) / 2)),//Texture X on screen
                y = (int)(y - ((texture.Height * size) / 2)),//Texture Y on screen
                w = (int)(texture.Width * size),//Scaled occurding to size
                h = (int)(texture.Height * size)
            };


            //TODO: Figure out what to do with the pointer needed from the ITexture object
            //var texturePtr = texture.GetData<PointerContainer>(1).UnpackPointer();
            var texturePtr = IntPtr.Zero;

            _sdl.SetTextureBlendMode(texturePtr, SDL_BlendMode.SDL_BLENDMODE_BLEND);
            _sdl.SetTextureColorMod(texturePtr, color.Red, color.Green, color.Blue);
            _sdl.SetTextureAlphaMod(texturePtr, color.Alpha);
            _sdl.RenderCopyEx(_rendererPtr, texturePtr, ref srcRect, ref destRect, angle, ref textureOrigin, SDL_RendererFlip.SDL_FLIP_NONE);
        }


        /// <summary>
        /// Renders the given <paramref name="text"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        public void Render(IText text, float x, float y) => Render(text, x, y, text.Color);


        /// <summary>
        /// Renders the given text at the given <paramref name="x"/> and <paramref name="y"/>
        /// location and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="x">The X coordinate location of where to render the text.</param>
        /// <param name="y">The Y coordinate location of where to render the text.</param>
        /// <param name="color">The color to render the text.</param>
        public void Render(IText text, float x, float y, GameColor color)
        {
            CheckRenderPointer();

            //TODO: Figure out how to deal with pointer from the IText object.
            //var texturePtr = text.GetData<PointerContainer>(1).UnpackPointer();
            var texturePtr = IntPtr.Zero;

            //TODO:  Check for color index values first
            _sdl.SetTextureColorMod(texturePtr, color.Red, color.Green, color.Blue);
            _sdl.SetTextureAlphaMod(texturePtr, color.Alpha);
            _sdl.SetTextureBlendMode(texturePtr, SDL_BlendMode.SDL_BLENDMODE_BLEND);

            var srcRect = new SDLRect()
            {
                x = 0,
                y = 0,
                w = text.Width,
                h = text.Height
            };

            var destRect = new SDLRect()
            {
                x = (int)x,
                y = (int)y,
                w = text.Width,
                h = text.Height
            };

            //Render texture to screen
            _sdl.RenderCopy(_rendererPtr, texturePtr, ref srcRect, ref destRect);
        }


        /// <summary>
        /// Renders an area of the given <paramref name="texture"/> at the given <paramref name="x"/>
        /// and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="area">The area/section of the texture to render.</param>
        /// <param name="x">The X coordinate location on the screen to render.</param>
        /// <param name="y">The Y coordinate location on the screen to render.</param>
        public void RenderTextureArea(ITexture texture, CoreRect area, float x, float y)
        {
            CheckRenderPointer();

            var textureOrigin = new Point()
            {
                x = texture.Width / 2,
                y = texture.Height / 2
            };

            var srcRect = new SDLRect()
            {
                x = (int)area.X,
                y = (int)area.Y,
                w = texture.Width,
                h = texture.Height
            };

            var destRect = new SDLRect()
            {
                x = (int)x,//Texture X on screen
                y = (int)y,//Texture Y on screen
                w = texture.Width,
                h = texture.Height
            };

            //TODO: Figure out what to do with the pointer needed from the ITexture object
            //var texturePtr = texture.GetData<PointerContainer>(1).UnpackPointer();
            var texturePtr = IntPtr.Zero;

            _sdl.RenderCopyEx(_rendererPtr, texturePtr, ref srcRect, ref destRect, 0.0, ref textureOrigin, SDL_RendererFlip.SDL_FLIP_NONE);
        }


        /// <summary>
        /// Renders a line using the given start and stop coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        public void RenderLine(float lineStartX, float lineStartY, float lineStopX, float lineStopY)
        {
            CheckRenderPointer();
            RenderLine(lineStartX, lineStartY, lineStopX, lineStopY, new GameColor(255, 255, 255, 255));
        }


        /// <summary>
        /// Renders a line using the given start and stop X and Y coordinates.
        /// </summary>
        /// <param name="lineStartX">The starting X coordinate of the line.</param>
        /// <param name="lineStartY">The starting Y coordinate of the line.</param>
        /// <param name="lineStopX">The ending X coordinate of the line.</param>
        /// <param name="lineStopY">The ending Y coordinate of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void RenderLine(float startX, float startY, float endX, float endY, GameColor color)
        {
            CheckRenderPointer();

            _sdl.SetRenderDrawColor(_rendererPtr, color.Red, color.Green, color.Blue, color.Alpha);

            _sdl.RenderDrawLine(_rendererPtr, (int)startX, (int)startY, (int)endX, (int)endY);
        }


        /// <summary>
        /// Creates a filled circle at the given <paramref name="x"/> and <paramref name="y"/> location
        /// with the given <paramref name="radius"/> and with the given <paramref name="color"/>.  The
        /// <paramref name="x"/> and <paramref name="y"/> coordinates represent the center of the circle.
        /// </summary>
        /// <param name="x">The X coordinate on the screen of where to render the circle.</param>
        /// <param name="y">The Y coordinate on the screen of where to render the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        public void FillCircle(float centerX, float centerY, float radius, GameColor color)
        {
            CheckRenderPointer();

            /*Midpoint Algorithm
             * 1. https://stackoverflow.com/questions/38334081/howto-draw-circles-arcs-and-vector-graphics-in-sdl
             * 2. https://en.wikipedia.org/wiki/Midpoint_circle_algorithm#C_Example
             */

            int diameter = (int)(radius * 2f);

            int x = (int)(radius - 1);
            int y = 0;
            int tx = 1;
            int ty = 1;
            int error = (tx - diameter);

            int centerXIntValue = (int)centerX;
            int centerYIntValue = (int)centerY;

            _sdl.SetRenderDrawColor(_rendererPtr, color.Red, color.Green, color.Blue, color.Alpha);

            while (x >= y)
            {
                //Each of the following renders an octant of the circle
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue + x, centerYIntValue - y);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue + x, centerYIntValue + y);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue - x, centerYIntValue - y);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue - x, centerYIntValue + y);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue + y, centerYIntValue - x);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue + y, centerYIntValue + x);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue - y, centerYIntValue - x);
                _sdl.RenderDrawPoint(_rendererPtr, centerXIntValue - y, centerYIntValue + x);

                if (error <= 0)
                {
                    ++y;
                    error += ty;
                    ty += 2;
                }

                if (error > 0)
                {
                    --x;
                    tx += 2;
                    error += (tx - diameter);
                }
            }
        }


        /// <summary>
        /// Renders a filled rectangle using the given <paramref name="rect"/>
        /// and using the given <paramref name="color"/>.
        /// </summary>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void FillRect(CoreRect rect, GameColor color)
        {
            var sdlRect = new SDLRect()
            {
                x = (int)rect.X,
                y = (int)rect.Y,
                w = (int)rect.Width,
                h = (int)rect.Height
            };

            _sdl.SetRenderDrawColor(_rendererPtr, color.Red, color.Green, color.Blue, color.Alpha);
            _sdl.RenderFillRect(_rendererPtr, ref sdlRect);
        }


        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The <see cref="PointerContainer"/> type to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        /// <exception cref="Exception">Thrown if the '<paramref name="data"/>' parameter is not of type <see cref="PointerContainer"/>.</exception>
        public void InjectData<T>(T data) where T : class
        {
            //TODO: Replace this with a custom exception called InjectDataException class
            if (data.GetType() != typeof(PointerContainer))
                throw new Exception($"Data getting injected into {nameof(SDLRenderer)} is not of type {nameof(PointerContainer)}.  Incorrect type is '{data.GetType().ToString()}'");

            _rendererPtr = (data as PointerContainer).UnpackPointer();
        }


        /// <summary>
        /// Gets any arbitrary data needed for use.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns></returns>
        public T GetData<T>(int option) where T : class
        {
            var ptrContainer = new PointerContainer();
            ptrContainer.PackPointer(_rendererPtr);


            return ptrContainer as T;
        }


        /// <summary>
        /// Properly destroys the SDL renderer.
        /// </summary>
        public void Dispose() => _sdl.DestroyRenderer(_rendererPtr);
        #endregion


        #region Private Methods
        private void CheckRenderPointer()
        {
            if (_rendererPtr == IntPtr.Zero)
                throw new Exception("The SDL renderer does not have an intialized renderer pointer.");
        }
        #endregion
    }
}
