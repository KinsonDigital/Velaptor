// <copyright file="KeyboardKeyGroups.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds groups of different keyboard keys.
/// </summary>
internal readonly record struct KeyboardKeyGroups
{
    /// <summary>
    /// Gets a character that represents an invalid character.
    /// </summary>
    public static char InvalidCharacter => '□';

    /// <summary>
    /// Gets a list of all letter keys including the space.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyList<KeyCode> LetterKeys { get; } = new[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
        KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
        KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
        KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
        KeyCode.Z, KeyCode.Space,
    };

    /// <summary>
    /// Gets a list of the standard number keys above the letter keys.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyList<KeyCode> StandardNumberKeys { get; } = new[]
    {
        KeyCode.D0, KeyCode.D1, KeyCode.D2,
        KeyCode.D3, KeyCode.D4, KeyCode.D5,
        KeyCode.D6, KeyCode.D7, KeyCode.D8,
        KeyCode.D9,
    };

    /// <summary>
    /// Gets a list of number pad number keys.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyList<KeyCode> NumpadNumberKeys { get; } = new[]
    {
        KeyCode.KeyPad0, KeyCode.KeyPad1, KeyCode.KeyPad2,
        KeyCode.KeyPad3, KeyCode.KeyPad4, KeyCode.KeyPad5,
        KeyCode.KeyPad6, KeyCode.KeyPad7, KeyCode.KeyPad8,
        KeyCode.KeyPad9,
    };

    /// <summary>
    /// Gets a list of symbol keys.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyList<KeyCode> SymbolKeys { get; } = new[]
    {
        KeyCode.Semicolon, KeyCode.Equal, KeyCode.Comma, KeyCode.Minus, KeyCode.Period, KeyCode.Slash,
        KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Apostrophe, KeyCode.KeyPadDivide, KeyCode.KeyPadMultiply,
        KeyCode.KeyPadSubtract, KeyCode.KeyPadAdd, KeyCode.KeyPadDecimal, KeyCode.Backslash, KeyCode.GraveAccent,
    };

    /// <summary>
    /// Gets a list of the modifier keys.
    /// </summary>
    public static readonly IReadOnlyList<KeyCode> ModifierKeys = new[]
    {
        KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift, KeyCode.RightShift,
        KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.CapsLock,
    };

    /// <summary>
    /// Gets a list of the cursor movement keys.
    /// </summary>
    public static readonly IReadOnlyList<KeyCode> CursorMovementKeys = new[]
    {
        KeyCode.Left, KeyCode.Right, KeyCode.Up, KeyCode.Down, KeyCode.Home,
        KeyCode.End, KeyCode.Tab, KeyCode.PageUp, KeyCode.PageDown,
    };

    /// <summary>
    /// Gets a list of the function keys.
    /// </summary>
    public static readonly IReadOnlyList<KeyCode> FunctionKeys = new[]
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
    public static IReadOnlyDictionary<KeyCode, char> NoShiftLetterKeys { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.A, 'a' }, { KeyCode.B, 'b' }, { KeyCode.C, 'c' }, { KeyCode.D, 'd' }, { KeyCode.E, 'e' },
        { KeyCode.F, 'f' }, { KeyCode.G, 'g' }, { KeyCode.H, 'h' }, { KeyCode.I, 'i' }, { KeyCode.J, 'j' },
        { KeyCode.K, 'k' }, { KeyCode.L, 'l' }, { KeyCode.M, 'm' }, { KeyCode.N, 'n' }, { KeyCode.O, 'o' },
        { KeyCode.P, 'p' }, { KeyCode.Q, 'q' }, { KeyCode.R, 'r' }, { KeyCode.S, 's' }, { KeyCode.T, 't' },
        { KeyCode.U, 'u' }, { KeyCode.V, 'v' }, { KeyCode.W, 'w' }, { KeyCode.X, 'x' }, { KeyCode.Y, 'y' },
        { KeyCode.Z, 'z' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// letter keys when any of the shift modifier keys are in the down position.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyDictionary<KeyCode, char> WithShiftLetterKeys { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.A, 'A' }, { KeyCode.B, 'B' }, { KeyCode.C, 'C' }, { KeyCode.D, 'D' }, { KeyCode.E, 'E' },
        { KeyCode.F, 'F' }, { KeyCode.G, 'G' }, { KeyCode.H, 'H' }, { KeyCode.I, 'I' }, { KeyCode.J, 'J' },
        { KeyCode.K, 'K' }, { KeyCode.L, 'L' }, { KeyCode.M, 'M' }, { KeyCode.N, 'N' }, { KeyCode.O, 'O' },
        { KeyCode.P, 'P' }, { KeyCode.Q, 'Q' }, { KeyCode.R, 'R' }, { KeyCode.S, 'S' }, { KeyCode.T, 'T' },
        { KeyCode.U, 'U' }, { KeyCode.V, 'V' }, { KeyCode.W, 'W' }, { KeyCode.X, 'X' }, { KeyCode.Y, 'Y' },
        { KeyCode.Z, 'Z' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// standard number keys when no shift modifier keys are in the down position.
    /// </summary>
    public static readonly IReadOnlyDictionary<KeyCode, char> NoShiftStandardNumberCharacters = new Dictionary<KeyCode, char>
    {
        { KeyCode.D0, '0' }, { KeyCode.D1, '1' }, { KeyCode.D2, '2' },
        { KeyCode.D3, '3' }, { KeyCode.D4, '4' }, { KeyCode.D5, '5' },
        { KeyCode.D6, '6' }, { KeyCode.D7, '7' }, { KeyCode.D8, '8' }, { KeyCode.D9, '9' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// standard number keys when no shift modifier keys are in the down position.
    /// </summary>
    public static readonly IReadOnlyDictionary<KeyCode, char> WithShiftStandardNumberCharacters = new Dictionary<KeyCode, char>
    {
        { KeyCode.D1, '!' }, { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
        { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' }, { KeyCode.D0, ')' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by the
    /// keyboard when no shift modifier keys are in the down position.
    /// </summary>
    public static readonly IReadOnlyDictionary<KeyCode, char> NoShiftSymbolCharacters = new Dictionary<KeyCode, char>
    {
        { KeyCode.Equal, '=' }, { KeyCode.Comma, ',' }, { KeyCode.Minus, '-' }, { KeyCode.Period, '.' }, { KeyCode.Slash, '/' },
        { KeyCode.Backslash, '\\' }, { KeyCode.LeftBracket, '[' }, { KeyCode.RightBracket, ']' },
        { KeyCode.Apostrophe, '\'' }, { KeyCode.Semicolon, ';' }, { KeyCode.KeyPadAdd, '+' }, { KeyCode.KeyPadDecimal, '.' },
        { KeyCode.KeyPadDivide, '/' }, { KeyCode.KeyPadMultiply, '*' }, { KeyCode.KeyPadSubtract, '-' },
        { KeyCode.GraveAccent, '`' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/>produced by
    /// the keyboard when any shift modifier keys are in the down position.
    /// </summary>
    public static readonly IReadOnlyDictionary<KeyCode, char> WithShiftSymbolCharacters = new Dictionary<KeyCode, char>
    {
        { KeyCode.Equal, '+' }, { KeyCode.Comma, '<' }, { KeyCode.Minus, '_' }, { KeyCode.Period, '>' }, { KeyCode.Slash, '?' },
        { KeyCode.Backslash, '|' }, { KeyCode.LeftBracket, '{' }, { KeyCode.RightBracket, '}' },
        { KeyCode.Apostrophe, '"' }, { KeyCode.Semicolon, ':' }, { KeyCode.D1, '!' },
        { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
        { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' },
        { KeyCode.D0, ')' }, { KeyCode.KeyPadDivide, '/' }, { KeyCode.KeyPadMultiply, '*' }, { KeyCode.KeyPadSubtract, '-' },
        { KeyCode.KeyPadAdd, '+' }, { KeyCode.KeyPadDecimal, '.' }, { KeyCode.GraveAccent, '~' },
    };

    /// <summary>
    /// Gets the characters associated with the correct <see cref="KeyCode"/> produced by by
    /// the keyboard when no shift modifier keys are in the down position.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not worth testing")]
    public static IReadOnlyDictionary<KeyCode, char> NoShiftNumpadNumberCharacters { get; } = new Dictionary<KeyCode, char>
    {
        { KeyCode.KeyPad0, '0' }, { KeyCode.KeyPad1, '1' }, { KeyCode.KeyPad2, '2' }, { KeyCode.KeyPad3, '3' }, { KeyCode.KeyPad4, '4' },
        { KeyCode.KeyPad5, '5' }, { KeyCode.KeyPad6, '6' }, { KeyCode.KeyPad7, '7' }, { KeyCode.KeyPad8, '8' }, { KeyCode.KeyPad9, '9' },
    };
}
