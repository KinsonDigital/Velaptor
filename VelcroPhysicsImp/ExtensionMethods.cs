using System.Linq;
using VelcroPhysics.Primitives;

namespace VelcroPhysicsImp
{
    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Private Fields
        /// <summary>
        /// The physics unit to pixel conversion value.
        /// </summary>
        private const float unitToPixel = 100.0f;

        /// <summary>
        /// The pixel to unit conversion value.
        /// </summary>
        private const float pixelToUnit = 1f / unitToPixel;
        #endregion


        #region Public Methods
        /// <summary>
        /// Converts the given physics unit <paramref name="value"/> to pixel units.
        /// </summary>
        /// <param name="value">The physics value to convert.</param>
        /// <returns></returns>
        public static float ToPixels(this float value) => value * unitToPixel;


        /// <summary>
        /// Converts the given pixel units <paramref name="value"/> to physics units.
        /// </summary>
        /// <param name="value">The pixel units to convert.</param>
        /// <returns></returns>
        public static float ToPhysics(this float value) => value * pixelToUnit;

        
        /// <summary>
        /// Converts all of the given physics <paramref name="values"/> to pixel values.
        /// </summary>
        /// <param name="values">The list of physics values to convert.</param>
        /// <returns></returns>
        public static float[] ToPixels(this float[] values) => (from p in values select p.ToPixels()).ToArray();


        /// <summary>
        /// Converts the given pixel unit <see cref="Vector2"/> <paramref name="value"/> to a 
        /// physics unit <see cref="Vector2"/> value.
        /// </summary>
        /// <param name="value">The pixel unit vector to convert.</param>
        /// <returns></returns>
        public static Vector2 ToPhysics(this Vector2 value) => new Vector2(value.X.ToPhysics(), value.Y.ToPhysics());
        #endregion
    }
}
