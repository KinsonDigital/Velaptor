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

using System.Runtime.InteropServices;
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

        if (Sdl.Init(Sdl.InitJoystick | Sdl.InitEvents) != 0)
        {
            return false;
        }

        return true;
    }

    public string GetError()
    {
        string errorMsg;

        unsafe
        {
            errorMsg = Marshal.PtrToStringAnsi((nint)Sdl.GetError()) ?? string.Empty;
        }

        return errorMsg;
    }

    public int JoystickRumble(nint joystick, ushort low_frequency_rumble, ushort high_frequency_rumble, uint duration_ms)
    {
        unsafe
        {
            return Sdl.JoystickRumble((Joystick*)joystick, low_frequency_rumble, high_frequency_rumble, duration_ms);
        }
    }

    public void JoystickClose(nint joystick)
    {
        unsafe
        {
            Sdl.JoystickClose((Joystick*)joystick);
        }
    }

    public nint JoystickOpen(int device_index)
    {
        unsafe
        {
            return (nint)Sdl.JoystickOpen(device_index);
        }
    }

    public string JoystickName(nint joystick)
    {
        unsafe
        {
            return Marshal.PtrToStringAnsi((nint)Sdl.JoystickName((Joystick*)joystick)) ?? string.Empty;
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

    public bool JoystickGetDeviceGUID(int device_index)
    {
        var stuff = Sdl.JoystickGetDeviceGUID(device_index);
    }
    
    // NOTE: We must call this in the WgpuWindow update loop as long as we are not on macOS.
    // If on macOS, we DO NOT CALL THIS
    public void PumpEvents() => Sdl.PumpEvents();

    public void Quit() => Sdl.Quit();
}
