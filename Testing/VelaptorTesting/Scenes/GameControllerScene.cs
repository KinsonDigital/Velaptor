// <copyright file="GameControllerScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InvocationIsCultureSensitive

namespace VelaptorTesting.Scenes;

using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;
using Velum;
using VelUpdatable = Velaptor.IUpdatable;

public enum ControllerTestState
{
    Buttons,

    Axes,

    Rumble
}

/// <summary>
/// Used to test that the game controller works correctly.
/// </summary>
public class GameControllerScene : SceneBase
{
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly IGameController gameController;
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblInstructions;
    private readonly Label lblControllers;
    private readonly Label lblFaceButton;
    private readonly Label lblDPad;
    private readonly Label lblBumperStick;
    private readonly Label lblMisc;
    private readonly Label lblPaddle;
    private readonly Label lblLeftThumbStick;
    private readonly Label lblRightThumbStick;
    private readonly Label lblTriggers;
    private readonly StringBuilder controllersText = new ();
    private readonly StringBuilder faceButtonText = new ();
    private readonly StringBuilder dPadText = new ();
    private readonly StringBuilder bumperStickText = new ();
    private readonly StringBuilder miscText = new ();
    private readonly StringBuilder paddleText = new ();
    private readonly StringBuilder leftThumbstickText = new ();
    private readonly StringBuilder rightThumbstickText = new ();
    private readonly StringBuilder triggersText = new ();
    private GameControllerState prevCtrlState;
    private KeyboardState prevKeyState;
    private bool rumbleTestingEnabled;
    private (bool active, int id)[] ctrlIds = [];
    private int largestThumbstickTextWidth = 0;
    private ControllerTestState currentTestState = ControllerTestState.Buttons;

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
        this.lblControllers = new Label { FontSize = 10 };

        this.lblFaceButton = new Label { FontSize = 10 };
        this.lblFaceButton.DisabledColor = this.lblFaceButton.DisabledColor.IncreaseBrightness(0.75f);

        this.lblDPad = new Label { FontSize = 10 };
        this.lblBumperStick = new Label { FontSize = 10 };
        this.lblMisc = new Label { FontSize = 10 };
        this.lblPaddle = new Label { FontSize = 10 };
        this.lblLeftThumbStick = new Label { FontSize = 10 };
        this.lblRightThumbStick = new Label { FontSize = 10 };
        this.lblTriggers = new Label { FontSize = 10 };
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
        this.lblControllers.Load();
        this.lblFaceButton.Load();
        this.lblDPad.Load();
        this.lblBumperStick.Load();
        this.lblMisc.Load();
        this.lblPaddle.Load();
        this.lblLeftThumbStick.Load();
        this.lblRightThumbStick.Load();
        this.lblTriggers.Load();

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
        this.lblControllers.Unload();
        this.lblFaceButton.Unload();
        this.lblDPad.Unload();
        this.lblBumperStick.Unload();
        this.lblMisc.Unload();
        this.lblPaddle.Unload();
        this.lblLeftThumbStick.Unload();
        this.lblRightThumbStick.Unload();
        this.lblTriggers.Unload();

        this.backgroundManager.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        ProcessKeyboardInput();

        this.controllersText.Clear();
        this.faceButtonText.Clear();
        this.dPadText.Clear();
        this.bumperStickText.Clear();
        this.miscText.Clear();
        this.paddleText.Clear();
        this.leftThumbstickText.Clear();
        this.rightThumbstickText.Clear();
        this.triggersText.Clear();

        this.lblFaceButton.Text = string.Empty;

        var activeId = this.ctrlIds.FirstOrDefault((i) => i.active).id;
        this.ctrlIds = this.gameController.ConnectedDeviceIds.Select((d) => (d == activeId, d)).ToArray();

