// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    /// <summary>
    /// Represents the keys on a keyboard.
    /// </summary>
    public enum KeyCode
    {
        /// <summary>
        /// An unknown key.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The left shift key.
        /// </summary>
        LeftShift = 1,

        /// <summary>
        /// The right shift key.
        /// </summary>
        RightShift = 2,

        /// <summary>
        /// The left control key.
        /// </summary>
        LeftControl = 3,

        /// <summary>
        /// The right control key.
        /// </summary>
        RightControl = 4,

        /// <summary>
        /// The left alt key.
        /// </summary>
        LeftAlt = 5,

        /// <summary>
        /// The right alt key.
        /// </summary>
        RightAlt = 6,

        /// <summary>
        /// The left windows key.
        /// </summary>
        LeftWindows = 7,

        /// <summary>
        /// The right windows key.
        /// </summary>
        RightWindows = 8,

        /// <summary>
        /// The menu key.
        /// </summary>
        Menu = 9,

        /// <summary>
        /// The command key.
        /// </summary>
        Command = 10,

        /// <summary>
        /// The F1 key.
        /// </summary>
        F1 = 11,

        /// <summary>
        /// The F2key.
        /// </summary>
        F2 = 12,

        /// <summary>
        /// The F3 key.
        /// </summary>
        F3 = 13,

        /// <summary>
        /// The F4 key.
        /// </summary>
        F4 = 14,

        /// <summary>
        /// The F5 key.
        /// </summary>
        F5 = 15,

        /// <summary>
        /// The F6 key.
        /// </summary>
        F6 = 16,

        /// <summary>
        /// The F7 key.
        /// </summary>
        F7 = 17,

        /// <summary>
        /// The F8 key.
        /// </summary>
        F8 = 18,

        /// <summary>
        /// The F9 key.
        /// </summary>
        F9 = 19,

        /// <summary>
        /// The F10 key.
        /// </summary>
        F10 = 20,

        /// <summary>
        /// The F11 key.
        /// </summary>
        F11 = 21,

        /// <summary>
        /// The F12 key.
        /// </summary>
        F12 = 22,

        /// <summary>
        /// The F13 key.
        /// </summary>
        F13 = 23,

        /// <summary>
        /// The F14 key.
        /// </summary>
        F14 = 24,

        /// <summary>
        /// The F15 key.
        /// </summary>
        F15 = 25,

        /// <summary>
        /// The F16 key.
        /// </summary>
        F16 = 26,

        /// <summary>
        /// The F17 key.
        /// </summary>
        F17 = 27,

        /// <summary>
        /// The F18 key.
        /// </summary>
        F18 = 28,

        /// <summary>
        /// The F19 key.
        /// </summary>
        F19 = 29,

        /// <summary>
        /// The F20 key.
        /// </summary>
        F20 = 30,

        /// <summary>
        /// The F21 key.
        /// </summary>
        F21 = 31,

        /// <summary>
        /// The F22 key.
        /// </summary>
        F22 = 32,

        /// <summary>
        /// The F23 key.
        /// </summary>
        F23 = 33,

        /// <summary>
        /// The F24 key.
        /// </summary>
        F24 = 34,

        /// <summary>
        /// The F25 key.
        /// </summary>
        F25 = 35,

        /// <summary>
        /// The F26 key.
        /// </summary>
        F26 = 36,

        /// <summary>
        /// The F27 key.
        /// </summary>
        F27 = 37,

        /// <summary>
        /// The F28 key.
        /// </summary>
        F28 = 38,

        /// <summary>
        /// The F29 key.
        /// </summary>
        F29 = 39,

        /// <summary>
        /// The F30 key.
        /// </summary>
        F30 = 40,

        /// <summary>
        /// The F31 key.
        /// </summary>
        F31 = 41,

        /// <summary>
        /// The F32 key.
        /// </summary>
        F32 = 42,

        /// <summary>
        /// The F33 key.
        /// </summary>
        F33 = 43,

        /// <summary>
        /// The F34 key.
        /// </summary>
        F34 = 44,

        /// <summary>
        /// The F35 key.
        /// </summary>
        F35 = 45,

        /// <summary>
        /// The up key.
        /// </summary>
        Up = 46,

        /// <summary>
        /// The down key.
        /// </summary>
        Down = 47,

        /// <summary>
        /// The left key.
        /// </summary>
        Left = 48,

        /// <summary>
        /// The right key.
        /// </summary>
        Right = 49,

        /// <summary>
        /// The enter key.
        /// </summary>
        Enter = 50,

        /// <summary>
        /// The escape key.
        /// </summary>
        Escape = 51,

        /// <summary>
        /// The space key.
        /// </summary>
        Space = 52,

        /// <summary>
        /// The tab key.
        /// </summary>
        Tab = 53,

        /// <summary>
        /// The back space key.
        /// </summary>
        BackSpace = 54,

        /// <summary>
        /// The insert key.
        /// </summary>
        Insert = 55,

        /// <summary>
        /// The delete key.
        /// </summary>
        Delete = 56,

        /// <summary>
        /// The page up key.
        /// </summary>
        PageUp = 57,

        /// <summary>
        /// The page down key.
        /// </summary>
        PageDown = 58,

        /// <summary>
        /// The home key.
        /// </summary>
        Home = 59,

        /// <summary>
        /// The end key.
        /// </summary>
        End = 60,

        /// <summary>
        /// The caps lock key.
        /// </summary>
        CapsLock = 61,

        /// <summary>
        /// The scroll lock key.
        /// </summary>
        ScrollLock = 62,

        /// <summary>
        /// The print screen key.
        /// </summary>
        PrintScreen = 63,

        /// <summary>
        /// The pause key.
        /// </summary>
        Pause = 64,

        /// <summary>
        /// The number lock key.
        /// </summary>
        NumLock = 65,

        /// <summary>
        /// The clear key.
        /// </summary>
        Clear = 66,

        /// <summary>
        /// The sleep key.
        /// </summary>
        Sleep = 67,

        /// <summary>
        /// The numpad 0 key.
        /// </summary>
        Numpad0 = 68,

        /// <summary>
        /// The numpad 1 key.
        /// </summary>
        Numpad1 = 69,

        /// <summary>
        /// The numpad 2 key.
        /// </summary>
        Numpad2 = 70,

        /// <summary>
        /// The numpad 3 key.
        /// </summary>
        Numpad3 = 71,

        /// <summary>
        /// The numpad 4 key.
        /// </summary>
        Numpad4 = 72,

        /// <summary>
        /// The numpad 5 key.
        /// </summary>
        Numpad5 = 73,

        /// <summary>
        /// The numpad 6 key.
        /// </summary>
        Numpad6 = 74,

        /// <summary>
        /// The numpad 7 key.
        /// </summary>
        Numpad7 = 75,

        /// <summary>
        /// The numpad 8 key.
        /// </summary>
        Numpad8 = 76,

        /// <summary>
        /// The numpad 9 key.
        /// </summary>
        Numpad9 = 77,

        /// <summary>
        /// The numpad divide key.
        /// </summary>
        NumpadDivide = 78,

        /// <summary>
        /// The numpad multiply key.
        /// </summary>
        NumpadMultiply = 79,

        /// <summary>
        /// The numpad minus key.
        /// </summary>
        NumpadMinus = 80,

        /// <summary>
        /// The numpad plus key.
        /// </summary>
        NumpadPlus = 81,

        /// <summary>
        /// The numpad decimal key.
        /// </summary>
        NumpadDecimal = 82,

        /// <summary>
        /// The numpad enter key.
        /// </summary>
        NumpadEnter = 83,

        /// <summary>
        /// The letter A key.
        /// </summary>
        A = 84,

        /// <summary>
        /// The letter B key.
        /// </summary>
        B = 85,

        /// <summary>
        /// The letter C key.
        /// </summary>
        C = 86,

        /// <summary>
        /// The letter D key.
        /// </summary>
        D = 87,

        /// <summary>
        /// The letter E key.
        /// </summary>
        E = 88,

        /// <summary>
        /// The letter F key.
        /// </summary>
        F = 89,

        /// <summary>
        /// The letter G key.
        /// </summary>
        G = 90,

        /// <summary>
        /// The letter H key.
        /// </summary>
        H = 91,

        /// <summary>
        /// The letter I key.
        /// </summary>
        I = 92,

        /// <summary>
        /// The letter J key.
        /// </summary>
        J = 93,

        /// <summary>
        /// The letter K key.
        /// </summary>
        K = 94,

        /// <summary>
        /// The letter L key.
        /// </summary>
        L = 95,

        /// <summary>
        /// The letter M key.
        /// </summary>
        M = 96,

        /// <summary>
        /// The letter N key.
        /// </summary>
        N = 97,

        /// <summary>
        /// The letter O key.
        /// </summary>
        O = 98,

        /// <summary>
        /// The letter P key.
        /// </summary>
        P = 99,

        /// <summary>
        /// The letter Q key.
        /// </summary>
        Q = 100,

        /// <summary>
        /// The letter R key.
        /// </summary>
        R = 101,

        /// <summary>
        /// The letter S key.
        /// </summary>
        S = 102,

        /// <summary>
        /// The letter T key.
        /// </summary>
        T = 103,

        /// <summary>
        /// The letter U key.
        /// </summary>
        U = 104,

        /// <summary>
        /// The letter V key.
        /// </summary>
        V = 105,

        /// <summary>
        /// The letter W key.
        /// </summary>
        W = 106,

        /// <summary>
        /// The letter X key.
        /// </summary>
        X = 107,

        /// <summary>
        /// The letter Y key.
        /// </summary>
        Y = 108,

        /// <summary>
        /// The letter Z key.
        /// </summary>
        Z = 109,

        /// <summary>
        /// The number 0 key.
        /// </summary>
        Number0 = 110,

        /// <summary>
        /// The number 1 key.
        /// </summary>
        Number1 = 111,

        /// <summary>
        /// The number 2 key.
        /// </summary>
        Number2 = 112,

        /// <summary>
        /// The number 3 key.
        /// </summary>
        Number3 = 113,

        /// <summary>
        /// The number 4key.
        /// </summary>
        Number4 = 114,

        /// <summary>
        /// The number 5 key.
        /// </summary>
        Number5 = 115,

        /// <summary>
        /// The number 6 key.
        /// </summary>
        Number6 = 116,

        /// <summary>
        /// The number 7 key.
        /// </summary>
        Number7 = 117,

        /// <summary>
        /// The number 8 key.
        /// </summary>
        Number8 = 118,

        /// <summary>
        /// The number 9 key.
        /// </summary>
        Number9 = 119,

        /// <summary>
        /// The tilde key.
        /// </summary>
        Tilde = 120,

        /// <summary>
        /// The minus key.
        /// </summary>
        Minus = 121,

        /// <summary>
        /// The plus key.
        /// </summary>
        Plus = 122,

        /// <summary>
        /// The left bracket key.
        /// </summary>
        LeftBracket = 123,

        /// <summary>
        /// The right bracket key.
        /// </summary>
        RightBracket = 124,

        /// <summary>
        /// The semi-colon key.
        /// </summary>
        Semicolon = 125,

        /// <summary>
        /// The quote key.
        /// </summary>
        Quote = 126,

        /// <summary>
        /// The comma key.
        /// </summary>
        Comma = 127,

        /// <summary>
        /// The period key.
        /// </summary>
        Period = 128,

        /// <summary>
        /// The forward slash key.
        /// </summary>
        ForwardSlash = 129,

        /// <summary>
        /// The back slash key.
        /// </summary>
        BackSlash = 130,

        /// <summary>
        /// The non US back slash key.
        /// </summary>
        NonUSBackSlash = 131,
    }

    /// <summary>
    /// Represents the buttons on a mouse.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// Represents no mouse input at all.
        /// </summary>
        None = -1,

        /// <summary>
        /// Represents the left mouse button.
        /// </summary>
        LeftButton = 0,

        /// <summary>
        /// Represents the right mouse button.
        /// </summary>
        RightButton = 1,

        /// <summary>
        /// Represents the middle mouse button.
        /// </summary>
        MiddleButton = 2,
    }
}
