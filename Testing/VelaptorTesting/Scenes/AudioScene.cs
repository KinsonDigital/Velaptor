// <copyright file="AudioScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Scene;

public class AudioScene : SceneBase
{
    private const int WindowPadding = 10;
    private readonly ControlFactory ctrlFactory;
    private IControlGroup? grpInfoCtrls;
    private IControlGroup? grpAudioCtrls;
    private BackgroundManager? backgroundManager;
    private ILoader<IAudio>? loader;
    private IAudio? audio;
    private ISlider? sldPosition;
    private string? lblRepeatsName;
    private string? lblLengthName;
    private string? lblCurrentTimeName;
    private string? lblStateName;
    private string? lblAudioTypeName;
    private string? currentAudioType = "OGG";

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioScene"/> class.
    /// </summary>
    public AudioScene() => this.ctrlFactory = new ControlFactory();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.loader = ContentLoaderFactory.CreateAudioLoader();
        this.audio = this.loader.Load("ridley-draygon-theme.ogg", AudioBuffer.Stream);

        CreateInfoCtrls();
        CreateAudioCtrls();

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentTime = GetFormattedTime(this.audio.Position.Minutes, this.audio.Position.Seconds);

        var lblAudioType = this.grpInfoCtrls.GetControl<ILabel>(this.lblAudioTypeName);
        lblAudioType.Text = $"Audio Type: {this.currentAudioType}";

