// <copyright file="TestRenderTextScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using VelaptorTesting.Core;

    /// <summary>
    /// Used to test out if text is properly being rendered to the screen.
    /// </summary>
    public class TestRenderTextScene : SceneBase
    {
        private const float AngularVelocity = 10f;
        private const float SizeChangeAmount = 0.5f;
        private const string SingleLineText = "Change me using the buttons below.";
        private const string MultiLineText = "Change me using\nthe buttons below.";
        private IFont? textFont;
        private Button? btnRotateCW;
        private Button? btnRotateCCW;
        private Button? btnIncreaseRenderSize;
        private Button? btnDecreaseRenderSize;
        private Button? btnSetMultiLine;
        private Button? btnSetColor;
        private Button? btnSetStyle;
        private Button? btnIncreaseFontSize;
        private Button? btnDecreaseFontSize;
        private bool cwButtonDown;
        private bool ccwButtonDown;
        private bool increaseBtnDown;
        private bool decreaseBtnDown;
        private bool isMultiLine = true;
        private bool isClrSet;
        private float angle;
        private float renderSize = 1f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRenderTextScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestRenderTextScene(IContentLoader contentLoader)
            : base(contentLoader)
        {
        }

        /// <inheritdoc cref="IScene.LoadContent"/>
        public override void LoadContent()
        {
            ThrowExceptionIfLoadingWhenDisposed();

            if (IsLoaded)
            {
                return;
            }

            this.textFont = ContentLoader.LoadFont("times", 12);

            // // Rotate CW Button
            this.btnRotateCW = new Button(ContentLoader);
            this.btnRotateCW.Text = "CW";
            this.btnRotateCW.MouseDown += (_, _) =>
            {
                this.cwButtonDown = true;
                this.ccwButtonDown = false;
            };

            this.btnRotateCW.MouseUp += (_, _) =>
            {
                this.cwButtonDown = false;
                this.ccwButtonDown = false;
            };

            // Rotate CCW Button
            this.btnRotateCCW = new Button(ContentLoader);
            this.btnRotateCCW.Text = "CCW";

            this.btnRotateCCW.MouseDown += (_, _) =>
            {
                this.ccwButtonDown = true;
                this.cwButtonDown = false;
            };

            this.btnRotateCCW.MouseUp += (_, _) =>
            {
                this.ccwButtonDown = false;
                this.cwButtonDown = false;
            };

            // Increase Render Size Button
            this.btnIncreaseRenderSize = new Button(ContentLoader);
            this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
            this.btnIncreaseRenderSize.FaceTextureName = "button-face-extra-large";

            this.btnIncreaseRenderSize.MouseDown += (_, _) =>
            {
                this.increaseBtnDown = true;
                this.decreaseBtnDown = false;
            };

            this.btnIncreaseRenderSize.MouseUp += (_, _) =>
            {
                this.increaseBtnDown = false;
                this.decreaseBtnDown = false;
                this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
                this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
            };

            // Decrease Render Size Button
            this.btnDecreaseRenderSize = new Button(ContentLoader);
            this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
            this.btnDecreaseRenderSize.FaceTextureName = "button-face-extra-large";

            this.btnDecreaseRenderSize.MouseDown += (_, _) =>
            {
                this.decreaseBtnDown = true;
                this.increaseBtnDown = false;
            };

            this.btnDecreaseRenderSize.MouseUp += (_, _) =>
            {
                this.increaseBtnDown = false;
                this.decreaseBtnDown = false;
                this.btnDecreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) -";
                this.btnIncreaseRenderSize.Text = $"Render Size({Math.Round(this.renderSize, 2)}) +";
            };

            // Set Multi-Line
            this.btnSetMultiLine = new Button(ContentLoader);
            this.btnSetMultiLine.Text = $"Multi-Line: {this.isMultiLine}";
            this.btnSetMultiLine.FaceTextureName = "button-face-large";

            this.btnSetMultiLine.MouseUp += (_, _) =>
            {
                this.isMultiLine = !this.isMultiLine;

                this.btnSetMultiLine.Text = $"Multi-Line: {this.isMultiLine}";
            };

            // Set Color
            this.btnSetColor = new Button(ContentLoader);
            this.btnSetColor.Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}";
            this.btnSetColor.FaceTextureName = "button-face-large";

            this.btnSetColor.MouseUp += (_, _) =>
            {
                this.isClrSet = !this.isClrSet;
                this.btnSetColor.Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}";
            };

            // Set the font style to bold
            this.btnSetStyle = new Button(ContentLoader);
            this.btnSetStyle.Text = $"Style: {this.textFont.Style}";
            this.btnSetStyle.FaceTextureName = "button-face-large";

            this.btnSetStyle.MouseUp += (_, _) =>
            {
                this.textFont.Style = this.textFont.Style switch
                {
                    FontStyle.Regular => FontStyle.Bold,
                    FontStyle.Bold => FontStyle.Italic,
                    FontStyle.Italic => FontStyle.Bold | FontStyle.Italic,
                    FontStyle.Bold | FontStyle.Italic => FontStyle.Regular,
                    _ => throw new ArgumentOutOfRangeException()
                };

                this.btnSetStyle.Text = $"Style: {this.textFont.Style}";
            };

            // Increase font size
            this.btnIncreaseFontSize = new Button(ContentLoader);
            this.btnIncreaseFontSize.Text = $"Font Size({this.textFont.Size}) +";
            this.btnIncreaseFontSize.FaceTextureName = "button-face-large";
            this.btnIncreaseFontSize.MouseUp += (_, _) =>
            {
                this.textFont.Size += 1;
                this.btnIncreaseFontSize.Text = $"Font Size({this.textFont.Size}) +";
                this.btnDecreaseFontSize.Text = $"Font Size({this.textFont.Size}) -";
            };

            // Decrease font size
            this.btnDecreaseFontSize = new Button(ContentLoader);
            this.btnDecreaseFontSize.Text = $"Font Size({this.textFont.Size}) -";
            this.btnDecreaseFontSize.FaceTextureName = "button-face-large";
            this.btnDecreaseFontSize.MouseUp += (_, _) =>
            {
                this.textFont.Size -= this.textFont.Size == 0u ? 0u : 1u;
                this.btnIncreaseFontSize.Text = $"Font Size({this.textFont.Size}) +";
                this.btnDecreaseFontSize.Text = $"Font Size({this.textFont.Size}) -";
            };

            AddControl(this.btnRotateCW);
            AddControl(this.btnRotateCCW);
            AddControl(this.btnIncreaseRenderSize);
            AddControl(this.btnDecreaseRenderSize);
            AddControl(this.btnSetMultiLine);
            AddControl(this.btnSetColor);
            AddControl(this.btnSetStyle);
            AddControl(this.btnIncreaseFontSize);
            AddControl(this.btnDecreaseFontSize);

            base.LoadContent();

            SetupUIControlPositioning();
        }

        /// <inheritdoc cref="IScene.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            UnloadSceneContent();

            base.UnloadContent();
        }

        public override void Update(FrameTime frameTime)
        {
            // Rotate CW
            if (this.cwButtonDown && this.ccwButtonDown is false)
            {
                this.angle += AngularVelocity * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            // Rotate CCW
            if (this.ccwButtonDown && this.cwButtonDown is false)
            {
                this.angle -= AngularVelocity * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            // Increase Size
            if (this.increaseBtnDown && this.decreaseBtnDown is false)
            {
                this.renderSize += SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            // Decrease Size
            if (this.decreaseBtnDown && this.increaseBtnDown is false)
            {
                this.renderSize -= SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            this.renderSize = this.renderSize < 0f ? 0f : this.renderSize;

            base.Update(frameTime);
        }

        /// <inheritdoc cref="IDrawable.Render"/>
        public override void Render(ISpriteBatch spriteBatch)
        {
            var xPos = (int)(MainWindow.WindowWidth / 2f);
            var yPos = (int)MainWindow.WindowHeight / 2;

            spriteBatch.Render(
                this.textFont,
                this.isMultiLine ? MultiLineText : SingleLineText,
                xPos,
                yPos,
                this.renderSize,
                this.angle,
                this.isClrSet ? Color.CornflowerBlue : Color.White);

            this.btnRotateCW.Render(spriteBatch);
            this.btnRotateCCW.Render(spriteBatch);
            this.btnIncreaseRenderSize.Render(spriteBatch);
            this.btnDecreaseRenderSize.Render(spriteBatch);
            this.btnSetMultiLine.Render(spriteBatch);
            this.btnSetColor.Render(spriteBatch);

            base.Render(spriteBatch);
        }

        /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !IsLoaded)
            {
                return;
            }

            if (disposing)
            {
                UnloadSceneContent();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Unloads the scenes content.
        /// </summary>
        private void UnloadSceneContent()
        {
            ContentLoader.UnloadFont(this.textFont);
        }

        /// <summary>
        /// Sets up the positioning of all the UI controls in the window.
        /// </summary>
        private void SetupUIControlPositioning()
        {
            // Control Positioning
            const int buttonSpacing = 15;
            var largestHalfWidth = (int)new[]
                                   {
                                       this.btnRotateCW.Width,
                                       this.btnRotateCCW.Width,
                                       this.btnIncreaseRenderSize.Width,
                                       this.btnDecreaseRenderSize.Width,
                                       this.btnSetMultiLine.Width,
                                       this.btnSetColor.Width,
                                       this.btnSetStyle.Width,
                                       this.btnIncreaseFontSize.Width,
                                       this.btnDecreaseFontSize.Width,
                                   }.Max() / 2;

            var leftMargin = 15 + largestHalfWidth;

            this.btnRotateCW.Position = new Point(leftMargin, (int)(this.btnRotateCW.Height / 2) + buttonSpacing);

            this.btnRotateCCW.Position = new Point(
                leftMargin,
                this.btnRotateCW.Bottom + (int)(this.btnRotateCCW.Height / 2) + buttonSpacing);

            this.btnIncreaseRenderSize.Position = new Point(
                leftMargin,
                this.btnRotateCCW.Bottom + (int)(this.btnIncreaseRenderSize.Height / 2) + buttonSpacing);

            this.btnDecreaseRenderSize.Position = new Point(
                leftMargin,
                this.btnIncreaseRenderSize.Bottom + ((int)this.btnDecreaseRenderSize.Height / 2) + buttonSpacing);

            this.btnSetMultiLine.Position = new Point(
                leftMargin,
                this.btnDecreaseRenderSize.Bottom + ((int)this.btnSetMultiLine.Height / 2) + buttonSpacing);

            this.btnSetColor.Position = new Point(
                leftMargin,
                this.btnSetMultiLine.Bottom + ((int)this.btnSetColor.Height / 2) + buttonSpacing);

            this.btnSetStyle.Position = new Point(
                leftMargin,
                this.btnSetColor.Bottom + ((int)this.btnSetStyle.Height / 2) + buttonSpacing);

            this.btnIncreaseFontSize.Position = new Point(
                leftMargin,
                this.btnSetStyle.Bottom + ((int)this.btnIncreaseFontSize.Height / 2) + buttonSpacing);

            this.btnDecreaseFontSize.Position = new Point(
                leftMargin,
                this.btnIncreaseFontSize.Bottom + ((int)this.btnDecreaseFontSize.Height / 2) + buttonSpacing);
        }
    }
}
