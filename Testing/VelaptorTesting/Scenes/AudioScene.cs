// <copyright file="AudioScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using UILib;
using Velaptor;
using Velaptor.Content;
using Velaptor.Scene;

/// <summary>
/// Tests out audio functionality using the new UILib controls.
/// </summary>
public class AudioScene : SceneBase
{
    private const int WindowPadding = 10;
    private readonly IContentManager contentManager;
    private BackgroundManager? backgroundManager;
    private IAudio? audio;
    private string currentAudioType = "OGG";

    // Info label
    private Label? lblInfo;

    // Audio controls
    private Container? conAudio;
    private Layout? layMain;
    private DropDown? drpAudioFile;
    private Slider? sldVolume;
    private Slider? sldPosition;
    private Button? btnRewind;
    private Button? btnFastForward;
    private Button? btnPause;
    private Button? btnStop;
    private Button? btnPlay;
    private CheckBox? chkRepeat;
    private Layout? layAudioFile;
    private Layout? layVolume;
    private Layout? layPosition;
    private Label? lblAudioFile;
    private Label? lblVolume;
    private bool updatingPositionSlider;
    private Label? lblPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioScene"/> class.
    /// </summary>
    public AudioScene() => this.contentManager = ContentManager.Create();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.audio = this.contentManager.LoadAudio("ridley-draygon-theme.ogg", AudioBuffer.Stream);

        CreateInfoLabel();
        CreateAudioCtrls();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        if (this.audio is not null)
        {
            this.contentManager.Unload(this.audio);
        }

        this.backgroundManager?.Unload();

        this.drpAudioFile.SelectedItemChanged -= DrpAudioFile_SelectedItemChanged;
        this.sldVolume.ValueChanged -= SldVolume_ValueChanged;
        this.sldPosition.ValueChanged -= SldPosition_ValueChanged;
        this.btnRewind.Click -= BtnRewind_Click;
        this.btnFastForward.Click -= BtnFastForward_Click;
        this.btnPause.Click -= BtnPause_Click;
        this.btnStop.Click -= BtnStop_Click;
        this.btnPlay.Click -= BtnPlay_Click;
        this.chkRepeat.CheckedChanged -= ChkRepeat_CheckedChanged;

        this.lblInfo.Unload();
        this.conAudio.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentTime = GetFormattedTime(this.audio.Position.Minutes, this.audio.Position.Seconds);
        var length = GetFormattedTime(this.audio.Length.Minutes, this.audio.Length.Seconds);

        var textLines = new[]
        {
            "Use the audio controls to manipulate the audio.",
            "---------------------------------------------------------",
            $"Repeat Enabled: {(this.audio.IsLooping ? "yes" : "no")}",
            $"Audio Type: {this.currentAudioType}",
            $"Audio Length: {length}",
            $"Current Time: {currentTime}",
            $"Audio State: {GetState()}",
        };

        this.lblInfo.Text = string.Join(Environment.NewLine, textLines);
        this.lblInfo.Position = new Vector2(WindowCenter.X - this.lblInfo.HalfWidth, WindowPadding);

        this.updatingPositionSlider = true;
        this.sldPosition.Value = (float)this.audio.Position.TotalSeconds;
        this.updatingPositionSlider = false;

        this.conAudio.Position = new Vector2(
            WindowCenter.X - this.conAudio.HalfWidth,
            WindowCenter.Y - this.conAudio.HalfHeight);

        this.conAudio.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager?.Render();

        this.lblInfo.Render();
        this.conAudio.Render();

