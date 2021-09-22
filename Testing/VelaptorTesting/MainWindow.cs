using Velaptor;
using Velaptor.UI;

namespace VelaptorTesting
{
    public class MainWindow : Window
    {
        public MainWindow(IWindow window)
            : base(window)
        {
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnUpdate(FrameTime frameTime)
        {
            base.OnUpdate(frameTime);
        }

        public override void OnDraw(FrameTime frameTime)
        {
            base.OnDraw(frameTime);
        }

        public override void OnResize() => base.OnResize();

        public override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
