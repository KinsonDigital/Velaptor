// <copyright file="SoundScene.cs" company="KinsonDigital">
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

public class SoundScene : SceneBase
{
    private const int WindowPadding = 10;
    private IControlGroup? grpInfoCtrls;
    private IControlGroup? grpSoundCtrls;
    private BackgroundManager? backgroundManager;
    private ILoader<ISound>? soundLoader;
    private ISound? sound;
    private string? lblCurrentTimeName;
    private string? lblSoundStateName;
    private string? lblSoundRepeatsName;
    private string? lblSoundLengthName;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.soundLoader = ContentLoaderFactory.CreateSoundLoader();
        this.sound = this.soundLoader.Load("test-song");

        CreateInfoCtrls();
        CreateSoundCtrls();

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentSoundTime = GetFormattedSoundTime(this.sound.Position.Minutes, this.sound.Position.Seconds);

        var lblSoundLengthCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblSoundLengthName);
        lblSoundLengthCtrl.Text = $"Sound Length: {GetFormattedSoundTime(this.sound.Length.Minutes, this.sound.Length.Seconds)}";

        var lblCurrentTimeCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblCurrentTimeName);
        lblCurrentTimeCtrl.Text = $"Current Time: {currentSoundTime}";

        base.Update(frameTime);
    }

    public override void Render()
    {
        this.backgroundManager?.Render();

        this.grpInfoCtrls.Render();
        this.grpSoundCtrls.Render();

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
            this.soundLoader.Unload(this.sound);
        }

        this.backgroundManager?.Unload();

        this.grpInfoCtrls.Dispose();
        this.grpSoundCtrls.Dispose();
        this.grpInfoCtrls = null;
        this.grpSoundCtrls = null;

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

    private void CreateInfoCtrls()
    {
        var lblDesc = TestingApp.Container.GetInstance<ILabel>();
        lblDesc.Name = nameof(lblDesc);
        lblDesc.Text = "Use the sound controls to manipulate the sound.";

        var lblSoundState = TestingApp.Container.GetInstance<ILabel>();
        lblSoundState.Name = nameof(lblSoundState);
        this.lblSoundStateName = nameof(lblSoundState);
        lblSoundState.Text = "Sound State: Stopped";

        var lblCurrentTime = TestingApp.Container.GetInstance<ILabel>();
        this.lblCurrentTimeName = nameof(lblCurrentTime);
        lblCurrentTime.Name = nameof(lblCurrentTime);
        lblCurrentTime.Text = "Current Time: 00:00";

        var lblSoundLength = TestingApp.Container.GetInstance<ILabel>();
        this.lblSoundLengthName = nameof(lblSoundLength);
        lblSoundLength.Name = nameof(lblSoundLength);
        lblSoundLength.Text = "Sound Length: 00:00";

        var lblSoundRepeats = TestingApp.Container.GetInstance<ILabel>();
        lblSoundRepeats.Name = nameof(lblSoundRepeats);
        this.lblSoundRepeatsName = nameof(lblSoundRepeats);
        lblSoundRepeats.Text = "Repeat Enabled: no";

        this.grpInfoCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpInfoCtrls.Title = "Sound Info";
        this.grpInfoCtrls.AutoSizeToFitContent = true;
        this.grpInfoCtrls.TitleBarVisible = false;
        this.grpInfoCtrls.Initialized += (_, _) =>
        {
            this.grpInfoCtrls.Position = new Point(
                WindowCenter.X - (this.grpInfoCtrls.Width + WindowPadding),
                WindowCenter.Y - this.grpInfoCtrls.HalfHeight);
        };

        this.grpInfoCtrls.Add(lblSoundRepeats);
        this.grpInfoCtrls.Add(lblSoundLength);
        this.grpInfoCtrls.Add(lblCurrentTime);
        this.grpInfoCtrls.Add(lblSoundState);
        this.grpInfoCtrls.Add(lblDesc);
    }

    private void CreateSoundCtrls()
    {
        var btnPlay = TestingApp.Container.GetInstance<IButton>();
        btnPlay.Name = nameof(btnPlay);
        btnPlay.Text = "Play";
        btnPlay.Click += (_, _) =>
        {
            this.sound.Play();

            var lblSoundStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblSoundStateName);
            lblSoundStateCtrl.Text = $"Sound State: {this.sound.State.ToString()}";
        };

        var btnStop = TestingApp.Container.GetInstance<IButton>();
        btnStop.Name = nameof(btnStop);
        btnStop.Text = "Stop";
        btnStop.Click += (_, _) =>
        {
            this.sound.Stop();

            var lblSoundStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblSoundStateName);
            lblSoundStateCtrl.Text = $"Sound State: {this.sound.State.ToString()}";
        };

        var btnPause = TestingApp.Container.GetInstance<IButton>();
        btnPause.Name = nameof(btnPause);
        btnPause.Text = "Pause";
        btnPause.Click += (_, _) =>
        {
            this.sound.Pause();

            var lblSoundStateCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblSoundStateName);
            lblSoundStateCtrl.Text = $"Sound State: {this.sound.State.ToString()}";
        };

        var btnFastForward = TestingApp.Container.GetInstance<IButton>();
        btnFastForward.Name = nameof(btnFastForward);
        btnFastForward.Text = "Fast Forward 10 Sec";
        btnFastForward.Click += (_, _) =>
        {
            this.sound.FastForward(10f);
        };

        var btnRewind = TestingApp.Container.GetInstance<IButton>();
        btnRewind.Name = nameof(btnRewind);
        btnRewind.Text = "Rewind 10 Sec";
        btnRewind.Click += (_, _) =>
        {
            this.sound.Rewind(10f);
        };

        var chkRepeat = TestingApp.Container.GetInstance<ICheckBox>();
        chkRepeat.Name = nameof(chkRepeat);
        chkRepeat.LabelWhenChecked = "Does Repeat";
        chkRepeat.LabelWhenUnchecked = "Do Not Repeat";
        chkRepeat.CheckedChanged += (_, isChecked) =>
        {
            this.sound.IsLooping = isChecked;

            var lblSoundRepeatsCtrl = this.grpInfoCtrls.GetControl<ILabel>(this.lblSoundRepeatsName);
            lblSoundRepeatsCtrl.Text = $"Repeat Enabled: {(this.sound.IsLooping ? "yes" : "no")}";
        };

        this.grpSoundCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpSoundCtrls.Title = "Sound Controls";
        this.grpSoundCtrls.AutoSizeToFitContent = true;
        this.grpSoundCtrls.TitleBarVisible = false;
        this.grpSoundCtrls.Initialized += (_, _) =>
        {
            this.grpSoundCtrls.Position = new Point(
                WindowCenter.X + WindowPadding,
                WindowCenter.Y - this.grpSoundCtrls.HalfHeight);
        };
        this.grpSoundCtrls.Add(btnRewind);
        this.grpSoundCtrls.Add(btnFastForward);
        this.grpSoundCtrls.Add(btnPause);
        this.grpSoundCtrls.Add(btnStop);
        this.grpSoundCtrls.Add(btnPlay);
        this.grpSoundCtrls.Add(chkRepeat);
    }
}
