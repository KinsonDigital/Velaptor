// <copyright file="GameController.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // Do not use default value type constructor
namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Data;
using Carbonate;
using Carbonate.OneWay;
using Factories;
using NativeInterop.GLFW;
using ReactableData;
using Services;

internal sealed class GameController : IAppInput<GameControllerState>
{
    private static readonly int Capacity = Enum.GetNames<ControllerButton>().Length;
    private readonly Dictionary<ControllerButton, bool> btnStates = new (Capacity);
    private readonly IDisposable gameCtrlButtonUnsubscriber;
    private readonly IDisposable gameCtrlThumbstickUnsubscriber;
    private readonly IDisposable gameCtrlTriggerUnsubscriber;
    private GameCtrlThumbstickStateData leftGameCtrlThumbstickStateData;
    private GameCtrlThumbstickStateData rightGameCtrlThumbstickStateData;
    private GameCtrlTriggerStateData leftGameCtrlTriggerStateData;
    private GameCtrlTriggerStateData rightGameCtrlTriggerStateData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="glfwInvoker"></param>
    /// <param name="resourceLoaderService"></param>
    /// <param name="reactableFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public GameController(
        IGlfwInvoker glfwInvoker,
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(glfwInvoker);
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        var dbData = resourceLoaderService.LoadResource("game-controller-db.txt");
        glfwInvoker.UpdateGamepadMappings(dbData);

        var gameCtrlDataReactable = reactableFactory.CreateGameCtrlButtonReactable();
        var gameCtrlThumbstickReactable = reactableFactory.CreateGameCtrlThumbstickReactable();
        var gameCtrlTriggerReactable = reactableFactory.CreateGameCtrlTriggerReactable();

        this.gameCtrlButtonUnsubscriber = gameCtrlDataReactable.CreateOneWayReceive(
            PushNotifications.GameCtrlButtonStateChangedId,
            (data) =>
            {
                this.btnStates[data.Button] = data.IsDown;
            },
            () => this.gameCtrlButtonUnsubscriber?.Dispose());

        this.gameCtrlThumbstickUnsubscriber = gameCtrlThumbstickReactable.CreateOneWayReceive(
            PushNotifications.GameCtrlThumbstickStateChangedId,
            (data) =>
            {
                switch (data)
                {
                    case { IsLeftStick: true, IsRightStick: false }:
                        this.leftGameCtrlThumbstickStateData = data;
                        break;
                    case { IsRightStick: true, IsLeftStick: false }:
                        this.rightGameCtrlThumbstickStateData = data;
                        break;
                    default:
                        // TODO: Change to better exception
                        throw new Exception("Game controller thumbstick not recognized.");
                }
            },
            () => this.gameCtrlThumbstickUnsubscriber?.Dispose());

        this.gameCtrlTriggerUnsubscriber = gameCtrlTriggerReactable.CreateOneWayReceive(
            PushNotifications.GameCtrlTriggerStateChangedId,
            (data) =>
            {
                switch (data)
                {
                    case { IsLeftTrigger: true, IsRightTrigger: false }:
                        this.leftGameCtrlTriggerStateData = data;
                        break;
                    case { IsRightTrigger: true, IsLeftTrigger: false }:
                        this.rightGameCtrlTriggerStateData = data;
                        break;
                    default:
                        // TODO: Change to better exception
                        throw new Exception("Game controller trigger not recognized.");
                }
            },
            () => this.gameCtrlTriggerUnsubscriber?.Dispose());
    }

    // TODO: Add dispose() method to dispose of unsubscribers
    // TODO: Add property that represents if any game pads are connected at all
    // TODO: Add property that represents the total number of game pads connected

    public GameControllerState GetState()
    {
        // TODO: Check if the window has been loaded, and if not, throw an exception
        // The theory here is, and has not been confirmed, that if the window load process has
        // not been completed, we cannot create an IInputContext using window.CreateInput().
        // If we cannot, then we need to throw an exception to prevent users from getting state
        // before the load process has been completed.

        var gameCtrlState = new GameControllerState();

        foreach ((ControllerButton key, var state) in this.btnStates)
        {
            gameCtrlState.ButtonStates[key] = state;
        }

        gameCtrlState.LeftGameCtrlThumbstick = this.leftGameCtrlThumbstickStateData;
        gameCtrlState.RightGameCtrlThumbstick = this.rightGameCtrlThumbstickStateData;
        gameCtrlState.LeftGameCtrlTrigger = this.leftGameCtrlTriggerStateData;
        gameCtrlState.RightGameCtrlTrigger = this.rightGameCtrlTriggerStateData;

        return gameCtrlState;
    }
}
