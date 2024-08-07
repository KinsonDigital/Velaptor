// <copyright file="KeyboardKeyGroups.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Holds groups of different keyboard keys.
/// </summary>
internal readonly record struct KeyboardKeyGroups
{
    /// <summary>
    /// Gets a character that represents an invalid character.
    /// </summary>
    public const char InvalidCharacter = '□';

    private static KeyCode[]? noShiftLetterCharacters;
    private static KeyCode[]? noShiftSymbolCharacters;
    private static KeyCode[]? noShiftStandardNumberCharacters;
    private static KeyCode[]? noShiftNumpadNumberCharacters;

    /// <summary>
    /// Gets a list of the cursor movement keys.
    /// </summary>
    public static IReadOnlyList<KeyCode> CursorMovementKeys { get; } = new[]
    {
        KeyCode.Left, KeyCode.Right, KeyCode.Up, KeyCode.Down, KeyCode.Home,
        KeyCode.End, KeyCode.Tab, KeyCode.PageUp, KeyCode.PageDown,
    };

    /// <summary>
    /// Gets a list of the function keys.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Kept for future use.")]
    public static IReadOnlyList<KeyCode> FunctionKeys { get; } = new[]
    {
        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5,
        KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10,
        KeyCode.F11, KeyCode.F12, KeyCode.F13, KeyCode.F14, KeyCode.F15,
        KeyCode.F16, KeyCode.F17, KeyCode.F18, KeyCode.F19, KeyCode.F20,
        KeyCode.F21, KeyCode.F22, KeyCode.F23, KeyCode.F24,
    };

    /// <summary>
    /// Gets the list of keyboard keys used for deletion.
    /// </summary>
    public static readonly IReadOnlyList<KeyCode> DeletionKeys = new[] { KeyCode.Delete, KeyCode.Backspace, };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// letter keys when no shift modifier keys are in the down position.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static FrozenDictionary<KeyCode, char> NoShiftLetterCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.A, 'a' }, { KeyCode.B, 'b' }, { KeyCode.C, 'c' }, { KeyCode.D, 'd' }, { KeyCode.E, 'e' },
        { KeyCode.F, 'f' }, { KeyCode.G, 'g' }, { KeyCode.H, 'h' }, { KeyCode.I, 'i' }, { KeyCode.J, 'j' },
        { KeyCode.K, 'k' }, { KeyCode.L, 'l' }, { KeyCode.M, 'm' }, { KeyCode.N, 'n' }, { KeyCode.O, 'o' },
        { KeyCode.P, 'p' }, { KeyCode.Q, 'q' }, { KeyCode.R, 'r' }, { KeyCode.S, 's' }, { KeyCode.T, 't' },
        { KeyCode.U, 'u' }, { KeyCode.V, 'v' }, { KeyCode.W, 'w' }, { KeyCode.X, 'x' }, { KeyCode.Y, 'y' },
        { KeyCode.Z, 'z' }, { KeyCode.Space, ' ' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// letter keys when any of the shift modifier keys are in the down position.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static FrozenDictionary<KeyCode, char> WithShiftLetterCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.A, 'A' }, { KeyCode.B, 'B' }, { KeyCode.C, 'C' }, { KeyCode.D, 'D' }, { KeyCode.E, 'E' },
        { KeyCode.F, 'F' }, { KeyCode.G, 'G' }, { KeyCode.H, 'H' }, { KeyCode.I, 'I' }, { KeyCode.J, 'J' },
        { KeyCode.K, 'K' }, { KeyCode.L, 'L' }, { KeyCode.M, 'M' }, { KeyCode.N, 'N' }, { KeyCode.O, 'O' },
        { KeyCode.P, 'P' }, { KeyCode.Q, 'Q' }, { KeyCode.R, 'R' }, { KeyCode.S, 'S' }, { KeyCode.T, 'T' },
        { KeyCode.U, 'U' }, { KeyCode.V, 'V' }, { KeyCode.W, 'W' }, { KeyCode.X, 'X' }, { KeyCode.Y, 'Y' },
        { KeyCode.Z, 'Z' }, { KeyCode.Space, ' ' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// standard number keys when no shift modifier keys are in the down position.
    /// </summary>
    public static FrozenDictionary<KeyCode, char> NoShiftStandardNumberCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.D0, '0' }, { KeyCode.D1, '1' }, { KeyCode.D2, '2' },
        { KeyCode.D3, '3' }, { KeyCode.D4, '4' }, { KeyCode.D5, '5' },
        { KeyCode.D6, '6' }, { KeyCode.D7, '7' }, { KeyCode.D8, '8' }, { KeyCode.D9, '9' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// standard number keys when no shift modifier keys are in the down position.
    /// </summary>
    public static FrozenDictionary<KeyCode, char> WithShiftStandardNumberCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.D1, '!' }, { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
        { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' }, { KeyCode.D0, ')' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// keyboard when no shift modifier keys are in the down position.
    /// </summary>
    public static FrozenDictionary<KeyCode, char> NoShiftSymbolCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.Equal, '=' }, { KeyCode.Comma, ',' }, { KeyCode.Minus, '-' }, { KeyCode.Period, '.' }, { KeyCode.Slash, '/' },
        { KeyCode.Backslash, '\\' }, { KeyCode.LeftBracket, '[' }, { KeyCode.RightBracket, ']' },
        { KeyCode.Apostrophe, '\'' }, { KeyCode.Semicolon, ';' }, { KeyCode.KeyPadAdd, '+' }, { KeyCode.KeyPadDecimal, '.' },
        { KeyCode.KeyPadDivide, '/' }, { KeyCode.KeyPadMultiply, '*' }, { KeyCode.KeyPadSubtract, '-' },
        { KeyCode.GraveAccent, '`' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/>produced by
    /// the keyboard when any shift modifier keys are in the down position.
    /// </summary>
    public static FrozenDictionary<KeyCode, char> WithShiftSymbolCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.Equal, '+' }, { KeyCode.Comma, '<' }, { KeyCode.Minus, '_' }, { KeyCode.Period, '>' }, { KeyCode.Slash, '?' },
        { KeyCode.Backslash, '|' }, { KeyCode.LeftBracket, '{' }, { KeyCode.RightBracket, '}' },
        { KeyCode.Apostrophe, '"' }, { KeyCode.Semicolon, ':' }, { KeyCode.D1, '!' },
        { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
        { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' },
        { KeyCode.D0, ')' }, { KeyCode.KeyPadDivide, '/' }, { KeyCode.KeyPadMultiply, '*' }, { KeyCode.KeyPadSubtract, '-' },
        { KeyCode.KeyPadAdd, '+' }, { KeyCode.KeyPadDecimal, '.' }, { KeyCode.GraveAccent, '~' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by
    /// the keyboard when no shift modifier keys are in the down position.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static FrozenDictionary<KeyCode, char> NoShiftNumpadNumberCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.KeyPad0, '0' }, { KeyCode.KeyPad1, '1' }, { KeyCode.KeyPad2, '2' }, { KeyCode.KeyPad3, '3' }, { KeyCode.KeyPad4, '4' },
        { KeyCode.KeyPad5, '5' }, { KeyCode.KeyPad6, '6' }, { KeyCode.KeyPad7, '7' }, { KeyCode.KeyPad8, '8' }, { KeyCode.KeyPad9, '9' },
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets a list of all the keys that produce a letter character when no shift modifier keys are in the down position.
    /// </summary>
    /// <returns>The keys that produce a lowercase letter character.</returns>
    public static KeyCode[] GetLetterKeys()
    {
        noShiftLetterCharacters ??= NoShiftLetterCharacters.Keys.ToArray();
        return noShiftLetterCharacters;
    }

    /// <summary>
    /// Gets a list of all the keys that produce a standard number character when no shift modifier keys are in the down position.
    /// </summary>
    /// <returns>The keys that produce a standard number character.</returns>
    public static KeyCode[] GetStandardNumberKeys()
    {
        noShiftStandardNumberCharacters ??= NoShiftStandardNumberCharacters.Keys.ToArray();
        return noShiftStandardNumberCharacters;
    }

    /// <summary>
    /// Gets a list of all the keys that produce a numpad number character when no shift modifier keys are in the down position.
    /// </summary>
    /// <returns>The keys that produce a numpad number character.</returns>
    public static KeyCode[] GetNumpadNumberKeys()
    {
        noShiftNumpadNumberCharacters ??= NoShiftNumpadNumberCharacters.Keys.ToArray();
        return noShiftNumpadNumberCharacters;
    }

    /// <summary>
    /// Gets a list of all the keys that produce a symbol character when no shift modifier keys are in the down position.
    /// </summary>
    /// <returns>The keys that produce a symbol character.</returns>
    public static KeyCode[] GetSymbolKeys()
    {
        noShiftSymbolCharacters ??= NoShiftSymbolCharacters.Keys.ToArray();
        return noShiftSymbolCharacters;
    }
}
