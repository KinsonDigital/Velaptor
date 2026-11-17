// <copyright file="ISdlInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

/*
 * RESOURCES:
 * https://wiki.libsdl.org/SDL2/FrontPage
 * https://wiki.libsdl.org/SDL2/CategoryAPI
 * More info on macOS specific joystick detection:
 *   https://github.com/libsdl-org/SDL/issues/12886
 * SDL Game Controller DB:
 *   https://github.com/mdqinc/SDL_GameControllerDB
 */

namespace Velaptor.NativeInterop.SDL;

using Silk.NET.SDL;

internal interface ISdlInvoker
{
    Sdl Sdl { get; }

    bool Init();

    string GetError();

    // TODO: Make sure to add information about low vs high. Low is more intense rumble and high is less intense, but more responsive
    int JoystickRumble(nint joystick, ushort low_frequency_rumble, ushort high_frequency_rumble, uint duration_ms);

    void JoystickClose(nint joystick);

    nint JoystickOpen(int device_index);

    string JoystickName(nint joystick);

    uint NumJoysticks();

    bool JoystickHasRumble(nint joyStickPtr);

    // NOTE: We must call this in the WgpuWindow update loop as long as we are not on macOS.
    // If on macOS, we DO NOT CALL THIS
    void PumpEvents();

    void Quit();
}
