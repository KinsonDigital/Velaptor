// <copyright file="SoundScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Velaptor.Scene;
using Velaptor;
using Velaptor.Content;
using Velaptor.UI;

public class SoundScene : SceneBase
{
    private const int BottomMargin = 10;
    private const int HoriBtnSpacing = 10;
    private const int VertLabelSpacing = 15;
    private const int LeftMargin = 10;
    private Label? lblState;
    private Label? lblCurrentTime;
    private Label? lblRepeat;
    private Button? btnRepeat;
    private BackgroundManager? backgroundManager;
    private ISound? sound;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.sound = ContentLoader.LoadSound("test-song");

        CreateLabels();
        CreateButtons();

        base.LoadContent();

        LayoutButtonsBottom();
        LayoutLabelsLeft();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentSoundTime = GetFormattedSoundTime(this.sound.Position.Minutes, this.sound.Position.Seconds);

        this.lblState.Text = $"Sound State: {this.sound.State.ToString()}";
        this.lblRepeat.Text = $"Enable Repeat: {(this.sound.IsLooping ? "yes" : "no")}";
        this.btnRepeat.Text = this.sound.IsLooping ? "Disable Repeat" : "Enable Repeat";
        this.lblCurrentTime.Text = $"Current Time: {currentSoundTime}";

        LayoutLabelsLeft();
        base.Update(frameTime);
    }

    public override void Render()
    {
        this.backgroundManager?.Render();
        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        if (this.sound is not null)
        {
            this.sound.Stop();
            ContentLoader.UnloadSound(this.sound);
        }

        this.backgroundManager?.Unload();
        base.UnloadContent();
    }

    /// <summary>
    /// Gets the time of the sound as a formatted string.
    /// </summary>
    /// <param name="minutes">The minutes of the time.</param>
    /// <param name="seconds">The seconds of the time.</param>
    /// <returns>The formatted time.</returns>
    private static string GetFormattedSoundTime(float minutes, float seconds)
    {
        var minuteStr = ((int)minutes).ToString(CultureInfo.InvariantCulture);
        var secondStr = Math.Round(seconds, 0).ToString(CultureInfo.InvariantCulture);

        minuteStr = minuteStr.Length <= 1
            ? $"0{minuteStr}"
            : minuteStr;

        secondStr = secondStr.Length <= 1
            ? $"0{secondStr}"
            : secondStr;

        return $"{minuteStr}:{secondStr}";
    }

    /// <summary>
    /// Creates all of the labels.
    /// </summary>
    private void CreateLabels()
    {
        var lblDescription = new Label
        {
            Text = "Use the buttons below to manipulate the sound.",
            Color = Color.White,
        };

        this.lblState = new Label
        {
            Text = "Sound State:",
            Color = Color.White,
        };

        // Current Sound time
        this.lblCurrentTime = new Label
        {
            Text = "Current Time: 0:0",
            Color = Color.White,
        };

        var soundLength = GetFormattedSoundTime(this.sound.Length.Minutes, this.sound.Length.Seconds);

        // Total Sound Length
        var lblSoundLength = new Label
        {
            Text = $"Sound Length: {soundLength}",
            Color = Color.White,
        };

        // Loop Setting
        this.lblRepeat = new Label
        {
            Text = "Enable Repeat: no",
            Color = Color.White,
        };

        AddControl(lblDescription);
        AddControl(this.lblState);
        AddControl(this.lblCurrentTime);
        AddControl(lblSoundLength);
        AddControl(this.lblRepeat);
    }

    /// <summary>
    /// Loads all of the buttons.
    /// </summary>
    private void CreateButtons()
    {
        // Play Sound
        var btnPlaySound = new Button
        {
            Text = "Play",
        };
        btnPlaySound.Click += (_, _) =>
        {
            this.sound.Play();
        };

        // Stop Sound
        var btnStopSound = new Button
        {
            Text = "Stop",
        };
        btnStopSound.Click += (_, _) =>
        {
            this.sound.Stop();
        };

        // Pause Sound
        var btnPauseSound = new Button
        {
            Text = "Pause",
        };
        btnPauseSound.Click += (_, _) =>
        {
            this.sound.Pause();
        };

        // Fast Forward 10 Seconds
        var btnFastForward10Sec = new Button
        {
            Text = "Fast Forward 10 Sec",
        };
        btnFastForward10Sec.Click += (_, _) =>
        {
            this.sound.FastForward(10f);
        };

        // Rewind 10 Seconds
        var btnRewind10Sec = new Button
        {
            Text = "Rewind 10 Sec",
        };
        btnRewind10Sec.Click += (_, _) =>
        {
            this.sound.Rewind(10f);
        };

        // Loop Setting
        this.btnRepeat = new Button
        {
            Text = "Enable Repeat",
        };
        this.btnRepeat.Click += (_, _) =>
        {
            this.sound.IsLooping = !this.sound.IsLooping;
        };

        AddControl(btnPlaySound);
        AddControl(btnStopSound);
        AddControl(btnPauseSound);
        AddControl(btnFastForward10Sec);
        AddControl(btnRewind10Sec);
        AddControl(this.btnRepeat);
    }

    /// <summary>
    /// Lays out the buttons at the bottom of the window.
    /// </summary>
    private void LayoutButtonsBottom()
    {
        var buttons = Controls.Where(c => c is Button).ToImmutableArray();

        var totalWidth = (from l in buttons
            select (int)l.Width).Sum();
        totalWidth += (buttons.Length - 1) * HoriBtnSpacing;

        var totalHalfWidth = totalWidth / 2;

        IControl? prevButton = null;

        foreach (var button in buttons)
        {
            button.Bottom = (int)(WindowSize.Height - BottomMargin);
            button.Left = prevButton?.Right + HoriBtnSpacing ?? WindowCenter.X - totalHalfWidth;
            prevButton = button;
        }
    }

    /// <summary>
    /// Lays out the labels on the left side of the window.
    /// </summary>
    private void LayoutLabelsLeft()
    {
        var labels = Controls.Where(c => c is Label).ToImmutableArray();
        var totalHeight = (from b in labels
            select (int)b.Height).Sum();

        totalHeight += (labels.Length - 1) * VertLabelSpacing;

        var totalHalfHeight = totalHeight / 2;

        IControl? prevLabel = null;

        foreach (var label in labels)
        {
            label.Left = LeftMargin;
            label.Top = prevLabel?.Bottom + VertLabelSpacing ?? WindowCenter.Y - totalHalfHeight;
            prevLabel = label;
        }
    }
}