        var lblLengthCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblLengthName);
        lblLengthCtrl.Text = $"Audio Length: {GetFormattedTime(this.audio.Length.Minutes, this.audio.Length.Seconds)}";

        var lblCurrentTimeCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblCurrentTimeName);
        lblCurrentTimeCtrl.Text = $"Current Time: {currentTime}";

        this.sldPosition.Value = (float)this.audio.Position.TotalSeconds;

        this.grpInfoCtrls.Position = new Point(WindowCenter.X - this.grpInfoCtrls.HalfWidth, WindowPadding);
        this.grpAudioCtrls.Position = new Point(WindowCenter.X - this.grpAudioCtrls.HalfWidth, WindowCenter.Y - this.grpAudioCtrls.HalfHeight);
        base.Update(frameTime);
    }

    /// <summary>
    /// <inheritdoc cref="IDrawable.Render"/>
    /// </summary>
    public override void Render()
    {
        this.backgroundManager?.Render();

        this.grpInfoCtrls.Render();
        this.grpAudioCtrls.Render();

        base.Render();
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
            this.audio.Stop();
            this.loader.Unload(this.audio);
        }

        this.backgroundManager?.Unload();

        this.grpInfoCtrls.Dispose();
        this.grpAudioCtrls.Dispose();
        this.grpInfoCtrls = null;
        this.grpAudioCtrls = null;

        base.UnloadContent();
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

    private void CreateInfoCtrls()
    {
        var lblDesc = this.ctrlFactory.CreateLabel();
        lblDesc.Name = nameof(lblDesc);
        lblDesc.Text = "Use the audio controls to manipulate the audio.";
        lblDesc.Text += $"\n---------------------------------------------------------";

        var lblRepeats = this.ctrlFactory.CreateLabel();
        lblRepeats.Name = nameof(lblRepeats);
        this.lblRepeatsName = nameof(lblRepeats);
        lblRepeats.Text = "Repeat Enabled: no";

        var lblAudioType = this.ctrlFactory.CreateLabel();
        lblAudioType.Name = nameof(lblAudioType);
        this.lblAudioTypeName = nameof(lblAudioType);
        lblAudioType.Text = $"Audio Type: {this.currentAudioType ?? string.Empty}";

        var lblLength = this.ctrlFactory.CreateLabel();
        this.lblLengthName = nameof(lblLength);
        lblLength.Name = nameof(lblLength);
        lblLength.Text = "Audio Length: 00:00";

        var lblCurrentTime = this.ctrlFactory.CreateLabel();
        this.lblCurrentTimeName = nameof(lblCurrentTime);
        lblCurrentTime.Name = nameof(lblCurrentTime);
        lblCurrentTime.Text = "Current Time: 00:00";

        var lblState = this.ctrlFactory.CreateLabel();
        lblState.Name = nameof(lblState);
        this.lblStateName = nameof(lblState);
        lblState.Text = "Audio State: Stopped";

        this.grpInfoCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpInfoCtrls.Title = "Audio Info";
        this.grpInfoCtrls.AutoSizeToFitContent = true;
        this.grpInfoCtrls.TitleBarVisible = false;

        this.grpInfoCtrls.Add(lblDesc);
        this.grpInfoCtrls.Add(lblRepeats);
        this.grpInfoCtrls.Add(lblAudioType);
        this.grpInfoCtrls.Add(lblLength);
        this.grpInfoCtrls.Add(lblCurrentTime);
        this.grpInfoCtrls.Add(lblState);
    }

    private void CreateAudioCtrls()
    {
        var audioList = this.ctrlFactory.CreateComboBox();
        audioList.Label = "Audio File";

        audioList.Items.Add("Ridley Draygon Theme (OGG)");
        audioList.Items.Add("Ridleys Hideout (MP3)");
        audioList.Items.Add("Mother Brain Final Battle (OGG)");
        audioList.SelectedItemIndexChanged += (_, i) =>
        {
            this.audio.Stop();
            this.loader.Unload(this.audio);

            var chosenItem = audioList.Items[i];
            var audioName = chosenItem switch
            {
                "Ridley Draygon Theme (OGG)" => "ridley-draygon-theme.ogg",
                "Ridleys Hideout (MP3)" => "ridleys-hideout.mp3",
                "Mother Brain Final Battle (OGG)" => "mother-brain-final-battle.ogg",
                _ => throw new ArgumentException($"The audio item '{chosenItem}' is not supported."),
            };

            this.audio = this.loader.Load(audioName, AudioBuffer.Stream);

            this.currentAudioType = Path.GetExtension(this.audio.FilePath).ToUpper().TrimStart('.');
        };

        var sldVolume = this.ctrlFactory.CreateSlider();
        sldVolume.Name = nameof(sldVolume);
        sldVolume.Min = 0f;
        sldVolume.Max = 100f;
        sldVolume.Value = 100f;
        sldVolume.Text = "Volume";
        sldVolume.ValueChanged += (_, value) =>
        {
            this.audio.Volume = value;
        };

        this.sldPosition = this.ctrlFactory.CreateSlider();
        this.sldPosition.Name = nameof(this.sldPosition);
        this.sldPosition.Min = 0f;
        this.sldPosition.Max = (float)this.audio.Length.TotalSeconds;
        this.sldPosition.Text = "Position";
        this.sldPosition.ValueChanged += (_, value) =>
        {
            this.audio.SetTimePosition(value);
        };

        var btnPlay = this.ctrlFactory.CreateButton();
        btnPlay.Name = nameof(btnPlay);
        btnPlay.Text = "Play";
        btnPlay.Click += (_, _) =>
        {
            this.audio.Play();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnStop = this.ctrlFactory.CreateButton();
        btnStop.Name = nameof(btnStop);
        btnStop.Text = "Stop";
        btnStop.Click += (_, _) =>
        {
            this.audio.Stop();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnPause = this.ctrlFactory.CreateButton();
        btnPause.Name = nameof(btnPause);
        btnPause.Text = "Pause";
        btnPause.Click += (_, _) =>
        {
            this.audio.Pause();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnFastForward = this.ctrlFactory.CreateButton();
        btnFastForward.Name = nameof(btnFastForward);
        btnFastForward.Text = "Fast Forward 10 Sec";
        btnFastForward.Click += (_, _) =>
        {
            this.audio.FastForward(10f);
        };

        var btnRewind = this.ctrlFactory.CreateButton();
        btnRewind.Name = nameof(btnRewind);
        btnRewind.Text = "Rewind 10 Sec";
        btnRewind.Click += (_, _) =>
        {
            this.audio.Rewind(10f);
        };

        var chkRepeat = this.ctrlFactory.CreateCheckbox();
        chkRepeat.Name = nameof(chkRepeat);
        chkRepeat.LabelWhenChecked = "Does Repeat";
        chkRepeat.LabelWhenUnchecked = "Does Not Repeat";
        chkRepeat.CheckedChanged += (_, isChecked) =>
        {
            this.audio.IsLooping = isChecked;

            var lblRepeatsCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblRepeatsName);
            lblRepeatsCtrl.Text = $"Repeat Enabled: {(this.audio.IsLooping ? "yes" : "no")}";
        };

        this.grpAudioCtrls = this.ctrlFactory.CreateControlGroup();
        this.grpAudioCtrls.Title = "Audio Controls";
        this.grpAudioCtrls.AutoSizeToFitContent = true;
        this.grpAudioCtrls.TitleBarVisible = false;

        this.grpAudioCtrls.Add(audioList);
        this.grpAudioCtrls.Add(sldVolume);
        this.grpAudioCtrls.Add(this.sldPosition);
        this.grpAudioCtrls.Add(btnRewind);
        this.grpAudioCtrls.Add(btnFastForward);
        this.grpAudioCtrls.Add(btnPause);
        this.grpAudioCtrls.Add(btnStop);
        this.grpAudioCtrls.Add(btnPlay);
        this.grpAudioCtrls.Add(chkRepeat);

        return;

        string GetState()
        {
            if (this.audio.IsPlaying)
            {
                return "Playing";
            }

            if (this.audio.IsPaused)
            {
                return "Stopped";
            }

            return this.audio.IsStopped ? "Stopped" : "Unknown";
        }
    }
}
