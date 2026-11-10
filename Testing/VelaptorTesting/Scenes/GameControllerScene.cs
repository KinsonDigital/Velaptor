// <copyright file="GameControllerScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable CompareOfFloatsByEqualityOperator

// ReSharper disable InvocationIsCultureSensitive
namespace VelaptorTesting.Scenes;

using System;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;
using Velum;

/// <summary>
/// Used to test that the game controller works correctly.
/// </summary>
public class GameControllerScene : SceneBase
{
    private const string Instructions = "Try all buttons and joysticks to see if they are correct.";
    private readonly IAppInput<GameControllerState> gameController;
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblInstructions;
    private readonly Label gameCtrlBtns;
    private readonly StringBuilder downKeyText = new (Instructions);

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerScene"/> class.
    /// </summary>
    public GameControllerScene()
    {
        this.gameController = HardwareFactory.GetGameController();
        this.backgroundManager = new BackgroundManager();

        this.lblInstructions = new Label { Text = Instructions, };

        this.gameCtrlBtns = new Label();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>.
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblInstructions.Load();
        this.lblInstructions.Position =
            new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, 100);

        this.gameCtrlBtns.Load();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>.
    public override void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.lblInstructions.Unload();
        this.gameCtrlBtns.Unload();
        this.backgroundManager.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        var currentGameCtrlState = this.gameController.GetState();

        if (currentGameCtrlState.GetDownButtons().Length > 0)
        {
            this.downKeyText.Clear();

            var btns = currentGameCtrlState.GetDownButtons();
            for (var i = 0; i < btns.Length; i++)
            {
                this.downKeyText.Append(Enum.GetName(btns[i]).SplitByUpperCase());
                this.downKeyText.AppendLine(i == btns.Length - 1 ? string.Empty : ", ");
            }
        }
        else
        {
            this.downKeyText.Clear();
            this.downKeyText.AppendLine("No Buttons Pressed");
        }

        // Show left thumbstick info
        var leftThumbDirection = Math.Round(currentGameCtrlState.GetThumbstickDirection(Thumbsticks.Left), 2);
        var leftThumbX = Math.Round(currentGameCtrlState.GetThumbstickX(Thumbsticks.Left), 4);
        var leftThumbY = Math.Round(currentGameCtrlState.GetThumbstickY(Thumbsticks.Left), 4);
        this.downKeyText.AppendLine($"Left Thumbstick Direction: {(leftThumbDirection == -1 ? "Center" : leftThumbDirection)}");
        this.downKeyText.AppendLine($"Left Thumbstick X: {leftThumbX}");
        this.downKeyText.AppendLine($"Left Thumbstick Y: {leftThumbY}");

        this.downKeyText.AppendLine();

        // Show right thumbstick info
        var rightThumbDirection = Math.Round(currentGameCtrlState.GetThumbstickDirection(Thumbsticks.Right), 2);
        var rightThumbX = Math.Round(currentGameCtrlState.GetThumbstickX(Thumbsticks.Right), 4);
        var rightThumbY = Math.Round(currentGameCtrlState.GetThumbstickY(Thumbsticks.Right), 4);
        this.downKeyText.AppendLine($"Right Thumbstick Direction: {(rightThumbDirection == -1 ? "Center" : rightThumbDirection)}");
        this.downKeyText.AppendLine($"Right Thumbstick X: {rightThumbX}");
        this.downKeyText.AppendLine($"Right Thumbstick Y: {rightThumbY}");

        // Show left trigger info
        var leftTriggerPos = Math.Round(currentGameCtrlState.GetTriggerPosition(Triggers.Left), 2);
        this.downKeyText.AppendLine($"Left Trigger Position: {leftTriggerPos}%");

        // Show right trigger info
        var rightTriggerPos = Math.Round(currentGameCtrlState.GetTriggerPosition(Triggers.Right), 2);
        this.downKeyText.AppendLine($"Right Trigger Position: {rightTriggerPos}%");

        this.gameCtrlBtns.Text = this.downKeyText.ToString();
        this.gameCtrlBtns.Position =
            new Vector2(WindowCenter.X - 275, WindowCenter.Y - this.gameCtrlBtns.HalfHeight + 50);
        this.gameCtrlBtns.Update();
        this.lblInstructions.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        this.lblInstructions.Render();
        this.gameCtrlBtns.Render();

        base.Render();
    }
}
