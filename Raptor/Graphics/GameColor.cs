namespace Raptor.Graphics
{
    /// <summary>
    /// Represents a color made up of the 4 color components alpha, red, green and blue.
    /// </summary>
    public struct GameColor
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="GameColor"/>.
        /// </summary>
        /// <param name="alpha">The alpa component.</param>
        /// <param name="red">The red component.</param>
        /// <param name="green">The green component.</param>
        /// <param name="blue">The blue component.</param>
        public GameColor(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the alpha component of the <see cref="GameColor"/>.
        /// </summary>
        public byte Alpha { get; set; }

        /// <summary>
        /// Gets or sets the red component of the <see cref="GameColor"/>.
        /// </summary>
        public byte Red { get; set; }

        /// <summary>
        /// Gets or sets the green component of the <see cref="GameColor"/>.
        /// </summary>
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the blue component of the <see cref="GameColor"/>.
        /// </summary>
        public byte Blue { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Returns a result indicating if this <see cref="GameColor"/>
        /// is equal to the given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var clrObj = (GameColor)obj;


            return Alpha == clrObj.Alpha && Red == clrObj.Red && Green == clrObj.Green && Blue == clrObj.Blue;
        }


        /// <summary>
        /// Returns the hash code of this object that makes this object unique.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -712724744;

            hashCode = hashCode * -1521134295 + Red.GetHashCode();
            hashCode = hashCode * -1521134295 + Green.GetHashCode();
            hashCode = hashCode * -1521134295 + Blue.GetHashCode();
            hashCode = hashCode * -1521134295 + Alpha.GetHashCode();


            return hashCode;
        }
        #endregion
    }
}
