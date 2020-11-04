using Raptor.Factories;

namespace RaptorSandBox
{
    public class Program
    {
        public static void Main()
        {
            var window = WindowFactory.CreateWindow(1020, 800);

            var gameWindow = new MyWindow(window);
            gameWindow.Show();
        }
    }
}
