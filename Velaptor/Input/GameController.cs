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
using Carbonate;
using Factories;
using NativeInterop.GLFW;
using Services;
using Silk.NET.Input;

/// <summary>
/// Provides functionality for game controllers.
/// </summary>
internal sealed class GameController : IGameController
{
    private readonly Dictionary<(int deviceIndex, ControllerButton btn), bool> btnStates = new ();
    private readonly Dictionary<int, string> devices = new ();
    private readonly ControllerButton[] controllerButtons = Enum.GetValues<ControllerButton>();
    private readonly IGameControllerRumble gameCtrlRumble;
    private readonly IDisposable initUnsubscriber;
    private readonly IDisposable rescanUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly INativeInputFactory inputFactory;
    private readonly Dictionary<int, GameCtrlThumbstickState> leftGameCtrlThumbstickState = new ();
    private readonly Dictionary<int, GameCtrlThumbstickState> rightGameCtrlThumbstickState = new ();
    private FrozenSet<string> nameNames = FrozenSet<string>.Empty;
    private FrozenSet<int> deviceIds = FrozenSet<int>.Empty;
    private Dictionary<int, GameCtrlTriggerState> leftGameCtrlTriggerState = new ();
    private Dictionary<int, GameCtrlTriggerState> rightGameCtrlTriggerState = new ();
    private bool enabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="gameCtrlRumble">Manages rumble/virations for game controllers.</param>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="resourceLoaderService">Loads the embedded <c>game-controller-db</c> resource file.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="inputFactory">Creates native input objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    public GameController(
        IGameControllerRumble gameCtrlRumble,
        IGlfwInvoker glfwInvoker,
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IReactableFactory reactableFactory,
        INativeInputFactory inputFactory)
    {
        // TODO: Test null params
        ArgumentNullException.ThrowIfNull(gameCtrlRumble);
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(inputFactory);

        this.gameCtrlRumble = gameCtrlRumble;

        var dbData = resourceLoaderService.LoadResource("game-controller-db.txt");
        glfwInvoker.UpdateGamepadMappings(dbData);

        var eventsReactable = reactableFactory.CreateNoDataPushReactable();

        this.inputFactory = inputFactory;

        this.initUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.WgpuReady,
            Init,
            () => this.initUnsubscriber?.Dispose());

        this.rescanUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.Update,
            Rescan,
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
    public IReadOnlyCollection<int> ConnectedDeviceIds => this.deviceIds;

    /// <inheritdoc/>
    public GameControllerState GetState() => GetState(0);