        if (this.ctrlIds.Length <= 0)
        {
            this.controllersText.Append("No controllers connected.");
        }
        else
        {
            // If no ctrl IDs are active, make the first one active
            if (this.ctrlIds.All(c => !c.active))
            {
                this.ctrlIds[0].active = true;
            }

            this.controllersText.AppendLine("Connected Controllers: ");

            // Display the connected controller names
            foreach (var (active, id) in this.ctrlIds)
            {
                var ctrlState = this.gameController.GetState(id);

                this.controllersText.AppendLine($"{id + 1}: ({(active ? "Active" : "Inactive")}) {ctrlState.Name}");
                this.controllersText.AppendLine($"    - ID: {ctrlState.GUID}");
                this.controllersText.AppendLine($"    - Enabled: {(ctrlState.Enabled ? "Yes" : "No")}");
            }

            var activeStateIndex = Array.FindIndex(this.ctrlIds, (i) => i.active);

            if (activeStateIndex != -1)
            {
                var currentCtrlState = this.gameController.GetState(this.ctrlIds[activeStateIndex].id);

                switch (this.currentTestState)
                {
                    case ControllerTestState.Buttons:
                        this.faceButtonText.AppendLine($"North: {currentCtrlState.IsButtonDown(ControllerButton.North)}");
                        this.faceButtonText.AppendLine($"East: {currentCtrlState.IsButtonDown(ControllerButton.East)}");
                        this.faceButtonText.AppendLine($"South: {currentCtrlState.IsButtonDown(ControllerButton.South)}");
                        this.faceButtonText.AppendLine($"West: {currentCtrlState.IsButtonDown(ControllerButton.West)}");

                        this.bumperStickText.AppendLine(
                            $"Right Bumper/Shoulder: {currentCtrlState.IsButtonDown(ControllerButton.RightShoulder)}");
                        this.bumperStickText.AppendLine($"Left Bumper/Shoulder: {currentCtrlState.IsButtonDown(ControllerButton.LeftShoulder)}");

                        this.bumperStickText.AppendLine($"Right Stick: {currentCtrlState.IsButtonDown(ControllerButton.RightStick)}");
                        this.bumperStickText.AppendLine($"Left Stick: {currentCtrlState.IsButtonDown(ControllerButton.LeftStick)}");

                        this.dPadText.AppendLine($"DPad Up: {currentCtrlState.IsButtonDown(ControllerButton.DPadUp)}");
                        this.dPadText.AppendLine($"DPad Right: {currentCtrlState.IsButtonDown(ControllerButton.DPadRight)}");
                        this.dPadText.AppendLine($"DPad Down: {currentCtrlState.IsButtonDown(ControllerButton.DPadDown)}");
                        this.dPadText.AppendLine($"DPad Left: {currentCtrlState.IsButtonDown(ControllerButton.DPadLeft)}");

                        this.miscText.AppendLine($"Guide/Home: {currentCtrlState.IsButtonDown(ControllerButton.Guide)}");
                        this.miscText.AppendLine($"Start: {currentCtrlState.IsButtonDown(ControllerButton.Start)}");
                        this.miscText.AppendLine($"Back: {currentCtrlState.IsButtonDown(ControllerButton.Back)}");

                        this.paddleText.AppendLine($"Paddle 1: {currentCtrlState.IsButtonDown(ControllerButton.Paddle1)}");
                        this.paddleText.AppendLine($"Paddle 2: {currentCtrlState.IsButtonDown(ControllerButton.Paddle2)}");
                        this.paddleText.AppendLine($"Paddle 3: {currentCtrlState.IsButtonDown(ControllerButton.Paddle3)}");
                        this.paddleText.AppendLine($"Paddle 4: {currentCtrlState.IsButtonDown(ControllerButton.Paddle4)}");
                        break;
                    case ControllerTestState.Axes:
                        // Show left thumbstick info
                        var leftThumbDirection = Math.Round(currentCtrlState.GetThumbstickDirection(Thumbsticks.Left), 2);
                        var leftThumbX = Math.Round(currentCtrlState.GetThumbstickX(Thumbsticks.Left), 4);
                        var leftThumbY = Math.Round(currentCtrlState.GetThumbstickY(Thumbsticks.Left), 4);
                        var leftThumbPosition = Math.Round(currentCtrlState.GetThumbstickPosition(Thumbsticks.Left), 2);
                        this.leftThumbstickText.AppendLine(
                            $"Left Thumbstick Direction: {(leftThumbDirection == -1 ? "Center" : leftThumbDirection)}");
                        this.leftThumbstickText.AppendLine($"Left Thumbstick X: {leftThumbX}");
                        this.leftThumbstickText.AppendLine($"Left Thumbstick Y: {leftThumbY}");
                        this.leftThumbstickText.AppendLine($"Left Thumbstick Position: {leftThumbPosition}");

                        // Show right thumbstick info
                        var rightThumbDirection = Math.Round(currentCtrlState.GetThumbstickDirection(Thumbsticks.Right), 2);
                        var rightThumbX = Math.Round(currentCtrlState.GetThumbstickX(Thumbsticks.Right), 4);
                        var rightThumbY = Math.Round(currentCtrlState.GetThumbstickY(Thumbsticks.Right), 4);
                        var rightThumbPosition = Math.Round(currentCtrlState.GetThumbstickPosition(Thumbsticks.Right), 2);
                        this.rightThumbstickText.AppendLine(
                            $"Right Thumbstick Direction: {(rightThumbDirection == -1 ? "Center" : rightThumbDirection)}");
                        this.rightThumbstickText.AppendLine($"Right Thumbstick X: {rightThumbX}");
                        this.rightThumbstickText.AppendLine($"Right Thumbstick Y: {rightThumbY}");
                        this.rightThumbstickText.AppendLine($"Right Thumbstick Position: {rightThumbPosition}");

                        // Show left trigger info
                        var leftTriggerPos = Math.Round(currentCtrlState.GetTriggerPosition(Triggers.Left), 2);
                        this.triggersText.AppendLine($"Left Trigger Position: {leftTriggerPos}%");

                        // Show right trigger info
                        var rightTriggerPos = Math.Round(currentCtrlState.GetTriggerPosition(Triggers.Right), 2);
                        this.triggersText.AppendLine($"Right Trigger Position: {rightTriggerPos}%");
                        break;
                    case ControllerTestState.Rumble:
                        break;
                }

                if (this.rumbleTestingEnabled)
                {
                    // Show rumble info
                    // this.downKeyText.AppendLine($"Controller Has Rumble: {(this.gameController.HasRumble(currentCtrlState.Id) ? "Yes" : "No")}");
                    // this.downKeyText.AppendLine($"Heavy Rumble Intensity (Left Trigger): {currentCtrlState.HeavyRumbleIntensity}%");
                    // this.downKeyText.AppendLine($"Light Rumble Intensity (Right Trigger): {currentCtrlState.LightRumbleIntensity}%");
                    //
                    // var leftTriggerPos = this.gameController.HasRumble(currentCtrlState.Id)
                    //     ? currentCtrlState.GetTriggerPosition(Triggers.Left)
                    //     : 0;
                    // var rightTriggerPos = this.gameController.HasRumble(currentCtrlState.Id)
                    //     ? currentCtrlState.GetTriggerPosition(Triggers.Right)
                    //     : 0;
                    //
                    // this.gameController.RumbleHeavy(leftTriggerPos);
                    // this.gameController.RumbleLight(rightTriggerPos);
                    // this.gameCtrlLabels.Enabled = !this.rumbleTestingEnabled || this.gameController.HasRumble(currentCtrlState.Id);
                }
                else
                {
                    this.gameController.StopAllRumble();
                    // Single empty line to separate buttons pressed and the rest of the stick and trigger info
                    this.faceButtonText.AppendLine();
                    this.faceButtonText.AppendLine();
                    this.faceButtonText.AppendLine();
                }
            }
        }

