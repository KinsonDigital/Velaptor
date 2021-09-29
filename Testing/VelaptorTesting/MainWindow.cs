// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Linq;

namespace VelaptorTesting
{
    using System.Collections.Generic;
    using System.Drawing;
    using Velaptor;
    using Velaptor.Factories;
    using Velaptor.UI;
    using VelaptorTesting.Core;
    using VelaptorTesting.Scenes;

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
                Name = ProcessName(nameof(TestRenderTextScene)),
            };

            var testMouseScene = new TestMouseScene(contentLoader)
            {
                Name = ProcessName(nameof(TestMouseScene)),
            };

            var testKeyboardScene = new TestKeyboardScene(contentLoader)
            {
                Name = ProcessName(nameof(TestKeyboardScene)),
            };

            var renderGraphicsScene = new TestRenderGraphicsScene(contentLoader)
            {
                Name = ProcessName(nameof(TestRenderGraphicsScene)),
            };

            this.sceneManager.AddScene(testRenderTextScene);
            this.sceneManager.AddScene(testMouseScene);
            this.sceneManager.AddScene(testKeyboardScene);
            this.sceneManager.AddScene(renderGraphicsScene);

            // TODO: Update the static window width and height when the size of the window changes
        }

        public static int WindowWidth { get; private set; }

        public static int WindowHeight { get; private set; }

        public override void OnLoad()
        {
            this.sceneManager.Load();
            base.OnLoad();
        }

        public override void OnUpdate(FrameTime frameTime)
        {
            this.sceneManager.Update(frameTime);

            Title = $"Scene: {this.sceneManager.CurrentScene?.Name ?? "No Scene Loaded"}";

            base.OnUpdate(frameTime);
        }

        public override void OnDraw(FrameTime frameTime)
        {
            this.sceneManager.Render();

            base.OnDraw(frameTime);
        }

        public override void OnResize()
        {
            WindowWidth = Width;
            WindowHeight = Height;

            base.OnResize();
        }

        /// <summary>
        /// Processes a scene name by converting a camel cased name to
        /// space separated words.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <returns>The processed name.</returns>
        /// <remarks>
        ///     If the value was 'MyScene', it would return 'My Scene'.
        /// </remarks>
        protected static string ProcessName(string value)
        {
            var sections = SplitByUpperCase(value);
            var result = sections.Aggregate(string.Empty, (current, section) => current + $"{section} ");

            return result.TrimEnd(' ');
        }

        /// <summary>
        /// Splits the given <param name="value"></param> based on uppercase characters.
        /// </summary>
        /// <param name="value">The value to split.</param>
        /// <returns>The value returned as a list of sections.</returns>
        /// <remarks>
        ///     Example: If the value was 'HelloWorld', the result would be ['Hello', 'World'].
        /// </remarks>
        private static IEnumerable<string> SplitByUpperCase(string value)
        {
            var result = new List<string>();

            var currentSection = string.Empty;

            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];

                if (UpperCaseChars.Contains(character) && i != 0)
                {
                    result.Add(currentSection);

                    currentSection = string.Empty;
                    currentSection += character;
                }
                else
                {
                    currentSection += character;
                }
            }

            result.Add(currentSection);

            return result.ToArray();
        }
    }
}