        base.Render();
    }

    /// <summary>
    /// Gets the time of the audio as a formatted string.
    /// </summary>
    /// <param name="minutes">The minutes of the time.</param>
    /// <param name="seconds">The seconds of the time.</param>
    /// <returns>The formatted time.</returns>
    private static string GetFormattedTime(float minutes, float seconds)
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
    /// Gets the current state of the audio as a string.
    /// </summary>
    /// <returns>The audio state string.</returns>
    private string GetState()
    {
        if (this.audio.IsPlaying)
        {
            return "Playing";
        }

        if (this.audio.IsPaused)
        {
            return "Paused";
        }

        return this.audio.IsStopped ? "Stopped" : "Unknown";
    }

    /// <summary>
    /// Creates the info label at the top of the window.
    /// </summary>
    private void CreateInfoLabel()
    {
        this.lblInfo = new Label
        {
            Text = "Use the audio controls to manipulate the audio.\n---------------------------------------------------------",
        };

        this.lblInfo.Load();
    }

    /// <summary>
    /// Creates all the audio controls inside a container.
    /// </summary>
    private void CreateAudioCtrls()
    {
        // Audio file dropdown
        this.lblAudioFile = new Label { Text = "Audio File:" };
        this.drpAudioFile = new DropDown();
        this.drpAudioFile.Width = 400;
        this.drpAudioFile.AddItem("Ridley Draygon Theme (OGG)");
        this.drpAudioFile.AddItem("Ridley's Hideout (MP3)");
        this.drpAudioFile.AddItem("Mother Brain Final Battle (OGG)");
        this.drpAudioFile.SelectedItemChanged += DrpAudioFile_SelectedItemChanged;

        this.layAudioFile = new Layout
        {
            StackDirection = StackDirection.Horizontal,
            Centered = true,
        };
        this.layAudioFile.AddControl(this.lblAudioFile);
        this.layAudioFile.AddControl(this.drpAudioFile);

        // Volume slider
        this.lblVolume = new Label { Text = "Volume:" };
        this.sldVolume = new Slider
        {
            Min = 0f,
            Max = 100f,
            Value = 100f,
        };
        this.sldVolume.ValueChanged += SldVolume_ValueChanged;

        this.layVolume = new Layout
        {
            StackDirection = StackDirection.Horizontal,
            Centered = true,
        };
        this.layVolume.AddControl(this.lblVolume);
        this.layVolume.AddControl(this.sldVolume);

        // Position slider
        this.lblPosition = new Label { Text = "Position:" };
        this.sldPosition = new Slider
        {
            Min = 0f,
            Max = (float)this.audio.Length.TotalSeconds,
        };
        this.sldPosition.ValueChanged += SldPosition_ValueChanged;

        this.layPosition = new Layout
        {
            StackDirection = StackDirection.Horizontal,
            Centered = true,
        };
        this.layPosition.AddControl(this.lblPosition);
        this.layPosition.AddControl(this.sldPosition);

        // Buttons
        this.btnRewind = new Button { Text = "Rewind 10 Sec" };
        this.btnRewind.Click += BtnRewind_Click;

        this.btnFastForward = new Button { Text = "Fast Forward 10 Sec" };
        this.btnFastForward.Click += BtnFastForward_Click;

        this.btnPause = new Button { Text = "Pause" };
        this.btnPause.Click += BtnPause_Click;

        this.btnStop = new Button { Text = "Stop" };
        this.btnStop.Click += BtnStop_Click;

        this.btnPlay = new Button { Text = "Play" };
        this.btnPlay.Click += BtnPlay_Click;

        // Repeat checkbox
        this.chkRepeat = new CheckBox { Text = "Does Not Repeat" };
        this.chkRepeat.CheckedChanged += ChkRepeat_CheckedChanged;

        // Main layout
        this.layMain = new Layout
        {
            StackDirection = StackDirection.Vertical,
        };
        // this.layMain.AddControl(this.layAudioFile);
        // this.layMain.AddControl(this.layVolume);
        // this.layMain.AddControl(this.layPosition);
        // this.layMain.AddControl(this.btnRewind);
        // this.layMain.AddControl(this.btnFastForward);
        // this.layMain.AddControl(this.btnPause);
        // this.layMain.AddControl(this.btnStop);
        // this.layMain.AddControl(this.btnPlay);
        // this.layMain.AddControl(this.chkRepeat);

        // Container
        this.conAudio = new Container
        {
            Name = "Audio Container",
            Title = "Audio Controls",
        };
        this.conAudio.AddLayoutControl(this.layMain);

        this.conAudio.Load();
    }

    /// <summary>
    /// Invoked when the selected audio file changes in the dropdown.
    /// </summary>
    private void DrpAudioFile_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
    {
        this.audio.Stop();
        this.contentManager.Unload(this.audio);

        var audioName = e.NewValue switch
        {
            "Ridley Draygon Theme (OGG)" => "ridley-draygon-theme.ogg",
            "Ridley's Hideout (MP3)" => "ridleys-hideout.mp3",
            "Mother Brain Final Battle (OGG)" => "mother-brain-final-battle.ogg",
            _ => throw new ArgumentException($"The audio item '{e.NewValue}' is not supported."),
        };

        this.audio = this.contentManager.LoadAudio(audioName, AudioBuffer.Stream);
        this.currentAudioType = Path.GetExtension(this.audio.FilePath).ToUpper().TrimStart('.');

        this.sldPosition.Max = (float)this.audio.Length.TotalSeconds;
    }

    /// <summary>
    /// Invoked when the volume slider value changes.
    /// </summary>
    private void SldVolume_ValueChanged(object? sender, ValueChangedEventArgs e) =>
        this.audio.Volume = e.NewValue;

    /// <summary>
    /// Invoked when the position slider value changes.
    /// </summary>
    private void SldPosition_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (this.updatingPositionSlider)
        {
            return;
        }

        this.audio.SetTimePosition(e.NewValue);
    }

    /// <summary>
    /// Invoked when the rewind button is clicked.
    /// </summary>
    private void BtnRewind_Click(object? sender, EventArgs e) => this.audio.Rewind(10f);

    /// <summary>
    /// Invoked when the fast forward button is clicked.
    /// </summary>
    private void BtnFastForward_Click(object? sender, EventArgs e) => this.audio.FastForward(10f);

    /// <summary>
    /// Invoked when the pause button is clicked.
    /// </summary>
    private void BtnPause_Click(object? sender, EventArgs e) => this.audio.Pause();

    /// <summary>
    /// Invoked when the stop button is clicked.
    /// </summary>
    private void BtnStop_Click(object? sender, EventArgs e) => this.audio.Stop();

    /// <summary>
    /// Invoked when the play button is clicked.
    /// </summary>
    private void BtnPlay_Click(object? sender, EventArgs e) => this.audio.Play();

    /// <summary>
    /// Invoked when the repeat checkbox checked state changes.
    /// </summary>
    private void ChkRepeat_CheckedChanged(object? sender, CheckChangedEventArgs e)
    {
        this.audio.IsLooping = e.IsChecked;
        this.chkRepeat.Text = e.IsChecked ? "Does Repeat" : "Does Not Repeat";
    }
}
