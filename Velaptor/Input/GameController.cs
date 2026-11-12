// <copyright file="GameController.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1013
namespace Velaptor.Input;

using System;
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
internal sealed class GameController : IAppInput<GameControllerState>
{
    private static readonly int Capacity = Enum.GetNames<ControllerButton>().Length;
    private readonly Dictionary<ControllerButton, bool> btnStates = new (Capacity);
    private readonly IDisposable initUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly INativeInputFactory inputFactory;
    private GameCtrlThumbstickState leftGameCtrlThumbstickState;
    private GameCtrlThumbstickState rightGameCtrlThumbstickState;
    private GameCtrlTriggerState leftGameCtrlTriggerState;
    private GameCtrlTriggerState rightGameCtrlTriggerState;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="glfwInvoker">Invokes GLFW functions.</param>
    /// <param name="resourceLoaderService">Loads the embedded <c>game-controller-db</c> resource file.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="inputFactory">Creates native input objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    public GameController(
        IGlfwInvoker glfwInvoker,
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IReactableFactory reactableFactory,
        INativeInputFactory inputFactory)
    {
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(inputFactory);

        var dbData = resourceLoaderService.LoadResource("game-controller-db.txt");
        glfwInvoker.UpdateGamepadMappings(dbData);

        var eventsReactable = reactableFactory.CreateNoDataPushReactable();

        this.inputFactory = inputFactory;

        this.initUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.WgpuReady,
            Init,
            () => this.initUnsubscriber?.Dispose());

        this.shutDownUnsubscriber = eventsReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.shutDownUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public int TotalDevices => this.inputFactory.CreateInput().Gamepads.Count;

    /// <inheritdoc/>
    public IReadOnlyCollection<string> ConnectedDevices =>
        [.. this.inputFactory.CreateInput().Gamepads.Select((pad) => $"{pad.Name}({pad.Index})")];

    /// <summary>
    /// Gets the current state of the game controller.
    /// </summary>
    /// <returns>The date of the game controller.</returns>
    public GameControllerState GetState()
    {
        var gameCtrlState = new GameControllerState();

        foreach ((ControllerButton key, var state) in this.btnStates)
        {
            gameCtrlState.ButtonStates[key] = state;
        }

        gameCtrlState.LeftGameCtrlThumbstick = this.leftGameCtrlThumbstickState;
        gameCtrlState.RightGameCtrlThumbstick = this.rightGameCtrlThumbstickState;
        gameCtrlState.LeftGameCtrlTrigger = this.leftGameCtrlTriggerState;
        gameCtrlState.RightGameCtrlTrigger = this.rightGameCtrlTriggerState;

        return gameCtrlState;
    }

    /// <summary>
    /// Initializes the game pad events.
    /// </summary>
    private void Init()
    {
        var inputContext = this.inputFactory.CreateInput();
        inputContext.Gamepads[0].ButtonDown += GameCtrl_OnButtonDown;
        inputContext.Gamepads[0].ButtonUp += GameCtrl_OnButtonUp;
        inputContext.Gamepads[0].ThumbstickMoved += GameCtrl_OnThumbstickMoved;
        inputContext.Gamepads[0].TriggerMoved += GameCtrl_OnTriggerMoved;
    }

    /// <summary>
    /// Unsubscribes from all of the game pad events.
    /// </summary>
    private void ShutDown()
    {
        var inputContext = this.inputFactory.CreateInput();
        inputContext.Gamepads[0].ButtonDown -= GameCtrl_OnButtonDown;
        inputContext.Gamepads[0].ButtonUp -= GameCtrl_OnButtonUp;
        inputContext.Gamepads[0].ThumbstickMoved -= GameCtrl_OnThumbstickMoved;
        inputContext.Gamepads[0].TriggerMoved -= GameCtrl_OnTriggerMoved;
    }

    /// <summary>
    /// Invoked when a game controller button transitions to the down position.
    /// </summary>
    private void GameCtrl_OnButtonDown(IGamepad pad, Button btn) => this.btnStates[(ControllerButton)btn.Name] = btn.Pressed;

    /// <summary>
    /// Invoked when a game controller button transitions to the up position.
    /// </summary>
    private void GameCtrl_OnButtonUp(IGamepad pad, Button btn) => this.btnStates[(ControllerButton)btn.Name] = btn.Pressed;

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
            this.leftGameCtrlThumbstickState = gameCtrlStickData with { IsLeftStick = true, IsRightStick = false, };
        }

        if (pad.RightThumbstick().Index == stick.Index)
        {
            this.rightGameCtrlThumbstickState = gameCtrlStickData with { IsRightStick = true, IsLeftStick = false };
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
            this.leftGameCtrlTriggerState = gameCtrlTriggerData with { IsLeftTrigger = true, IsRightTrigger = false };
        }

        if (trigger.Index == 1)
        {
            this.rightGameCtrlTriggerState = gameCtrlTriggerData with { IsRightTrigger = true, IsLeftTrigger = false };
        }
    }
}
