// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting
{
    using System;
    using System.Threading.Tasks;
    using Velaptor.Factories;
    using Velaptor.UI;

    /// <summary>
    /// The main program entry point.
    /// </summary>
    public static class Program
    {
        private static IWindow? window;
        private static MainWindow? gameWindow;

        public static void Main()
        {
            window = WindowFactory.CreateWindow(1020, 800);
            gameWindow = new MainWindow(window);

            // Run the game synchronously
            //gameWindow.Show();

            // Run the game asynchronously
            RunGame().Wait();
            gameWindow.Dispose();
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
