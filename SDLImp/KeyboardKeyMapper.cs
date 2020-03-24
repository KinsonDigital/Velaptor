using KDScorpionCore.Input;
using SDL2;
using System.Collections.Generic;
using System.Linq;

namespace SDLScorpPlugin
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
        public static Dictionary<SDL.SDL_Keycode, KeyCodes> SDLToStandardMappings => new Dictionary<SDL.SDL_Keycode, KeyCodes>()
        {
            { SDL.SDL_Keycode.SDLK_UNKNOWN, KeyCodes.None },
            { SDL.SDL_Keycode.SDLK_BACKSPACE, KeyCodes.Back },
            { SDL.SDL_Keycode.SDLK_TAB, KeyCodes.Tab },
            { SDL.SDL_Keycode.SDLK_RETURN, KeyCodes.Enter },
            { SDL.SDL_Keycode.SDLK_ESCAPE, KeyCodes.Escape },
            { SDL.SDL_Keycode.SDLK_SPACE, KeyCodes.Space },
            { SDL.SDL_Keycode.SDLK_QUOTE, KeyCodes.OemQuotes },
            { SDL.SDL_Keycode.SDLK_COMMA, KeyCodes.OemComma },
            { SDL.SDL_Keycode.SDLK_MINUS, KeyCodes.OemMinus },
            { SDL.SDL_Keycode.SDLK_PERIOD, KeyCodes.OemPeriod },
            { SDL.SDL_Keycode.SDLK_SLASH, KeyCodes.OemQuestion },
            { SDL.SDL_Keycode.SDLK_0, KeyCodes.D0 },
            { SDL.SDL_Keycode.SDLK_1, KeyCodes.D1 },
            { SDL.SDL_Keycode.SDLK_2, KeyCodes.D2 },
            { SDL.SDL_Keycode.SDLK_3, KeyCodes.D3 },
            { SDL.SDL_Keycode.SDLK_4, KeyCodes.D4 },
            { SDL.SDL_Keycode.SDLK_5, KeyCodes.D5 },
            { SDL.SDL_Keycode.SDLK_6, KeyCodes.D6 },
            { SDL.SDL_Keycode.SDLK_7, KeyCodes.D7 },
            { SDL.SDL_Keycode.SDLK_8, KeyCodes.D8 },
            { SDL.SDL_Keycode.SDLK_9, KeyCodes.D9 },
            { SDL.SDL_Keycode.SDLK_SEMICOLON, KeyCodes.OemSemicolon },
            { SDL.SDL_Keycode.SDLK_EQUALS, KeyCodes.OemPlus },
            { SDL.SDL_Keycode.SDLK_LEFTBRACKET, KeyCodes.OemOpenBrackets },
            { SDL.SDL_Keycode.SDLK_BACKSLASH, KeyCodes.OemPipe },
            { SDL.SDL_Keycode.SDLK_RIGHTBRACKET, KeyCodes.OemCloseBrackets },
            { SDL.SDL_Keycode.SDLK_BACKQUOTE, KeyCodes.OemTilde },
            { SDL.SDL_Keycode.SDLK_a, KeyCodes.A },
            { SDL.SDL_Keycode.SDLK_b, KeyCodes.B },
            { SDL.SDL_Keycode.SDLK_c, KeyCodes.C },
            { SDL.SDL_Keycode.SDLK_d, KeyCodes.D },
            { SDL.SDL_Keycode.SDLK_e, KeyCodes.E },
            { SDL.SDL_Keycode.SDLK_f, KeyCodes.F },
            { SDL.SDL_Keycode.SDLK_g, KeyCodes.G },
            { SDL.SDL_Keycode.SDLK_h, KeyCodes.H },
            { SDL.SDL_Keycode.SDLK_i, KeyCodes.I },
            { SDL.SDL_Keycode.SDLK_j, KeyCodes.J },
            { SDL.SDL_Keycode.SDLK_k, KeyCodes.K },
            { SDL.SDL_Keycode.SDLK_l, KeyCodes.L },
            { SDL.SDL_Keycode.SDLK_m, KeyCodes.M },
            { SDL.SDL_Keycode.SDLK_n, KeyCodes.N },
            { SDL.SDL_Keycode.SDLK_o, KeyCodes.O },
            { SDL.SDL_Keycode.SDLK_p, KeyCodes.P },
            { SDL.SDL_Keycode.SDLK_q, KeyCodes.Q },
            { SDL.SDL_Keycode.SDLK_r, KeyCodes.R },
            { SDL.SDL_Keycode.SDLK_s, KeyCodes.S },
            { SDL.SDL_Keycode.SDLK_t, KeyCodes.T },
            { SDL.SDL_Keycode.SDLK_u, KeyCodes.U },
            { SDL.SDL_Keycode.SDLK_v, KeyCodes.V },
            { SDL.SDL_Keycode.SDLK_w, KeyCodes.W },
            { SDL.SDL_Keycode.SDLK_x, KeyCodes.X },
            { SDL.SDL_Keycode.SDLK_y, KeyCodes.Y },
            { SDL.SDL_Keycode.SDLK_z, KeyCodes.Z },
            { SDL.SDL_Keycode.SDLK_DELETE, KeyCodes.Delete },
            { SDL.SDL_Keycode.SDLK_CAPSLOCK, KeyCodes.CapsLock },
            { SDL.SDL_Keycode.SDLK_F1, KeyCodes.F1 },
            { SDL.SDL_Keycode.SDLK_F2, KeyCodes.F2 },
            { SDL.SDL_Keycode.SDLK_F3, KeyCodes.F3 },
            { SDL.SDL_Keycode.SDLK_F4, KeyCodes.F4 },
            { SDL.SDL_Keycode.SDLK_F5, KeyCodes.F5 },
            { SDL.SDL_Keycode.SDLK_F6, KeyCodes.F6 },
            { SDL.SDL_Keycode.SDLK_F7, KeyCodes.F7 },
            { SDL.SDL_Keycode.SDLK_F8, KeyCodes.F8 },
            { SDL.SDL_Keycode.SDLK_F9, KeyCodes.F9 },
            { SDL.SDL_Keycode.SDLK_F10, KeyCodes.F10 },
            { SDL.SDL_Keycode.SDLK_F11, KeyCodes.F11 },
            { SDL.SDL_Keycode.SDLK_F12, KeyCodes.F12 },
            { SDL.SDL_Keycode.SDLK_SCROLLLOCK, KeyCodes.Scroll },
            { SDL.SDL_Keycode.SDLK_PAUSE, KeyCodes.Pause },
            { SDL.SDL_Keycode.SDLK_INSERT, KeyCodes.Insert },
            { SDL.SDL_Keycode.SDLK_HOME, KeyCodes.Home },
            { SDL.SDL_Keycode.SDLK_PAGEUP, KeyCodes.PageUp },
            { SDL.SDL_Keycode.SDLK_END, KeyCodes.End },
            { SDL.SDL_Keycode.SDLK_PAGEDOWN, KeyCodes.PageDown },
            { SDL.SDL_Keycode.SDLK_RIGHT, KeyCodes.Right },
            { SDL.SDL_Keycode.SDLK_LEFT, KeyCodes.Left },
            { SDL.SDL_Keycode.SDLK_DOWN, KeyCodes.Down },
            { SDL.SDL_Keycode.SDLK_UP, KeyCodes.Up },
            { SDL.SDL_Keycode.SDLK_NUMLOCKCLEAR, KeyCodes.NumLock },
            { SDL.SDL_Keycode.SDLK_KP_DIVIDE, KeyCodes.Divide },
            { SDL.SDL_Keycode.SDLK_KP_MULTIPLY, KeyCodes.Multiply },
            { SDL.SDL_Keycode.SDLK_KP_MINUS, KeyCodes.Subtract },
            { SDL.SDL_Keycode.SDLK_KP_PLUS, KeyCodes.Add },
            { SDL.SDL_Keycode.SDLK_KP_ENTER, KeyCodes.Enter },
            { SDL.SDL_Keycode.SDLK_KP_1, KeyCodes.NumPad1 },
            { SDL.SDL_Keycode.SDLK_KP_2, KeyCodes.NumPad2 },
            { SDL.SDL_Keycode.SDLK_KP_3, KeyCodes.NumPad3 },
            { SDL.SDL_Keycode.SDLK_KP_4, KeyCodes.NumPad4 },
            { SDL.SDL_Keycode.SDLK_KP_5, KeyCodes.NumPad5 },
            { SDL.SDL_Keycode.SDLK_KP_6, KeyCodes.NumPad6 },
            { SDL.SDL_Keycode.SDLK_KP_7, KeyCodes.NumPad7 },
            { SDL.SDL_Keycode.SDLK_KP_8, KeyCodes.NumPad8 },
            { SDL.SDL_Keycode.SDLK_KP_9, KeyCodes.NumPad9 },
            { SDL.SDL_Keycode.SDLK_KP_0, KeyCodes.NumPad0 },
            { SDL.SDL_Keycode.SDLK_KP_PERIOD, KeyCodes.Decimal },
            { SDL.SDL_Keycode.SDLK_APPLICATION, KeyCodes.Apps },
            { SDL.SDL_Keycode.SDLK_F13, KeyCodes.F13 },
            { SDL.SDL_Keycode.SDLK_F14, KeyCodes.F14 },
            { SDL.SDL_Keycode.SDLK_F15, KeyCodes.F15 },
            { SDL.SDL_Keycode.SDLK_F16, KeyCodes.F16 },
            { SDL.SDL_Keycode.SDLK_F17, KeyCodes.F17 },
            { SDL.SDL_Keycode.SDLK_F18, KeyCodes.F18 },
            { SDL.SDL_Keycode.SDLK_F19, KeyCodes.F19 },
            { SDL.SDL_Keycode.SDLK_F20, KeyCodes.F20 },
            { SDL.SDL_Keycode.SDLK_F21, KeyCodes.F21 },
            { SDL.SDL_Keycode.SDLK_F22, KeyCodes.F22 },
            { SDL.SDL_Keycode.SDLK_F23, KeyCodes.F23 },
            { SDL.SDL_Keycode.SDLK_F24, KeyCodes.F24 },
            { SDL.SDL_Keycode.SDLK_EXECUTE, KeyCodes.Execute },
            { SDL.SDL_Keycode.SDLK_HELP, KeyCodes.Help },
            { SDL.SDL_Keycode.SDLK_SELECT, KeyCodes.Select },
            { SDL.SDL_Keycode.SDLK_VOLUMEUP, KeyCodes.VolumeUp },
            { SDL.SDL_Keycode.SDLK_VOLUMEDOWN, KeyCodes.VolumeDown },
            { SDL.SDL_Keycode.SDLK_SEPARATOR, KeyCodes.Separator },
            { SDL.SDL_Keycode.SDLK_CRSEL, KeyCodes.Crsel },
            { SDL.SDL_Keycode.SDLK_EXSEL, KeyCodes.Exsel },
            { SDL.SDL_Keycode.SDLK_LCTRL, KeyCodes.LeftControl },
            { SDL.SDL_Keycode.SDLK_LSHIFT, KeyCodes.LeftShift },
            { SDL.SDL_Keycode.SDLK_LALT, KeyCodes.LeftAlt },
            { SDL.SDL_Keycode.SDLK_LGUI, KeyCodes.LeftWindows },
            { SDL.SDL_Keycode.SDLK_RCTRL, KeyCodes.RightControl },
            { SDL.SDL_Keycode.SDLK_RSHIFT, KeyCodes.RightShift },
            { SDL.SDL_Keycode.SDLK_RALT, KeyCodes.RightAlt },
            { SDL.SDL_Keycode.SDLK_RGUI, KeyCodes.RightWindows },
            { SDL.SDL_Keycode.SDLK_AUDIONEXT, KeyCodes.MediaNextTrack },
            { SDL.SDL_Keycode.SDLK_AUDIOPREV, KeyCodes.MediaPreviousTrack },
            { SDL.SDL_Keycode.SDLK_AUDIOSTOP, KeyCodes.MediaStop },
            { SDL.SDL_Keycode.SDLK_AUDIOPLAY, KeyCodes.MediaPlayPause },
            { SDL.SDL_Keycode.SDLK_AUDIOMUTE, KeyCodes.VolumeMute },
            { SDL.SDL_Keycode.SDLK_MEDIASELECT, KeyCodes.SelectMedia },
            { SDL.SDL_Keycode.SDLK_MAIL, KeyCodes.LaunchMail },
            { SDL.SDL_Keycode.SDLK_AC_SEARCH, KeyCodes.BrowserSearch },
            { SDL.SDL_Keycode.SDLK_AC_HOME, KeyCodes.BrowserHome },
            { SDL.SDL_Keycode.SDLK_AC_BACK, KeyCodes.BrowserBack },
            { SDL.SDL_Keycode.SDLK_AC_FORWARD, KeyCodes.BrowserForward },
            { SDL.SDL_Keycode.SDLK_AC_STOP, KeyCodes.BrowserStop },
            { SDL.SDL_Keycode.SDLK_AC_REFRESH, KeyCodes.BrowserRefresh },
            { SDL.SDL_Keycode.SDLK_AC_BOOKMARKS, KeyCodes.BrowserFavorites }
        };

        /// <summary>
        /// Holds the mappings of standard key codes to SDL key codes.
        /// </summary>
        public static Dictionary<KeyCodes, SDL.SDL_Keycode> StandardToSDLMappings => new Dictionary<KeyCodes, SDL.SDL_Keycode>()
        {
            { KeyCodes.None, SDL.SDL_Keycode.SDLK_UNKNOWN },
            { KeyCodes.Back, SDL.SDL_Keycode.SDLK_BACKSPACE },
            { KeyCodes.Tab, SDL.SDL_Keycode.SDLK_TAB },
            { KeyCodes.Enter, SDL.SDL_Keycode.SDLK_RETURN },
            { KeyCodes.Escape, SDL.SDL_Keycode.SDLK_ESCAPE },
            { KeyCodes.Space, SDL.SDL_Keycode.SDLK_SPACE },
            { KeyCodes.OemQuotes, SDL.SDL_Keycode.SDLK_QUOTE },
            { KeyCodes.OemComma, SDL.SDL_Keycode.SDLK_COMMA },
            { KeyCodes.OemMinus, SDL.SDL_Keycode.SDLK_MINUS },
            { KeyCodes.OemPeriod, SDL.SDL_Keycode.SDLK_PERIOD },
            { KeyCodes.OemQuestion, SDL.SDL_Keycode.SDLK_SLASH },
            { KeyCodes.D0, SDL.SDL_Keycode.SDLK_0 },
            { KeyCodes.D1, SDL.SDL_Keycode.SDLK_1 },
            { KeyCodes.D2, SDL.SDL_Keycode.SDLK_2 },
            { KeyCodes.D3, SDL.SDL_Keycode.SDLK_3 },
            { KeyCodes.D4, SDL.SDL_Keycode.SDLK_4 },
            { KeyCodes.D5, SDL.SDL_Keycode.SDLK_5 },
            { KeyCodes.D6, SDL.SDL_Keycode.SDLK_6 },
            { KeyCodes.D7, SDL.SDL_Keycode.SDLK_7 },
            { KeyCodes.D8, SDL.SDL_Keycode.SDLK_8 },
            { KeyCodes.D9, SDL.SDL_Keycode.SDLK_9 },
            { KeyCodes.OemSemicolon, SDL.SDL_Keycode.SDLK_SEMICOLON },
            { KeyCodes.OemPlus, SDL.SDL_Keycode.SDLK_EQUALS },
            { KeyCodes.OemOpenBrackets, SDL.SDL_Keycode.SDLK_LEFTBRACKET },
            { KeyCodes.OemPipe, SDL.SDL_Keycode.SDLK_BACKSLASH },
            { KeyCodes.OemCloseBrackets, SDL.SDL_Keycode.SDLK_RIGHTBRACKET },
            { KeyCodes.OemTilde, SDL.SDL_Keycode.SDLK_BACKQUOTE },
            { KeyCodes.A, SDL.SDL_Keycode.SDLK_a },
            { KeyCodes.B, SDL.SDL_Keycode.SDLK_b },
            { KeyCodes.C, SDL.SDL_Keycode.SDLK_c },
            { KeyCodes.D, SDL.SDL_Keycode.SDLK_d },
            { KeyCodes.E, SDL.SDL_Keycode.SDLK_e },
            { KeyCodes.F, SDL.SDL_Keycode.SDLK_f },
            { KeyCodes.G, SDL.SDL_Keycode.SDLK_g },
            { KeyCodes.H, SDL.SDL_Keycode.SDLK_h },
            { KeyCodes.I, SDL.SDL_Keycode.SDLK_i },
            { KeyCodes.J, SDL.SDL_Keycode.SDLK_j },
            { KeyCodes.K, SDL.SDL_Keycode.SDLK_k },
            { KeyCodes.L, SDL.SDL_Keycode.SDLK_l },
            { KeyCodes.M, SDL.SDL_Keycode.SDLK_m },
            { KeyCodes.N, SDL.SDL_Keycode.SDLK_n },
            { KeyCodes.O, SDL.SDL_Keycode.SDLK_o },
            { KeyCodes.P, SDL.SDL_Keycode.SDLK_p },
            { KeyCodes.Q, SDL.SDL_Keycode.SDLK_q },
            { KeyCodes.R, SDL.SDL_Keycode.SDLK_r },
            { KeyCodes.S, SDL.SDL_Keycode.SDLK_s },
            { KeyCodes.T, SDL.SDL_Keycode.SDLK_t },
            { KeyCodes.U, SDL.SDL_Keycode.SDLK_u },
            { KeyCodes.V, SDL.SDL_Keycode.SDLK_v },
            { KeyCodes.W, SDL.SDL_Keycode.SDLK_w },
            { KeyCodes.X, SDL.SDL_Keycode.SDLK_x },
            { KeyCodes.Y, SDL.SDL_Keycode.SDLK_y },
            { KeyCodes.Z, SDL.SDL_Keycode.SDLK_z },
            { KeyCodes.Delete, SDL.SDL_Keycode.SDLK_DELETE },
            { KeyCodes.CapsLock, SDL.SDL_Keycode.SDLK_CAPSLOCK },
            { KeyCodes.F1, SDL.SDL_Keycode.SDLK_F1 },
            { KeyCodes.F2, SDL.SDL_Keycode.SDLK_F2 },
            { KeyCodes.F3, SDL.SDL_Keycode.SDLK_F3 },
            { KeyCodes.F4, SDL.SDL_Keycode.SDLK_F4 },
            { KeyCodes.F5, SDL.SDL_Keycode.SDLK_F5 },
            { KeyCodes.F6, SDL.SDL_Keycode.SDLK_F6 },
            { KeyCodes.F7, SDL.SDL_Keycode.SDLK_F7 },
            { KeyCodes.F8, SDL.SDL_Keycode.SDLK_F8 },
            { KeyCodes.F9, SDL.SDL_Keycode.SDLK_F9 },
            { KeyCodes.F10, SDL.SDL_Keycode.SDLK_F10 },
            { KeyCodes.F11, SDL.SDL_Keycode.SDLK_F11 },
            { KeyCodes.F12, SDL.SDL_Keycode.SDLK_F12 },
            { KeyCodes.Scroll, SDL.SDL_Keycode.SDLK_SCROLLLOCK },
            { KeyCodes.Pause, SDL.SDL_Keycode.SDLK_PAUSE },
            { KeyCodes.Insert, SDL.SDL_Keycode.SDLK_INSERT },
            { KeyCodes.Home, SDL.SDL_Keycode.SDLK_HOME },
            { KeyCodes.PageUp, SDL.SDL_Keycode.SDLK_PAGEUP },
            { KeyCodes.End, SDL.SDL_Keycode.SDLK_END },
            { KeyCodes.PageDown, SDL.SDL_Keycode.SDLK_PAGEDOWN },
            { KeyCodes.Right, SDL.SDL_Keycode.SDLK_RIGHT },
            { KeyCodes.Left, SDL.SDL_Keycode.SDLK_LEFT },
            { KeyCodes.Down, SDL.SDL_Keycode.SDLK_DOWN },
            { KeyCodes.Up, SDL.SDL_Keycode.SDLK_UP },
            { KeyCodes.NumLock, SDL.SDL_Keycode.SDLK_NUMLOCKCLEAR },
            { KeyCodes.Divide, SDL.SDL_Keycode.SDLK_KP_DIVIDE },
            { KeyCodes.Multiply, SDL.SDL_Keycode.SDLK_KP_MULTIPLY },
            { KeyCodes.Subtract, SDL.SDL_Keycode.SDLK_KP_MINUS },
            { KeyCodes.Add, SDL.SDL_Keycode.SDLK_KP_PLUS },
            { KeyCodes.NumPad1, SDL.SDL_Keycode.SDLK_KP_1 },
            { KeyCodes.NumPad2, SDL.SDL_Keycode.SDLK_KP_2 },
            { KeyCodes.NumPad3, SDL.SDL_Keycode.SDLK_KP_3 },
            { KeyCodes.NumPad4, SDL.SDL_Keycode.SDLK_KP_4 },
            { KeyCodes.NumPad5, SDL.SDL_Keycode.SDLK_KP_5 },
            { KeyCodes.NumPad6, SDL.SDL_Keycode.SDLK_KP_6 },
            { KeyCodes.NumPad7, SDL.SDL_Keycode.SDLK_KP_7 },
            { KeyCodes.NumPad8, SDL.SDL_Keycode.SDLK_KP_8 },
            { KeyCodes.NumPad9, SDL.SDL_Keycode.SDLK_KP_9 },
            { KeyCodes.NumPad0, SDL.SDL_Keycode.SDLK_KP_0 },
            { KeyCodes.Decimal, SDL.SDL_Keycode.SDLK_KP_PERIOD },
            { KeyCodes.Apps, SDL.SDL_Keycode.SDLK_APPLICATION },
            { KeyCodes.F13, SDL.SDL_Keycode.SDLK_F13 },
            { KeyCodes.F14, SDL.SDL_Keycode.SDLK_F14 },
            { KeyCodes.F15, SDL.SDL_Keycode.SDLK_F15 },
            { KeyCodes.F16, SDL.SDL_Keycode.SDLK_F16 },
            { KeyCodes.F17, SDL.SDL_Keycode.SDLK_F17 },
            { KeyCodes.F18, SDL.SDL_Keycode.SDLK_F18 },
            { KeyCodes.F19, SDL.SDL_Keycode.SDLK_F19 },
            { KeyCodes.F20, SDL.SDL_Keycode.SDLK_F20 },
            { KeyCodes.F21, SDL.SDL_Keycode.SDLK_F21 },
            { KeyCodes.F22, SDL.SDL_Keycode.SDLK_F22 },
            { KeyCodes.F23, SDL.SDL_Keycode.SDLK_F23 },
            { KeyCodes.F24, SDL.SDL_Keycode.SDLK_F24 },
            { KeyCodes.Execute, SDL.SDL_Keycode.SDLK_EXECUTE },
            { KeyCodes.Help, SDL.SDL_Keycode.SDLK_HELP },
            { KeyCodes.Select, SDL.SDL_Keycode.SDLK_SELECT },
            { KeyCodes.VolumeUp, SDL.SDL_Keycode.SDLK_VOLUMEUP },
            { KeyCodes.VolumeDown, SDL.SDL_Keycode.SDLK_VOLUMEDOWN },
            { KeyCodes.Separator, SDL.SDL_Keycode.SDLK_SEPARATOR },
            { KeyCodes.Crsel, SDL.SDL_Keycode.SDLK_CRSEL },
            { KeyCodes.Exsel, SDL.SDL_Keycode.SDLK_EXSEL },
            { KeyCodes.LeftControl, SDL.SDL_Keycode.SDLK_LCTRL },
            { KeyCodes.LeftShift, SDL.SDL_Keycode.SDLK_LSHIFT },
            { KeyCodes.LeftAlt, SDL.SDL_Keycode.SDLK_LALT },
            { KeyCodes.LeftWindows, SDL.SDL_Keycode.SDLK_LGUI },
            { KeyCodes.RightControl, SDL.SDL_Keycode.SDLK_RCTRL },
            { KeyCodes.RightShift, SDL.SDL_Keycode.SDLK_RSHIFT },
            { KeyCodes.RightAlt, SDL.SDL_Keycode.SDLK_RALT },
            { KeyCodes.RightWindows, SDL.SDL_Keycode.SDLK_RGUI },
            { KeyCodes.MediaNextTrack, SDL.SDL_Keycode.SDLK_AUDIONEXT },
            { KeyCodes.MediaPreviousTrack, SDL.SDL_Keycode.SDLK_AUDIOPREV },
            { KeyCodes.MediaStop, SDL.SDL_Keycode.SDLK_AUDIOSTOP },
            { KeyCodes.MediaPlayPause, SDL.SDL_Keycode.SDLK_AUDIOPLAY },
            { KeyCodes.VolumeMute, SDL.SDL_Keycode.SDLK_AUDIOMUTE },
            { KeyCodes.SelectMedia, SDL.SDL_Keycode.SDLK_MEDIASELECT },
            { KeyCodes.LaunchMail, SDL.SDL_Keycode.SDLK_MAIL },
            { KeyCodes.BrowserSearch, SDL.SDL_Keycode.SDLK_AC_SEARCH },
            { KeyCodes.BrowserHome, SDL.SDL_Keycode.SDLK_AC_HOME },
            { KeyCodes.BrowserBack, SDL.SDL_Keycode.SDLK_AC_BACK },
            { KeyCodes.BrowserForward, SDL.SDL_Keycode.SDLK_AC_FORWARD },
            { KeyCodes.BrowserStop, SDL.SDL_Keycode.SDLK_AC_STOP },
            { KeyCodes.BrowserRefresh, SDL.SDL_Keycode.SDLK_AC_REFRESH },
            { KeyCodes.BrowserFavorites, SDL.SDL_Keycode.SDLK_AC_BOOKMARKS }
        };
        #endregion


        #region Public Methods
        /// <summary>
        /// Converts the given <paramref name="key"/> of type <see cref="SDL.SDL_Keycode"/> to 
        /// the keycode of type <see cref="KeyCodes"/>.
        /// </summary>
        /// <param name="key">The SDL key code used to map to the standard key code.</param>
        /// <returns></returns>
        public static KeyCodes ToStandardKeyCode(SDL.SDL_Keycode key)
        {
            //Need to recogize the SDLK_RETURN and SDLK_KP_ENTER keys as the same KeyCode.Enter key code
            if (key == SDL.SDL_Keycode.SDLK_RETURN || key == SDL.SDL_Keycode.SDLK_KP_ENTER)
                return SDLToStandardMappings[SDL.SDL_Keycode.SDLK_RETURN];


            return SDLToStandardMappings[key];
        }


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="SDL.SDL_Keycode"/> to 
        /// the keycode of type <see cref="KeyCodes"/>.
        /// </summary>
        /// <param name="key">The SDL key codes used to map to the standard key codes.</param>
        /// <returns></returns>
        public static KeyCodes[] ToStandardKeyCodes(SDL.SDL_Keycode[] keys)
        {
            return keys.Select(k =>
            {
                if (k == SDL.SDL_Keycode.SDLK_RETURN || k == SDL.SDL_Keycode.SDLK_KP_ENTER)
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
        /// the keycode of type <see cref="SDL.SDL_Keycode"/>.
        /// </summary>
        /// <param name="standardKeyCode">The standard key code used to map to the SDL key code.</param>
        /// <returns></returns>
        public static SDL.SDL_Keycode ToSDLKeyCode(KeyCodes standardKeyCode) => StandardToSDLMappings[standardKeyCode];


        /// <summary>
        /// Converts the given <paramref name="keys"/> of type <see cref="KeyCodes"/> to
        /// the keycode of type <see cref="SDL.SDL_Keycode"/>.
        /// </summary>
        /// <param name="key">The standard key codes used to map to the SDL key code.</param>
        /// <returns></returns>
        public static SDL.SDL_Keycode[] ToSDLKeyCodes(KeyCodes[] keys) => (from k in keys select ToSDLKeyCode(k)).ToArray();
        #endregion
    }
}
