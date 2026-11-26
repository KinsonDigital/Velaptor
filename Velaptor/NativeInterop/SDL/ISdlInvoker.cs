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

    int PollEvent(ref Event @event);

    // NOTE: We must call this in the WgpuWindow update loop as long as we are not on macOS.
    // If on macOS, we DO NOT CALL THIS
    void PumpEvents();

    void Quit();

    string GetError();

    void ClearError();

    // TODO: Make sure to add information about low vs high. Low is more intense rumble and high is less intense, but more responsive
    int JoystickRumble(nint joystick, ushort low_frequency_rumble, ushort high_frequency_rumble, uint duration_ms);

    /*
     * NOTE: The 'device_index' of the members below is the position of the joystick in SDL's
     * current list of attached joysticks (in the range 0..NumJoysticks()-1). It shifts whenever
     * any joystick is plugged in or unplugged and is NOT a stable identity. Use
     * JoystickGetDeviceGUIDString to match devices across the GLFW and SDL libraries.
     */

    nint GameControllerOpen(int joystick_index);

    void GameControllerClose(nint gameController);

    string GameControllerName(nint gameControllerPtr);

    bool GameControllerGetAttached(nint gameControllerPtr);

    byte GameControllerGetButton(nint gameController, GameControllerButton button);

    short GameControllerGetAxis(nint gameControllerPtr, GameControllerAxis axis);

    /// <summary>
    /// Gets the SDL compatible GUID of the joystick at the given <paramref name="device_index"/>,
    /// as a 32 character hexadecimal string. Can be called before any joysticks are opened.
    /// </summary>
    /// <param name="device_index">The index of the joystick in SDL's current list of attached joysticks.</param>
    /// <returns>The GUID string, or an empty string if the index is invalid.</returns>
    string JoystickGetDeviceGUIDString(int device_index);

    int GameControllerAddMapping(string mappingData);

    /// <summary>
    /// Gets the instance id of the joystick at the given <paramref name="device_index"/> without opening it.
    /// </summary>
    /// <param name="device_index">The index of the joystick in SDL's current list of attached joysticks.</param>
    /// <returns>The instance id, or -1 if the index is invalid.</returns>
    int JoystickGetDeviceInstanceID(int device_index);

    /// <summary>
    /// Gets whether the given opened joystick is still attached to the system.
    /// </summary>
    /// <param name="joystick">The pointer to the opened joystick.</param>
    /// <returns><c>true</c> if the joystick is still attached.</returns>
    bool JoystickGetAttached(nint joystick);

    uint NumJoysticks();

    bool JoystickHasRumble(nint joyStickPtr);
}
