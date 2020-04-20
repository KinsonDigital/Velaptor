using Raptor.Input;
using SDLCore;
using System.Collections.Generic;
using System.Linq;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Holds keyboard keycode mappings for SDL and standard key codes.
    /// </summary>
    public static class KeyboardKeyMapper
    {
        #region Props
        /// <summary>
        /// Holds the mappings of SDL key codes to standard key codes.
        /// </summary>
        public static Dictionary<Keycode, KeyCode> SDLToStandardMappings => new Dictionary<Keycode, KeyCode>()
        {
            { Keycode.Unknown, KeyCodes.None },
            { Keycode.Backspace, KeyCodes.Back },
            { Keycode.Tab, KeyCodes.Tab },
            { Keycode.Return, KeyCodes.Enter },
            { Keycode.Escape, KeyCodes.Escape },
            { Keycode.Space, KeyCodes.Space },
            { Keycode.SingleQuote, KeyCodes.OemQuotes },
            { Keycode.Comma, KeyCodes.OemComma },
            { Keycode.Minus, KeyCodes.OemMinus },
            { Keycode.Period, KeyCodes.OemPeriod },
            { Keycode.Slash, KeyCodes.OemQuestion },
            { Keycode.Num0, KeyCodes.D0 },
            { Keycode.Num1, KeyCodes.D1 },
            { Keycode.Num2, KeyCodes.D2 },
            { Keycode.Num3, KeyCodes.D3 },
            { Keycode.Num4, KeyCodes.D4 },
            { Keycode.Num5, KeyCodes.D5 },
            { Keycode.Num6, KeyCodes.D6 },
            { Keycode.Num7, KeyCodes.D7 },
            { Keycode.Num8, KeyCodes.D8 },
            { Keycode.Num9, KeyCodes.D9 },
            { Keycode.Semicolon, KeyCodes.OemSemicolon },
            { Keycode.Equals, KeyCodes.OemPlus },
            { Keycode.LeftBracket, KeyCodes.OemOpenBrackets },
            { Keycode.BackSlash, KeyCodes.OemPipe },
            { Keycode.RightBracket, KeyCodes.OemCloseBrackets },
            { Keycode.Backquote, KeyCodes.OemTilde },
            { Keycode.a, KeyCodes.A },
            { Keycode.b, KeyCodes.B },
            { Keycode.c, KeyCodes.C },
            { Keycode.d, KeyCodes.D },
            { Keycode.e, KeyCodes.E },
            { Keycode.f, KeyCodes.F },
            { Keycode.g, KeyCodes.G },
            { Keycode.h, KeyCodes.H },
            { Keycode.i, KeyCodes.I },
            { Keycode.j, KeyCodes.J },
            { Keycode.k, KeyCodes.K },
            { Keycode.l, KeyCodes.L },
            { Keycode.m, KeyCodes.M },
            { Keycode.n, KeyCodes.N },
            { Keycode.o, KeyCodes.O },
            { Keycode.p, KeyCodes.P },
            { Keycode.q, KeyCodes.Q },
            { Keycode.r, KeyCodes.R },
            { Keycode.s, KeyCodes.S },
            { Keycode.t, KeyCodes.T },
            { Keycode.u, KeyCodes.U },
            { Keycode.v, KeyCodes.V },
            { Keycode.w, KeyCodes.W },
            { Keycode.x, KeyCodes.X },
            { Keycode.y, KeyCodes.Y },
            { Keycode.z, KeyCodes.Z },
            { Keycode.Delete, KeyCodes.Delete },
            { Keycode.Capslock, KeyCodes.CapsLock },
            { Keycode.F1, KeyCodes.F1 },
            { Keycode.F2, KeyCodes.F2 },
            { Keycode.F3, KeyCodes.F3 },
            { Keycode.F4, KeyCodes.F4 },
            { Keycode.F5, KeyCodes.F5 },
            { Keycode.F6, KeyCodes.F6 },
            { Keycode.F7, KeyCodes.F7 },
            { Keycode.F8, KeyCodes.F8 },
            { Keycode.F9, KeyCodes.F9 },
            { Keycode.F10, KeyCodes.F10 },
            { Keycode.F11, KeyCodes.F11 },
            { Keycode.F12, KeyCodes.F12 },
            { Keycode.ScrollLock, KeyCodes.Scroll },
            { Keycode.Pause, KeyCodes.Pause },
            { Keycode.Insert, KeyCodes.Insert },
            { Keycode.Home, KeyCodes.Home },
            { Keycode.PageUp, KeyCodes.PageUp },
            { Keycode.End, KeyCodes.End },
            { Keycode.PageDown, KeyCodes.PageDown },
            { Keycode.Right, KeyCodes.Right },
            { Keycode.Left, KeyCodes.Left },
            { Keycode.Down, KeyCodes.Down },
            { Keycode.Up, KeyCodes.Up },
            { Keycode.NumLockClear, KeyCodes.NumLock },
            { Keycode.KeypadDivide, KeyCodes.Divide },
            { Keycode.KeypadMultiply, KeyCodes.Multiply },
            { Keycode.KeypadMinus, KeyCodes.Subtract },
            { Keycode.KeypadPlus, KeyCodes.Add },
            { Keycode.KeypadEnter, KeyCodes.Enter },
            { Keycode.Keypad1, KeyCodes.NumPad1 },
            { Keycode.Keypad2, KeyCodes.NumPad2 },
            { Keycode.Keypad3, KeyCodes.NumPad3 },
            { Keycode.Keypad4, KeyCodes.NumPad4 },
            { Keycode.Keypad5, KeyCodes.NumPad5 },
            { Keycode.Keypad6, KeyCodes.NumPad6 },
            { Keycode.Keypad7, KeyCodes.NumPad7 },
            { Keycode.Keypad8, KeyCodes.NumPad8 },
            { Keycode.Keypad9, KeyCodes.NumPad9 },
            { Keycode.Keypad0, KeyCodes.NumPad0 },
            { Keycode.KeypadPeriod, KeyCodes.Decimal },
            { Keycode.Application, KeyCodes.Apps },
            { Keycode.F13, KeyCodes.F13 },
            { Keycode.F14, KeyCodes.F14 },
            { Keycode.F15, KeyCodes.F15 },
            { Keycode.F16, KeyCodes.F16 },
            { Keycode.F17, KeyCodes.F17 },
            { Keycode.F18, KeyCodes.F18 },
            { Keycode.F19, KeyCodes.F19 },
            { Keycode.F20, KeyCodes.F20 },
            { Keycode.F21, KeyCodes.F21 },
            { Keycode.F22, KeyCodes.F22 },
            { Keycode.F23, KeyCodes.F23 },
            { Keycode.F24, KeyCodes.F24 },
            { Keycode.Execute, KeyCodes.Execute },
            { Keycode.Help, KeyCodes.Help },
            { Keycode.Select, KeyCodes.Select },
            { Keycode.VolumeUp, KeyCodes.VolumeUp },
            { Keycode.VolumeDown, KeyCodes.VolumeDown },
            { Keycode.Separator, KeyCodes.Separator },
            { Keycode.CRSEL, KeyCodes.Crsel },
            { Keycode.EXSEL, KeyCodes.Exsel },
            { Keycode.LCtrl, KeyCodes.LeftControl },
            { Keycode.LShift, KeyCodes.LeftShift },
            { Keycode.LAlt, KeyCodes.LeftAlt },
            { Keycode.LGUI, KeyCodes.LeftWindows },
            { Keycode.RCtrl, KeyCodes.RightControl },
            { Keycode.RShift, KeyCodes.RightShift },
            { Keycode.RAlt, KeyCodes.RightAlt },
            { Keycode.RGUI, KeyCodes.RightWindows },
            { Keycode.AudioNext, KeyCodes.MediaNextTrack },
            { Keycode.AudioPrevious, KeyCodes.MediaPreviousTrack },
            { Keycode.AudioStop, KeyCodes.MediaStop },
            { Keycode.AudioPlay, KeyCodes.MediaPlayPause },
            { Keycode.AudioMute, KeyCodes.VolumeMute },
            { Keycode.MediaSelect, KeyCodes.SelectMedia },
            { Keycode.Mail, KeyCodes.LaunchMail },
            { Keycode.ACSearch, KeyCodes.BrowserSearch },
            { Keycode.ACHome, KeyCodes.BrowserHome },
            { Keycode.ACBack, KeyCodes.BrowserBack },
            { Keycode.ACForward, KeyCodes.BrowserForward },
            { Keycode.ACStop, KeyCodes.BrowserStop },
            { Keycode.ACRefresh, KeyCodes.BrowserRefresh },
            { Keycode.ACBookmarks, KeyCodes.BrowserFavorites }
        };

        /// <summary>
        /// Holds the mappings of standard key codes to SDL key codes.
        /// </summary>
        public static Dictionary<KeyCode, Keycode> StandardToSDLMappings => new Dictionary<KeyCode, Keycode>()
        {
            { KeyCodes.None, Keycode.Unknown },
            { KeyCodes.Back, Keycode.Backspace},
            { KeyCodes.Tab, Keycode.Tab },
            { KeyCodes.Enter, Keycode.Return},
            { KeyCodes.Escape, Keycode.Escape},
            { KeyCodes.Space, Keycode.Space },
            { KeyCodes.OemQuotes, Keycode.SingleQuote},
            { KeyCodes.OemComma, Keycode.Comma },
            { KeyCodes.OemMinus, Keycode.Minus },
            { KeyCodes.OemPeriod, Keycode.Period},
            { KeyCodes.OemQuestion, Keycode.Slash},
            { KeyCodes.D0, Keycode.Num0 },
            { KeyCodes.D1, Keycode.Num1 },
            { KeyCodes.D2, Keycode.Num2 },
            { KeyCodes.D3, Keycode.Num3 },
            { KeyCodes.D4, Keycode.Num4 },
            { KeyCodes.D5, Keycode.Num5 },
            { KeyCodes.D6, Keycode.Num6 },
            { KeyCodes.D7, Keycode.Num7 },
            { KeyCodes.D8, Keycode.Num8 },
            { KeyCodes.D9, Keycode.Num9 },
            { KeyCodes.OemSemicolon, Keycode.Semicolon },
            { KeyCodes.OemPlus, Keycode.Equals },
            { KeyCodes.OemOpenBrackets, Keycode.LeftBracket },
            { KeyCodes.OemPipe, Keycode.BackSlash },
            { KeyCodes.OemCloseBrackets, Keycode.RightBracket},
            { KeyCodes.OemTilde, Keycode.Backquote },
            { KeyCodes.A, Keycode.a },
            { KeyCodes.B, Keycode.b },
            { KeyCodes.C, Keycode.c },
            { KeyCodes.D, Keycode.d },
            { KeyCodes.E, Keycode.e },
            { KeyCodes.F, Keycode.f },
            { KeyCodes.G, Keycode.g },
            { KeyCodes.H, Keycode.h },
            { KeyCodes.I, Keycode.i },
            { KeyCodes.J, Keycode.j },
            { KeyCodes.K, Keycode.k },
            { KeyCodes.L, Keycode.l },
            { KeyCodes.M, Keycode.m },
            { KeyCodes.N, Keycode.n },
            { KeyCodes.O, Keycode.o },
            { KeyCodes.P, Keycode.p },
            { KeyCodes.Q, Keycode.q },
            { KeyCodes.R, Keycode.r },
            { KeyCodes.S, Keycode.s },
            { KeyCodes.T, Keycode.t },
            { KeyCodes.U, Keycode.u },
            { KeyCodes.V, Keycode.v },
            { KeyCodes.W, Keycode.w },
            { KeyCodes.X, Keycode.x },
            { KeyCodes.Y, Keycode.y },
            { KeyCodes.Z, Keycode.z },
            { KeyCodes.Delete, Keycode.Delete },
            { KeyCodes.CapsLock, Keycode.Capslock },
            { KeyCodes.F1, Keycode.F1 },
            { KeyCodes.F2, Keycode.F2 },
            { KeyCodes.F3, Keycode.F3 },
            { KeyCodes.F4, Keycode.F4 },
            { KeyCodes.F5, Keycode.F5 },
            { KeyCodes.F6, Keycode.F6 },
            { KeyCodes.F7, Keycode.F7 },
            { KeyCodes.F8, Keycode.F8 },
            { KeyCodes.F9, Keycode.F9 },
            { KeyCodes.F10, Keycode.F10 },
            { KeyCodes.F11, Keycode.F11 },
            { KeyCodes.F12, Keycode.F12 },
            { KeyCodes.Scroll, Keycode.ScrollLock },
            { KeyCodes.Pause, Keycode.Pause },
            { KeyCodes.Insert, Keycode.Insert },
            { KeyCodes.Home, Keycode.Home },
            { KeyCodes.PageUp, Keycode.PageUp },
            { KeyCodes.End, Keycode.End },
            { KeyCodes.PageDown, Keycode.PageDown },
            { KeyCodes.Right, Keycode.Right },
            { KeyCodes.Left, Keycode.Left },
            { KeyCodes.Down, Keycode.Down },
            { KeyCodes.Up, Keycode.Up },
            { KeyCodes.NumLock, Keycode.NumLockClear },
            { KeyCodes.Divide, Keycode.KeypadDivide },
            { KeyCodes.Multiply, Keycode.KeypadMultiply },
            { KeyCodes.Subtract, Keycode.KeypadMinus },
            { KeyCodes.Add, Keycode.KeypadPlus },
            { KeyCodes.NumPad1, Keycode.Keypad1 },
            { KeyCodes.NumPad2, Keycode.Keypad2 },
            { KeyCodes.NumPad3, Keycode.Keypad3 },
            { KeyCodes.NumPad4, Keycode.Keypad4 },
            { KeyCodes.NumPad5, Keycode.Keypad5 },
            { KeyCodes.NumPad6, Keycode.Keypad6 },
            { KeyCodes.NumPad7, Keycode.Keypad7 },
            { KeyCodes.NumPad8, Keycode.Keypad8 },
            { KeyCodes.NumPad9, Keycode.Keypad9 },
            { KeyCodes.NumPad0, Keycode.Keypad0 },
            { KeyCodes.Decimal, Keycode.KeypadPeriod },
            { KeyCodes.Apps, Keycode.Application },
            { KeyCodes.F13, Keycode.F13 },
            { KeyCodes.F14, Keycode.F14 },
            { KeyCodes.F15, Keycode.F15 },
            { KeyCodes.F16, Keycode.F16 },
            { KeyCodes.F17, Keycode.F17 },
            { KeyCodes.F18, Keycode.F18 },
            { KeyCodes.F19, Keycode.F19 },
            { KeyCodes.F20, Keycode.F20 },
            { KeyCodes.F21, Keycode.F21 },
            { KeyCodes.F22, Keycode.F22 },
            { KeyCodes.F23, Keycode.F23 },
            { KeyCodes.F24, Keycode.F24 },
            { KeyCodes.Execute, Keycode.Execute },
            { KeyCodes.Help, Keycode.Help },
            { KeyCodes.Select, Keycode.Select },
            { KeyCodes.VolumeUp, Keycode.VolumeUp },
            { KeyCodes.VolumeDown, Keycode.VolumeDown },
            { KeyCodes.Separator, Keycode.Separator },
            { KeyCodes.Crsel, Keycode.CRSEL },
            { KeyCodes.Exsel, Keycode.EXSEL },
            { KeyCodes.LeftControl, Keycode.LCtrl },
            { KeyCodes.LeftShift, Keycode.LShift },
            { KeyCodes.LeftAlt, Keycode.LAlt },
            { KeyCodes.LeftWindows, Keycode.LGUI },
            { KeyCodes.RightControl, Keycode.RCtrl },
            { KeyCodes.RightShift, Keycode.RShift },
            { KeyCodes.RightAlt, Keycode.RAlt },
            { KeyCodes.RightWindows, Keycode.RGUI },
            { KeyCodes.MediaNextTrack, Keycode.AudioNext },
            { KeyCodes.MediaPreviousTrack, Keycode.AudioPrevious },
            { KeyCodes.MediaStop, Keycode.AudioStop },
            { KeyCodes.MediaPlayPause, Keycode.AudioPlay },
            { KeyCodes.VolumeMute, Keycode.AudioMute },
            { KeyCodes.SelectMedia, Keycode.MediaSelect },
            { KeyCodes.LaunchMail, Keycode.Mail },
            { KeyCodes.BrowserSearch, Keycode.ACSearch },
            { KeyCodes.BrowserHome, Keycode.ACHome },
            { KeyCodes.BrowserBack, Keycode.ACBack },
            { KeyCodes.BrowserForward, Keycode.ACForward },
            { KeyCodes.BrowserStop, Keycode.ACStop },
            { KeyCodes.BrowserRefresh, Keycode.ACRefresh },
            { KeyCodes.BrowserFavorites, Keycode.ACBookmarks }
        };
        #endregion


        #region Public Methods
        /// <summary>
        /// Converts the given <paramref name="key"/> of type <see cref="SDL.SDL_Keycode"/> to 
        /// the keycode of type <see cref="KeyCode"/>.
        /// </summary>
        /// <param name="key">The SDL key code used to map to the standard key code.</param>
        /// <returns></returns>
        public static KeyCode ToStandardKeyCode(Keycode key)
        {
            //Need to recogize the SDLK_RETURN and SDLK_KP_ENTER keys as the same KeyCode.Enter key code
            if (key == Keycode.Return || key == Keycode.KeypadEnter)
                return SDLToStandardMappings[Keycode.Return];


            return SDLToStandardMappings[key];
        }


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="Keycode"/> to 
        /// the keycode of type <see cref="KeyCode"/>.
        /// </summary>
        /// <param name="key">The SDL key codes used to map to the standard key codes.</param>
        /// <returns></returns>
        public static KeyCode[] ToStandardKeyCodes(Keycode[] keys)
        {
            return keys.Select(k =>
            {
                if (k == Keycode.Return || k == Keycode.KeypadEnter)
                {
                    return KeyCode.Enter;
                }
                else
                {
                    return ToStandardKeyCode(k);
                }
            }).ToArray();
        }


        /// <summary>
        /// Converts the given <paramref name="standardKeyCode"/> of type <see cref="KeyCode"/> to
        /// the keycode of type <see cref="Keycode"/>.
        /// </summary>
        /// <param name="standardKeyCode">The standard key code used to map to the SDL key code.</param>
        /// <returns></returns>
        public static Keycode ToSDLKeyCode(KeyCode standardKeyCode) => StandardToSDLMappings[standardKeyCode];


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="KeyCode"/> to
        /// the keycode of type <see cref="Keycode"/>.
        /// </summary>
        /// <param name="key">The standard key codes used to map to the SDL key code.</param>
        /// <returns></returns>
        public static Keycode[] ToSDLKeyCodes(KeyCode[] keys) => (from k in keys select ToSDLKeyCode(k)).ToArray();
        #endregion
    }
}
