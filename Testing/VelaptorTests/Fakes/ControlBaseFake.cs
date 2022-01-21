// <copyright file="ControlBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using System.Drawing;
    using Velaptor.UI;

    /// <summary>
    /// Used to test the abstract class <see cref="ControlBase"/>.
    /// </summary>
    public class ControlBaseFake : ControlBase
    {
        /// <summary>
        /// Gets the tint color value of the control base.
        /// </summary>
        public Color TintColorValue => TintColor;

        /// <summary>
        /// Invokes the load content base method for the purpose of testing.
        /// </summary>
        // ReSharper disable once RedundantOverriddenMember
        public override void LoadContent() => base.LoadContent();

        /// <summary>
        /// Invokes the <see cref="ControlBase.ThrowExceptionIfLoadingWhenDisposed"/>() method for the purpose
        /// of invoking an exception for testing.
        /// </summary>
        public void Invoke_Exception_In_Method_ThrowsExceptionIfLoadingWhenDisposed() => ThrowExceptionIfLoadingWhenDisposed();

        /// <summary>
        /// Invokes the <see cref="ControlBase.ThrowExceptionIfLoadingWhenDisposed"/>() method for the purpose
        /// of not invoking an exception for testing.
        /// </summary>
        public void Do_Not_Invoke_Exception_In_Method_ThrowsExceptionIfLoadingWhenDisposed() =>
            ThrowExceptionIfLoadingWhenDisposed();
    }
}
