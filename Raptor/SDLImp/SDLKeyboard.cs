using Raptor.Input;
using Raptor.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using SDLCore;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Provides functionality to interact with the keyboard using SDL.
    /// </summary>
    public class SDLKeyboard : IKeyboard
    {
        #region Private Fields
        private SDL _sdl;
        private readonly List<Keycode> _currentStateKeys = new List<Keycode>();
        private readonly List<Keycode> _prevStateKeys = new List<Keycode>();
        #endregion


        #region Props
        /// <summary>
        /// Gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn => (_sdl.GetModState() & Keymod.KMOD_CAPS) == Keymod.KMOD_CAPS;

        /// <summary>
        /// Gets a value indicating if the numlock key is on.
        /// </summary>
        public bool NumLockOn => (_sdl.GetModState() & Keymod.KMOD_NUM) == Keymod.KMOD_NUM;

        /// <summary>
        /// Gets a value indicating if the left shift key is being held down.
        /// </summary>
        public bool IsLeftShiftDown => (_sdl.GetModState() & Keymod.KMOD_LSHIFT) == Keymod.KMOD_LSHIFT;

        /// <summary>
        /// Gets a value indicating if the right shift key is being held down.
        /// </summary>
        public bool IsRightShiftDown => (_sdl.GetModState() & Keymod.KMOD_RSHIFT) == Keymod.KMOD_RSHIFT;

        /// <summary>
        /// Gets a value indicating if the left control key is being held down.
        /// </summary>
        public bool IsLeftCtrlDown => (_sdl.GetModState() & Keymod.KMOD_LCTRL) == Keymod.KMOD_LCTRL;

        /// <summary>
        /// Gets a value indicating if the right control key is being held down.
        /// </summary>
        public bool IsRightCtrlDown => (_sdl.GetModState() & Keymod.KMOD_RCTRL) == Keymod.KMOD_RCTRL;

        /// <summary>
        /// Gets a value indicating if the left alt key is being held down.
        /// </summary>
        public bool IsLeftAltDown => (_sdl.GetModState() & Keymod.KMOD_LALT) == Keymod.KMOD_LALT;

        /// <summary>
        /// Gets a value indicating if the right alt key is being held down.
        /// </summary>
        public bool IsRightAltDown => (_sdl.GetModState() & Keymod.KMOD_RALT) == Keymod.KMOD_RALT;
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
        public bool AreKeysDown(KeyCodes[] keys) => keys.Any(k => _currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(k)));


        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyDown(KeyCodes key) => _currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key));


        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyUp(KeyCodes key) => !IsKeyDown(key);


        /// <summary>
        /// Returns true if the given key has been put into the down position then released to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyPressed(KeyCodes key) => !_currentStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key)) &&
            _prevStateKeys.Contains(KeyboardKeyMapper.ToSDLKeyCode(key));


        /// <summary>
        /// Returns all of the currently pressed keys of the keyboard for the current frame.
        /// </summary>
        /// <returns></returns>
        public KeyCodes[] GetCurrentPressedKeys() => KeyboardKeyMapper.ToStandardKeyCodes(_currentStateKeys.ToArray());


        /// <summary>
        /// Returns all of the previously pressed keys of the keyborad from the last frame.
        /// </summary>
        /// <returns></returns>
        public KeyCodes[] GetPreviousPressedKeys() => KeyboardKeyMapper.ToStandardKeyCodes(_prevStateKeys.ToArray());


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
