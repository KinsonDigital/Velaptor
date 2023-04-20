﻿// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using Velaptor.Graphics;

public static class ExtensionMethods
{
    public static CornerRadius IncreaseTopLeft(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft + amount, radius.BottomLeft, radius.BottomRight, radius.TopRight);
    }

    public static CornerRadius DecreaseTopLeft(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft - amount, radius.BottomLeft, radius.BottomRight, radius.TopRight);
    }

    public static CornerRadius IncreaseBottomLeft(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft + amount, radius.BottomRight, radius.TopRight);
    }

    public static CornerRadius DecreaseBottomLeft(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft - amount, radius.BottomRight, radius.TopRight);
    }

    public static CornerRadius IncreaseBottomRight(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft, radius.BottomRight + amount, radius.TopRight);
    }

    public static CornerRadius DecreaseBottomRight(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft, radius.BottomRight - amount, radius.TopRight);
    }

    public static CornerRadius IncreaseTopRight(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft, radius.BottomRight, radius.TopRight + amount);
    }

    public static CornerRadius DecreaseTopRight(this CornerRadius radius, float amount)
    {
        return new CornerRadius(radius.TopLeft, radius.BottomLeft, radius.BottomRight, radius.TopRight - amount);
    }
}
