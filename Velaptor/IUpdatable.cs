// <copyright file="IUpdatable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    /// <summary>
    /// Provides the ability for an object to be updated.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="frameTime">The frame iteration time.</param>
        void Update(FrameTime frameTime);
    }
}
