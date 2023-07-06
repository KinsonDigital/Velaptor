// <copyright file="KeyCodeExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Input;

/// <summary>
/// Provides extension methods for the <see cref="KeyCode"/> enum.
/// </summary>
internal static class KeyCodeExtensions
{
    /// <summary>
    /// Returns a value indicating whether or not the key is a letter key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a letter key.</returns>
    public static bool IsLetterKey(this KeyCode key) => KeyboardKeyGroups.LetterKeys.Contains(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is a number key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a number key.</returns>
    public static bool IsNumberKey(this KeyCode key) => KeyboardKeyGroups.StandardNumberKeys.Contains(key) || KeyboardKeyGroups.NumpadNumberKeys.Contains(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is a symbol key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a symbol key.</returns>
    public static bool IsSymbolKey(this KeyCode key) => KeyboardKeyGroups.SymbolKeys.Contains(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is a visible key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a visible key.</returns>
    public static bool IsVisibleKey(this KeyCode key) => IsLetterKey(key) || IsNumberKey(key) || IsSymbolKey(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is not a visible key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is not a visible key.</returns>
    public static bool IsNotVisibleKey(this KeyCode key) => !IsVisibleKey(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is the left or right shift modifier key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a shift key.</returns>
    public static bool IsShiftKey(this KeyCode key) => key is KeyCode.LeftShift or KeyCode.RightShift;

    /// <summary>
    /// Returns a value indicating whether or not the key is any of the arrow keys.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is an arrow key.</returns>
    public static bool IsArrowKey(this KeyCode key) => key is KeyCode.Left or KeyCode.Right or KeyCode.Up or KeyCode.Down;

    /// <summary>
    /// Returns a value indicating whether or not the key is the left or right control modifier key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if it is a control key.</returns>
    public static bool IsCtrlKey(this KeyCode key) => key is KeyCode.LeftControl or KeyCode.RightControl;

    /// <summary>
    /// Converts the given key to a character based on if the shift key is being pressed down or not.
    /// </summary>
    /// <param name="key">The key to convert.</param>
    /// <param name="anyShiftKeysDown">True if any shift key is down.</param>
    /// <returns>The character equivalent of the key press.</returns>
    /// <remarks>Not all keyboard keys return characters.</remarks>
    public static char ToChar(this KeyCode key, bool anyShiftKeysDown)
    {
        if (KeyboardKeyGroups.LetterKeys.Contains(key))
        {
            return anyShiftKeysDown
                ? KeyboardKeyGroups.WithShiftLetterKeys[key]
                : KeyboardKeyGroups.NoShiftLetterKeys[key];
        }

        if (KeyboardKeyGroups.StandardNumberKeys.Contains(key))
        {
            return anyShiftKeysDown
                ? KeyboardKeyGroups.WithShiftStandardNumberCharacters[key]
                : KeyboardKeyGroups.NoShiftStandardNumberCharacters[key];
        }

        if (KeyboardKeyGroups.NumpadNumberKeys.Contains(key) && anyShiftKeysDown is false)
        {
            return KeyboardKeyGroups.NoShiftNumpadNumberCharacters[key];
        }

        if (KeyboardKeyGroups.SymbolKeys.Contains(key))
        {
            return anyShiftKeysDown
                ? KeyboardKeyGroups.WithShiftSymbolCharacters[key]
                : KeyboardKeyGroups.NoShiftSymbolCharacters[key];
        }

        return KeyboardKeyGroups.InvalidCharacter;
    }

    /// <summary>
    /// Returns a value indicating whether or not the key is a move cursor key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if a move cursor key.</returns>
    public static bool IsMoveCursorKey(this KeyCode key) =>
        key switch
        {
            KeyCode.Left => true,
            KeyCode.Right => true,
            KeyCode.PageUp => true,
            KeyCode.PageDown => true,
            KeyCode.Home => true,
            KeyCode.End => true,
            _ => false,
        };

    /// <summary>
    /// Returns a value indicating whether or not the key is not move cursor key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if not a move cursor key.</returns>
    public static bool IsNotMoveCursorKey(this KeyCode key) => !IsMoveCursorKey(key);

    /// <summary>
    /// Returns a value indicating whether or not the key is a deletion key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if a deletion key.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left public for future use.")]
    public static bool IsDeletionKey(this KeyCode key) => key is KeyCode.Delete or KeyCode.Backspace;

    /// <summary>
    /// Returns a value indicating whether or not the key is not a deletion key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if not a deletion key.</returns>
    public static bool IsNotDeletionKey(this KeyCode key) => !IsDeletionKey(key);
}
