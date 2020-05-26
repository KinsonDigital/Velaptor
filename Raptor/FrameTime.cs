using System;
using System.Diagnostics.CodeAnalysis;

namespace Raptor
{
    /// <summary>
    /// Holds timing information for a game loop.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public struct FrameTime : IEquatable<FrameTime>
    {
        #region Props
        /// <summary>
        /// The total time that has passed.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// The total time that has passed for the current frame.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }
        #endregion


        #region Public Methods
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is FrameTime frameTime))
                return false;


            return Equals(frameTime);
        }


        public bool Equals(FrameTime other)
            => other.TotalTime == TotalTime && other.ElapsedTime == ElapsedTime;


        public override int GetHashCode() => ElapsedTime.GetHashCode();


        public static bool operator ==(FrameTime left, FrameTime right) => left.Equals(right);


        public static bool operator !=(FrameTime left, FrameTime right) => !(left == right);
        #endregion
    }
}
