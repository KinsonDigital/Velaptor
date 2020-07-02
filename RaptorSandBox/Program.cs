using Raptor.Factories;

namespace RaptorSandBox
{
    public class Program
    {
        public static void Main()
        {
            var window = WindowFactory.CreateWindow(1020, 800);
            var contentLoader = ContentLoaderFactory.CreateContentLoader();

            var gameWindow = new MyWindow(window, contentLoader);
            gameWindow.Show();
        }
    }
}
