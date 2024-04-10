// <copyright file="AudioScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Scene;

public class AudioScene : SceneBase
{
    private const int WindowPadding = 10;
    private IControlGroup? grpInfoCtrls;
    private IControlGroup? grpAudioCtrls;
    private BackgroundManager? backgroundManager;
    private ILoader<IAudio>? loader;
    private IAudio? audio;
    private string? lblCurrentTimeName;
    private string? lblStateName;
    private string? lblRepeatsName;
    private string? lblLengthName;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.loader = ContentLoaderFactory.CreateAudioLoader();
        this.audio = this.loader.Load("test-song", AudioBuffer.Stream);

        CreateInfoCtrls();
        CreateAudioCtrls();

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentTime = GetFormattedTime(this.audio.Position.Minutes, this.audio.Position.Seconds);

        var lblLengthCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblLengthName);
        lblLengthCtrl.Text = $"Audio Length: {GetFormattedTime(this.audio.Length.Minutes, this.audio.Length.Seconds)}";

        var lblCurrentTimeCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblCurrentTimeName);
        lblCurrentTimeCtrl.Text = $"Current Time: {currentTime}";

        base.Update(frameTime);
    }

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
        var lblDesc = TestingApp.Container.GetInstance<ILabel>();
        lblDesc.Name = nameof(lblDesc);
        lblDesc.Text = "Use the audio controls to manipulate the audio.";

        var lblState = TestingApp.Container.GetInstance<ILabel>();
        lblState.Name = nameof(lblState);
        this.lblStateName = nameof(lblState);
        lblState.Text = "Audio State: Stopped";

        var lblCurrentTime = TestingApp.Container.GetInstance<ILabel>();
        this.lblCurrentTimeName = nameof(lblCurrentTime);
        lblCurrentTime.Name = nameof(lblCurrentTime);
        lblCurrentTime.Text = "Current Time: 00:00";

        var lblLength = TestingApp.Container.GetInstance<ILabel>();
        this.lblLengthName = nameof(lblLength);
        lblLength.Name = nameof(lblLength);
        lblLength.Text = "Audio Length: 00:00";

        var lblRepeats = TestingApp.Container.GetInstance<ILabel>();
        lblRepeats.Name = nameof(lblRepeats);
        this.lblRepeatsName = nameof(lblRepeats);
        lblRepeats.Text = "Repeat Enabled: no";

        this.grpInfoCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpInfoCtrls.Title = "Audio Info";
        this.grpInfoCtrls.AutoSizeToFitContent = true;
        this.grpInfoCtrls.TitleBarVisible = false;
        this.grpInfoCtrls.Initialized += (_, _) =>
        {
            this.grpInfoCtrls.Position = new Point(
                WindowCenter.X - (this.grpInfoCtrls.Width + WindowPadding),
                WindowCenter.Y - this.grpInfoCtrls.HalfHeight);
        };

        this.grpInfoCtrls.Add(lblRepeats);
        this.grpInfoCtrls.Add(lblLength);
        this.grpInfoCtrls.Add(lblCurrentTime);
        this.grpInfoCtrls.Add(lblState);
        this.grpInfoCtrls.Add(lblDesc);
    }

    private void CreateAudioCtrls()
    {
        var sldVolume = TestingApp.Container.GetInstance<ISlider>();
        sldVolume.Name = nameof(sldVolume);
        sldVolume.Min = 0f;
        sldVolume.Max = 100f;
        sldVolume.Value = 100f;
        sldVolume.Text = "Volume";
        sldVolume.ValueChanged += (_, value) =>
        {
            this.audio.Volume = value;
        };

        var btnPlay = TestingApp.Container.GetInstance<IButton>();
        btnPlay.Name = nameof(btnPlay);
        btnPlay.Text = "Play";
        btnPlay.Click += (_, _) =>
        {
            this.audio.Play();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnStop = TestingApp.Container.GetInstance<IButton>();
        btnStop.Name = nameof(btnStop);
        btnStop.Text = "Stop";
        btnStop.Click += (_, _) =>
        {
            this.audio.Stop();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnPause = TestingApp.Container.GetInstance<IButton>();
        btnPause.Name = nameof(btnPause);
        btnPause.Text = "Pause";
        btnPause.Click += (_, _) =>
        {
            this.audio.Pause();

            var lblStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblStateName);
            lblStateCtrl.Text = $"Audio State: {GetState()}";
        };

        var btnFastForward = TestingApp.Container.GetInstance<IButton>();
        btnFastForward.Name = nameof(btnFastForward);
        btnFastForward.Text = "Fast Forward 10 Sec";
        btnFastForward.Click += (_, _) =>
        {
            this.audio.FastForward(10f);
        };

        var btnRewind = TestingApp.Container.GetInstance<IButton>();
        btnRewind.Name = nameof(btnRewind);
        btnRewind.Text = "Rewind 10 Sec";
        btnRewind.Click += (_, _) =>
        {
            this.audio.Rewind(10f);
        };

        var chkRepeat = TestingApp.Container.GetInstance<ICheckBox>();
        chkRepeat.Name = nameof(chkRepeat);
        chkRepeat.LabelWhenChecked = "Does Repeat";
        chkRepeat.LabelWhenUnchecked = "Do Not Repeat";
        chkRepeat.CheckedChanged += (_, isChecked) =>
        {
            this.audio.IsLooping = isChecked;

            var lblRepeatsCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblRepeatsName);
            lblRepeatsCtrl.Text = $"Repeat Enabled: {(this.audio.IsLooping ? "yes" : "no")}";
        };

        this.grpAudioCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpAudioCtrls.Title = "Audio Controls";
        this.grpAudioCtrls.AutoSizeToFitContent = true;
        this.grpAudioCtrls.TitleBarVisible = false;
        this.grpAudioCtrls.Initialized += (_, _) =>
        {
            this.grpAudioCtrls.Position = new Point(
                WindowCenter.X + WindowPadding,
                WindowCenter.Y - this.grpAudioCtrls.HalfHeight);
        };

        this.grpAudioCtrls.Add(sldVolume);
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
