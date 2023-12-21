// <copyright file="KeyboardState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Represents a single keyboard state at a particular time.
/// </summary>
public struct KeyboardState : IEquatable<KeyboardState>
{
    private Dictionary<KeyCode, bool> keyStates;

    /*
    /// <summary>
    /// Gets a value indicating whether or not the caps lock key is on.
    /// </summary>
    public bool CapsLockOn => IsKeyDown(KeyCode.CapsLock);

    //// <summary>
    //// Gets or sets a value indicating whether or not the num lock key is on.
    //// </summary>
    public bool NumLockOn => IsKeyDown(KeyCode.NumLock);
    */

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if operands are equal.</returns>
    public static bool operator ==(KeyboardState left, KeyboardState right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if operands are not equal.</returns>
    public static bool operator !=(KeyboardState left, KeyboardState right) => !left.Equals(right);

    /// <summary>
    /// Returns all of the keys and their states.
    /// </summary>
    /// <returns>The keys and given state for each key.</returns>
    public Dictionary<KeyCode, bool> GetKeyStates()
    {
        InitKeys();

        return this.keyStates;
    }

    /// <summary>
    /// Gets a value indicating whether or not the left shift key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the left shift key is down.</returns>
    public bool IsLeftShiftKeyDown() => IsKeyDown(KeyCode.LeftShift);

    /// <summary>
    /// Gets a value indicating whether or not the right shift key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the right shift key is down.</returns>
    public bool IsRightShiftKeyDown() => IsKeyDown(KeyCode.RightShift);

    /// <summary>
    /// Gets a value indicating whether or not the left control key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the left control key is down.</returns>
    public bool IsLeftCtrlKeyDown() => IsKeyDown(KeyCode.LeftControl);

    /// <summary>
    /// Gets a value indicating whether or not the right control key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the right control key is down.</returns>
    public bool IsRightCtrlKeyDown() => IsKeyDown(KeyCode.RightControl);

    /// <summary>
    /// Gets a value indicating whether or not the left alt key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the left alt key is down.</returns>
    public bool IsLeftAltKeyDown() => IsKeyDown(KeyCode.LeftAlt);

    /// <summary>
    /// Gets a value indicating whether or not the right alt key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the right alt key is down.</returns>
    public bool IsRightAltKeyDown() => IsKeyDown(KeyCode.RightAlt);

    /// <summary>
    /// Returns all of the keys that are in the down position.
    /// </summary>
    /// <returns>A list of the keys that are currently in the down position.</returns>
    public KeyCode[] GetDownKeys()
    {
        IsRightAltKeyDown();
        InitKeys();

        var downKeys = new List<KeyCode>();

        foreach (var key in this.keyStates)
        {
            if (key.Value)
            {
                downKeys.Add(key.Key);
            }
        }

        return downKeys.ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether or not any keys are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any keys on the keyboard are in the down position.</returns>
    public bool AnyKeysDown()
    {
        InitKeys();

        return this.keyStates.Any(i => i.Value);
    }

    /// <summary>
    /// Returns a value indicating whether or not any of the given <paramref name="keys"/> are in the down position.
    /// </summary>
    /// <param name="keys">The list of key codes to check.</param>
    /// <returns><c>true</c> if any of the given <paramref name="keys"/> are in the down position.</returns>
    public bool AnyKeysDown(IEnumerable<KeyCode> keys)
    {
        InitKeys();

        var downKeys = new List<KeyCode>();

        foreach (var key in this.keyStates)
        {
            if (key.Value)
            {
                downKeys.Add(key.Key);
            }
        }

        return keys.Any(k => downKeys.Contains(k));
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="key"/> is in the down position.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the given <paramref name="key"/> is in the down position.</returns>
    public bool IsKeyDown(KeyCode key)
    {
        InitKeys();

        return this.keyStates[key];
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="key"/> is in the up position.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the given <paramref name="key"/> is in the up position.</returns>
    public bool IsKeyUp(KeyCode key)
    {
        InitKeys();

        return !this.keyStates[key];
    }

    /// <summary>
    /// Returns a value indicating whether or not any of the standard number keys,
    /// above the letter keys, are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the standard number keys are in the down position.</returns>
    public bool AnyStandardNumberKeysDown()
    {
        InitKeys();

        // Check all of the standard number keys
        foreach (var key in KeyboardKeyGroups.StandardNumberKeys)
        {
            if (IsKeyDown(key))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a value indicating whether or not any of the numpad number keys
    /// are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the numpad number keys are in the down position.</returns>
    public bool AnyNumpadNumberKeysDown()
    {
        // Check all of the numpad number keys
        foreach (var key in KeyboardKeyGroups.NumpadNumberKeys)
        {
            if (IsKeyDown(key))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a value indicating whether or not any of the shift keys are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the shift keys are down.</returns>
    public bool AnyShiftKeysDown() => IsKeyDown(KeyCode.LeftShift) || IsKeyDown(KeyCode.RightShift);

    /// <summary>
    /// Returns a value indicating whether or not any of the control keys are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the control keys are down.</returns>
    public bool AnyCtrlKeysDown() => IsKeyDown(KeyCode.LeftControl) || IsKeyDown(KeyCode.RightControl);

    /// <summary>
    /// Returns a value indicating whether or not any of the alt keys are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the control keys are down.</returns>
    public bool AnyAltKeysDown() => IsKeyDown(KeyCode.LeftAlt) || IsKeyDown(KeyCode.RightAlt);

    /// <summary>
    /// Returns the character equivalent of the given key if it is
    /// a letter, number or symbol key.  The value of 0 will be returned
    /// if the key is not a letter, number or symbol.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>The character that matches the given key.</returns>
    public char KeyToChar(KeyCode key)
    {
        if (AnyShiftKeysDown())
        {
            if (KeyboardKeyGroups.LetterKeys.Contains(key))
            {
                return key == KeyCode.Space ? ' ' : key.ToString().ToUpperInvariant()[0];
            }

            // When the shift key is down, the standard number keys and symbol keys return symbols.
            if (KeyboardKeyGroups.StandardNumberKeys.Contains(key))
            {
                return KeyboardKeyGroups.WithShiftSymbolCharacters[key];
            }

            return KeyboardKeyGroups.WithShiftSymbolCharacters[key];
        }

        if (KeyboardKeyGroups.LetterKeys.Contains(key))
        {
#pragma warning disable CA1304 // Specify CultureInfo
            return key == KeyCode.Space ? ' ' : key.ToString().ToLower()[0];
#pragma warning restore CA1304 // Specify CultureInfo
        }

        if (KeyboardKeyGroups.SymbolKeys.Contains(key))
        {
            return KeyboardKeyGroups.NoShiftSymbolCharacters[key];
        }

        // When the shift is up, the standard number keys return numbers.
        if (KeyboardKeyGroups.StandardNumberKeys.Contains(key))
        {
            return KeyboardKeyGroups.NoShiftStandardNumberCharacters[key];
        }

        return (char)0;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => this.keyStates.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not KeyboardState state)
        {
            return false;
        }

        return Equals(state);
    }

    /// <inheritdoc/>
    public bool Equals(KeyboardState other)
    {
        InitKeys();

        return !this.keyStates.Any(i => !other.GetKeyStates().Contains(i));
    }

    /// <summary>
    /// Sets the state of the given <paramref name="key"/> to the given <paramref name="state"/> value.
    /// </summary>
    /// <param name="key">The key to set the state to.</param>
    /// <param name="state">The state of the key.</param>
    public void SetKeyState(KeyCode key, bool state)
    {
        InitKeys();

        this.keyStates[key] = state;
    }

    /// <summary>
    /// Initializes the key states.
    /// </summary>
    private void InitKeys()
    {
        if (this.keyStates is not null && this.keyStates.Count > 0)
        {
            return;
        }

        this.keyStates = new Dictionary<KeyCode, bool>();

        var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

        foreach (var key in keys)
        {
            this.keyStates.Add(key, false);
        }
    }
}
