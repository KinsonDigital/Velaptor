// <copyright file="PointerContainer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;

    /// <summary>
    /// Holds a pointer to make it easier to pass the pointer struct
    /// into methods that only accepts class/reference types.
    /// </summary>
    public class PointerContainer
    {
        private IntPtr pointer;

        /// <summary>
        /// Packs the given <paramref name="pointerToPack"/> into the container for transport.
        /// </summary>
        /// <param name="pointerToPack">The pointer to pack.</param>
        public void PackPointer(IntPtr pointerToPack) => this.pointer = pointerToPack;

        /// <summary>
        /// Unpacks the pointer to be used by another system or process.
        /// </summary>
        /// <returns></returns>
        public IntPtr UnpackPointer() => this.pointer;
    }
}
