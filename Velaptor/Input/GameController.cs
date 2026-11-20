// <copyright file="GameController.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1013
namespace Velaptor.Input;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Carbonate;
using Factories;
using NativeInterop.GLFW;
using NativeInterop.SDL;
using Services;
using Silk.NET.Input;

/// <summary>
/// Provides functionality for game controllers.
/// </summary>
internal sealed class GameController : IGameController
{
    private const string WindowsSection = "# Windows";
    private const string MacOsSection = "# Mac OS X";
    private const string LinuxSection = "# Linux";
    private readonly Dictionary<(string deviceId, ControllerButton btn), bool> btnStates = new ();
    private readonly Dictionary<string, (int deviceIndex, string name, nint ctrlPtr)> devices = new (); // GUID, name
    private readonly ControllerButton[] controllerButtons = Enum.GetValues<ControllerButton>();
    private readonly IGameControllerRumble gameCtrlRumble;
    private readonly IGlfwInvoker glfwInvoker;
    private readonly ISdlInvoker sdlInvoker;
    private readonly IPlatform platform;
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
    private readonly IDisposable initUnsubscriber;
    private readonly IDisposable rescanUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly INativeInputFactory inputFactory;
    private readonly Dictionary<string, GameCtrlThumbstickState> leftGameCtrlThumbstickState = new ();
    private readonly Dictionary<string, GameCtrlThumbstickState> rightGameCtrlThumbstickState = new ();
    private FrozenSet<string> nameNames = FrozenSet<string>.Empty;
    private FrozenSet<string> deviceIds = FrozenSet<string>.Empty;
    private Dictionary<string, GameCtrlTriggerState> leftGameCtrlTriggerState = new ();
    private Dictionary<string, GameCtrlTriggerState> rightGameCtrlTriggerState = new ();
    private bool enabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="gameCtrlRumble">Manages rumble/virations for game controllers.</param>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="sdlInvoker">Invokes SDL functions.</param>
    /// <param name="resourceLoaderService">Loads the embedded <c>game-controller-db</c> resource file.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="inputFactory">Creates native input objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    public GameController(
        IGameControllerRumble gameCtrlRumble,
        IGlfwInvoker glfwInvoker,
        ISdlInvoker sdlInvoker,
        IPlatform platform,
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IReactableFactory reactableFactory,
        INativeInputFactory inputFactory // TODO: I think this is going to go away because this is GLFW
    )
    {
        // TODO: Test null params
        ArgumentNullException.ThrowIfNull(gameCtrlRumble);
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(sdlInvoker);
        ArgumentNullException.ThrowIfNull(platform);
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(inputFactory);

        this.gameCtrlRumble = gameCtrlRumble;
        this.glfwInvoker = glfwInvoker;
        this.sdlInvoker = sdlInvoker;
        this.platform = platform;
        this.resourceLoaderService = resourceLoaderService;

        var eventsReactable = reactableFactory.CreateNoDataPushReactable();

        this.inputFactory = inputFactory;

        this.initUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.WgpuReady,
            Init,
            () => this.initUnsubscriber?.Dispose());

        this.rescanUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.Update,
            Update,
            () => this.rescanUnsubscriber?.Dispose());

        this.shutDownUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.shutDownUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public int TotalDevices => this.inputFactory.CreateInput().Gamepads.Where(i => i.IsConnected).ToArray().Length;

    /// <inheritdoc/>
    public IReadOnlyCollection<string> ConnectedDeviceNames => this.nameNames;

    /// <inheritdoc/>
    public IReadOnlyCollection<string> ConnectedDeviceIds => this.deviceIds;

    /// <inheritdoc/>
    public GameControllerState GetState()
    {
        return default;
    }

    /// <inheritdoc/>
    public GameControllerState GetState(string id)
    {
        // TODO: This needs to change. All we are going to need to do here is to collect the state and 
        // simply return.  The state will be populated by the Update() method. 
        var result = this.devices.TryGetValue(id, out (int deviceIndex, string name, nint ctrlPtr) device);

        if (!result)
        {
            return new GameControllerState { Enabled = Enabled, IsConnected = false, Id = string.Empty };
        }

        var isConnected = this.sdlInvoker.GameControllerGetAttached(device.ctrlPtr);

        var gameCtrlState = new GameControllerState();
        gameCtrlState = gameCtrlState with { Enabled = Enabled, IsConnected = isConnected, Id = string.Empty };

        if (!gameCtrlState.IsConnected)
        {
            return gameCtrlState;
        }

        foreach (((var deviceId, ControllerButton key), var state) in this.btnStates)
        {
            if (deviceId == id)
            {
                gameCtrlState.ButtonStates[key] = this.enabled && state;
            }
        }

        gameCtrlState.Id = id;
        gameCtrlState.Name = this.devices[id].name;

        gameCtrlState.LightRumbleIntensity = this.enabled ? this.gameCtrlRumble.LightIntensity : 0;
        gameCtrlState.HeavyRumbleIntensity = this.enabled ? this.gameCtrlRumble.HeavyIntensity : 0;

        gameCtrlState.LeftGameCtrlThumbstick = this.enabled ? this.leftGameCtrlThumbstickState[id] : default;
        gameCtrlState.RightGameCtrlThumbstick = this.enabled ? this.rightGameCtrlThumbstickState[id] : default;

        gameCtrlState.LeftGameCtrlTrigger = this.enabled ? this.leftGameCtrlTriggerState[id] : default;
        gameCtrlState.RightGameCtrlTrigger = this.enabled ? this.rightGameCtrlTriggerState[id] : default;

        return gameCtrlState;
    }

    public bool Enabled
    {
        get => this.enabled;
        set
        {
            this.enabled = value;
            this.gameCtrlRumble.StopAllRumble();
        }
    }

    // TODO: This will probably become a method once we add multi-controller support
    public bool HasRumble(string guid) => this.gameCtrlRumble.HasRumble(guid);

    public void RumbleLight(float intensity) => RumbleLight(intensity, uint.MaxValue);

    public void RumbleLight(float intensity, uint durationMs)
    {
        if (!Enabled)
        {
            return;
        }

        this.gameCtrlRumble.RumbleLight(intensity, durationMs);
    }

    public void RumbleHeavy(float intensity) => RumbleHeavy(intensity, uint.MaxValue);

    public void RumbleHeavy(float intensity, uint durationMs)
    {
        if (!Enabled)
        {
            return;
        }

        this.gameCtrlRumble.RumbleHeavy(intensity, durationMs);
    }

    public void StopLightRumble()
    {
        if (!Enabled)
        {
            return;
        }

        this.gameCtrlRumble.StopLightRumble();
    }

    public void StopHeavyRumble()
    {
        if (!Enabled)
        {
            return;
        }

        this.gameCtrlRumble.StopHeavyRumble();
    }

    public void StopAllRumble()
    {
        if (!Enabled)
        {
            return;
        }

        this.gameCtrlRumble.StopAllRumble();
    }

    /// <summary>
    /// Initializes the game pad events.
    /// </summary>
    private void Init()
    {
        this.sdlInvoker.Init();
        LoadControllerMappings();

        var totalJoysticks = this.sdlInvoker.NumJoysticks();

        for (var deviceIndex = 0; deviceIndex < totalJoysticks; deviceIndex++)
        {
            var ctrlPtr = this.sdlInvoker.GameControllerOpen(deviceIndex);
            var ctrlGuid = this.sdlInvoker.JoystickGetDeviceGUIDString(deviceIndex);
            var deviceName = this.sdlInvoker.GameControllerName(ctrlPtr);

            this.devices.Add(ctrlGuid, (deviceIndex, deviceName, ctrlPtr));

            // Add all of the button states for this device
            foreach (var ctrlBtn in this.controllerButtons)
            {
                this.btnStates.Add((ctrlGuid, ctrlBtn), false);
            }

            // Create state items for both thumbsticks for the device
            this.leftGameCtrlThumbstickState.Add(ctrlGuid, default);
            this.rightGameCtrlThumbstickState.Add(ctrlGuid, default);

            // Create state items for both triggers for the device
            this.leftGameCtrlTriggerState.Add(ctrlGuid, default);
            this.rightGameCtrlTriggerState.Add(ctrlGuid, default);
        }

        this.nameNames = this.devices.Values.Select(d => d.name).ToFrozenSet();
        this.deviceIds = this.devices.Keys.ToFrozenSet();
    }

    private void LoadControllerMappings()
    {
        var dbData = this.resourceLoaderService.LoadResource("game-controller-db.txt");

        var platformSection = string.Empty;
        var otherSections = new List<string>();

        if (this.platform.CurrentPlatform == OSPlatform.Windows)
        {
            platformSection = WindowsSection;
            otherSections.Add(MacOsSection);
            otherSections.Add(LinuxSection);
        }
        else if (this.platform.CurrentPlatform == OSPlatform.OSX)
        {
            platformSection = MacOsSection;
            otherSections.Add(WindowsSection);
            otherSections.Add(LinuxSection);
        }
        else if (this.platform.CurrentPlatform == OSPlatform.Linux)
        {
            platformSection = LinuxSection;
            otherSections.Add(MacOsSection);
            otherSections.Add(WindowsSection);
        }
        else
        {
            // TODO: THROW EXCEPTION
        }

        var ifSectionReached = false;

        foreach (var line in dbData.AsSpan().EnumerateLines())
        {
            if (line.Length <= 0)
            {
                continue;
            }

            // If the section has been reached
            if (line.StartsWith(platformSection))
            {
                ifSectionReached = true;
                continue;
            }

            if (!ifSectionReached)
            {
                continue;
            }

            // If a new section has been detected, we are finished loading the OS specific
            // controller db mappings
            if (otherSections.Any(i => i != platformSection))
            {
                break;
            }

            this.sdlInvoker.GameControllerAddMapping(line.ToString());
        }
    }

    /// <summary>
    /// Check for controller changes
    /// </summary>
    private void Update()
    {
    }

    /// <summary>
    /// Unsubscribes from all of the game pad events.
    /// </summary>
    private void ShutDown()
    {
        var inputContext = this.inputFactory.CreateInput();

        foreach (IGamepad gamepad in inputContext.Gamepads)
        {
            gamepad.ButtonDown -= GameCtrl_OnButtonDown;
            gamepad.ButtonUp -= GameCtrl_OnButtonUp;
            gamepad.ThumbstickMoved -= GameCtrl_OnThumbstickMoved;
            gamepad.TriggerMoved -= GameCtrl_OnTriggerMoved;
        }

        this.gameCtrlRumble.Dispose();
    }

    /// <summary>
    /// Invoked when a device is connected or disconnected.
    /// </summary>
    private void InputContext_OnConnectionChanged(IInputDevice device, bool isConnected)
    {
        // TODO: This is going to go away
        return;
        // if (device is not IGamepad gamepad)
        // {
        //     return;
        // }
        //
        // var ctrlGuid = this.glfwInvoker.GetJoystickGUID(gamepad.Index);
        //
        // if (isConnected)
        // {
        //     if (gamepad.IsConnected)
        //     {
        //         this.devices.Add(ctrlGuid, gamepad.Name);
        //     }
        //
        //     // Add all of the button states for this device
        //     foreach (var ctrlBtn in this.controllerButtons)
        //     {
        //         this.btnStates.Add((ctrlGuid, ctrlBtn), false);
        //     }
        //
        //     // Create state items for both thumbsticks for the device
        //     this.leftGameCtrlThumbstickState.Add(ctrlGuid, default);
        //     this.rightGameCtrlThumbstickState.Add(ctrlGuid, default);
        //
        //     // Create state items for both triggers for the device
        //     this.leftGameCtrlTriggerState.Add(ctrlGuid, default);
        //     this.rightGameCtrlTriggerState.Add(ctrlGuid, default);
        //
        //     gamepad.ButtonDown += GameCtrl_OnButtonDown;
        //     gamepad.ButtonUp += GameCtrl_OnButtonUp;
        //     gamepad.ThumbstickMoved += GameCtrl_OnThumbstickMoved;
        //     gamepad.TriggerMoved += GameCtrl_OnTriggerMoved;
        // }
        // else
        // {
        //     if (!gamepad.IsConnected)
        //     {
        //         this.devices.Remove(ctrlGuid);
        //     }
        //
        //     gamepad.ButtonDown -= GameCtrl_OnButtonDown;
        //     gamepad.ButtonUp -= GameCtrl_OnButtonUp;
        //     gamepad.ThumbstickMoved -= GameCtrl_OnThumbstickMoved;
        //     gamepad.TriggerMoved -= GameCtrl_OnTriggerMoved;
        //
        //     foreach (var ctrlBtn in this.controllerButtons)
        //     {
        //         if (this.btnStates.ContainsKey((ctrlGuid, ctrlBtn)))
        //         {
        //             this.btnStates.Remove((ctrlGuid, ctrlBtn));
        //         }
        //     }
        //
        //     // Remove state items for both thumbsticks for the device
        //     this.leftGameCtrlThumbstickState.Remove(ctrlGuid);
        //     this.rightGameCtrlThumbstickState.Remove(ctrlGuid);
        //
        //     // Remove state items for both triggers for the device
        //     this.leftGameCtrlTriggerState.Remove(ctrlGuid);
        //     this.rightGameCtrlTriggerState.Remove(ctrlGuid);
        // }
        //
        // // TODO: Call rumble update controllers here
        // this.gameCtrlRumble.UpdateControllers(this.devices.Keys.ToArray());
        //
        // this.nameNames = this.devices.Values.Select(d => d).ToFrozenSet();
        // this.deviceIds = this.devices.Keys.ToFrozenSet();
    }

    /// <summary>
    /// Invoked when a game controller button transitions to the down position.
    /// </summary>
    private void GameCtrl_OnButtonDown(IGamepad pad, Button btn)
    {
        var guid = this.glfwInvoker.GetJoystickGUID(pad.Index);
        this.btnStates[(guid, (ControllerButton)btn.Name)] = btn.Pressed;
    }

    /// <summary>
    /// Invoked when a game controller button transitions to the up position.
    /// </summary>
    private void GameCtrl_OnButtonUp(IGamepad pad, Button btn)
    {
        var guid = this.glfwInvoker.GetJoystickGUID(pad.Index);

        this.btnStates[(guid, (ControllerButton)btn.Name)] = btn.Pressed;
    }

    /// <summary>
    /// Invoked when the game controller thumbstick states have changed.
    /// </summary>
    private void GameCtrl_OnThumbstickMoved(IGamepad pad, Thumbstick stick)
    {
        var gameCtrlStickData = new GameCtrlThumbstickState
        {
            Direction = stick.Direction, Position = stick.Position, X = stick.X, Y = stick.Y,
        };

        if (pad.LeftThumbstick().Index != stick.Index && pad.RightThumbstick().Index != stick.Index)
        {
            return;
        }

        var guid = this.glfwInvoker.GetJoystickGUID(pad.Index);

        // If the thumbstick is the left thumbstick
        if (pad.LeftThumbstick().Index == stick.Index)
        {
            this.leftGameCtrlThumbstickState[guid] = gameCtrlStickData with { IsLeftStick = true, IsRightStick = false, };
        }

        if (pad.RightThumbstick().Index == stick.Index)
        {
            this.rightGameCtrlThumbstickState[guid] = gameCtrlStickData with { IsRightStick = true, IsLeftStick = false };
        }
    }

    /// <summary>
    /// Invoked when game controller trigger states have changed.
    /// </summary>
    private void GameCtrl_OnTriggerMoved(IGamepad pad, Trigger trigger)
    {
        var gameCtrlTriggerData = new GameCtrlTriggerState { Position = trigger.Position, };

        if (trigger.Index is not (0 or 1))
        {
            return;
        }

        var guid = this.glfwInvoker.GetJoystickGUID(pad.Index);

        switch (trigger.Index)
        {
            case 0:
                this.leftGameCtrlTriggerState[guid] = gameCtrlTriggerData with { IsLeftTrigger = true, IsRightTrigger = false };
                break;
            case 1:
                this.rightGameCtrlTriggerState[guid] = gameCtrlTriggerData with { IsRightTrigger = true, IsLeftTrigger = false };
                break;
        }
    }
}
