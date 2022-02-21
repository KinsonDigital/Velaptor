// <copyright file="RectShape.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;

    /// <summary>
    /// Represents a rectangular shape with various attributes.
    /// </summary>
    public struct RectShape
    {
        private float width = 1f;
        private float height = 1f;
        private float borderThickness = 1f;
        private CornerRadius cornerRadius = new (1f, 1f, 1f, 1f);

        /// <summary>
        /// Initializes a new instance of the <see cref="RectShape"/> struct.
        /// </summary>
        public RectShape()
        {
        }

        /// <summary>
        /// Gets or sets the position of the rectangle.
        /// </summary>
        /// <remarks>
        ///     This position is the center of the rectangle.
        /// </remarks>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        /// <remarks>
        ///     If the value is less than 1, it will be set to 1.
        /// </remarks>
        public float Width
        {
            get => this.width;
            set
            {
                value = value < 1f ? 1f : value;

                this.width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        /// <remarks>
        ///     If the value is less than 1, it will be set to 1.
        /// </remarks>
        public float Height
        {
            get => this.height;
            set
            {
                value = value < 1f ? 1f : value;

                this.height = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the rectangle.
        /// </summary>
        /// <remarks>
        ///     Ignored if the <see cref="GradientType"/> is set to any value other than <see cref="ColorGradient.None"/>.
        /// </remarks>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets a value indicating whether or not the rectangle is filled or empty.
        /// </summary>
        public bool IsFilled { get; set; } = true;

        /// <summary>
        /// Gets or sets the thickness of the rectangle's border.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     Ignored if the <see cref="IsFilled"/> property is set to <c>true</c>.
        /// </para>
        ///
        /// <para>
        ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
        /// </para>
        /// </remarks>
        public float BorderThickness
        {
            get => this.borderThickness;
            set
            {
                /*
                 * Always have the smallest value between the width and height (divided by 2)
                 * as the maximum limit of what the border thickness can be.
                 * If the value was allowed to be larger then the smallest value between
                 * the width and height, it would produce strange rendering artifacts.
                 */
                var largestValueAllowed = (Width <= Height ? Width : Height) / 2f;

                value = value > largestValueAllowed ? largestValueAllowed : value;
                value = value < 1f ? 1f : value;

                this.borderThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of each corner of the rectangle.
        /// </summary>
        /// <remarks>
        ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
        /// </remarks>
        public CornerRadius CornerRadius
        {
            get => this.cornerRadius;
            set
            {
                /*
                 * Always have the smallest value between the width and height (divided by 2)
                 * as the maximum limit of what any corner radius can be.
                 * If the value was allowed to be larger then the smallest value between
                 * the width and height, it would produce strange rendering artifacts.
                 */
                var largestValueAllowed = (Width <= Height ? Width : Height) / 2f;

                value = value.TopLeft > largestValueAllowed ? SetTopLeft(value, largestValueAllowed) : value;
                value = value.BottomLeft > largestValueAllowed ? SetBottomLeft(value, largestValueAllowed) : value;
                value = value.BottomRight > largestValueAllowed ? SetBottomRight(value, largestValueAllowed) : value;
                value = value.TopRight > largestValueAllowed ? SetTopRight(value, largestValueAllowed) : value;

                value = value.TopLeft < 0 ? SetTopLeft(value, 0) : value;
                value = value.BottomLeft < 0 ? SetBottomLeft(value, 0) : value;
                value = value.BottomRight < 0 ? SetBottomRight(value, 0) : value;
                value = value.TopRight < 0 ? SetTopRight(value, 0) : value;

                this.cornerRadius = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of color gradient that will be applied to the rectangle.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     A value of <see cref="ColorGradient.None"/> will use the <see cref="Color"/>
        ///     property and render the rectangle with a solid color.
        /// </para>
        ///
        /// <para>
        ///     A value of <see cref="ColorGradient.Horizontal"/> will ignore the <see cref="Color"/>
        ///     property and use the <see cref="GradientStart"/> <see cref="GradientStop"/> properties.
        ///     This will render the rectangle with <see cref="GradientStart"/> color on the left side and gradually
        ///     render it to the right side as the <see cref="GradientStop"/> color.
        /// </para>
        ///
        /// <para>
        ///     A value of <see cref="ColorGradient.Vertical"/> will ignore the <see cref="Color"/>
        ///     property and use the <see cref="GradientStart"/> and <see cref="GradientStop"/> properties.
        ///     This will render the rectangle with <see cref="GradientStart"/> color on the top and gradually
        ///     render it to the bottom as the <see cref="GradientStop"/> color.
        /// </para>
        /// </remarks>
        public ColorGradient GradientType { get; set; } = ColorGradient.None;

        /// <summary>
        /// Gets or sets the starting color of the gradient.
        /// </summary>
        /// <remarks>
        ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
        /// </remarks>
        public Color GradientStart { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the ending color of the gradient.
        /// </summary>
        /// <remarks>
        ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
        /// </remarks>
        public Color GradientStop { get; set; } = Color.White;

        /// <summary>
        /// Sets the <see cref="CornerRadius"/> top left value.
        /// </summary>
        /// <param name="radius">The struct with the value to set.</param>
        /// <param name="topLeft">The new top left value.</param>
        /// <returns>The original <paramref name="radius"/> with its  <see cref="CornerRadius"/> top left value updated.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        public static CornerRadius SetTopLeft(CornerRadius radius, float topLeft) =>
            new (topLeft, radius.BottomLeft, radius.BottomRight, radius.TopRight);

        /// <summary>
        /// Sets the <see cref="CornerRadius"/> bottom left value.
        /// </summary>
        /// <param name="radius">The struct with the value to set.</param>
        /// <param name="bottomLeft">The new bottom left value.</param>
        /// <returns>The original <paramref name="radius"/> with its  <see cref="CornerRadius"/> bottom left value updated.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        public static CornerRadius SetBottomLeft(CornerRadius radius, float bottomLeft) =>
            new (radius.TopLeft, bottomLeft, radius.BottomRight, radius.TopRight);

        /// <summary>
        /// Sets the <see cref="CornerRadius"/> bottom right value.
        /// </summary>
        /// <param name="radius">The struct with the value to set.</param>
        /// <param name="bottomRight">The new bottom right value.</param>
        /// <returns>The original <paramref name="radius"/> with its  <see cref="CornerRadius"/> bottom right value updated.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        public static CornerRadius SetBottomRight(CornerRadius radius, float bottomRight) =>
            new (radius.TopLeft, radius.BottomLeft, bottomRight, radius.TopRight);

        /// <summary>
        /// Sets the <see cref="CornerRadius"/> top right value.
        /// </summary>
        /// <param name="radius">The struct with the value to set.</param>
        /// <param name="topRight">The new top right value.</param>
        /// <returns>The original <paramref name="radius"/> with its  <see cref="CornerRadius"/> top right value updated.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
        public static CornerRadius SetTopRight(CornerRadius radius, float topRight) =>
            new (radius.TopLeft, radius.BottomLeft, radius.BottomRight, topRight);

        /// <summary>
        /// Returns a value indicating whether or not the <see cref="RectShape"/> struct is empty.
        /// </summary>
        /// <returns>True if empty.</returns>
        public bool IsEmpty() =>
            Position == Vector2.Zero &&
            Width <= 1f &&
            Height <= 1f &&
            Color.IsEmpty &&
            IsFilled is false &&
            BorderThickness <= 1f &&
            CornerRadius.IsEmpty() &&
            GradientType == ColorGradient.None &&
            GradientStart.IsEmpty &&
            GradientStop.IsEmpty;

        /// <summary>
        /// Empties the struct.
        /// </summary>
        public void Empty()
        {
            Position = Vector2.Zero;
            Width = 0;
            Height = 0;
            Color = Color.Empty;
            IsFilled = false;
            BorderThickness = 0u;
            CornerRadius = new CornerRadius(0f, 0f, 0f, 0f);
            GradientType = ColorGradient.None;
            GradientStart = Color.Empty;
            GradientStop = Color.Empty;
        }
    }
}
