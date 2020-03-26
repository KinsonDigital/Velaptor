using Raptor.Input;
namespace Raptor.Plugins
{
    /// <summary>
    /// Provides functionality to interact with the keyboard.
    /// </summary>
    public interface IKeyboard
    {
        #region Props
        /// <summary>
        /// Gets a value indicating if the caps lock key is on.
        /// </summary>
        bool CapsLockOn { get; }

        /// <summary>
        /// Gets a value indicating if the numlock key is on.
        /// </summary>
        bool NumLockOn { get; }

        /// <summary>
        /// Gets a value indicating if the left shift key is being held down.
        /// </summary>
        bool IsLeftShiftDown { get; }

        /// <summary>
        /// Gets a value indicating if the right shift key is being held down.
        /// </summary>
        bool IsRightShiftDown { get; }

        /// <summary>
        /// Gets a value indicating if the left control key is being held down.
        /// </summary>
        bool IsLeftCtrlDown { get; }

        /// <summary>
        /// Gets a value indicating if the right control key is being held down.
        /// </summary>
        bool IsRightCtrlDown { get; }

        /// <summary>
        /// Gets a value indicating if the left alt key is being held down.
        /// </summary>
        bool IsLeftAltDown { get; }

        /// <summary>
        /// Gets a value indicating if the right alt key is being held down.
        /// </summary>
        bool IsRightAltDown { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Update the current state of the keyboard.
        /// </summary>
        void UpdateCurrentState();


        /// <summary>
        /// Update the previous state of the keyboard.
        /// </summary>
        void UpdatePreviousState();


        /// <summary>
        /// Returns a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns></returns>
        bool AreAnyKeysDown();


        /// <summary>
        /// Returns a value indicating if any of the given key codes are being held in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns></returns>
        bool AreKeysDown(KeyCodes[] keys);


        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        bool IsKeyDown(KeyCodes key);


        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        bool IsKeyUp(KeyCodes key);


        /// <summary>
        /// Returns true if the given key has been put into the down position then released to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        bool IsKeyPressed(KeyCodes key);


        /// <summary>
        /// Returns all of the currently pressed keys of the keyboard for the current frame.
        /// </summary>
        /// <returns></returns>
        KeyCodes[] GetCurrentPressedKeys();


        /// <summary>
        /// Returns all of the previously pressed keys of the keyborad from the last frame.
        /// </summary>
        /// <returns></returns>
        KeyCodes[] GetPreviousPressedKeys();
        #endregion
    }
}
