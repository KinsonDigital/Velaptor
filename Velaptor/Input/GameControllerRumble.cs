// <copyright file="GameControllerRumble.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NativeInterop.SDL;

// TODO: Add tests

/// <summary>
/// TODO: ADD DOCS
/// </summary>
internal sealed class GameControllerRumble : IGameControllerRumble
{
    private readonly ISdlInvoker sdl;
    private readonly IPlatform platform;
    private readonly List<(string name, nint joyStickPtr)> controllers = [];
    private uint totalJoysticksConnected;
    private bool isDisposed;
    private float lightRumbleIntensity; // The rumble intensity from 0% to 100%
    private float heavyRumbleIntensity; // The rumble intensity from 0% to 100%

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerRumble"/> class.
    /// </summary>
    /// <param name="sdlInvoker">Invokes SDL functions.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public GameControllerRumble(ISdlInvoker sdlInvoker, IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(sdlInvoker);
        ArgumentNullException.ThrowIfNull(platform);

        this.sdl = sdlInvoker;
        this.platform = platform;
    }

    /// <inheritdoc/>
    public void Init()
    {
        this.sdl.Init();
        ScanForControllers();
    }

    /// <inheritdoc/>
    public void ScanForControllers()
    {
        if (this.platform.CurrentPlatform != OSPlatform.OSX)
        {
            this.sdl.PumpEvents();
        }

        var totalSticks = this.sdl.NumJoysticks();

        // If a controller has been connected or disconnected,
        // close all joysticks and reinitialize them.
        if (totalSticks != this.totalJoysticksConnected)
        {
            StopAndDisconnectJoysticks();

            this.controllers.Clear();

            // Connected to each joystick
            for (var deviceIndex = 0; deviceIndex < totalSticks; deviceIndex++)
            {
                var stickPtr = this.sdl.JoystickOpen(deviceIndex);

                // If the joystick could not open
                if (stickPtr == 0)
                {
                    continue;
                }

                var name = this.sdl.JoystickName(stickPtr);

                /*
                 * INFO:
                 * Rumble Probe: intensity 1 out of 65535 for 0 ms is the weakest
                 * possible effect - too weak for the motors to actually move, so
                 * the pad stays silent. but SENDING it runs the whole rumble path
                 * (SDL -> driver -> hardware), so the return code '0' here means the pad really
                 * has working rumble hardware, and when the return code is != '0', it means it doesn't.
                 * On macOS that check goes through Apple's GameController framework
                 * (GCController.haptics). If the pad doesn't expose haptics to
                 * macOS, this fails - and nothing in this code can change that.
                 */
                if (this.sdl.JoystickRumble(stickPtr, 1, 1, 0) == 0)
                {
                    this.controllers.Add((name, stickPtr));
                }
                else
                {
                    this.sdl.JoystickClose(stickPtr);
                }
            }
        }

        this.totalJoysticksConnected = totalSticks;
    }

    /// <inheritdoc/>
    public float HeavyIntensity => this.heavyRumbleIntensity;

    /// <inheritdoc/>
    public float LightIntensity => this.lightRumbleIntensity;

    // TODO: This will probably have to become a method with a parameter to ask for which controller we want to query
    public bool HasRumble
    {
        get
        {
            // TODO: This will have to be changed to represent a single controller,
            // once we figure out how to represent multiple controllers.
            foreach (var (_, ctrlPtr) in this.controllers)
            {
                return this.sdl.JoystickHasRumble(ctrlPtr);
            }

            return false;
        }
    }

    // RUMBLES FOREVER ~49 DAYS
    public void RumbleLight(float intensity) => RumbleLight(intensity, uint.MaxValue);

    public void RumbleLight(float intensity, uint durationMs)
    {
        intensity = intensity < 0 ? 0 : intensity;
        intensity = intensity > 100 ? 100 : intensity;
        this.lightRumbleIntensity = intensity;

        // Map the value from 0% to 100% to a value that the low level SDL API understands.
        var heavyIntensity = (ushort)this.heavyRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);
        var lightIntensity = (ushort)this.lightRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);

        foreach (var (_, ctrlPtr) in this.controllers)
        {
            this.sdl.JoystickRumble(ctrlPtr, heavyIntensity, lightIntensity, durationMs);
        }
    }

    // RUMBLES FOREVER ~49 DAYS
    public void RumbleHeavy(float intensity) => RumbleHeavy(intensity, uint.MaxValue);

    public void RumbleHeavy(float intensity, uint durationMs)
    {
        intensity = intensity < 0 ? 0 : intensity;
        intensity = intensity > 100 ? 100 : intensity;
        this.heavyRumbleIntensity = intensity;

        // Map the value from 0% to 100% to a value that the low level SDL API understands.
        var heavyIntensity = (ushort)this.heavyRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);
        var lightIntensity = (ushort)this.lightRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);

        foreach (var (_, ctrlPtr) in this.controllers)
        {
            this.sdl.JoystickRumble(ctrlPtr, heavyIntensity, lightIntensity, durationMs);
        }
    }

    public void StopLightRumble()
    {
        this.lightRumbleIntensity = 0;

        var heavyIntensity = (ushort)this.heavyRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);

        foreach (var (_, ctrlPtr) in this.controllers)
        {
            this.sdl.JoystickRumble(ctrlPtr, heavyIntensity, 0, 0);
        }
    }

    public void StopHeavyRumble()
    {
        this.heavyRumbleIntensity = 0;

        var lightIntensity = (ushort)this.lightRumbleIntensity.MapValue(0f, 100f, 0, ushort.MaxValue);

        foreach (var (_, ctrlPtr) in this.controllers)
        {
            this.sdl.JoystickRumble(ctrlPtr, 0, lightIntensity, 0);
        }
    }

    public void StopAllRumble()
    {
        this.lightRumbleIntensity = 0;
        this.heavyRumbleIntensity = 0;

        foreach (var (_, ctrlPtr) in this.controllers)
        {
            this.sdl.JoystickRumble(ctrlPtr, 0, 0, 0);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        StopAndDisconnectJoysticks();

        this.isDisposed = true;
    }

    /// <summary>
    /// Stops any running vibrations and closes the joysticks.
    /// </summary>
    private void StopAndDisconnectJoysticks()
    {
        // Stop any running effect and close all rumblers we had, then enumerate from
        // scratch. Device indices shift when something is unplugged, so
        // incremental tracking would be fragile - a full rescan on count
        // changes is simple and cheap (it only runs on plug/unplug events).
        foreach (var (_, joyStickPtr) in this.controllers)
        {
            // Turns off any vibration
            this.sdl.JoystickRumble(joyStickPtr, 0, 0, 0u);
            this.sdl.JoystickClose(joyStickPtr);
        }
    }
}
