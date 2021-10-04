// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Velaptor;
    using Velaptor.Factories;
    using Velaptor.UI;
    using VelaptorTesting.Core;
    using VelaptorTesting.Scenes;

    /// <summary>
    /// The main window to the testing application.
    /// </summary>
    public class MainWindow : Window
    {
        private static readonly char[] UpperCaseChars =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z',
        };
        private readonly SceneManager sceneManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="window">The native window implementation.</param>
        public MainWindow(IWindow window)
            : base(window)
        {
            var contentLoader = ContentLoaderFactory.CreateContentLoader();

            Width = 1500;
            WindowWidth = Width;
            WindowHeight = Height;
            var spriteBatch = SpriteBatchFactory.CreateSpriteBatch(Width, Height);
            spriteBatch.ClearColor = Color.FromArgb(255, 42, 42, 46);

            this.sceneManager = new SceneManager(contentLoader, spriteBatch);

            var testRenderTextScene = new TestRenderTextScene(contentLoader)
            {
                Name = SplitByUpperCase(nameof(TestRenderTextScene)),
            };

            var testMouseScene = new TestMouseScene(contentLoader)
            {
                Name = SplitByUpperCase(nameof(TestMouseScene)),
            };

            var testKeyboardScene = new TestKeyboardScene(contentLoader)
            {
                Name = SplitByUpperCase(nameof(TestKeyboardScene)),
            };

            var renderGraphicsScene = new TestRenderGraphicsScene(contentLoader)
            {
                Name = SplitByUpperCase(nameof(TestRenderGraphicsScene)),
            };

            this.sceneManager.AddScene(testRenderTextScene);
            this.sceneManager.AddScene(testMouseScene);
            this.sceneManager.AddScene(testKeyboardScene);
            this.sceneManager.AddScene(renderGraphicsScene);

            // TODO: Update the static window width and height when the size of the window changes
        }

        /// <summary>
        /// Gets the width fo the window.
        /// </summary>
        public static int WindowWidth { get; private set; }

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        public static int WindowHeight { get; private set; }

        /// <inheritdoc cref="Window.OnLoad"/>
        public override void OnLoad()
        {
            this.sceneManager.LoadContent();
            base.OnLoad();
        }

        /// <inheritdoc cref="Window.OnUpdate"/>
        public override void OnUpdate(FrameTime frameTime)
        {
            this.sceneManager.Update(frameTime);

            Title = $"Scene: {this.sceneManager.CurrentScene?.Name ?? "No Scene Loaded"}";

            base.OnUpdate(frameTime);
        }

        /// <inheritdoc cref="Window.OnDraw"/>
        public override void OnDraw(FrameTime frameTime)
        {
            this.sceneManager.Render();

            base.OnDraw(frameTime);
        }

        /// <inheritdoc cref="Window.OnUnload"/>
        public override void OnUnload()
        {
            this.sceneManager.UnloadContent();
            base.OnUnload();
        }

        /// <inheritdoc cref="Window.OnResize"/>
        public override void OnResize()
        {
            WindowWidth = Width;
            WindowHeight = Height;

            base.OnResize();
        }

        /// <summary>
        /// Splits the given <param name="value"></param> based on uppercase characters.
        /// </summary>
        /// <param name="value">The value to split.</param>
        /// <returns>The value returned as a list of sections.</returns>
        private static string SplitByUpperCase(string value)
        {
            var sections = new List<string>();

            var currentSection = string.Empty;

            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];

                if (UpperCaseChars.Contains(character) && i != 0)
                {
                    sections.Add(currentSection);

                    currentSection = string.Empty;
                    currentSection += character;
                }
                else
                {
                    currentSection += character;
                }
            }

            sections.Add(currentSection);

            var result = sections.Aggregate(string.Empty, (current, section) => current + $"{section} ");

            return result.TrimEnd(' ');
        }
    }
}
