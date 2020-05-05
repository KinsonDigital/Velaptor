using Raptor.Input;
using Raptor.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using SDLCore;
using SDLKeyCode = SDLCore.KeyCode;
using RaptorKeyCode = Raptor.Input.KeyCode;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Provides functionality to interact with the keyboard using SDL.
    /// </summary>
    internal class SDLKeyboard : IKeyboard
    {
        #region Private Fields
        private SDL? _sdl = null;
        private readonly List<SDLKeyCode> _currentStateKeys = new List<SDLKeyCode>();
        private readonly List<SDLKeyCode> _prevStateKeys = new List<SDLKeyCode>();
        #endregion


        #region Props
        /// <summary>
        /// Gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn => (_sdl.GetModState() & Keymods.Caps) == Keymods.Caps;

        /// <summary>
        /// Gets a value indicating if the numlock key is on.
        /// </summary>
        public bool NumLockOn => (_sdl.GetModState() & Keymods.Num) == Keymods.Num;

        /// <summary>
        /// Gets a value indicating if the left shift key is being held down.
        /// </summary>
        public bool IsLeftShiftDown => (_sdl.GetModState() & Keymods.LShift) == Keymods.LShift;

        /// <summary>
        /// Gets a value indicating if the right shift key is being held down.
        /// </summary>
        public bool IsRightShiftDown => (_sdl.GetModState() & Keymods.RShift) == Keymods.RShift;

        /// <summary>
        /// Gets a value indicating if the left control key is being held down.
        /// </summary>
        public bool IsLeftCtrlDown => (_sdl.GetModState() & Keymods.LCtrl) == Keymods.LCtrl;

        /// <summary>
        /// Gets a value indicating if the right control key is being held down.
        /// </summary>
        public bool IsRightCtrlDown => (_sdl.GetModState() & Keymods.RCtrl) == Keymods.RCtrl;

        /// <summary>
        /// Gets a value indicating if the left alt key is being held down.
        /// </summary>
        public bool IsLeftAltDown => (_sdl.GetModState() & Keymods.LAlt) == Keymods.LAlt;

        /// <summary>
        /// Gets a value indicating if the right alt key is being held down.
        /// </summary>
        public bool IsRightAltDown => (_sdl.GetModState() & Keymods.RAlt) == Keymods.RAlt;
        #endregion


        #region Public Methods
        /// <summary>
        /// Update the current state of the keyboard.
        /// </summary>
        public void UpdateCurrentState()
        {
            _currentStateKeys.Clear();
            _currentStateKeys.AddRange(SDLEngineCore.CurrentKeyboardState);
        }


        /// <summary>
        /// Update the previous state of the keyboard.
        /// </summary>
        public void UpdatePreviousState()
        {
            _prevStateKeys.Clear();
            _prevStateKeys.AddRange(SDLEngineCore.PreviousKeyboardState);
        }


        /// <summary>
        /// Returns a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyKeysDown() => _currentStateKeys.Count > 0;


        /// <summary>
        /// Returns a value indicating if any of the given key codes are being held in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns></returns>
        public bool AreKeysDown(RaptorKeyCode[] keys) => keys.Any(k => _currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(k)));


        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyDown(RaptorKeyCode key) => _currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key));


        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyUp(RaptorKeyCode key) => !IsKeyDown(key);


        /// <summary>
        /// Returns true if the given key has been put into the down position then released to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyPressed(RaptorKeyCode key) => !_currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key)) &&
            _prevStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key));


        /// <summary>
        /// Returns all of the currently pressed keys of the keyboard for the current frame.
        /// </summary>
        /// <returns></returns>
        public RaptorKeyCode[] GetCurrentPressedKeys() => KeyboardKeyMapper.ToStandardKeyCodes(_currentStateKeys.ToArray());


        /// <summary>
        /// Returns all of the previously pressed keys of the keyborad from the last frame.
        /// </summary>
        /// <returns></returns>
        public RaptorKeyCode[] GetPreviousPressedKeys() => KeyboardKeyMapper.ToStandardKeyCodes(_prevStateKeys.ToArray());


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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static T GetData<T>(int option) where T : class
        {
            var otherOption = option;
            throw new NotImplementedException();
        }
        #endregion
    }
}
