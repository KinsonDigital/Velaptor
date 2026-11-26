// <copyright file="SdlInvoker.cs" company="KinsonDigital">
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

using System;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.SDL;

internal sealed class SdlInvoker : ISdlInvoker
{
    public SdlInvoker()
    {
        Sdl = Sdl.GetApi();
    }

    public Sdl Sdl { get; }

    public bool Init()
    {
        // TODO: Check if on macos and add 'Sdl.InitVideo' to flags

        if (Sdl.Init(Sdl.InitJoystick | Sdl.InitEvents | Sdl.InitGamecontroller) != 0)
        {
            return false;
        }

        return true;
    }

    // NOTE: We must call this in the WgpuWindow update loop as long as we are not on macOS.
    // If on macOS, we DO NOT CALL THIS
    public void PumpEvents() => Sdl.PumpEvents();

    public int PollEvent(ref Event @event) => Sdl.PollEvent(ref @event);

    public void Quit() => Sdl.Quit();

    public string GetError()
    {
        string errorMsg;

        unsafe
        {
            errorMsg = Marshal.PtrToStringAnsi((nint)Sdl.GetError()) ?? string.Empty;
        }

        return errorMsg;
    }

    public void ClearError() => Sdl.ClearError();

    public int JoystickRumble(nint joystick, ushort low_frequency_rumble, ushort high_frequency_rumble, uint duration_ms)
    {
        unsafe
        {
            return Sdl.JoystickRumble((Joystick*)joystick, low_frequency_rumble, high_frequency_rumble, duration_ms);
        }
    }

    /// <summary>
    /// Gets the 32-char hex GUID of a connected controller from its SDL instance ID.
    /// </summary>
    /// <param name="instanceId">The SDL instance ID of the connection.</param>
    /// <returns>The GUID string, or <see cref="string.Empty"/> if the device is not attached.</returns>
    public string GetGuidFromInstanceId(int instanceId)
    {
        var total = NumJoysticks();
        for (var i = 0; i < total; i++)
        {
            if (JoystickGetDeviceInstanceID(i) == instanceId)
            {
                return JoystickGetDeviceGUIDString(i);
            }
        }

        return string.Empty;
    }

    public nint GameControllerOpen(int joystick_index)
    {
        unsafe
        {
            var controller = Sdl.GameControllerOpen(joystick_index);

            return (nint)controller;
        }
    }

    public void GameControllerClose(nint gameController)
    {
        unsafe
        {
            Sdl.GameControllerClose((GameController*)gameController);
        }
    }

    public string GameControllerName(nint gameControllerPtr)
    {
        unsafe
        {
            var name = (nint)Sdl.GameControllerName((GameController*)gameControllerPtr);

            var controllerName = Marshal.PtrToStringAnsi(name);

            return controllerName;
        }
    }

    public bool GameControllerGetAttached(nint gameControllerPtr)
    {
        unsafe
        {
            var result = Sdl.GameControllerGetAttached((GameController*)gameControllerPtr);

            return result == SdlBool.True;
        }
    }

    public byte GameControllerGetButton(nint gameController, GameControllerButton button)
    {
        unsafe
        {
            return Sdl.GameControllerGetButton((GameController*)gameController, button);
        }
    }

    public short GameControllerGetAxis(nint gameControllerPtr, GameControllerAxis axis)
    {
        unsafe
        {
            return Sdl.GameControllerGetAxis((GameController*)gameControllerPtr, axis);
        }
    }

    // TODO: Look into setting the ID (GUID) into a 'Guid' type.
    // Probably it is best to convert the string to an actual 'Guid' type in the sdl invoker.  This is where it comes from as a source.
    public string JoystickGetDeviceGUIDString(int device_index)
    {
        var guid = Sdl.JoystickGetDeviceGUID(device_index);

        unsafe
        {
            var buffer = stackalloc byte[33];

            Sdl.JoystickGetGUIDString(guid, buffer, 33);

            var guidChars = new ReadOnlySpan<byte>(buffer, 33);
            var terminatorIndex = guidChars.IndexOf((byte)0);
            var length = terminatorIndex == -1 ? 32 : terminatorIndex;

            return Encoding.ASCII.GetString(guidChars[..length]);
        }
    }

    public int GameControllerAddMapping(string mappingData)
    {
        // TODO: Figure out what the return int value is and choose how to handle it
        return Sdl.GameControllerAddMapping(mappingData);
    }

    public int JoystickGetDeviceInstanceID(int device_index) => Sdl.JoystickGetDeviceInstanceID(device_index);

    public bool JoystickGetAttached(nint joystick)
    {
        unsafe
        {
            return Sdl.JoystickGetAttached((Joystick*)joystick) == SdlBool.True;
        }
    }

    public uint NumJoysticks() => (uint)Sdl.NumJoysticks();

    public bool JoystickHasRumble(nint joyStickPtr)
    {
        unsafe
        {
            return Sdl.JoystickHasRumble((Joystick*)joyStickPtr) == SdlBool.True;
        }
    }
}
