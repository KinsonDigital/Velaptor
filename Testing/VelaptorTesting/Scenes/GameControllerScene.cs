// <copyright file="GameControllerScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

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
            new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowCenter.Y - this.lblInstructions.HalfHeight);

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
                this.downKeyText.Append(i == btns.Length - 1 ? string.Empty : ", ");
            }
        }
        else
        {
            this.downKeyText.Clear();
            this.downKeyText.Append("No Buttons Pressed");
        }

        this.downKeyText.Clear(); // TODO: Remove this.  Just for debugging stick data only and erasing button data above

        var leftThumbDirection = currentGameCtrlState.GetThumbstickDirection(Thumbsticks.Left);
        this.downKeyText.Append($"Left Thumbstick Direction: {leftThumbDirection}");
        this.downKeyText.Append($" | X: {currentGameCtrlState.GetThumbstickX(Thumbsticks.Left)}");
        this.downKeyText.Append($" | Y: {currentGameCtrlState.GetThumbstickY(Thumbsticks.Left)}");

        this.gameCtrlBtns.Text = this.downKeyText.ToString();
        this.gameCtrlBtns.Position =
            new Vector2(WindowCenter.X - this.gameCtrlBtns.HalfWidth, WindowCenter.Y - this.gameCtrlBtns.HalfHeight + 50);
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