        const int leftOffset = 100;
        var widthQuarter = WindowSize.Width / 4;
        var heightQuarter = WindowSize.Height / 4;
        var widthEighth = WindowSize.Width / 8;
        var heightEighth = WindowSize.Height / 8;

        // TODO: Update based on the current testing

        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, 25);
        this.lblInstructions.Update();

        this.lblControllers.Text = this.controllersText.ToString();
        this.lblControllers.Position = new Vector2((WindowSize.Width / 2f) - (this.lblControllers.Width / 2f), this.lblInstructions.Bottom + 50);
        this.lblControllers.Update();

        // Left section of the window
        this.lblFaceButton.Text = this.faceButtonText.ToString();
        this.lblFaceButton.Position = new Vector2(leftOffset, heightQuarter - this.lblFaceButton.HalfHeight);
        this.lblFaceButton.Update();

        this.lblDPad.Text = this.dPadText.ToString();
        this.lblDPad.Position = new Vector2(leftOffset, (heightQuarter * 2) - this.lblDPad.HalfHeight);
        this.lblDPad.Update();

        this.lblBumperStick.Text = this.bumperStickText.ToString();
        this.lblBumperStick.Position = new Vector2(leftOffset, (heightQuarter * 3) - this.lblBumperStick.HalfHeight);
        this.lblBumperStick.Update();

        // Right section of the window
        this.lblPaddle.Text = this.paddleText.ToString();
        this.lblPaddle.Position = new Vector2(WindowSize.Width - (leftOffset * 6), (heightQuarter * 2) - this.lblPaddle.HalfHeight);
        this.lblPaddle.Update();

        this.lblMisc.Text = this.miscText.ToString();
        this.lblMisc.Position = new Vector2(WindowSize.Width - (leftOffset * 6), (heightQuarter * 3) - this.lblMisc.HalfHeight);
        this.lblMisc.Update();

        // Left Thumbstick
        this.lblLeftThumbStick.Text = this.leftThumbstickText.ToString();
        this.lblLeftThumbStick.Position = new Vector2(widthEighth * 3, heightEighth * 3);
        this.lblLeftThumbStick.Update();

        // Right Thumbstick
        this.lblRightThumbStick.Text = this.rightThumbstickText.ToString();
        this.lblRightThumbStick.Position = new Vector2(this.lblLeftThumbStick.Position.X, this.lblLeftThumbStick.Bottom + 25);
        this.lblRightThumbStick.Update();

        // Triggers
        this.lblTriggers.Text = this.triggersText.ToString();
        this.lblTriggers.Position = new Vector2(this.lblLeftThumbStick.Position.X, this.lblRightThumbStick.Bottom + 25);
        this.lblTriggers.Update();

        this.lblMisc.Update();

        // this.prevCtrlState = currentGameCtrlState;

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        // TODO: Render based on the current testing

        this.lblInstructions.Render();
        this.lblControllers.Render();
        this.lblFaceButton.Render();
        this.lblDPad.Render();
        this.lblBumperStick.Render();
        this.lblMisc.Render();
        this.lblPaddle.Render();
        this.lblLeftThumbStick.Render();
        this.lblRightThumbStick.Render();
        this.lblTriggers.Render();

        base.Render();
    }

    private void ProcessKeyboardInput()
    {
        var currentKeyState = this.keyboard.GetState();


        if (currentKeyState.IsKeyUp(KeyCode.Enter) && this.prevKeyState.IsKeyDown(KeyCode.Enter))
        {
            this.gameController.Enabled = !this.gameController.Enabled;
        }

        if (currentKeyState.IsKeyUp(KeyCode.Right) && this.prevKeyState.IsKeyDown(KeyCode.Right))
        {
            var min = Enum.GetValues<ControllerTestState>().Min(i => i);
            var max = Enum.GetValues<ControllerTestState>().Max(i => i);

            this.currentTestState = this.currentTestState == max ? min : this.currentTestState + 1;
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
}
