using System.Drawing;
using System.Numerics;

public static class ExtensionMethods
{
    public static Vector2 ToWorld(this Vector2 value, float width, float height)
    {
        return new Vector2(value.X + (width / 2f), value.Y + (height / 2f));
    }

    public static Vector2 ToLocal(this Vector2 value, float width, float height)
    {
        return new Vector2(value.X - (width / 2f), value.Y - (height / 2f));
    }

    public static Point ToPoint(this Vector2 value)
    {
        return new Point((int)value.X, (int)value.Y);
    }

    public static Vector2 ToVector2(this Point value)
    {
        return new Vector2(value.X, value.Y);
    }
}