    /// <inheritdoc/>
    public GameControllerState GetState(int id)
    {
        var gameCtrlState = new GameControllerState();
        gameCtrlState = gameCtrlState with { Enabled = Enabled, IsConnected = this.devices.ContainsKey(id), Id = -1 };

        if (!gameCtrlState.IsConnected)
        {
            return gameCtrlState;
        }

        foreach (((var deviceIndex, ControllerButton key), var state) in this.btnStates)
        {
            if (deviceIndex == id)
            {
                gameCtrlState.ButtonStates[key] = this.enabled && state;
            }
        }

        gameCtrlState.Id = id;
        gameCtrlState.Name = this.devices[id];

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
    public bool HasRumble => this.gameCtrlRumble.HasRumble;

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
    /// Rescans for any controllers that might have been connected, or disconnected.
    /// </summary>
    private void Rescan() => this.gameCtrlRumble.ScanForControllers();

    /// <summary>
    /// Initializes the game pad events.
    /// </summary>
    private void Init()
    {
        var inputContext = this.inputFactory.CreateInput();

        inputContext.ConnectionChanged += InputContextOnConnectionChanged;

        foreach (IGamepad gamepad in inputContext.Gamepads)
        {
            // Add the device to the connected device list
            if (gamepad.IsConnected)
            {
                this.devices.Add(gamepad.Index, gamepad.Name);
            }

            // Add all of the button states for this device
            foreach (var ctrlBtn in this.controllerButtons)
            {
                this.btnStates.Add((gamepad.Index, ctrlBtn), false);
            }

            // Create state items for both thumbsticks for the device
            this.leftGameCtrlThumbstickState.Add(gamepad.Index, default);
            this.rightGameCtrlThumbstickState.Add(gamepad.Index, default);

            // Create state items for both triggers for the device
            this.leftGameCtrlTriggerState.Add(gamepad.Index, default);
            this.rightGameCtrlTriggerState.Add(gamepad.Index, default);

            gamepad.ButtonDown += GameCtrl_OnButtonDown;
            gamepad.ButtonUp += GameCtrl_OnButtonUp;
            gamepad.ThumbstickMoved += GameCtrl_OnThumbstickMoved;
            gamepad.TriggerMoved += GameCtrl_OnTriggerMoved;
        }

        this.nameNames = this.devices.Values.ToFrozenSet();
        this.deviceIds = this.devices.Keys.ToFrozenSet();

        this.gameCtrlRumble.Init();
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
    private void InputContextOnConnectionChanged(IInputDevice device, bool isConnected)
    {
        if (device is not IGamepad gamepad)
        {
            return;
        }

        if (isConnected)
        {
            if (gamepad.IsConnected)
            {
                this.devices.Add(gamepad.Index, $"{gamepad.Name}({gamepad.Index})");
            }

            // Add all of the button states for this device
            foreach (var ctrlBtn in this.controllerButtons)
            {
                this.btnStates.Add((gamepad.Index, ctrlBtn), false);
            }

            // Create state items for both thumbsticks for the device
            this.leftGameCtrlThumbstickState.Add(gamepad.Index, default);
            this.rightGameCtrlThumbstickState.Add(gamepad.Index, default);

            // Create state items for both triggers for the device
            this.leftGameCtrlTriggerState.Add(gamepad.Index, default);
            this.rightGameCtrlTriggerState.Add(gamepad.Index, default);

            gamepad.ButtonDown += GameCtrl_OnButtonDown;
            gamepad.ButtonUp += GameCtrl_OnButtonUp;
            gamepad.ThumbstickMoved += GameCtrl_OnThumbstickMoved;
            gamepad.TriggerMoved += GameCtrl_OnTriggerMoved;
        }
        else
        {
            if (!gamepad.IsConnected)
            {
                this.devices.Remove(gamepad.Index);
            }

            gamepad.ButtonDown -= GameCtrl_OnButtonDown;
            gamepad.ButtonUp -= GameCtrl_OnButtonUp;
            gamepad.ThumbstickMoved -= GameCtrl_OnThumbstickMoved;
            gamepad.TriggerMoved -= GameCtrl_OnTriggerMoved;

            foreach (var ctrlBtn in this.controllerButtons)
            {
                var key = (device.Index, ctrlBtn);

                if (this.btnStates.ContainsKey(key))
                {
                    this.btnStates.Remove((gamepad.Index, ctrlBtn));
                }
            }

            // Remove state items for both thumbsticks for the device
            this.leftGameCtrlThumbstickState.Remove(gamepad.Index);
            this.rightGameCtrlThumbstickState.Remove(gamepad.Index);

            // Remove state items for both triggers for the device
            this.leftGameCtrlTriggerState.Remove(gamepad.Index);
            this.rightGameCtrlTriggerState.Remove(gamepad.Index);
        }

        this.nameNames = this.devices.Values.ToFrozenSet();
        this.deviceIds = this.devices.Keys.ToFrozenSet();
    }

    /// <summary>
    /// Invoked when a game controller button transitions to the down position.
    /// </summary>
    private void GameCtrl_OnButtonDown(IGamepad pad, Button btn) => this.btnStates[(pad.Index, (ControllerButton)btn.Name)] = btn.Pressed;

    /// <summary>
    /// Invoked when a game controller button transitions to the up position.
    /// </summary>
    private void GameCtrl_OnButtonUp(IGamepad pad, Button btn) => this.btnStates[(pad.Index, (ControllerButton)btn.Name)] = btn.Pressed;

    /// <summary>
    /// Invoked when the game controller thumbstick states have changed.
    /// </summary>
    private void GameCtrl_OnThumbstickMoved(IGamepad pad, Thumbstick stick)
    {
        var gameCtrlStickData = new GameCtrlThumbstickState
        {
            Direction = stick.Direction, Position = stick.Position, X = stick.X, Y = stick.Y,
        };

        // If the thumbstick is the left thumbstick
        if (pad.LeftThumbstick().Index == stick.Index)
        {
            this.leftGameCtrlThumbstickState[pad.Index] = gameCtrlStickData with { IsLeftStick = true, IsRightStick = false, };
        }

        if (pad.RightThumbstick().Index == stick.Index)
        {
            this.rightGameCtrlThumbstickState[pad.Index] = gameCtrlStickData with { IsRightStick = true, IsLeftStick = false };
        }
    }

    /// <summary>
    /// Invoked when game controller trigger states have changed.
    /// </summary>
    private void GameCtrl_OnTriggerMoved(IGamepad pad, Trigger trigger)
    {
        var gameCtrlTriggerData = new GameCtrlTriggerState { Position = trigger.Position, };

        if (trigger.Index == 0)
        {
            this.leftGameCtrlTriggerState[pad.Index] = gameCtrlTriggerData with { IsLeftTrigger = true, IsRightTrigger = false };
        }

        if (trigger.Index == 1)
        {
            this.rightGameCtrlTriggerState[pad.Index] = gameCtrlTriggerData with { IsRightTrigger = true, IsLeftTrigger = false };
        }
    }
}
