namespace Raptor.OpenGL
{
    // TODO: Try and make this a stuct and see if it works as an EventArgs type
    // This means you cannot inherit from EventArgs
    internal class FrameTimeEventArgs
    {
        public FrameTimeEventArgs(double frameTime) => FrameTime = frameTime;

        public double FrameTime { get; }
    }
}
