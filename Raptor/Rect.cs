namespace KDScorpionCore
{
    /// <summary>
    /// Represents a 2D rectangle with position and size.
    /// </summary>
    public struct Rect
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Rect"/>.
        /// </summary>
        /// <param name="x">The X location of the <see cref="Rect"/>.</param>
        /// <param name="y">The Y location of the <see cref="Rect"/>.</param>
        /// <param name="width">The width of the <see cref="Rect"/>.</param>
        /// <param name="height">The height of the <see cref="Rect"/>.</param>
        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the X position of the <see cref="Rect"/>.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the <see cref="Rect"/>.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the position of the <see cref="Rect"/>.
        /// </summary>
        public Vector Position
        {
            get => new Vector(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the width of the <see cref="Rect"/>.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="Rect"/>.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets half of the <see cref="Rect"/>'s <see cref="Width"/>.
        /// </summary>
        public float HalfWidth => Width / 2;

        /// <summary>
        /// Gets half of the <see cref="Rect"/>'s <see cref="Height"/>.
        /// </summary>
        public float HalfHeight => Height / 2;

        /// <summary>
        /// Gets X location of the left side of the <see cref="Rect"/>.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// Gets Y location of the rightside of the <see cref="Rect"/>.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        /// Gets Y location of the top of the <see cref="Rect"/>.
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Gets Y location of the bottom of the <see cref="Rect"/>.
        /// </summary>
        public float Bottom => Y + Height;
        #endregion


        #region Public Methods
        /// <summary>
        /// Gets a value indicating if the <see cref="Rect"/> contains the given <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </summary>
        /// <param name="x">The X component of the location that might be contained.</param>
        /// <param name="y">The Y component of the location that might be contained.</param>
        /// <returns></returns>
        public bool Contains(float x, float y) => x > Left && y > Top && x < Right && y < Bottom;


        /// <summary>
        /// Gets a value indicating if the <see cref="Rect"/> contains the given <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </summary>
        /// <param name="vector">The location that might be contained.</param>
        /// <returns></returns>
        public bool Contains(Vector vector) => vector.X > Left && vector.Y > Top && vector.X < Right && vector.Y < Bottom;
        #endregion
    }
}
