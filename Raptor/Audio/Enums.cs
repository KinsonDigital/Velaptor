namespace Raptor.Audio
{
    public enum AudioFormat
    {
        Mono8 = 1,
        Mono16 = 2,
        Mono32Float = 3,
        Stereo8 = 4,
        Stereo16 = 5,
        StereoFloat32 = 6
    }


    internal enum PlaybackState
    {
        Playing = 1,
        Paused = 2
    }
}
