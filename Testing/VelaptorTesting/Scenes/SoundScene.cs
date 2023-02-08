// <copyright file="SoundScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
    private Label? lblDescription;
    private Label? lblState;
    private Label? lblCurrentTime;
    private Label? lblSoundLength;
    private Label? lblRepeat;
    private Button? btnPlaySound;
    private Button? btnPauseSound;
    private Button? btnStopSound;
    private Button? btnFastForward10Sec;
    private Button? btnRewind10Sec;
    private Button? btnRepeat;
    private ISound? sound;

    public override void LoadContent()
    {
        CreateLabels();
        LoadButtons();

        this.sound = ContentLoader.LoadSound("test-song");

        base.LoadContent();

        LayoutButtonsBottom();
        PerformLabelLayout();

        var soundLength = GetFormattedSoundTime(this.sound.Length.Minutes, this.sound.Length.Seconds);
        this.lblSoundLength.Text = $"Sound Length: {soundLength}";
    }

    public override void Update(FrameTime frameTime)
    {
        var currentSoundTime = GetFormattedSoundTime(this.sound.Position.Minutes, this.sound.Position.Seconds);

        this.lblState.Text = $"Sound State: {this.sound.State.ToString()}";
        this.lblRepeat.Text = $"Enable Repeat: {(this.sound.IsLooping ? "yes" : "no")}";
        this.btnRepeat.Text = this.sound.IsLooping ? "Disable Repeat" : "Enable Repeat";
        this.lblCurrentTime.Text = $"Current Time: {currentSoundTime}";

        PerformLabelLayout();
        base.Update(frameTime);
    }

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

        base.UnloadContent();
    }

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

    private void CreateLabels()
    {
        this.lblDescription = new Label
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

        // Total Sound Length
        this.lblSoundLength = new Label
        {
            Text = "Sound Length: ",
            Color = Color.White,
        };

        // Loop Setting
        this.lblRepeat = new Label
        {
            Text = "Enable Repeat: no",
            Color = Color.White,
        };

        AddControl(this.lblDescription);
        AddControl(this.lblState);
        AddControl(this.lblCurrentTime);
        AddControl(this.lblSoundLength);
        AddControl(this.lblRepeat);
    }

    private void LoadButtons()
    {
        // Play Sound
        this.btnPlaySound = new Button
        {
            Text = "Play",
        };
        this.btnPlaySound.Click += (_, _) =>
        {
            this.sound.Play();
        };

        // Stop Sound
        this.btnStopSound = new Button
        {
            Text = "Stop",
        };
        this.btnStopSound.Click += (_, _) =>
        {
            this.sound.Stop();
        };

        // Pause Sound
        this.btnPauseSound = new Button
        {
            Text = "Pause",
        };
        this.btnPauseSound.Click += (_, _) =>
        {
            this.sound.Pause();
        };

        // Fast Forward 10 Seconds
        this.btnFastForward10Sec = new Button
        {
            Text = "Fast Forward 10 Sec",
        };
        this.btnFastForward10Sec.Click += (_, _) =>
        {
            this.sound.FastForward(10f);
        };

        // Rewind 10 Seconds
        this.btnRewind10Sec = new Button
        {
            Text = "Rewind 10 Sec",
        };
        this.btnRewind10Sec.Click += (_, _) =>
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

        AddControl(this.btnPlaySound);
        AddControl(this.btnStopSound);
        AddControl(this.btnPauseSound);
        AddControl(this.btnFastForward10Sec);
        AddControl(this.btnRewind10Sec);
        AddControl(this.btnRepeat);
    }

    private void LayoutButtonsBottom()
    {
        var buttons = Controls.Where(c => c is Button).ToImmutableArray();

        var totalWidth = (from l in buttons
            select (int)l.Width).ToArray().Sum();
        totalWidth += (buttons.Length - 1) * HoriBtnSpacing;

        var totalHalfWidth = totalWidth / 2;

        IControl? prevButton = null;

        foreach (var button in buttons)
        {
            button.Bottom = (int)(WindowSize.Height - BottomMargin);
            button.Left = prevButton is null
                ? button.Left = WindowCenter.X - totalHalfWidth
                : button.Left = prevButton.Right + HoriBtnSpacing;

            prevButton = button;
        }
    }

    private void PerformLabelLayout()
    {
        var labels = Controls.Where(c => c is Label).ToImmutableArray();
        var totalHeight = (from b in labels
            select (int)b.Height).ToArray().Sum();

        totalHeight += (labels.Length - 1) * VertLabelSpacing;

        var totalHalfHeight = totalHeight / 2;

        IControl? prevLabel = null;

        foreach (var label in labels)
        {
            label.Left = LeftMargin;
            label.Top = prevLabel is null
                ? label.Top = WindowCenter.Y - totalHalfHeight
                : label.Top = prevLabel.Bottom + VertLabelSpacing;

            prevLabel = label;
        }
    }
}
