// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting
{
    using System;
    using System.Threading.Tasks;
    using Velaptor;
    using Velaptor.Factories;
    using Velaptor.UI;

    /// <summary>
    /// The main program entry point.
    /// </summary>
    public static class Program
    {
        private const string ShowDebugConsole = "--show-debug-console";
        private const string True = "true";
        private static IWindow? window;
        private static MainWindow? gameWindow;

        public static void Main(string[] args)
        {
            window = WindowFactory.CreateWindow(1500, 800);
            gameWindow = new MainWindow(window);

            var containsValidArgs = args.Length > 0 && args[0].ToLower().StartsWith(ShowDebugConsole);

            if (containsValidArgs)
            {
                var argSections = args[0].ToLower().Split('=');

                if (argSections.Length >= 2 && argSections[1].ToLower() == True)
                {
                    gameWindow.ShowAsync(() =>
                    {
                        var command = string.Empty;
                        while (command != "exit")
                        {
                            command = Console.ReadLine();

                            switch (command)
                            {
                                case "show-textures":
                                    Console.WriteLine(AppStats.GetFontGlyphRenderingData());
                                    break;
                                case "show-loaded-fonts":
                                    Console.WriteLine(AppStats.GetLoadedFonts());
                                    break;
                            }
                        }
                    }, () =>
                    {
                        gameWindow.Dispose();
                        Environment.Exit(0);
                    });
                }
            }
            else
            {
                // Run the game asynchronously without debug console
                RunGame().Wait();
                gameWindow.Dispose();
            }
        }

        /// <summary>
        /// Runs the game asynchronously.
        /// </summary>
        /// <returns>The asynchronous result of the running game.</returns>
        private static async Task RunGame()
        {
            if (gameWindow is null)
            {
                throw new NullReferenceException($"The '{nameof(gameWindow)}' must not be null.");
            }

            await gameWindow.ShowAsync();
        }
    }
}
