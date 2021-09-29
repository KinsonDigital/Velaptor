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
    }
}
