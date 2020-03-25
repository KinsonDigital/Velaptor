using Raptor.Input;
using Raptor.Plugins;
using SDLCore;
using System;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Provides mouse input using SDL.
    /// </summary>
    public class SDLMouse : IMouse
    {
        #region Private Fields
        private SDL _sdl;
        private static bool _currentLeftButtonState;
        private static bool _currentRightButtonState;
        private static bool _currentMiddleButtonState;
        private static bool _prevLeftButtonState;
        private static bool _prevRightButtonState;
        private static bool _prevMiddleButtonState;
        #endregion


        #region Constructors
        public SDLMouse()
        {
            //TODO: Load the SDL libraries using library loaders
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets sets the X position of the mouse.
        /// </summary>
        public int X
        {
            get => (int)SDLEngineCore.MousePosition.X;
            set => SetPosition(value, Y);
        }

        /// <summary>
        /// Gets sets the Y position of the mouse.
        /// </summary>
        public int Y
        {
            get => (int)SDLEngineCore.MousePosition.Y;
            set => SetPosition(X, value);
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Update the current state of the mouse.
        /// </summary>
        public void UpdateCurrentState()
        {
            _currentLeftButtonState = SDLEngineCore.CurrentLeftMouseButtonState;
            _currentRightButtonState = SDLEngineCore.CurrentRightMouseButtonState;
            _currentMiddleButtonState = SDLEngineCore.CurrentMiddleMouseButtonState;
        }


        /// <summary>
        /// Update the previous state of the mouse.
        /// </summary>
        public void UpdatePreviousState()
        {
            _prevLeftButtonState = _currentLeftButtonState;
            _prevMiddleButtonState = _currentMiddleButtonState;
            _prevRightButtonState = _currentRightButtonState;
        }


        /// <summary>
        /// Sets the position of the mouse using the given <paramref name="x"/> and <paramref name="y"/> coordinate values.
        /// </summary>
        /// <param name="x">The horizontal X position to set the mouse in the game window.</param>
        /// <param name="y">The vertical Y position to set the mouse in the game window.</param>
        public void SetPosition(int x, int y) => _sdl.WarpMouseInWindow(SDLEngineCore.WindowPtr, x, y);


        /// <summary>
        /// Returns true if the given <see cref="input"/> is in the down position.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns></returns>
        public bool IsButtonDown(InputButton input)
        {
            //Return the down state of the given mouse input
            switch (input)
            {
                case InputButton.LeftButton:
                    return _currentLeftButtonState;
                case InputButton.RightButton:
                    return _currentRightButtonState;
                case InputButton.MiddleButton:
                    return _currentMiddleButtonState;
                default:
                    throw new ArgumentOutOfRangeException(nameof(input), input, null);
            }
        }


        /// <summary>
        /// Returns true if the given <paramref name="input"/> is in the up position.
        /// </summary>
        /// <param name="inputButton">The input button to check.</param>
        /// <returns></returns>
        public bool IsButtonUp(InputButton input) => !IsButtonDown(input);


        /// <summary>
        /// Returns true if the given mouse <paramref name="input"/> has been released from the down position.
        /// </summary>
        /// <param name="inputButton">The input button to check.</param>
        /// <returns></returns>
        public bool IsButtonPressed(InputButton input)
        {
            //Return the pressed state of the given mouse input
            switch (input)
            {
                case InputButton.RightButton:
                    return !_currentLeftButtonState && _prevLeftButtonState;
                case InputButton.LeftButton:
                    return !_currentRightButtonState && _prevRightButtonState;
                case InputButton.MiddleButton:
                    return !_currentMiddleButtonState && _prevMiddleButtonState;
                default:
                    throw new ArgumentOutOfRangeException(nameof(input), input, null);
            }
        }


        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        public void InjectData<T>(T data) where T : class => throw new NotImplementedException();


        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Used to pass in options for the <see cref="GetData{T}(int)"/> implementation to process.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns></returns>
        public T GetData<T>(int option) where T : class => throw new NotImplementedException();
        #endregion
    }
}
