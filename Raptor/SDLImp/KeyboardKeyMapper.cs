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
            { Keycode.SDLK_UNKNOWN, KeyCode.None },
            { Keycode.SDLK_BACKSPACE, KeyCode.Back },
            { Keycode.SDLK_TAB, KeyCode.Tab },
            { Keycode.SDLK_RETURN, KeyCode.Enter },
            { Keycode.SDLK_ESCAPE, KeyCode.Escape },
            { Keycode.SDLK_SPACE, KeyCode.Space },
            { Keycode.SDLK_QUOTE, KeyCode.OemQuotes },
            { Keycode.SDLK_COMMA, KeyCode.OemComma },
            { Keycode.SDLK_MINUS, KeyCode.OemMinus },
            { Keycode.SDLK_PERIOD, KeyCode.OemPeriod },
            { Keycode.SDLK_SLASH, KeyCode.OemQuestion },
            { Keycode.SDLK_0, KeyCode.D0 },
            { Keycode.SDLK_1, KeyCode.D1 },
            { Keycode.SDLK_2, KeyCode.D2 },
            { Keycode.SDLK_3, KeyCode.D3 },
            { Keycode.SDLK_4, KeyCode.D4 },
            { Keycode.SDLK_5, KeyCode.D5 },
            { Keycode.SDLK_6, KeyCode.D6 },
            { Keycode.SDLK_7, KeyCode.D7 },
            { Keycode.SDLK_8, KeyCode.D8 },
            { Keycode.SDLK_9, KeyCode.D9 },
            { Keycode.SDLK_SEMICOLON, KeyCode.OemSemicolon },
            { Keycode.SDLK_EQUALS, KeyCode.OemPlus },
            { Keycode.SDLK_LEFTBRACKET, KeyCode.OemOpenBrackets },
            { Keycode.SDLK_BACKSLASH, KeyCode.OemPipe },
            { Keycode.SDLK_RIGHTBRACKET, KeyCode.OemCloseBrackets },
            { Keycode.SDLK_BACKQUOTE, KeyCode.OemTilde },
            { Keycode.SDLK_a, KeyCode.A },
            { Keycode.SDLK_b, KeyCode.B },
            { Keycode.SDLK_c, KeyCode.C },
            { Keycode.SDLK_d, KeyCode.D },
            { Keycode.SDLK_e, KeyCode.E },
            { Keycode.SDLK_f, KeyCode.F },
            { Keycode.SDLK_g, KeyCode.G },
            { Keycode.SDLK_h, KeyCode.H },
            { Keycode.SDLK_i, KeyCode.I },
            { Keycode.SDLK_j, KeyCode.J },
            { Keycode.SDLK_k, KeyCode.K },
            { Keycode.SDLK_l, KeyCode.L },
            { Keycode.SDLK_m, KeyCode.M },
            { Keycode.SDLK_n, KeyCode.N },
            { Keycode.SDLK_o, KeyCode.O },
            { Keycode.SDLK_p, KeyCode.P },
            { Keycode.SDLK_q, KeyCode.Q },
            { Keycode.SDLK_r, KeyCode.R },
            { Keycode.SDLK_s, KeyCode.S },
            { Keycode.SDLK_t, KeyCode.T },
            { Keycode.SDLK_u, KeyCode.U },
            { Keycode.SDLK_v, KeyCode.V },
            { Keycode.SDLK_w, KeyCode.W },
            { Keycode.SDLK_x, KeyCode.X },
            { Keycode.SDLK_y, KeyCode.Y },
            { Keycode.SDLK_z, KeyCode.Z },
            { Keycode.SDLK_DELETE, KeyCode.Delete },
            { Keycode.SDLK_CAPSLOCK, KeyCode.CapsLock },
            { Keycode.SDLK_F1, KeyCode.F1 },
            { Keycode.SDLK_F2, KeyCode.F2 },
            { Keycode.SDLK_F3, KeyCode.F3 },
            { Keycode.SDLK_F4, KeyCode.F4 },
            { Keycode.SDLK_F5, KeyCode.F5 },
            { Keycode.SDLK_F6, KeyCode.F6 },
            { Keycode.SDLK_F7, KeyCode.F7 },
            { Keycode.SDLK_F8, KeyCode.F8 },
            { Keycode.SDLK_F9, KeyCode.F9 },
            { Keycode.SDLK_F10, KeyCode.F10 },
            { Keycode.SDLK_F11, KeyCode.F11 },
            { Keycode.SDLK_F12, KeyCode.F12 },
            { Keycode.SDLK_SCROLLLOCK, KeyCode.Scroll },
            { Keycode.SDLK_PAUSE, KeyCode.Pause },
            { Keycode.SDLK_INSERT, KeyCode.Insert },
            { Keycode.SDLK_HOME, KeyCode.Home },
            { Keycode.SDLK_PAGEUP, KeyCode.PageUp },
            { Keycode.SDLK_END, KeyCode.End },
            { Keycode.SDLK_PAGEDOWN, KeyCode.PageDown },
            { Keycode.SDLK_RIGHT, KeyCode.Right },
            { Keycode.SDLK_LEFT, KeyCode.Left },
            { Keycode.SDLK_DOWN, KeyCode.Down },
            { Keycode.SDLK_UP, KeyCode.Up },
            { Keycode.SDLK_NUMLOCKCLEAR, KeyCode.NumLock },
            { Keycode.SDLK_KP_DIVIDE, KeyCode.Divide },
            { Keycode.SDLK_KP_MULTIPLY, KeyCode.Multiply },
            { Keycode.SDLK_KP_MINUS, KeyCode.Subtract },
            { Keycode.SDLK_KP_PLUS, KeyCode.Add },
            { Keycode.SDLK_KP_ENTER, KeyCode.Enter },
            { Keycode.SDLK_KP_1, KeyCode.NumPad1 },
            { Keycode.SDLK_KP_2, KeyCode.NumPad2 },
            { Keycode.SDLK_KP_3, KeyCode.NumPad3 },
            { Keycode.SDLK_KP_4, KeyCode.NumPad4 },
            { Keycode.SDLK_KP_5, KeyCode.NumPad5 },
            { Keycode.SDLK_KP_6, KeyCode.NumPad6 },
            { Keycode.SDLK_KP_7, KeyCode.NumPad7 },
            { Keycode.SDLK_KP_8, KeyCode.NumPad8 },
            { Keycode.SDLK_KP_9, KeyCode.NumPad9 },
            { Keycode.SDLK_KP_0, KeyCode.NumPad0 },
            { Keycode.SDLK_KP_PERIOD, KeyCode.Decimal },
            { Keycode.SDLK_APPLICATION, KeyCode.Apps },
            { Keycode.SDLK_F13, KeyCode.F13 },
            { Keycode.SDLK_F14, KeyCode.F14 },
            { Keycode.SDLK_F15, KeyCode.F15 },
            { Keycode.SDLK_F16, KeyCode.F16 },
            { Keycode.SDLK_F17, KeyCode.F17 },
            { Keycode.SDLK_F18, KeyCode.F18 },
            { Keycode.SDLK_F19, KeyCode.F19 },
            { Keycode.SDLK_F20, KeyCode.F20 },
            { Keycode.SDLK_F21, KeyCode.F21 },
            { Keycode.SDLK_F22, KeyCode.F22 },
            { Keycode.SDLK_F23, KeyCode.F23 },
            { Keycode.SDLK_F24, KeyCode.F24 },
            { Keycode.SDLK_EXECUTE, KeyCode.Execute },
            { Keycode.SDLK_HELP, KeyCode.Help },
            { Keycode.SDLK_SELECT, KeyCode.Select },
            { Keycode.SDLK_VOLUMEUP, KeyCode.VolumeUp },
            { Keycode.SDLK_VOLUMEDOWN, KeyCode.VolumeDown },
            { Keycode.SDLK_SEPARATOR, KeyCode.Separator },
            { Keycode.SDLK_CRSEL, KeyCode.Crsel },
            { Keycode.SDLK_EXSEL, KeyCode.Exsel },
            { Keycode.SDLK_LCTRL, KeyCode.LeftControl },
            { Keycode.SDLK_LSHIFT, KeyCode.LeftShift },
            { Keycode.SDLK_LALT, KeyCode.LeftAlt },
            { Keycode.SDLK_LGUI, KeyCode.LeftWindows },
            { Keycode.SDLK_RCTRL, KeyCode.RightControl },
            { Keycode.SDLK_RSHIFT, KeyCode.RightShift },
            { Keycode.SDLK_RALT, KeyCode.RightAlt },
            { Keycode.SDLK_RGUI, KeyCode.RightWindows },
            { Keycode.SDLK_AUDIONEXT, KeyCode.MediaNextTrack },
            { Keycode.SDLK_AUDIOPREV, KeyCode.MediaPreviousTrack },
            { Keycode.SDLK_AUDIOSTOP, KeyCode.MediaStop },
            { Keycode.SDLK_AUDIOPLAY, KeyCode.MediaPlayPause },
            { Keycode.SDLK_AUDIOMUTE, KeyCode.VolumeMute },
            { Keycode.SDLK_MEDIASELECT, KeyCode.SelectMedia },
            { Keycode.SDLK_MAIL, KeyCode.LaunchMail },
            { Keycode.SDLK_AC_SEARCH, KeyCode.BrowserSearch },
            { Keycode.SDLK_AC_HOME, KeyCode.BrowserHome },
            { Keycode.SDLK_AC_BACK, KeyCode.BrowserBack },
            { Keycode.SDLK_AC_FORWARD, KeyCode.BrowserForward },
            { Keycode.SDLK_AC_STOP, KeyCode.BrowserStop },
            { Keycode.SDLK_AC_REFRESH, KeyCode.BrowserRefresh },
            { Keycode.SDLK_AC_BOOKMARKS, KeyCode.BrowserFavorites }
        };

        /// <summary>
        /// Holds the mappings of standard key codes to SDL key codes.
        /// </summary>
        public static Dictionary<KeyCode, Keycode> StandardToSDLMappings => new Dictionary<KeyCode, Keycode>()
        {
            { KeyCode.None, Keycode.SDLK_UNKNOWN },
            { KeyCode.Back, Keycode.SDLK_BACKSPACE },
            { KeyCode.Tab, Keycode.SDLK_TAB },
            { KeyCode.Enter, Keycode.SDLK_RETURN },
            { KeyCode.Escape, Keycode.SDLK_ESCAPE },
            { KeyCode.Space, Keycode.SDLK_SPACE },
            { KeyCode.OemQuotes, Keycode.SDLK_QUOTE },
            { KeyCode.OemComma, Keycode.SDLK_COMMA },
            { KeyCode.OemMinus, Keycode.SDLK_MINUS },
            { KeyCode.OemPeriod, Keycode.SDLK_PERIOD },
            { KeyCode.OemQuestion, Keycode.SDLK_SLASH },
            { KeyCode.D0, Keycode.SDLK_0 },
            { KeyCode.D1, Keycode.SDLK_1 },
            { KeyCode.D2, Keycode.SDLK_2 },
            { KeyCode.D3, Keycode.SDLK_3 },
            { KeyCode.D4, Keycode.SDLK_4 },
            { KeyCode.D5, Keycode.SDLK_5 },
            { KeyCode.D6, Keycode.SDLK_6 },
            { KeyCode.D7, Keycode.SDLK_7 },
            { KeyCode.D8, Keycode.SDLK_8 },
            { KeyCode.D9, Keycode.SDLK_9 },
            { KeyCode.OemSemicolon, Keycode.SDLK_SEMICOLON },
            { KeyCode.OemPlus, Keycode.SDLK_EQUALS },
            { KeyCode.OemOpenBrackets, Keycode.SDLK_LEFTBRACKET },
            { KeyCode.OemPipe, Keycode.SDLK_BACKSLASH },
            { KeyCode.OemCloseBrackets, Keycode.SDLK_RIGHTBRACKET },
            { KeyCode.OemTilde, Keycode.SDLK_BACKQUOTE },
            { KeyCode.A, Keycode.SDLK_a },
            { KeyCode.B, Keycode.SDLK_b },
            { KeyCode.C, Keycode.SDLK_c },
            { KeyCode.D, Keycode.SDLK_d },
            { KeyCode.E, Keycode.SDLK_e },
            { KeyCode.F, Keycode.SDLK_f },
            { KeyCode.G, Keycode.SDLK_g },
            { KeyCode.H, Keycode.SDLK_h },
            { KeyCode.I, Keycode.SDLK_i },
            { KeyCode.J, Keycode.SDLK_j },
            { KeyCode.K, Keycode.SDLK_k },
            { KeyCode.L, Keycode.SDLK_l },
            { KeyCode.M, Keycode.SDLK_m },
            { KeyCode.N, Keycode.SDLK_n },
            { KeyCode.O, Keycode.SDLK_o },
            { KeyCode.P, Keycode.SDLK_p },
            { KeyCode.Q, Keycode.SDLK_q },
            { KeyCode.R, Keycode.SDLK_r },
            { KeyCode.S, Keycode.SDLK_s },
            { KeyCode.T, Keycode.SDLK_t },
            { KeyCode.U, Keycode.SDLK_u },
            { KeyCode.V, Keycode.SDLK_v },
            { KeyCode.W, Keycode.SDLK_w },
            { KeyCode.X, Keycode.SDLK_x },
            { KeyCode.Y, Keycode.SDLK_y },
            { KeyCode.Z, Keycode.SDLK_z },
            { KeyCode.Delete, Keycode.SDLK_DELETE },
            { KeyCode.CapsLock, Keycode.SDLK_CAPSLOCK },
            { KeyCode.F1, Keycode.SDLK_F1 },
            { KeyCode.F2, Keycode.SDLK_F2 },
            { KeyCode.F3, Keycode.SDLK_F3 },
            { KeyCode.F4, Keycode.SDLK_F4 },
            { KeyCode.F5, Keycode.SDLK_F5 },
            { KeyCode.F6, Keycode.SDLK_F6 },
            { KeyCode.F7, Keycode.SDLK_F7 },
            { KeyCode.F8, Keycode.SDLK_F8 },
            { KeyCode.F9, Keycode.SDLK_F9 },
            { KeyCode.F10, Keycode.SDLK_F10 },
            { KeyCode.F11, Keycode.SDLK_F11 },
            { KeyCode.F12, Keycode.SDLK_F12 },
            { KeyCode.Scroll, Keycode.SDLK_SCROLLLOCK },
            { KeyCode.Pause, Keycode.SDLK_PAUSE },
            { KeyCode.Insert, Keycode.SDLK_INSERT },
            { KeyCode.Home, Keycode.SDLK_HOME },
            { KeyCode.PageUp, Keycode.SDLK_PAGEUP },
            { KeyCode.End, Keycode.SDLK_END },
            { KeyCode.PageDown, Keycode.SDLK_PAGEDOWN },
            { KeyCode.Right, Keycode.SDLK_RIGHT },
            { KeyCode.Left, Keycode.SDLK_LEFT },
            { KeyCode.Down, Keycode.SDLK_DOWN },
            { KeyCode.Up, Keycode.SDLK_UP },
            { KeyCode.NumLock, Keycode.SDLK_NUMLOCKCLEAR },
            { KeyCode.Divide, Keycode.SDLK_KP_DIVIDE },
            { KeyCode.Multiply, Keycode.SDLK_KP_MULTIPLY },
            { KeyCode.Subtract, Keycode.SDLK_KP_MINUS },
            { KeyCode.Add, Keycode.SDLK_KP_PLUS },
            { KeyCode.NumPad1, Keycode.SDLK_KP_1 },
            { KeyCode.NumPad2, Keycode.SDLK_KP_2 },
            { KeyCode.NumPad3, Keycode.SDLK_KP_3 },
            { KeyCode.NumPad4, Keycode.SDLK_KP_4 },
            { KeyCode.NumPad5, Keycode.SDLK_KP_5 },
            { KeyCode.NumPad6, Keycode.SDLK_KP_6 },
            { KeyCode.NumPad7, Keycode.SDLK_KP_7 },
            { KeyCode.NumPad8, Keycode.SDLK_KP_8 },
            { KeyCode.NumPad9, Keycode.SDLK_KP_9 },
            { KeyCode.NumPad0, Keycode.SDLK_KP_0 },
            { KeyCode.Decimal, Keycode.SDLK_KP_PERIOD },
            { KeyCode.Apps, Keycode.SDLK_APPLICATION },
            { KeyCode.F13, Keycode.SDLK_F13 },
            { KeyCode.F14, Keycode.SDLK_F14 },
            { KeyCode.F15, Keycode.SDLK_F15 },
            { KeyCode.F16, Keycode.SDLK_F16 },
            { KeyCode.F17, Keycode.SDLK_F17 },
            { KeyCode.F18, Keycode.SDLK_F18 },
            { KeyCode.F19, Keycode.SDLK_F19 },
            { KeyCode.F20, Keycode.SDLK_F20 },
            { KeyCode.F21, Keycode.SDLK_F21 },
            { KeyCode.F22, Keycode.SDLK_F22 },
            { KeyCode.F23, Keycode.SDLK_F23 },
            { KeyCode.F24, Keycode.SDLK_F24 },
            { KeyCode.Execute, Keycode.SDLK_EXECUTE },
            { KeyCode.Help, Keycode.SDLK_HELP },
            { KeyCode.Select, Keycode.SDLK_SELECT },
            { KeyCode.VolumeUp, Keycode.SDLK_VOLUMEUP },
            { KeyCode.VolumeDown, Keycode.SDLK_VOLUMEDOWN },
            { KeyCode.Separator, Keycode.SDLK_SEPARATOR },
            { KeyCode.Crsel, Keycode.SDLK_CRSEL },
            { KeyCode.Exsel, Keycode.SDLK_EXSEL },
            { KeyCode.LeftControl, Keycode.SDLK_LCTRL },
            { KeyCode.LeftShift, Keycode.SDLK_LSHIFT },
            { KeyCode.LeftAlt, Keycode.SDLK_LALT },
            { KeyCode.LeftWindows, Keycode.SDLK_LGUI },
            { KeyCode.RightControl, Keycode.SDLK_RCTRL },
            { KeyCode.RightShift, Keycode.SDLK_RSHIFT },
            { KeyCode.RightAlt, Keycode.SDLK_RALT },
            { KeyCode.RightWindows, Keycode.SDLK_RGUI },
            { KeyCode.MediaNextTrack, Keycode.SDLK_AUDIONEXT },
            { KeyCode.MediaPreviousTrack, Keycode.SDLK_AUDIOPREV },
            { KeyCode.MediaStop, Keycode.SDLK_AUDIOSTOP },
            { KeyCode.MediaPlayPause, Keycode.SDLK_AUDIOPLAY },
            { KeyCode.VolumeMute, Keycode.SDLK_AUDIOMUTE },
            { KeyCode.SelectMedia, Keycode.SDLK_MEDIASELECT },
            { KeyCode.LaunchMail, Keycode.SDLK_MAIL },
            { KeyCode.BrowserSearch, Keycode.SDLK_AC_SEARCH },
            { KeyCode.BrowserHome, Keycode.SDLK_AC_HOME },
            { KeyCode.BrowserBack, Keycode.SDLK_AC_BACK },
            { KeyCode.BrowserForward, Keycode.SDLK_AC_FORWARD },
            { KeyCode.BrowserStop, Keycode.SDLK_AC_STOP },
            { KeyCode.BrowserRefresh, Keycode.SDLK_AC_REFRESH },
            { KeyCode.BrowserFavorites, Keycode.SDLK_AC_BOOKMARKS }
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
            if (key == Keycode.SDLK_RETURN || key == Keycode.SDLK_KP_ENTER)
                return SDLToStandardMappings[Keycode.SDLK_RETURN];


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
                if (k == Keycode.SDLK_RETURN || k == Keycode.SDLK_KP_ENTER)
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
