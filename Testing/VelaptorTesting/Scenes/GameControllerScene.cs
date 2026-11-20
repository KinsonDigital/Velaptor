// <copyright file="GameControllerScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InvocationIsCultureSensitive

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;
using Velum;
using VelUpdatable = Velaptor.IUpdatable;

/// <summary>
/// Used to test that the game controller works correctly.
/// </summary>
public class GameControllerScene : SceneBase
{
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly IGameController gameController;
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblInstructions;
    private readonly Label gameCtrlLabels;
    private readonly StringBuilder downKeyText = new ();
    private GameControllerState prevCtrlState;
    private KeyboardState prevKeyState;
    private bool rumbleTestingEnabled;
    private (bool active, string id)[] ctrlIds = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerScene"/> class.
    /// </summary>
    public GameControllerScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.gameController = HardwareFactory.GetGameController();
        this.backgroundManager = new BackgroundManager();

        var instructions = new[]
        {
            "Try all buttons, joysticks, triggers, and rumble.", "Press 'space' to change test modes.",
            "Press 'enter' to enable/disable controller.",
        };

        this.lblInstructions = new Label { Text = string.Join('\n', instructions) };

        this.gameCtrlLabels = new Label();
        this.gameCtrlLabels.DisabledColor = this.gameCtrlLabels.DisabledColor.IncreaseBrightness(0.75f);
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

        this.gameCtrlLabels.Load();

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
        this.gameCtrlLabels.Unload();
        this.backgroundManager.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        ProcessKeyboardInput();

