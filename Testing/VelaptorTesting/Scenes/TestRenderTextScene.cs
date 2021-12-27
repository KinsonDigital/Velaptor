// <copyright file="TestRenderTextScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Content;
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
        private const string SingleLineText = "Rotate me or change my size.";
        private const string MultiLineText = "Rotate me or\nchange my size.";
        private IFont? font;
        private Button? btnRotateCW;
        private Button? btnRotateCCW;
        private Button? btnIncreaseSize;
        private Button? btnDecreaseSize;
        private Button? btnSetMultiLine;
        private Button? btnSetColor;
        private bool cwButtonDown;
        private bool ccwButtonDown;
        private bool increaseBtnDown;
        private bool decreaseBtnDown;
        private bool isMultiLine;
        private bool isClrSet;
        private float angle;
        private float size = 1f;

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

            this.font = ContentLoader.Load<IFont>("TimesNewRoman");

            // Rotate CW Button
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
            this.btnRotateCW.LoadContent();
            this.btnRotateCW.Size = 0.50f;

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
            this.btnRotateCCW.LoadContent();
            this.btnRotateCCW.Size = 0.50f;

            // Increase Size Button
            this.btnIncreaseSize = new Button(ContentLoader);
            this.btnIncreaseSize.Text = "Size +";

            this.btnIncreaseSize.MouseDown += (_, _) =>
            {
                this.increaseBtnDown = true;
                this.decreaseBtnDown = false;
            };

            this.btnIncreaseSize.MouseUp += (_, _) =>
            {
                this.increaseBtnDown = false;
                this.decreaseBtnDown = false;
            };
            this.btnIncreaseSize.LoadContent();
            this.btnIncreaseSize.Size = 0.50f;

            // Decrease Size Button
            this.btnDecreaseSize = new Button(ContentLoader);
            this.btnDecreaseSize.Text = "Size -";

            this.btnDecreaseSize.MouseDown += (_, _) =>
            {
                this.decreaseBtnDown = true;
                this.increaseBtnDown = false;
            };

            this.btnDecreaseSize.MouseUp += (_, _) =>
            {
                this.increaseBtnDown = false;
                this.decreaseBtnDown = false;
            };
            this.btnDecreaseSize.LoadContent();
            this.btnDecreaseSize.Size = 0.50f;

            // Set Multi-Line
            this.btnSetMultiLine = new Button(ContentLoader);
            this.btnSetMultiLine.Text = $"Multi-Line: {this.isMultiLine}";
            this.btnSetMultiLine.FaceTextureName = "button-face-large";

            this.btnSetMultiLine.MouseUp += (_, _) =>
            {
                this.isMultiLine = !this.isMultiLine;
                this.btnSetMultiLine.Text = $"Multi-Line: {this.isMultiLine}";
            };
            this.btnSetMultiLine.LoadContent();
            this.btnSetMultiLine.Size = 0.50f;

            // Set Color
            this.btnSetColor = new Button(ContentLoader);
            this.btnSetColor.Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}";
            this.btnSetColor.FaceTextureName = "button-face-large";

            this.btnSetColor.MouseUp += (_, _) =>
            {
                this.isClrSet = !this.isClrSet;
                this.btnSetColor.Text = $"Set Color: {(this.isClrSet ? "On" : "Off")}";
            };
            this.btnSetColor.LoadContent();
            this.btnSetColor.Size = 0.50f;

            SetupUIControlPositioning();

            base.LoadContent();
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
                this.size += SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            // Decrease Size
            if (this.decreaseBtnDown && this.increaseBtnDown is false)
            {
                this.size -= SizeChangeAmount * (float)frameTime.ElapsedTime.TotalSeconds;
            }

            this.size = this.size < 0f ? 0f : this.size;

            this.btnRotateCW.Update(frameTime);
            this.btnRotateCCW.Update(frameTime);
            this.btnIncreaseSize.Update(frameTime);
            this.btnDecreaseSize.Update(frameTime);
            this.btnSetMultiLine.Update(frameTime);
            this.btnSetColor.Update(frameTime);

            base.Update(frameTime);
        }

        /// <inheritdoc cref="IDrawable.Render"/>
        public override void Render(ISpriteBatch spriteBatch)
        {
            var xPos = (int)(MainWindow.WindowWidth / 2f);
            var yPos = (int)MainWindow.WindowHeight / 2;

            spriteBatch.Render(
                this.font,
                this.isMultiLine ? MultiLineText : SingleLineText,
                xPos,
                yPos,
                this.size,
                this.angle,
                this.isClrSet ? Color.CornflowerBlue : Color.White);

            this.btnRotateCW.Render(spriteBatch);
            this.btnRotateCCW.Render(spriteBatch);
            this.btnIncreaseSize.Render(spriteBatch);
            this.btnDecreaseSize.Render(spriteBatch);
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
        private void UnloadSceneContent() => this.font = null;

        /// <summary>
        /// Sets up the positioning of all the UI controls in the window.
        /// </summary>
        private void SetupUIControlPositioning()
        {
            // Control Positioning
            const int buttonSpacing = 15;
            var leftMargin = 15 + (int)(this.btnRotateCW.Width / 2);

            var buttonTops = (int)(MainWindow.WindowHeight - (new[] { this.btnRotateCW.Height, this.btnRotateCCW.Height }.Max() + 20));

            this.btnRotateCW.Position = new Point(leftMargin, buttonTops);

            this.btnRotateCCW.Position = new Point(
                this.btnRotateCW.Position.X +
                ((int)this.btnRotateCW.Width / 2) +
                ((int)this.btnRotateCCW.Width / 2) +
                buttonSpacing,
                buttonTops);

            this.btnIncreaseSize.Position = new Point(
                this.btnRotateCCW.Position.X +
                ((int)this.btnRotateCCW.Width / 2) +
                ((int)this.btnIncreaseSize.Width / 2) +
                buttonSpacing,
                buttonTops);

            this.btnDecreaseSize.Position = new Point(
                this.btnIncreaseSize.Position.X +
                ((int)this.btnIncreaseSize.Width / 2) +
                ((int)this.btnDecreaseSize.Width / 2) +
                buttonSpacing,
                buttonTops);

            this.btnSetMultiLine.Position = new Point(
                this.btnDecreaseSize.Position.X +
                ((int)this.btnDecreaseSize.Width / 2) +
                ((int)this.btnSetMultiLine.Width / 2) +
                buttonSpacing,
                buttonTops);

            this.btnSetColor.Position = new Point(
                this.btnSetMultiLine.Position.X +
                ((int)this.btnSetMultiLine.Width / 2) +
                ((int)this.btnSetColor.Width / 2) +
                buttonSpacing,
                buttonTops);
        }
    }
}
