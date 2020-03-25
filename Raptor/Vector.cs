using System;

namespace Raptor
{
    /// <summary>
    /// Represents a vector in 2D space.
    /// </summary>
    public struct Vector
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Vector"/>.
        /// </summary>
        /// <param name="x">The x position of the vector.</param>
        /// <param name="y">The y position of the vector.</param>
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }
        #endregion


        #region Props
        /// <summary>
        /// The x position of the <see cref="Vector"/>.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The y position of the <see cref="Vector"/>.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets the length of the <see cref="Vector"/>.
        /// </summary>
        public float Length => GetLength(this);

        /// <summary>
        /// Returns a vector with the X and Y components set to 0.
        /// </summary>
        public static Vector Zero => new Vector(0, 0);
        #endregion


        #region Overloaded Operators
        /// <summary>
        /// Subtracts the 2 given vectors.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <returns></returns>
        public static Vector operator -(Vector vectorA, Vector vectorB) => new Vector(vectorA.X - vectorB.X, vectorA.Y - vectorB.Y);


        /// <summary>
        /// Subtracts the given <paramref name="scalar"/> to the X and Y componentes of the given <paramref name="vector"/>.
        /// </summary>
        /// <param name="vector">The vector to add the scalar to.</param>
        /// <param name="scalar">The scalar to add the vector to.</param>
        /// <returns></returns>
        public static Vector operator -(Vector vector, float scalar) => new Vector(vector.X - scalar, vector.Y - scalar);


        /// <summary>
        /// Adds the 2 given vectors.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <returns></returns>
        public static Vector operator +(Vector vectorA, Vector vectorB) => new Vector(vectorA.X + vectorB.X, vectorA.Y + vectorB.Y);


        /// <summary>
        /// Adds the given <paramref name="scalar"/> to the X and Y componentes of the given <paramref name="vector"/>.
        /// </summary>
        /// <param name="vector">The vector to add the scalar to.</param>
        /// <param name="scalar">The scalar to add the vector to.</param>
        /// <returns></returns>
        public static Vector operator +(Vector vector, float scalar) => new Vector(vector.X + scalar, vector.Y + scalar);


        /// <summary>
        /// Multiplies the 2 given vectors, returning the Dot product.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <returns></returns>
        public static Vector operator *(Vector vectorA, Vector vectorB) => new Vector(vectorA.X * vectorB.X, vectorA.Y * vectorB.Y);


        /// <summary>
        /// Multiplies the given vector by the given scalar value.
        /// </summary>
        /// <param name="vector">The vector to multiply by the scalar value.</param>
        /// <param name="scalar">The scalar value to multiply by the vector.</param>
        /// <returns></returns>
        public static Vector operator *(Vector vector, float scalar) => new Vector(vector.X * scalar, vector.Y * scalar);


        /// <summary>
        /// Multiplies the given vector by the given scalar value.
        /// </summary>
        /// <param name="scalar">The scalar value to multiply by the vector.</param>
        /// <param name="vector">The vector to multiply by the scalar value.</param>
        /// <returns></returns>
        public static Vector operator *(float mult, Vector v) => new Vector(v.X * mult, v.Y * mult);


        /// <summary>
        /// Divides the given <see cref="Vector"/> by the given <paramref name="divisor"/>
        /// </summary>
        /// <param name="v">The vector to divide by the divsor value.</param>
        /// <param name="divisor">The divisor to use.</param>
        /// <returns></returns>
        public static Vector operator /(Vector v, float divisor)
        {
            //TODO: Try to convert this to a single liner and see any performance differences.
            float factor = 1 / divisor;
            v.X *= factor;
            v.Y *= factor;


            return v;
        }


        /// <summary>
        /// Returns a value indicating if the 2 given <see cref="vector"/>s have the same component values.
        /// </summary>
        /// <param name="vectorA">The first vector in the comparison.</param>
        /// <param name="vectorB">The second vector in the comparison.</param>
        /// <returns></returns>
        public static bool operator ==(Vector vectorA, Vector vectorB) => vectorA.X == vectorB.X && vectorA.Y == vectorB.Y;


        /// <summary>
        /// Returns a value indicating if the 2 given <see cref="vector"/>s have different component values.
        /// </summary>
        /// <param name="vectorA">The first vector in the comparison.</param>
        /// <param name="vectorB">The second vector in the comparison.</param>
        /// <returns></returns>
        public static bool operator !=(Vector vectorA, Vector vectorB) => !(vectorA == vectorB);
        #endregion


        #region Public Methods
        /// <summary>
        /// Returns a value indicating if this <see cref="Vector"/> is zero.
        /// </summary>
        /// <returns></returns>
        public bool IsZero() => X == 0 && Y == 0;


        /// <summary>
        /// Calculates the cross product of this vector by the given vector.
        /// </summary>
        /// <param name="vector">The vector use in the cross product calculation.</param>
        /// <returns></returns>
        public float Cross(Vector vector) => CalculateCrossProduct(this, vector);


        /// <summary>
        /// Calculates the cross product of the 2 given <see cref="Vector"/>s.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <returns></returns>
        public static float Cross(Vector vectorA, Vector vectorB) => CalculateCrossProduct(vectorA, vectorB);


        /// <summary>
        /// Returns the length of the <see cref="Vector"/>.
        /// </summary>
        /// <returns></returns>
        public static float GetLength(Vector vector) => (float)Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));


        /// <summary>
        /// Checks if the given <paramref name="obj"/> is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns></returns>
        public override bool Equals(object obj) => ((Vector)obj).GetHashCode() == GetHashCode();


        /// <summary>
        /// Converts this <see cref="Vector"/> into its string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"X: {X}, Y: {Y}";


        /// <summary>
        /// Convers this <see cref="Vector"/> into its string representation.
        /// </summary>
        /// <param name="round">The amount to round the components of the <see cref="Vector"/>.</param>
        /// <returns></returns>
        public string ToString(int round) => $"X: {Math.Round(X, round)}, Y: {Math.Round(Y, round)}";


        /// <summary>
        /// Returns the hash code of this object that makes this object unique.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -621239040;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();


            return hashCode;
        }
        #endregion


        #region Static Methods
        /// <summary>
        /// Returns the given <see cref="Vector"/> normalized.  This is the same as a unit vector.
        /// </summary>
        /// <param name="v">The vector to normalize.</param>
        /// <returns></returns>
        public static Vector Normalize(Vector v)
        {
            //TODO: Try to improve performance by creating a large sqrt value table
            //If the value does not exist in the table, just use the Math.sqrt() function
            float val = 1.0f / (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y));


            return new Vector(v.X * val, v.Y * val);
        }


        /// <summary>
        /// Negate's the given <see cref="Vector"/>.
        /// </summary>
        /// <param name="v">The vector to negate.</param>
        /// <returns></returns>
        /// <remarks>Negate means to set the sign of both components to its opposite sign.  Example: x:5,y:-6 = x:-5,y:6</remarks>
        public static Vector Negate(Vector v) => new Vector(v.X *= -1, v.Y *= -1);
        #endregion


        #region Private Methods
        /// <summary>
        /// Calculates the cross product of the 2 given <see cref="Vector"/>s.
        /// </summary>
        /// <param name="vectorA">The first vector to use.</param>
        /// <param name="vectorB">The second vector to use.</param>
        /// <returns></returns>
        private static float CalculateCrossProduct(Vector vectorA, Vector vectorB) => vectorA.X * vectorB.Y - vectorA.Y * vectorB.X;
        #endregion
    }
}