        this.lblInstructions.Position =
            new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, 100);

        this.downKeyText.Clear();
        this.gameCtrlLabels.Text = string.Empty;

        var activeId = this.ctrlIds.FirstOrDefault((i) => i.active).id;
        this.ctrlIds = this.gameController.ConnectedDeviceIds.Select((d) => (d == activeId, d)).ToArray();

        if (this.ctrlIds.Length <= 0)
        {
            this.downKeyText.Append("No controllers connected.");
        }
        else
        {
            // If no ctrl IDs are active, make the first one active
            if (this.ctrlIds.All(c => !c.active))
            {
                this.ctrlIds[0].active = true;
            }

            this.downKeyText.AppendLine("Connected Controllers: ");

            // Display the connected controller names
            foreach (var (active, id) in this.ctrlIds)
            {
                var ctrlState = this.gameController.GetState(id);

                this.downKeyText.AppendLine(
                    $"   - ({active}) {ctrlState.Name} (ID: {ctrlState.Id}) | Enabled: {(ctrlState.Enabled ? "Yes" : "No")}");
            }

            this.downKeyText.AppendLine();

            var activeStateIndex = Array.FindIndex(this.ctrlIds, (i) => i.active);

            if (activeStateIndex != -1)
            {
                var currentCtrlState = this.gameController.GetState(this.ctrlIds[activeStateIndex].id);

                if (this.rumbleTestingEnabled)
                {
                    // Show rumble info
                    this.downKeyText.AppendLine($"Controller Has Rumble: {(this.gameController.HasRumble(currentCtrlState.Id) ? "Yes" : "No")}");
                    this.downKeyText.AppendLine($"Heavy Rumble Intensity (Left Trigger): {currentCtrlState.HeavyRumbleIntensity}%");
                    this.downKeyText.AppendLine($"Light Rumble Intensity (Right Trigger): {currentCtrlState.LightRumbleIntensity}%");

                    var leftTriggerPos = this.gameController.HasRumble(currentCtrlState.Id)
                        ? currentCtrlState.GetTriggerPosition(Triggers.Left)
                        : 0;
                    var rightTriggerPos = this.gameController.HasRumble(currentCtrlState.Id)
                        ? currentCtrlState.GetTriggerPosition(Triggers.Right)
                        : 0;

                    this.gameController.RumbleHeavy(leftTriggerPos);
                    this.gameController.RumbleLight(rightTriggerPos);
                    this.gameCtrlLabels.Enabled = !this.rumbleTestingEnabled || this.gameController.HasRumble(currentCtrlState.Id);
                }
                else
                {
                    this.gameController.StopAllRumble();

                    if (currentCtrlState.GetDownButtons().Length > 0)
                    {
                        this.downKeyText.Append("Buttons Pressed: ");

                        var btns = currentCtrlState.GetDownButtons();
                        for (var i = 0; i < btns.Length; i++)
                        {
                            this.downKeyText.Append(Enum.GetName(btns[i])?.SplitByUpperCase());
                            this.downKeyText.Append(i == btns.Length - 1 ? string.Empty : ", ");
                        }
                    }
                    else
                    {
                        this.downKeyText.Append("No Buttons Pressed");
                    }

                    // Single empty line to separate buttons pressed and the rest of the stick and trigger info
                    this.downKeyText.AppendLine();
                    this.downKeyText.AppendLine();

                    // Show left thumbstick info
                    var leftThumbDirection = Math.Round(currentCtrlState.GetThumbstickDirection(Thumbsticks.Left), 2);
                    var leftThumbX = Math.Round(currentCtrlState.GetThumbstickX(Thumbsticks.Left), 4);
                    var leftThumbY = Math.Round(currentCtrlState.GetThumbstickY(Thumbsticks.Left), 4);
                    this.downKeyText.AppendLine($"Left Thumbstick Direction: {(leftThumbDirection == -1 ? "Center" : leftThumbDirection)}");
                    this.downKeyText.AppendLine($"Left Thumbstick X: {leftThumbX}");
                    this.downKeyText.AppendLine($"Left Thumbstick Y: {leftThumbY}");

                    this.downKeyText.AppendLine();

                    // Show right thumbstick info
                    var rightThumbDirection = Math.Round(currentCtrlState.GetThumbstickDirection(Thumbsticks.Right), 2);
                    var rightThumbX = Math.Round(currentCtrlState.GetThumbstickX(Thumbsticks.Right), 4);
                    var rightThumbY = Math.Round(currentCtrlState.GetThumbstickY(Thumbsticks.Right), 4);
                    this.downKeyText.AppendLine($"Right Thumbstick Direction: {(rightThumbDirection == -1 ? "Center" : rightThumbDirection)}");
                    this.downKeyText.AppendLine($"Right Thumbstick X: {rightThumbX}");
                    this.downKeyText.AppendLine($"Right Thumbstick Y: {rightThumbY}");

                    this.downKeyText.AppendLine();

                    // Show left trigger info
                    var leftTriggerPos = Math.Round(currentCtrlState.GetTriggerPosition(Triggers.Left), 2);
                    this.downKeyText.AppendLine($"Left Trigger Position: {leftTriggerPos}%");

                    // Show right trigger info
                    var rightTriggerPos = Math.Round(currentCtrlState.GetTriggerPosition(Triggers.Right), 2);
                    this.downKeyText.AppendLine($"Right Trigger Position: {rightTriggerPos}%");

                    this.downKeyText.AppendLine();
                }
            }
        }

        this.gameCtrlLabels.Text = this.downKeyText.ToString();
        this.gameCtrlLabels.Position = new Vector2(WindowCenter.X - 275, WindowCenter.Y - this.gameCtrlLabels.HalfHeight + 50);
        this.gameCtrlLabels.Update();
        this.lblInstructions.Update();

        // this.prevCtrlState = currentGameCtrlState;

        base.Update(frameTime);
    }

    private void ProcessKeyboardInput()
    {
        var currentKeyState = this.keyboard.GetState();

        // Turn on/off rumble
        if (currentKeyState.IsKeyUp(KeyCode.Space) && this.prevKeyState.IsKeyDown(KeyCode.Space))
        {
            this.rumbleTestingEnabled = !this.rumbleTestingEnabled;
        }

        if (currentKeyState.IsKeyUp(KeyCode.Enter) && this.prevKeyState.IsKeyDown(KeyCode.Enter))
        {
            this.gameController.Enabled = !this.gameController.Enabled;
        }

        if (currentKeyState.IsKeyUp(KeyCode.Down) && this.prevKeyState.IsKeyDown(KeyCode.Down))
        {
            if (this.ctrlIds.Length > 0)
            {
                var currentActiveIndex = Array.FindIndex(this.ctrlIds, (i) => i.active);
                var nextIndex = (currentActiveIndex + 1) % this.ctrlIds.Length;

                this.ctrlIds[currentActiveIndex].active = false;
                this.ctrlIds[nextIndex].active = true;
            }
        }

        if (currentKeyState.IsKeyUp(KeyCode.Up) && this.prevKeyState.IsKeyDown(KeyCode.Up))
        {
            if (this.ctrlIds.Length > 0)
            {
                var currentActiveIndex = Array.FindIndex(this.ctrlIds, (i) => i.active);
                var nextActiveIndex = currentActiveIndex <= 0 ? this.ctrlIds.Length - 1 : currentActiveIndex - 1;
                this.ctrlIds[currentActiveIndex].active = false;
                this.ctrlIds[nextActiveIndex].active = true;
            }
        }

        this.prevKeyState = currentKeyState;
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        this.lblInstructions.Render();
        this.gameCtrlLabels.Render();

        base.Render();
    }
}
