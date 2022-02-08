// <copyright file="TestSoundsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.UI;
    using VelaptorTesting.Core;
    using ISound = Velaptor.Content.ISound;

    public class TestSoundsScene : SceneBase
    {
        private const int BottomMargin = 50;
        private const int HorBtnSpacing = 10;
        private const int VertLabelSpacing = 15;
        private const int LeftMargin = 50;
        private readonly Point windowCenter;
        private readonly int windowBottom;
        private readonly List<IControl> buttons = new ();
        private readonly List<IControl> labels = new ();
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

        public TestSoundsScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
            this.windowCenter.X = (int)MainWindow.WindowWidth / 2;
            this.windowCenter.Y = (int)MainWindow.WindowHeight / 2;

            this.windowBottom = (int)MainWindow.WindowHeight;
        }

        public override void LoadContent()
        {
            LoadLabels();
            LoadButtons();

            this.sound = ContentLoader.LoadSound("test-song");

            base.LoadContent();

            PerformButtonLayout();
            PerformLabelLayout();

            this.lblSoundLength.Text =
                $"Sound Length: {Math.Round(this.sound.Length.Minutes, 0)}:{Math.Round(this.sound.Length.Seconds, 0)}";
        }

        public override void Update(FrameTime frameTime)
        {
            var minutes = Math.Round(this.sound.Position.Minutes, 0);
            var seconds = Math.Round(this.sound.Position.Seconds, 0);

            this.lblState.Text = $"Sound State: {this.sound.State.ToString()}";
            this.lblRepeat.Text = $"Enable Repeat: {(this.sound.IsLooping ? "yes" : "no")}";
            this.btnRepeat.Text = this.sound.IsLooping ? "Disable Repeat" : "Enable Repeat";
            this.lblCurrentTime.Text = $"Current Time: {minutes}:{seconds}";

            PerformLabelLayout();
            base.Update(frameTime);
        }

        public override void UnloadContent()
        {
            this.sound.Stop();

            this.buttons.Clear();
            ContentLoader.UnloadSound(this.sound);

            base.UnloadContent();
        }

        private void LoadLabels()
        {
            this.lblDescription = new Label
            {
                Text = "Use the buttons below to manipulate the sound.",
                Position = new Point(this.windowCenter.X, 50),
                Color = Color.White,
            };
            AddControl(this.lblDescription);

            this.lblState = new Label
            {
                Text = "Sound State:",
                Color = Color.White,
            };
            this.labels.Add(this.lblState);
            AddControl(this.lblState);

            // Current Sound time
            this.lblCurrentTime = new Label
            {
                Text = "Current Time: 0:0",
                Color = Color.White,
            };
            this.labels.Add(this.lblCurrentTime);
            AddControl(this.lblCurrentTime);

            // Total Sound Length
            this.lblSoundLength = new Label
            {
                Text = "Sound Length: ",
                Color = Color.White,
            };
            this.labels.Add(this.lblSoundLength);
            AddControl(this.lblSoundLength);

            // Loop Setting
            this.lblRepeat = new Label
            {
                Text = "Enable Repeat: no",
                Color = Color.White,
            };
            this.labels.Add(this.lblRepeat);
            AddControl(this.lblRepeat);
        }

        private void LoadButtons()
        {
            // Play Sound
            this.btnPlaySound = new Button
            {
                Text = "Play",
            };
            this.buttons.Add(this.btnPlaySound);
            this.btnPlaySound.Click += (_, _) =>
            {
                this.sound.Play();
            };
            AddControl(this.btnPlaySound);

            // Stop Sound
            this.btnStopSound = new Button
            {
                Text = "Stop",
            };
            this.buttons.Add(this.btnStopSound);
            this.btnStopSound.Click += (_, _) =>
            {
                this.sound.Stop();
            };
            AddControl(this.btnStopSound);

            // Pause Sound
            this.btnPauseSound = new Button
            {
                Text = "Pause",
            };
            this.buttons.Add(this.btnPauseSound);
            this.btnPauseSound.Click += (_, _) =>
            {
                this.sound.Pause();
            };
            AddControl(this.btnPauseSound);

            // Fast Forward 10 Seconds
            this.btnFastForward10Sec = new Button
            {
                Text = "Fast Forward 10 Sec",
                FaceTextureName = "button-face-extra-large",
            };
            this.buttons.Add(this.btnFastForward10Sec);
            this.btnFastForward10Sec.Click += (_, _) =>
            {
                this.sound.FastForward(10f);
            };
            AddControl(this.btnFastForward10Sec);

            // Rewind 10 Seconds
            this.btnRewind10Sec = new Button
            {
                Text = "Rewind 10 Sec",
                FaceTextureName = "button-face-extra-large",
            };
            this.buttons.Add(this.btnRewind10Sec);
            this.btnRewind10Sec.Click += (_, _) =>
            {
                this.sound.Rewind(10f);
            };
            AddControl(this.btnRewind10Sec);

            // Loop Setting
            this.btnRepeat = new Button
            {
                Text = "Enable Repeat",
                FaceTextureName = "button-face-extra-large",
            };
            this.buttons.Add(this.btnRepeat);
            this.btnRepeat.Click += (_, _) =>
            {
                this.sound.IsLooping = !this.sound.IsLooping;
            };
            AddControl(this.btnRepeat);
        }

        private void PerformButtonLayout()
        {
            var totalWidth = (from b in this.buttons
                select (int)b.Width).ToArray().Sum();
            totalWidth += (this.buttons.Count - 1) * HorBtnSpacing;

            var totalHalfWidth = totalWidth / 2;

            IControl? prevButton = null;

            foreach (var button in this.buttons)
            {
                if (prevButton is null)
                {
                    button.Position =
                        new Point((int)(this.windowCenter.X + (button.Width / 2) - totalHalfWidth),
                            this.windowBottom - BottomMargin);
                }
                else
                {
                    button.Left = prevButton.Right + HorBtnSpacing;
                    button.Top = (int)(this.windowBottom - BottomMargin - (button.Height / 2));
                }

                prevButton = button;
            }
        }

        private void PerformLabelLayout()
        {
            var totalHeight = (from b in this.labels
                select (int)b.Height).ToArray().Sum();
            totalHeight += (this.labels.Count - 1) * VertLabelSpacing;

            var totalHalfHeight = totalHeight / 2;

            IControl? prevLabel = null;

            foreach (var label in this.labels)
            {
                if (prevLabel is null)
                {
                    label.Left = LeftMargin;
                    label.Top = this.windowCenter.Y - totalHalfHeight;
                }
                else
                {
                    label.Top = prevLabel.Bottom + VertLabelSpacing;
                    label.Left = LeftMargin;
                }

                prevLabel = label;
            }
        }
    }
}
