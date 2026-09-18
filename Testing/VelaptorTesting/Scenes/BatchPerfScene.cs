// <copyright file="BatchPerfScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using Velum;

public class BatchPerfScene : SceneBase
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly Random random = new ();
    private readonly List<Line> lines = new ();
    private readonly Label lblTotal;
    private readonly Label lblInstructions;

    public BatchPerfScene()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.keyboard = HardwareFactory.GetKeyboard();

        this.lblTotal = new Label();
        this.lblTotal.BackgroundColor = Color.White;
        this.lblTotal.TextColor = Color.Black;

        this.lblInstructions = new Label();
        this.lblInstructions.BackgroundColor = Color.White;
        this.lblInstructions.TextColor = Color.Black;
    }

    public override void LoadContent()
    {
        for (var i = 0; i < 10; i++)
        {
            var newLine = CreateLine();

            this.lines.Add(newLine);
        }

        var instructions = new[]
        {
            "Up Arrow: Increase total lines by 1",
            "Down Arrow: Decrease total lines by 1",
            "Any Shift + Up/Down Arrow: Increase or decrease lines by 5",
        };

        var instructionsText = string.Join(Environment.NewLine, instructions);

        this.lblInstructions.Load();
        this.lblInstructions.Text = instructionsText;
        this.lblInstructions.Position = new Vector2((WindowSize.Width / 2f) - (this.lblInstructions.Width / 2f), 25);

        this.lblTotal.Load();
        this.lblTotal.Text = $"Total Lines: {this.lines.Count}";
        this.lblTotal.Position = new Vector2(25, WindowSize.Height - (this.lblTotal.Height + 25));

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.lblTotal.Unload();
        this.lblInstructions.Unload();

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        var removeAmount = currentKeyState.AnyShiftKeysDown() ? 5 : 1;

        if (currentKeyState.IsKeyDown(KeyCode.Up))
        {
            var newLine = CreateLine();

            for (var i = 0; i < removeAmount; i++)
            {
                this.lines.Add(newLine);
            }

            this.lblTotal.Text = $"Total Lines: {this.lines.Count}";
        }

        if (currentKeyState.IsKeyDown(KeyCode.Down))
        {
            if (this.lines.Count > 0)
            {
                removeAmount = this.lines.Count >= 5
                    ? removeAmount
                    : 1;

                for (var i = 0; i < removeAmount; i++)
                {
                    this.lines.RemoveAt(this.lines.Count - 1);
                }

                this.lblTotal.Text = $"Total Lines: {this.lines.Count}";
            }
        }

        this.lblTotal.Update();
        this.lblInstructions.Update();

        base.Update(frameTime);
    }

    public override void Render()
    {
        for (var i = 0; i < this.lines.Count; i++)
        {
            var line = this.lines[i];
            var layer = (i % 4) + 1;

            this.shapeRenderer.Render(line, layer);
        }

        // TODO: for some reason, the lines are still being rendered on top of the text
        this.lblTotal.Render(this.lines.Count + 10);
        this.lblInstructions.Render(this.lines.Count + 10);

        base.Render();
    }

    private Line CreateLine()
    {
        var start = new Vector2(this.random.Next(0, (int)WindowSize.Width), this.random.Next(0, (int)WindowSize.Height));
        var end = new Vector2(this.random.Next(0, (int)WindowSize.Width), this.random.Next(0, (int)WindowSize.Height));

        var newLine = new Line(start, end);
        newLine.Thickness = this.random.Next(1, 10);

        var alpha = this.random.Next(0, 255);
        var red = this.random.Next(0, 255);
        var green = this.random.Next(0, 255);
        var blue = this.random.Next(0, 255);

        newLine.Color = Color.FromArgb(alpha, red, green, blue);

        return newLine;
    }
}
