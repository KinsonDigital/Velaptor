// <copyright file="TestMouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes
{
    using System.Drawing;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Velaptor.Input;
    using Velaptor.UI;
    using VelaptorTesting.Core;

    /// <summary>
    /// Used to test that the mouse works correctly.
    /// </summary>
    public class TestMouseScene : SceneBase
    {
        private readonly IAppInput<MouseState> mouse;
        private Label? mouseInfoLabel;
        private MouseState currentMouseState;
        private MouseScrollDirection scrollDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMouseScene"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for the scene.</param>
        public TestMouseScene(IContentLoader contentLoader)
            : base(contentLoader)
            => this.mouse = AppInputFactory.CreateMouse();

        /// <inheritdoc cref="IScene.LoadContent"/>
        public override void LoadContent()
        {
            if (IsLoaded)
            {
                return;
            }

            this.mouseInfoLabel = new Label() { Color = Color.White };

            this.mouseInfoLabel.LoadContent();
            this.mouseInfoLabel.Position = new Point((int)MainWindow.WindowWidth / 2, (int)MainWindow.WindowHeight / 2);

            AddControl(this.mouseInfoLabel);

            base.LoadContent();
        }

        /// <inheritdoc cref="IUpdatable.Update"/>
        public override void Update(FrameTime frameTime)
        {
            this.currentMouseState = this.mouse.GetState();

            var mouseInfo = $"Mouse Position: {this.currentMouseState.GetX()}, {this.currentMouseState.GetY()}";
            mouseInfo += $"\nMouse Left Button: {(this.currentMouseState.IsLeftButtonDown() ? "Down" : "Up")}";
            mouseInfo += $"\nMouse Right Button: {(this.currentMouseState.IsRightButtonDown() ? "Down" : "Up")}";
            mouseInfo += $"\nMouse Middle Button: {(this.currentMouseState.IsMiddleButtonDown() ? "Down" : "Up")}";

            if (this.currentMouseState.GetScrollWheelValue() != 0)
            {
                this.scrollDirection = this.currentMouseState.GetScrollDirection();
            }

            mouseInfo += $"\nMouse Scroll Direction: {this.scrollDirection}";

            this.mouseInfoLabel.Text = mouseInfo;

            base.Update(frameTime);
        }

        /// <inheritdoc cref="IScene.UnloadContent"/>
        public override void UnloadContent()
        {
            if (!IsLoaded || IsDisposed)
            {
                return;
            }

            base.UnloadContent();
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
                UnloadContent();
            }

            base.Dispose(disposing);
        }
    }
}
