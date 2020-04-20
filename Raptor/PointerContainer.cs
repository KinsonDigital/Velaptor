using System;

namespace Raptor
{
    /// <summary>
    /// Holds a pointer to make it easier to pass the pointer struct
    /// into methods that only accepts class/reference types.
    /// </summary>
    public class PointerContainer
    {
        #region Private Fields
        private IntPtr _pointer;
        #endregion


        #region Public Methods
        /// <summary>
        /// Packs the given <paramref name="pointerToPack"/> into the container for transport.
        /// </summary>
        /// <param name="pointerToPack">The pointer to pack.</param>
        public void PackPointer(IntPtr pointerToPack) => _pointer = pointerToPack;


        /// <summary>
        /// Unpacks the pointer to be used by another system or process.
        /// </summary>
        /// <returns></returns>
        public IntPtr UnpackPointer() => _pointer;
        #endregion
    }
}
