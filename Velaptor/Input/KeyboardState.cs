// <copyright file="KeyboardState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1642 // This warning shows it should be described as a class but it is a struct
namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;

/// <summary>
/// Represents a single keyboard state at a particular time.
/// </summary>
public record struct KeyboardState
{
    private static readonly int Capacity = Enum.GetNames(typeof(KeyCode)).Length;

    /// <summary>
    /// Gets the state of the keys.
    /// </summary>
    internal Dictionary<KeyCode, bool> KeyStates { get; private set; } = new (Capacity);

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardState"/> struct.
    /// </summary>
    public KeyboardState()
    {
    }

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
    /// Gets a value indicating whether or not the right shift key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the right shift key is down.</returns>
    public bool IsRightShiftKeyDown() => IsKeyDown(KeyCode.RightShift);

    /// <summary>
    /// Gets a value indicating whether or not the left shift key is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the left shift key is down.</returns>
    public bool IsLeftShiftKeyDown() => IsKeyDown(KeyCode.LeftShift);

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
    public Span<KeyCode> GetDownKeys()
    {
        InitKeyStates();

        var results = new List<KeyCode>();
        foreach ((KeyCode key, var state) in KeyStates)
        {
            if (state)
            {
                results.Add(key);
            }
        }

        return results.ToArray().AsSpan();
    }

    /// <summary>
    /// Gets a value indicating whether or not any keys are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any keys on the keyboard are in the down position.</returns>
    public bool AnyKeysDown()
    {
        InitKeyStates();

        foreach (var keyState in KeyStates)
        {
            if (keyState.Value)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a value indicating whether or not any of the given <paramref name="keys"/> are in the down position.
    /// </summary>
    /// <param name="keys">The list of key codes to check.</param>
    /// <returns><c>true</c> if any of the given <paramref name="keys"/> are in the down position.</returns>
    public bool AnyKeysDown(IEnumerable<KeyCode> keys)
    {
        if (KeyStates is null)
        {
            return false;
        }

        var states = KeyStates;

        return keys.Any(k => states[k]);
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="key"/> is in the down position.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the given <paramref name="key"/> is in the down position.</returns>
    public bool IsKeyDown(KeyCode key)
    {
        InitKeyStates();

        if (!KeyStates.TryGetValue(key, out var value))
        {
            return false;
        }

        return value;
    }

    /// <summary>
    /// Returns a value indicating whether or not the given <paramref name="key"/> is in the up position.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the given <paramref name="key"/> is in the up position.</returns>
    public bool IsKeyUp(KeyCode key) => !IsKeyDown(key);

    /// <summary>
    /// Returns a value indicating whether or not any of the standard number keys,
    /// above the letter keys, are in the down position.
    /// </summary>
    /// <returns><c>true</c> if any of the standard number keys are in the down position.</returns>
    public bool AnyStandardNumberKeysDown()
    {
        // Check all of the standard number keys
        // TODO: Need to perf test if the recommend linq method is faster than the foreach loop below
        foreach (var key in KeyboardKeyGroups.GetStandardNumberKeys())
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
        // TODO: Need to perf test if the recommend linq method is faster than the foreach loop below
        foreach (var key in KeyboardKeyGroups.GetNumpadNumberKeys())
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
    public char KeyToChar(KeyCode key) => key.ToChar(AnyShiftKeysDown());

    /// <summary>
    /// Sets the state of the given <paramref name="key"/> to the given <paramref name="state"/> value.
    /// </summary>
    /// <param name="key">The key to set the state to.</param>
    /// <param name="state">The state of the key.</param>
    public void SetKeyState(KeyCode key, bool state)
    {
        InitKeyStates();
        KeyStates[key] = state;
    }

    /// <summary>
    /// Initializes the key states if they are null.
    /// </summary>
    private void InitKeyStates() => KeyStates ??= new (Capacity);
}
