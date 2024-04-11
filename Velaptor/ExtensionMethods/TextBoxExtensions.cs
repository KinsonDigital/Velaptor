// <copyright file="TextBoxExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UI;

/// <summary>
/// Provides extension methods for the <see cref="TextBox"/> control.
/// </summary>
internal static class TextBoxExtensions
{
    /// <summary>
    /// Moves each character in the given <paramref name="charBounds"/> by the given <paramref name="amount"/> to the left.
    /// </summary>
    /// <param name="charBounds">The list of characters to update.</param>
    /// <param name="amount">The amount to apply.</param>
    public static void BumpAllToLeft(this List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.X -= amount;
            charBounds[i] = currItem;
        }
    }

    /// <summary>
    /// Moves each character in the given <paramref name="charBounds"/> by the given <paramref name="amount"/> to the right.
    /// </summary>
    /// <param name="charBounds">The list of characters to update.</param>
    /// <param name="amount">The amount to apply.</param>
    public static void BumpAllToRight(this List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.X += amount;
            charBounds[i] = currItem;
        }
    }

    /// <summary>
    /// Moves each character in the given <paramref name="charBounds"/> by the given <paramref name="amount"/> in the up direction.
    /// </summary>
    /// <param name="charBounds">The list of characters to update.</param>
    /// <param name="amount">The amount to apply.</param>
    public static void BumpAllUp(this List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.Y -= amount;
            charBounds[i] = currItem;
        }
    }

    /// <summary>
    /// Moves each character in the given <paramref name="charBounds"/> by the given <paramref name="amount"/> in the down direction.
    /// </summary>
    /// <param name="charBounds">The list of characters to update.</param>
    /// <param name="amount">The amount to apply.</param>
    public static void BumpAllDown(this List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.Y += amount;
            charBounds[i] = currItem;
        }
    }

    /// <summary>
    /// Returns the X position of the character that is farthest to the left of all the characters in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <returns>The farthest left X position.</returns>
    /// <remarks>This would be the left side of the farthest left character.</remarks>
    public static int TextLeft(this List<(char character, RectangleF bounds)> charBounds)
    {
        if (charBounds.Count <= 0)
        {
            return 0;
        }

        return (int)charBounds[0].bounds.Left;
    }

    /// <summary>
    /// Returns the X position of the character that is farthest to the right of all the characters in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <returns>The farthest right X position.</returns>
    /// <remarks>This would be the right side of the farthest right character.</remarks>
    public static int TextRight(this List<(char character, RectangleF bounds)> charBounds)
    {
        if (charBounds.Count <= 0)
        {
            return 0;
        }

        return (int)charBounds[^1].bounds.Right;
    }

    /// <summary>
    /// Returns the entire width of all the text in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <returns>The width of the text.</returns>
    public static int TextWidth(this List<(char character, RectangleF bounds)> charBounds) => charBounds.TextRight() - charBounds.TextLeft();

    /// <summary>
    /// Returns the X position of the left side of the character at the given <paramref name="index"/> in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <param name="index">The index of the character.</param>
    /// <returns>The X position of the left side of the character.</returns>
    /// <remarks>Returns 0 if the index is out of range of the characters.</remarks>
    public static float CharLeft(this List<(char character, RectangleF bounds)> charBounds, int index)
    {
        if (index < 0 || index >= charBounds.Count)
        {
            return 0f;
        }

        return charBounds[index].bounds.Left;
    }

    /// <summary>
    /// Returns the X position of the right side of the character at the given <paramref name="index"/> in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <param name="index">The index of the character.</param>
    /// <returns>The X position of the right side of the character.</returns>
    /// <remarks>Returns 0 if the index is out of range of the characters.</remarks>
    public static float CharRight(this List<(char character, RectangleF bounds)> charBounds, int index)
    {
        if (index < 0 || index >= charBounds.Count)
        {
            return 0f;
        }

        return charBounds[index].bounds.Right;
    }

    /// <summary>
    /// Gets the X position of the center of all the characters in the given <paramref name="charBounds"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <returns>The X position of the center.</returns>
    public static float CenterPositionX(this List<(char character, RectangleF bounds)>? charBounds)
    {
        if (charBounds is null || charBounds.Count <= 0)
        {
            return 0f;
        }

        var left = charBounds.Min(cb => cb.bounds.Left);
        var right = charBounds.Max(cb => cb.bounds.Right);
        var width = Math.Abs(left - right);

        return left + width.Half();
    }

    /// <summary>
    /// Returns a value indicating whether there is a gap between the right end of the text in the given <paramref name="charBounds"/>
    /// and the given <paramref name="rightEndLimitX"/>.
    /// </summary>
    /// <param name="charBounds">The list of characters.</param>
    /// <param name="rightEndLimitX">The X position to compare with the right end.</param>
    /// <returns><c>true</c> if their is a gap.</returns>
    public static bool GapAtRightEnd(this List<(char character, RectangleF bounds)>? charBounds, float rightEndLimitX) =>
        charBounds is not null && charBounds.Count > 0 && charBounds.TextRight() < rightEndLimitX;
}
