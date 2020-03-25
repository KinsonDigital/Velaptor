using System;

namespace RaptorCore
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
        /// Packs the given <paramref name="pointer"/> into the container for transport.
        /// </summary>
        /// <param name="pointer">The pointer to pack.</param>
        public void PackPointer(IntPtr pointer) => _pointer = pointer;


        /// <summary>
        /// Unpacks the pointer to be used by another system or process.
        /// </summary>
        /// <returns></returns>
        public IntPtr UnpackPointer() => _pointer;
        #endregion
    }
}
