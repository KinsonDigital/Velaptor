// <copyright file="ControlBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using System.Drawing;
    using Velaptor.Input;
    using Velaptor.UI;

    /// <summary>
    /// Used to test the abstract class <see cref="ControlBase"/>.
    /// </summary>
    public class ControlBaseFake : ControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlBaseFake"/> class.
        /// </summary>
        /// <param name="mouseMock">Mouse input mock.</param>
        public ControlBaseFake(IAppInput<MouseState> mouseMock)
            : base(mouseMock)
        {
        }

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
