using RaptorCore.Input;
using SDLCore;
using System.Collections.Generic;
using System.Linq;

namespace SDLImp
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
        public static Dictionary<Keycode, KeyCodes> SDLToStandardMappings => new Dictionary<Keycode, KeyCodes>()
        {
            { Keycode.SDLK_UNKNOWN, KeyCodes.None },
            { Keycode.SDLK_BACKSPACE, KeyCodes.Back },
            { Keycode.SDLK_TAB, KeyCodes.Tab },
            { Keycode.SDLK_RETURN, KeyCodes.Enter },
            { Keycode.SDLK_ESCAPE, KeyCodes.Escape },
            { Keycode.SDLK_SPACE, KeyCodes.Space },
            { Keycode.SDLK_QUOTE, KeyCodes.OemQuotes },
            { Keycode.SDLK_COMMA, KeyCodes.OemComma },
            { Keycode.SDLK_MINUS, KeyCodes.OemMinus },
            { Keycode.SDLK_PERIOD, KeyCodes.OemPeriod },
            { Keycode.SDLK_SLASH, KeyCodes.OemQuestion },
            { Keycode.SDLK_0, KeyCodes.D0 },
            { Keycode.SDLK_1, KeyCodes.D1 },
            { Keycode.SDLK_2, KeyCodes.D2 },
            { Keycode.SDLK_3, KeyCodes.D3 },
            { Keycode.SDLK_4, KeyCodes.D4 },
            { Keycode.SDLK_5, KeyCodes.D5 },
            { Keycode.SDLK_6, KeyCodes.D6 },
            { Keycode.SDLK_7, KeyCodes.D7 },
            { Keycode.SDLK_8, KeyCodes.D8 },
            { Keycode.SDLK_9, KeyCodes.D9 },
            { Keycode.SDLK_SEMICOLON, KeyCodes.OemSemicolon },
            { Keycode.SDLK_EQUALS, KeyCodes.OemPlus },
            { Keycode.SDLK_LEFTBRACKET, KeyCodes.OemOpenBrackets },
            { Keycode.SDLK_BACKSLASH, KeyCodes.OemPipe },
            { Keycode.SDLK_RIGHTBRACKET, KeyCodes.OemCloseBrackets },
            { Keycode.SDLK_BACKQUOTE, KeyCodes.OemTilde },
            { Keycode.SDLK_a, KeyCodes.A },
            { Keycode.SDLK_b, KeyCodes.B },
            { Keycode.SDLK_c, KeyCodes.C },
            { Keycode.SDLK_d, KeyCodes.D },
            { Keycode.SDLK_e, KeyCodes.E },
            { Keycode.SDLK_f, KeyCodes.F },
            { Keycode.SDLK_g, KeyCodes.G },
            { Keycode.SDLK_h, KeyCodes.H },
            { Keycode.SDLK_i, KeyCodes.I },
            { Keycode.SDLK_j, KeyCodes.J },
            { Keycode.SDLK_k, KeyCodes.K },
            { Keycode.SDLK_l, KeyCodes.L },
            { Keycode.SDLK_m, KeyCodes.M },
            { Keycode.SDLK_n, KeyCodes.N },
            { Keycode.SDLK_o, KeyCodes.O },
            { Keycode.SDLK_p, KeyCodes.P },
            { Keycode.SDLK_q, KeyCodes.Q },
            { Keycode.SDLK_r, KeyCodes.R },
            { Keycode.SDLK_s, KeyCodes.S },
            { Keycode.SDLK_t, KeyCodes.T },
            { Keycode.SDLK_u, KeyCodes.U },
            { Keycode.SDLK_v, KeyCodes.V },
            { Keycode.SDLK_w, KeyCodes.W },
            { Keycode.SDLK_x, KeyCodes.X },
            { Keycode.SDLK_y, KeyCodes.Y },
            { Keycode.SDLK_z, KeyCodes.Z },
            { Keycode.SDLK_DELETE, KeyCodes.Delete },
            { Keycode.SDLK_CAPSLOCK, KeyCodes.CapsLock },
            { Keycode.SDLK_F1, KeyCodes.F1 },
            { Keycode.SDLK_F2, KeyCodes.F2 },
            { Keycode.SDLK_F3, KeyCodes.F3 },
            { Keycode.SDLK_F4, KeyCodes.F4 },
            { Keycode.SDLK_F5, KeyCodes.F5 },
            { Keycode.SDLK_F6, KeyCodes.F6 },
            { Keycode.SDLK_F7, KeyCodes.F7 },
            { Keycode.SDLK_F8, KeyCodes.F8 },
            { Keycode.SDLK_F9, KeyCodes.F9 },
            { Keycode.SDLK_F10, KeyCodes.F10 },
            { Keycode.SDLK_F11, KeyCodes.F11 },
            { Keycode.SDLK_F12, KeyCodes.F12 },
            { Keycode.SDLK_SCROLLLOCK, KeyCodes.Scroll },
            { Keycode.SDLK_PAUSE, KeyCodes.Pause },
            { Keycode.SDLK_INSERT, KeyCodes.Insert },
            { Keycode.SDLK_HOME, KeyCodes.Home },
            { Keycode.SDLK_PAGEUP, KeyCodes.PageUp },
            { Keycode.SDLK_END, KeyCodes.End },
            { Keycode.SDLK_PAGEDOWN, KeyCodes.PageDown },
            { Keycode.SDLK_RIGHT, KeyCodes.Right },
            { Keycode.SDLK_LEFT, KeyCodes.Left },
            { Keycode.SDLK_DOWN, KeyCodes.Down },
            { Keycode.SDLK_UP, KeyCodes.Up },
            { Keycode.SDLK_NUMLOCKCLEAR, KeyCodes.NumLock },
            { Keycode.SDLK_KP_DIVIDE, KeyCodes.Divide },
            { Keycode.SDLK_KP_MULTIPLY, KeyCodes.Multiply },
            { Keycode.SDLK_KP_MINUS, KeyCodes.Subtract },
            { Keycode.SDLK_KP_PLUS, KeyCodes.Add },
            { Keycode.SDLK_KP_ENTER, KeyCodes.Enter },
            { Keycode.SDLK_KP_1, KeyCodes.NumPad1 },
            { Keycode.SDLK_KP_2, KeyCodes.NumPad2 },
            { Keycode.SDLK_KP_3, KeyCodes.NumPad3 },
            { Keycode.SDLK_KP_4, KeyCodes.NumPad4 },
            { Keycode.SDLK_KP_5, KeyCodes.NumPad5 },
            { Keycode.SDLK_KP_6, KeyCodes.NumPad6 },
            { Keycode.SDLK_KP_7, KeyCodes.NumPad7 },
            { Keycode.SDLK_KP_8, KeyCodes.NumPad8 },
            { Keycode.SDLK_KP_9, KeyCodes.NumPad9 },
            { Keycode.SDLK_KP_0, KeyCodes.NumPad0 },
            { Keycode.SDLK_KP_PERIOD, KeyCodes.Decimal },
            { Keycode.SDLK_APPLICATION, KeyCodes.Apps },
            { Keycode.SDLK_F13, KeyCodes.F13 },
            { Keycode.SDLK_F14, KeyCodes.F14 },
            { Keycode.SDLK_F15, KeyCodes.F15 },
            { Keycode.SDLK_F16, KeyCodes.F16 },
            { Keycode.SDLK_F17, KeyCodes.F17 },
            { Keycode.SDLK_F18, KeyCodes.F18 },
            { Keycode.SDLK_F19, KeyCodes.F19 },
            { Keycode.SDLK_F20, KeyCodes.F20 },
            { Keycode.SDLK_F21, KeyCodes.F21 },
            { Keycode.SDLK_F22, KeyCodes.F22 },
            { Keycode.SDLK_F23, KeyCodes.F23 },
            { Keycode.SDLK_F24, KeyCodes.F24 },
            { Keycode.SDLK_EXECUTE, KeyCodes.Execute },
            { Keycode.SDLK_HELP, KeyCodes.Help },
            { Keycode.SDLK_SELECT, KeyCodes.Select },
            { Keycode.SDLK_VOLUMEUP, KeyCodes.VolumeUp },
            { Keycode.SDLK_VOLUMEDOWN, KeyCodes.VolumeDown },
            { Keycode.SDLK_SEPARATOR, KeyCodes.Separator },
            { Keycode.SDLK_CRSEL, KeyCodes.Crsel },
            { Keycode.SDLK_EXSEL, KeyCodes.Exsel },
            { Keycode.SDLK_LCTRL, KeyCodes.LeftControl },
            { Keycode.SDLK_LSHIFT, KeyCodes.LeftShift },
            { Keycode.SDLK_LALT, KeyCodes.LeftAlt },
            { Keycode.SDLK_LGUI, KeyCodes.LeftWindows },
            { Keycode.SDLK_RCTRL, KeyCodes.RightControl },
            { Keycode.SDLK_RSHIFT, KeyCodes.RightShift },
            { Keycode.SDLK_RALT, KeyCodes.RightAlt },
            { Keycode.SDLK_RGUI, KeyCodes.RightWindows },
            { Keycode.SDLK_AUDIONEXT, KeyCodes.MediaNextTrack },
            { Keycode.SDLK_AUDIOPREV, KeyCodes.MediaPreviousTrack },
            { Keycode.SDLK_AUDIOSTOP, KeyCodes.MediaStop },
            { Keycode.SDLK_AUDIOPLAY, KeyCodes.MediaPlayPause },
            { Keycode.SDLK_AUDIOMUTE, KeyCodes.VolumeMute },
            { Keycode.SDLK_MEDIASELECT, KeyCodes.SelectMedia },
            { Keycode.SDLK_MAIL, KeyCodes.LaunchMail },
            { Keycode.SDLK_AC_SEARCH, KeyCodes.BrowserSearch },
            { Keycode.SDLK_AC_HOME, KeyCodes.BrowserHome },
            { Keycode.SDLK_AC_BACK, KeyCodes.BrowserBack },
            { Keycode.SDLK_AC_FORWARD, KeyCodes.BrowserForward },
            { Keycode.SDLK_AC_STOP, KeyCodes.BrowserStop },
            { Keycode.SDLK_AC_REFRESH, KeyCodes.BrowserRefresh },
            { Keycode.SDLK_AC_BOOKMARKS, KeyCodes.BrowserFavorites }
        };

        /// <summary>
        /// Holds the mappings of standard key codes to SDL key codes.
        /// </summary>
        public static Dictionary<KeyCodes, Keycode> StandardToSDLMappings => new Dictionary<KeyCodes, Keycode>()
        {
            { KeyCodes.None, Keycode.SDLK_UNKNOWN },
            { KeyCodes.Back, Keycode.SDLK_BACKSPACE },
            { KeyCodes.Tab, Keycode.SDLK_TAB },
            { KeyCodes.Enter, Keycode.SDLK_RETURN },
            { KeyCodes.Escape, Keycode.SDLK_ESCAPE },
            { KeyCodes.Space, Keycode.SDLK_SPACE },
            { KeyCodes.OemQuotes, Keycode.SDLK_QUOTE },
            { KeyCodes.OemComma, Keycode.SDLK_COMMA },
            { KeyCodes.OemMinus, Keycode.SDLK_MINUS },
            { KeyCodes.OemPeriod, Keycode.SDLK_PERIOD },
            { KeyCodes.OemQuestion, Keycode.SDLK_SLASH },
            { KeyCodes.D0, Keycode.SDLK_0 },
            { KeyCodes.D1, Keycode.SDLK_1 },
            { KeyCodes.D2, Keycode.SDLK_2 },
            { KeyCodes.D3, Keycode.SDLK_3 },
            { KeyCodes.D4, Keycode.SDLK_4 },
            { KeyCodes.D5, Keycode.SDLK_5 },
            { KeyCodes.D6, Keycode.SDLK_6 },
            { KeyCodes.D7, Keycode.SDLK_7 },
            { KeyCodes.D8, Keycode.SDLK_8 },
            { KeyCodes.D9, Keycode.SDLK_9 },
            { KeyCodes.OemSemicolon, Keycode.SDLK_SEMICOLON },
            { KeyCodes.OemPlus, Keycode.SDLK_EQUALS },
            { KeyCodes.OemOpenBrackets, Keycode.SDLK_LEFTBRACKET },
            { KeyCodes.OemPipe, Keycode.SDLK_BACKSLASH },
            { KeyCodes.OemCloseBrackets, Keycode.SDLK_RIGHTBRACKET },
            { KeyCodes.OemTilde, Keycode.SDLK_BACKQUOTE },
            { KeyCodes.A, Keycode.SDLK_a },
            { KeyCodes.B, Keycode.SDLK_b },
            { KeyCodes.C, Keycode.SDLK_c },
            { KeyCodes.D, Keycode.SDLK_d },
            { KeyCodes.E, Keycode.SDLK_e },
            { KeyCodes.F, Keycode.SDLK_f },
            { KeyCodes.G, Keycode.SDLK_g },
            { KeyCodes.H, Keycode.SDLK_h },
            { KeyCodes.I, Keycode.SDLK_i },
            { KeyCodes.J, Keycode.SDLK_j },
            { KeyCodes.K, Keycode.SDLK_k },
            { KeyCodes.L, Keycode.SDLK_l },
            { KeyCodes.M, Keycode.SDLK_m },
            { KeyCodes.N, Keycode.SDLK_n },
            { KeyCodes.O, Keycode.SDLK_o },
            { KeyCodes.P, Keycode.SDLK_p },
            { KeyCodes.Q, Keycode.SDLK_q },
            { KeyCodes.R, Keycode.SDLK_r },
            { KeyCodes.S, Keycode.SDLK_s },
            { KeyCodes.T, Keycode.SDLK_t },
            { KeyCodes.U, Keycode.SDLK_u },
            { KeyCodes.V, Keycode.SDLK_v },
            { KeyCodes.W, Keycode.SDLK_w },
            { KeyCodes.X, Keycode.SDLK_x },
            { KeyCodes.Y, Keycode.SDLK_y },
            { KeyCodes.Z, Keycode.SDLK_z },
            { KeyCodes.Delete, Keycode.SDLK_DELETE },
            { KeyCodes.CapsLock, Keycode.SDLK_CAPSLOCK },
            { KeyCodes.F1, Keycode.SDLK_F1 },
            { KeyCodes.F2, Keycode.SDLK_F2 },
            { KeyCodes.F3, Keycode.SDLK_F3 },
            { KeyCodes.F4, Keycode.SDLK_F4 },
            { KeyCodes.F5, Keycode.SDLK_F5 },
            { KeyCodes.F6, Keycode.SDLK_F6 },
            { KeyCodes.F7, Keycode.SDLK_F7 },
            { KeyCodes.F8, Keycode.SDLK_F8 },
            { KeyCodes.F9, Keycode.SDLK_F9 },
            { KeyCodes.F10, Keycode.SDLK_F10 },
            { KeyCodes.F11, Keycode.SDLK_F11 },
            { KeyCodes.F12, Keycode.SDLK_F12 },
            { KeyCodes.Scroll, Keycode.SDLK_SCROLLLOCK },
            { KeyCodes.Pause, Keycode.SDLK_PAUSE },
            { KeyCodes.Insert, Keycode.SDLK_INSERT },
            { KeyCodes.Home, Keycode.SDLK_HOME },
            { KeyCodes.PageUp, Keycode.SDLK_PAGEUP },
            { KeyCodes.End, Keycode.SDLK_END },
            { KeyCodes.PageDown, Keycode.SDLK_PAGEDOWN },
            { KeyCodes.Right, Keycode.SDLK_RIGHT },
            { KeyCodes.Left, Keycode.SDLK_LEFT },
            { KeyCodes.Down, Keycode.SDLK_DOWN },
            { KeyCodes.Up, Keycode.SDLK_UP },
            { KeyCodes.NumLock, Keycode.SDLK_NUMLOCKCLEAR },
            { KeyCodes.Divide, Keycode.SDLK_KP_DIVIDE },
            { KeyCodes.Multiply, Keycode.SDLK_KP_MULTIPLY },
            { KeyCodes.Subtract, Keycode.SDLK_KP_MINUS },
            { KeyCodes.Add, Keycode.SDLK_KP_PLUS },
            { KeyCodes.NumPad1, Keycode.SDLK_KP_1 },
            { KeyCodes.NumPad2, Keycode.SDLK_KP_2 },
            { KeyCodes.NumPad3, Keycode.SDLK_KP_3 },
            { KeyCodes.NumPad4, Keycode.SDLK_KP_4 },
            { KeyCodes.NumPad5, Keycode.SDLK_KP_5 },
            { KeyCodes.NumPad6, Keycode.SDLK_KP_6 },
            { KeyCodes.NumPad7, Keycode.SDLK_KP_7 },
            { KeyCodes.NumPad8, Keycode.SDLK_KP_8 },
            { KeyCodes.NumPad9, Keycode.SDLK_KP_9 },
            { KeyCodes.NumPad0, Keycode.SDLK_KP_0 },
            { KeyCodes.Decimal, Keycode.SDLK_KP_PERIOD },
            { KeyCodes.Apps, Keycode.SDLK_APPLICATION },
            { KeyCodes.F13, Keycode.SDLK_F13 },
            { KeyCodes.F14, Keycode.SDLK_F14 },
            { KeyCodes.F15, Keycode.SDLK_F15 },
            { KeyCodes.F16, Keycode.SDLK_F16 },
            { KeyCodes.F17, Keycode.SDLK_F17 },
            { KeyCodes.F18, Keycode.SDLK_F18 },
            { KeyCodes.F19, Keycode.SDLK_F19 },
            { KeyCodes.F20, Keycode.SDLK_F20 },
            { KeyCodes.F21, Keycode.SDLK_F21 },
            { KeyCodes.F22, Keycode.SDLK_F22 },
            { KeyCodes.F23, Keycode.SDLK_F23 },
            { KeyCodes.F24, Keycode.SDLK_F24 },
            { KeyCodes.Execute, Keycode.SDLK_EXECUTE },
            { KeyCodes.Help, Keycode.SDLK_HELP },
            { KeyCodes.Select, Keycode.SDLK_SELECT },
            { KeyCodes.VolumeUp, Keycode.SDLK_VOLUMEUP },
            { KeyCodes.VolumeDown, Keycode.SDLK_VOLUMEDOWN },
            { KeyCodes.Separator, Keycode.SDLK_SEPARATOR },
            { KeyCodes.Crsel, Keycode.SDLK_CRSEL },
            { KeyCodes.Exsel, Keycode.SDLK_EXSEL },
            { KeyCodes.LeftControl, Keycode.SDLK_LCTRL },
            { KeyCodes.LeftShift, Keycode.SDLK_LSHIFT },
            { KeyCodes.LeftAlt, Keycode.SDLK_LALT },
            { KeyCodes.LeftWindows, Keycode.SDLK_LGUI },
            { KeyCodes.RightControl, Keycode.SDLK_RCTRL },
            { KeyCodes.RightShift, Keycode.SDLK_RSHIFT },
            { KeyCodes.RightAlt, Keycode.SDLK_RALT },
            { KeyCodes.RightWindows, Keycode.SDLK_RGUI },
            { KeyCodes.MediaNextTrack, Keycode.SDLK_AUDIONEXT },
            { KeyCodes.MediaPreviousTrack, Keycode.SDLK_AUDIOPREV },
            { KeyCodes.MediaStop, Keycode.SDLK_AUDIOSTOP },
            { KeyCodes.MediaPlayPause, Keycode.SDLK_AUDIOPLAY },
            { KeyCodes.VolumeMute, Keycode.SDLK_AUDIOMUTE },
            { KeyCodes.SelectMedia, Keycode.SDLK_MEDIASELECT },
            { KeyCodes.LaunchMail, Keycode.SDLK_MAIL },
            { KeyCodes.BrowserSearch, Keycode.SDLK_AC_SEARCH },
            { KeyCodes.BrowserHome, Keycode.SDLK_AC_HOME },
            { KeyCodes.BrowserBack, Keycode.SDLK_AC_BACK },
            { KeyCodes.BrowserForward, Keycode.SDLK_AC_FORWARD },
            { KeyCodes.BrowserStop, Keycode.SDLK_AC_STOP },
            { KeyCodes.BrowserRefresh, Keycode.SDLK_AC_REFRESH },
            { KeyCodes.BrowserFavorites, Keycode.SDLK_AC_BOOKMARKS }
        };
        #endregion


        #region Public Methods
        /// <summary>
        /// Converts the given <paramref name="key"/> of type <see cref="SDL.SDL_Keycode"/> to 
        /// the keycode of type <see cref="KeyCodes"/>.
        /// </summary>
        /// <param name="key">The SDL key code used to map to the standard key code.</param>
        /// <returns></returns>
        public static KeyCodes ToStandardKeyCode(Keycode key)
        {
            //Need to recogize the SDLK_RETURN and SDLK_KP_ENTER keys as the same KeyCode.Enter key code
            if (key == Keycode.SDLK_RETURN || key == Keycode.SDLK_KP_ENTER)
                return SDLToStandardMappings[Keycode.SDLK_RETURN];


            return SDLToStandardMappings[key];
        }


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="Keycode"/> to 
        /// the keycode of type <see cref="KeyCodes"/>.
        /// </summary>
        /// <param name="key">The SDL key codes used to map to the standard key codes.</param>
        /// <returns></returns>
        public static KeyCodes[] ToStandardKeyCodes(Keycode[] keys)
        {
            return keys.Select(k =>
            {
                if (k == Keycode.SDLK_RETURN || k == Keycode.SDLK_KP_ENTER)
                {
                    return KeyCodes.Enter;
                }
                else
                {
                    return ToStandardKeyCode(k);
                }
            }).ToArray();
        }


        /// <summary>
        /// Converts the given <paramref name="standardKeyCode"/> of type <see cref="KeyCodes"/> to
        /// the keycode of type <see cref="Keycode"/>.
        /// </summary>
        /// <param name="standardKeyCode">The standard key code used to map to the SDL key code.</param>
        /// <returns></returns>
        public static Keycode ToSDLKeyCode(KeyCodes standardKeyCode) => StandardToSDLMappings[standardKeyCode];


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="KeyCodes"/> to
        /// the keycode of type <see cref="Keycode"/>.
        /// </summary>
        /// <param name="key">The standard key codes used to map to the SDL key code.</param>
        /// <returns></returns>
        public static Keycode[] ToSDLKeyCodes(KeyCodes[] keys) => (from k in keys select ToSDLKeyCode(k)).ToArray();
        #endregion
    }
}
