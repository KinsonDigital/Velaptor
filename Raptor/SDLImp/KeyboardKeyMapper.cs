using Raptor.Input;
using SDLCore;
using System.Collections.Generic;
using System.Linq;
using SDLKeyCode = SDLCore.Keycode;
using RaptorKeyCode = Raptor.Input.KeyCode;

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
        public static Dictionary<SDLKeyCode, RaptorKeyCode> SDLToStandardMappings => new Dictionary<SDLKeyCode, RaptorKeyCode>()
        {
            { SDLKeyCode.Unknown, RaptorKeyCode.None },
            { SDLKeyCode.Backspace, RaptorKeyCode.Back },
            { SDLKeyCode.Tab, RaptorKeyCode.Tab },
            { SDLKeyCode.Return, RaptorKeyCode.Enter },
            { SDLKeyCode.Escape, RaptorKeyCode.Escape },
            { SDLKeyCode.Space, RaptorKeyCode.Space },
            { SDLKeyCode.SingleQuote, RaptorKeyCode.OemQuotes },
            { SDLKeyCode.Comma, RaptorKeyCode.OemComma },
            { SDLKeyCode.Minus, RaptorKeyCode.OemMinus },
            { SDLKeyCode.Period, RaptorKeyCode.OemPeriod },
            { SDLKeyCode.Slash, RaptorKeyCode.OemQuestion },
            { SDLKeyCode.Num0, RaptorKeyCode.D0 },
            { SDLKeyCode.Num1, RaptorKeyCode.D1 },
            { SDLKeyCode.Num2, RaptorKeyCode.D2 },
            { SDLKeyCode.Num3, RaptorKeyCode.D3 },
            { SDLKeyCode.Num4, RaptorKeyCode.D4 },
            { SDLKeyCode.Num5, RaptorKeyCode.D5 },
            { SDLKeyCode.Num6, RaptorKeyCode.D6 },
            { SDLKeyCode.Num7, RaptorKeyCode.D7 },
            { SDLKeyCode.Num8, RaptorKeyCode.D8 },
            { SDLKeyCode.Num9, RaptorKeyCode.D9 },
            { SDLKeyCode.Semicolon, RaptorKeyCode.OemSemicolon },
            { SDLKeyCode.Equals, RaptorKeyCode.OemPlus },
            { SDLKeyCode.LeftBracket, RaptorKeyCode.OemOpenBrackets },
            { SDLKeyCode.BackSlash, RaptorKeyCode.OemPipe },
            { SDLKeyCode.RightBracket, RaptorKeyCode.OemCloseBrackets },
            { SDLKeyCode.Backquote, RaptorKeyCode.OemTilde },
            { SDLKeyCode.a, RaptorKeyCode.A },
            { SDLKeyCode.b, RaptorKeyCode.B },
            { SDLKeyCode.c, RaptorKeyCode.C },
            { SDLKeyCode.d, RaptorKeyCode.D },
            { SDLKeyCode.e, RaptorKeyCode.E },
            { SDLKeyCode.f, RaptorKeyCode.F },
            { SDLKeyCode.g, RaptorKeyCode.G },
            { SDLKeyCode.h, RaptorKeyCode.H },
            { SDLKeyCode.i, RaptorKeyCode.I },
            { SDLKeyCode.j, RaptorKeyCode.J },
            { SDLKeyCode.k, RaptorKeyCode.K },
            { SDLKeyCode.l, RaptorKeyCode.L },
            { SDLKeyCode.m, RaptorKeyCode.M },
            { SDLKeyCode.n, RaptorKeyCode.N },
            { SDLKeyCode.o, RaptorKeyCode.O },
            { SDLKeyCode.p, RaptorKeyCode.P },
            { SDLKeyCode.q, RaptorKeyCode.Q },
            { SDLKeyCode.r, RaptorKeyCode.R },
            { SDLKeyCode.s, RaptorKeyCode.S },
            { SDLKeyCode.t, RaptorKeyCode.T },
            { SDLKeyCode.u, RaptorKeyCode.U },
            { SDLKeyCode.v, RaptorKeyCode.V },
            { SDLKeyCode.w, RaptorKeyCode.W },
            { SDLKeyCode.x, RaptorKeyCode.X },
            { SDLKeyCode.y, RaptorKeyCode.Y },
            { SDLKeyCode.z, RaptorKeyCode.Z },
            { SDLKeyCode.Delete, RaptorKeyCode.Delete },
            { SDLKeyCode.Capslock, RaptorKeyCode.CapsLock },
            { SDLKeyCode.F1, RaptorKeyCode.F1 },
            { SDLKeyCode.F2, RaptorKeyCode.F2 },
            { SDLKeyCode.F3, RaptorKeyCode.F3 },
            { SDLKeyCode.F4, RaptorKeyCode.F4 },
            { SDLKeyCode.F5, RaptorKeyCode.F5 },
            { SDLKeyCode.F6, RaptorKeyCode.F6 },
            { SDLKeyCode.F7, RaptorKeyCode.F7 },
            { SDLKeyCode.F8, RaptorKeyCode.F8 },
            { SDLKeyCode.F9, RaptorKeyCode.F9 },
            { SDLKeyCode.F10, RaptorKeyCode.F10 },
            { SDLKeyCode.F11, RaptorKeyCode.F11 },
            { SDLKeyCode.F12, RaptorKeyCode.F12 },
            { SDLKeyCode.ScrollLock, RaptorKeyCode.Scroll },
            { SDLKeyCode.Pause, RaptorKeyCode.Pause },
            { SDLKeyCode.Insert, RaptorKeyCode.Insert },
            { SDLKeyCode.Home, RaptorKeyCode.Home },
            { SDLKeyCode.PageUp, RaptorKeyCode.PageUp },
            { SDLKeyCode.End, RaptorKeyCode.End },
            { SDLKeyCode.PageDown, RaptorKeyCode.PageDown },
            { SDLKeyCode.Right, RaptorKeyCode.Right },
            { SDLKeyCode.Left, RaptorKeyCode.Left },
            { SDLKeyCode.Down, RaptorKeyCode.Down },
            { SDLKeyCode.Up, RaptorKeyCode.Up },
            { SDLKeyCode.NumLockClear, RaptorKeyCode.NumLock },
            { SDLKeyCode.KeypadDivide, RaptorKeyCode.Divide },
            { SDLKeyCode.KeypadMultiply, RaptorKeyCode.Multiply },
            { SDLKeyCode.KeypadMinus, RaptorKeyCode.Subtract },
            { SDLKeyCode.KeypadPlus, RaptorKeyCode.Add },
            { SDLKeyCode.KeypadEnter, RaptorKeyCode.Enter },
            { SDLKeyCode.Keypad1, RaptorKeyCode.NumPad1 },
            { SDLKeyCode.Keypad2, RaptorKeyCode.NumPad2 },
            { SDLKeyCode.Keypad3, RaptorKeyCode.NumPad3 },
            { SDLKeyCode.Keypad4, RaptorKeyCode.NumPad4 },
            { SDLKeyCode.Keypad5, RaptorKeyCode.NumPad5 },
            { SDLKeyCode.Keypad6, RaptorKeyCode.NumPad6 },
            { SDLKeyCode.Keypad7, RaptorKeyCode.NumPad7 },
            { SDLKeyCode.Keypad8, RaptorKeyCode.NumPad8 },
            { SDLKeyCode.Keypad9, RaptorKeyCode.NumPad9 },
            { SDLKeyCode.Keypad0, RaptorKeyCode.NumPad0 },
            { SDLKeyCode.KeypadPeriod, RaptorKeyCode.Decimal },
            { SDLKeyCode.Application, RaptorKeyCode.Apps },
            { SDLKeyCode.F13, RaptorKeyCode.F13 },
            { SDLKeyCode.F14, RaptorKeyCode.F14 },
            { SDLKeyCode.F15, RaptorKeyCode.F15 },
            { SDLKeyCode.F16, RaptorKeyCode.F16 },
            { SDLKeyCode.F17, RaptorKeyCode.F17 },
            { SDLKeyCode.F18, RaptorKeyCode.F18 },
            { SDLKeyCode.F19, RaptorKeyCode.F19 },
            { SDLKeyCode.F20, RaptorKeyCode.F20 },
            { SDLKeyCode.F21, RaptorKeyCode.F21 },
            { SDLKeyCode.F22, RaptorKeyCode.F22 },
            { SDLKeyCode.F23, RaptorKeyCode.F23 },
            { SDLKeyCode.F24, RaptorKeyCode.F24 },
            { SDLKeyCode.Execute, RaptorKeyCode.Execute },
            { SDLKeyCode.Help, RaptorKeyCode.Help },
            { SDLKeyCode.Select, RaptorKeyCode.Select },
            { SDLKeyCode.VolumeUp, RaptorKeyCode.VolumeUp },
            { SDLKeyCode.VolumeDown, RaptorKeyCode.VolumeDown },
            { SDLKeyCode.Separator, RaptorKeyCode.Separator },
            { SDLKeyCode.CRSEL, RaptorKeyCode.Crsel },
            { SDLKeyCode.EXSEL, RaptorKeyCode.Exsel },
            { SDLKeyCode.LCtrl, RaptorKeyCode.LeftControl },
            { SDLKeyCode.LShift, RaptorKeyCode.LeftShift },
            { SDLKeyCode.LAlt, RaptorKeyCode.LeftAlt },
            { SDLKeyCode.LGUI, RaptorKeyCode.LeftWindows },
            { SDLKeyCode.RCtrl, RaptorKeyCode.RightControl },
            { SDLKeyCode.RShift, RaptorKeyCode.RightShift },
            { SDLKeyCode.RAlt, RaptorKeyCode.RightAlt },
            { SDLKeyCode.RGUI, RaptorKeyCode.RightWindows },
            { SDLKeyCode.AudioNext, RaptorKeyCode.MediaNextTrack },
            { SDLKeyCode.AudioPrevious, RaptorKeyCode.MediaPreviousTrack },
            { SDLKeyCode.AudioStop, RaptorKeyCode.MediaStop },
            { SDLKeyCode.AudioPlay, RaptorKeyCode.MediaPlayPause },
            { SDLKeyCode.AudioMute, RaptorKeyCode.VolumeMute },
            { SDLKeyCode.MediaSelect, RaptorKeyCode.SelectMedia },
            { SDLKeyCode.Mail, RaptorKeyCode.LaunchMail },
            { SDLKeyCode.ACSearch, RaptorKeyCode.BrowserSearch },
            { SDLKeyCode.ACHome, RaptorKeyCode.BrowserHome },
            { SDLKeyCode.ACBack, RaptorKeyCode.BrowserBack },
            { SDLKeyCode.ACForward, RaptorKeyCode.BrowserForward },
            { SDLKeyCode.ACStop, RaptorKeyCode.BrowserStop },
            { SDLKeyCode.ACRefresh, RaptorKeyCode.BrowserRefresh },
            { SDLKeyCode.ACBookmarks, RaptorKeyCode.BrowserFavorites }
        };

        /// <summary>
        /// Holds the mappings of standard key codes to SDL key codes.
        /// </summary>
        public static Dictionary<KeyCode, Keycode> StandardToSDLMappings => new Dictionary<KeyCode, Keycode>()
        {
            { RaptorKeyCode.None, SDLKeyCode.Unknown },
            { RaptorKeyCode.Back, SDLKeyCode.Backspace},
            { RaptorKeyCode.Tab, SDLKeyCode.Tab },
            { RaptorKeyCode.Enter, SDLKeyCode.Return},
            { RaptorKeyCode.Escape, SDLKeyCode.Escape},
            { RaptorKeyCode.Space, SDLKeyCode.Space },
            { RaptorKeyCode.OemQuotes, SDLKeyCode.SingleQuote},
            { RaptorKeyCode.OemComma, SDLKeyCode.Comma },
            { RaptorKeyCode.OemMinus, SDLKeyCode.Minus },
            { RaptorKeyCode.OemPeriod, SDLKeyCode.Period},
            { RaptorKeyCode.OemQuestion, SDLKeyCode.Slash},
            { RaptorKeyCode.D0, SDLKeyCode.Num0 },
            { RaptorKeyCode.D1, SDLKeyCode.Num1 },
            { RaptorKeyCode.D2, SDLKeyCode.Num2 },
            { RaptorKeyCode.D3, SDLKeyCode.Num3 },
            { RaptorKeyCode.D4, SDLKeyCode.Num4 },
            { RaptorKeyCode.D5, SDLKeyCode.Num5 },
            { RaptorKeyCode.D6, SDLKeyCode.Num6 },
            { RaptorKeyCode.D7, SDLKeyCode.Num7 },
            { RaptorKeyCode.D8, SDLKeyCode.Num8 },
            { RaptorKeyCode.D9, SDLKeyCode.Num9 },
            { RaptorKeyCode.OemSemicolon, SDLKeyCode.Semicolon },
            { RaptorKeyCode.OemPlus, SDLKeyCode.Equals },
            { RaptorKeyCode.OemOpenBrackets, SDLKeyCode.LeftBracket },
            { RaptorKeyCode.OemPipe, SDLKeyCode.BackSlash },
            { RaptorKeyCode.OemCloseBrackets, SDLKeyCode.RightBracket},
            { RaptorKeyCode.OemTilde, SDLKeyCode.Backquote },
            { RaptorKeyCode.A, SDLKeyCode.a },
            { RaptorKeyCode.B, SDLKeyCode.b },
            { RaptorKeyCode.C, SDLKeyCode.c },
            { RaptorKeyCode.D, SDLKeyCode.d },
            { RaptorKeyCode.E, SDLKeyCode.e },
            { RaptorKeyCode.F, SDLKeyCode.f },
            { RaptorKeyCode.G, SDLKeyCode.g },
            { RaptorKeyCode.H, SDLKeyCode.h },
            { RaptorKeyCode.I, SDLKeyCode.i },
            { RaptorKeyCode.J, SDLKeyCode.j },
            { RaptorKeyCode.K, SDLKeyCode.k },
            { RaptorKeyCode.L, SDLKeyCode.l },
            { RaptorKeyCode.M, SDLKeyCode.m },
            { RaptorKeyCode.N, SDLKeyCode.n },
            { RaptorKeyCode.O, SDLKeyCode.o },
            { RaptorKeyCode.P, SDLKeyCode.p },
            { RaptorKeyCode.Q, SDLKeyCode.q },
            { RaptorKeyCode.R, SDLKeyCode.r },
            { RaptorKeyCode.S, SDLKeyCode.s },
            { RaptorKeyCode.T, SDLKeyCode.t },
            { RaptorKeyCode.U, SDLKeyCode.u },
            { RaptorKeyCode.V, SDLKeyCode.v },
            { RaptorKeyCode.W, SDLKeyCode.w },
            { RaptorKeyCode.X, SDLKeyCode.x },
            { RaptorKeyCode.Y, SDLKeyCode.y },
            { RaptorKeyCode.Z, SDLKeyCode.z },
            { RaptorKeyCode.Delete, SDLKeyCode.Delete },
            { RaptorKeyCode.CapsLock, SDLKeyCode.Capslock },
            { RaptorKeyCode.F1, SDLKeyCode.F1 },
            { RaptorKeyCode.F2, SDLKeyCode.F2 },
            { RaptorKeyCode.F3, SDLKeyCode.F3 },
            { RaptorKeyCode.F4, SDLKeyCode.F4 },
            { RaptorKeyCode.F5, SDLKeyCode.F5 },
            { RaptorKeyCode.F6, SDLKeyCode.F6 },
            { RaptorKeyCode.F7, SDLKeyCode.F7 },
            { RaptorKeyCode.F8, SDLKeyCode.F8 },
            { RaptorKeyCode.F9, SDLKeyCode.F9 },
            { RaptorKeyCode.F10, SDLKeyCode.F10 },
            { RaptorKeyCode.F11, SDLKeyCode.F11 },
            { RaptorKeyCode.F12, SDLKeyCode.F12 },
            { RaptorKeyCode.Scroll, SDLKeyCode.ScrollLock },
            { RaptorKeyCode.Pause, SDLKeyCode.Pause },
            { RaptorKeyCode.Insert, SDLKeyCode.Insert },
            { RaptorKeyCode.Home, SDLKeyCode.Home },
            { RaptorKeyCode.PageUp, SDLKeyCode.PageUp },
            { RaptorKeyCode.End, SDLKeyCode.End },
            { RaptorKeyCode.PageDown, SDLKeyCode.PageDown },
            { RaptorKeyCode.Right, SDLKeyCode.Right },
            { RaptorKeyCode.Left, SDLKeyCode.Left },
            { RaptorKeyCode.Down, SDLKeyCode.Down },
            { RaptorKeyCode.Up, SDLKeyCode.Up },
            { RaptorKeyCode.NumLock, SDLKeyCode.NumLockClear },
            { RaptorKeyCode.Divide, SDLKeyCode.KeypadDivide },
            { RaptorKeyCode.Multiply, SDLKeyCode.KeypadMultiply },
            { RaptorKeyCode.Subtract, SDLKeyCode.KeypadMinus },
            { RaptorKeyCode.Add, SDLKeyCode.KeypadPlus },
            { RaptorKeyCode.NumPad1, SDLKeyCode.Keypad1 },
            { RaptorKeyCode.NumPad2, SDLKeyCode.Keypad2 },
            { RaptorKeyCode.NumPad3, SDLKeyCode.Keypad3 },
            { RaptorKeyCode.NumPad4, SDLKeyCode.Keypad4 },
            { RaptorKeyCode.NumPad5, SDLKeyCode.Keypad5 },
            { RaptorKeyCode.NumPad6, SDLKeyCode.Keypad6 },
            { RaptorKeyCode.NumPad7, SDLKeyCode.Keypad7 },
            { RaptorKeyCode.NumPad8, SDLKeyCode.Keypad8 },
            { RaptorKeyCode.NumPad9, SDLKeyCode.Keypad9 },
            { RaptorKeyCode.NumPad0, SDLKeyCode.Keypad0 },
            { RaptorKeyCode.Decimal, SDLKeyCode.KeypadPeriod },
            { RaptorKeyCode.Apps, SDLKeyCode.Application },
            { RaptorKeyCode.F13, SDLKeyCode.F13 },
            { RaptorKeyCode.F14, SDLKeyCode.F14 },
            { RaptorKeyCode.F15, SDLKeyCode.F15 },
            { RaptorKeyCode.F16, SDLKeyCode.F16 },
            { RaptorKeyCode.F17, SDLKeyCode.F17 },
            { RaptorKeyCode.F18, SDLKeyCode.F18 },
            { RaptorKeyCode.F19, SDLKeyCode.F19 },
            { RaptorKeyCode.F20, SDLKeyCode.F20 },
            { RaptorKeyCode.F21, SDLKeyCode.F21 },
            { RaptorKeyCode.F22, SDLKeyCode.F22 },
            { RaptorKeyCode.F23, SDLKeyCode.F23 },
            { RaptorKeyCode.F24, SDLKeyCode.F24 },
            { RaptorKeyCode.Execute, SDLKeyCode.Execute },
            { RaptorKeyCode.Help, SDLKeyCode.Help },
            { RaptorKeyCode.Select, SDLKeyCode.Select },
            { RaptorKeyCode.VolumeUp, SDLKeyCode.VolumeUp },
            { RaptorKeyCode.VolumeDown, SDLKeyCode.VolumeDown },
            { RaptorKeyCode.Separator, SDLKeyCode.Separator },
            { RaptorKeyCode.Crsel, SDLKeyCode.CRSEL },
            { RaptorKeyCode.Exsel, SDLKeyCode.EXSEL },
            { RaptorKeyCode.LeftControl, SDLKeyCode.LCtrl },
            { RaptorKeyCode.LeftShift, SDLKeyCode.LShift },
            { RaptorKeyCode.LeftAlt, SDLKeyCode.LAlt },
            { RaptorKeyCode.LeftWindows, SDLKeyCode.LGUI },
            { RaptorKeyCode.RightControl, SDLKeyCode.RCtrl },
            { RaptorKeyCode.RightShift, SDLKeyCode.RShift },
            { RaptorKeyCode.RightAlt, SDLKeyCode.RAlt },
            { RaptorKeyCode.RightWindows, SDLKeyCode.RGUI },
            { RaptorKeyCode.MediaNextTrack, SDLKeyCode.AudioNext },
            { RaptorKeyCode.MediaPreviousTrack, SDLKeyCode.AudioPrevious },
            { RaptorKeyCode.MediaStop, SDLKeyCode.AudioStop },
            { RaptorKeyCode.MediaPlayPause, SDLKeyCode.AudioPlay },
            { RaptorKeyCode.VolumeMute, SDLKeyCode.AudioMute },
            { RaptorKeyCode.SelectMedia, SDLKeyCode.MediaSelect },
            { RaptorKeyCode.LaunchMail, SDLKeyCode.Mail },
            { RaptorKeyCode.BrowserSearch, SDLKeyCode.ACSearch },
            { RaptorKeyCode.BrowserHome, SDLKeyCode.ACHome },
            { RaptorKeyCode.BrowserBack, SDLKeyCode.ACBack },
            { RaptorKeyCode.BrowserForward, SDLKeyCode.ACForward },
            { RaptorKeyCode.BrowserStop, SDLKeyCode.ACStop },
            { RaptorKeyCode.BrowserRefresh, SDLKeyCode.ACRefresh },
            { RaptorKeyCode.BrowserFavorites, SDLKeyCode.ACBookmarks }
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
            //Need to recogize the SDLK_RETURN and SDLK_KP_ENTER keys as the same RaptorKeyCode.Enter key code
            if (key == SDLKeyCode.Return || key == SDLKeyCode.KeypadEnter)
                return SDLToStandardMappings[SDLKeyCode.Return];


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
                if (k == SDLKeyCode.Return || k == SDLKeyCode.KeypadEnter)
                {
                    return RaptorKeyCode.Enter;
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
