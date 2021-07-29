using System;
using System.Threading.Tasks;
using Velaptor.Factories;
using Velaptor.UI;

namespace VelaptorTesting
{
    public class Program
    {
        private static IWindow? window;
        private static MyWindow? gameWindow;

        public static void Main()
        {
            window = WindowFactory.CreateWindow(1020, 800);
            gameWindow = new MyWindow(window);

            // Run the game synchronously
            //gameWindow.Show();

            // Run the game asynchronously
            RunGame().Wait();
            gameWindow.Dispose();
        }

        /// <summary>
        /// Runs the game asynchronously.
        /// </summary>
        /// <returns></returns>
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
