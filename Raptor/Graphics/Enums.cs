// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    /// <summary>
    /// Represents reasons for an invalid animation meta data formats.
    /// </summary>
    public enum InValidReason
    {
        TooManyLeftBrackets = 0,
        TooManyRightBrackets = 1,
        TooManyDashes = 2,
        LeftBracketNotFirstChar = 3,
        RightBracketNotLastChar = 4,
        BracketOrderIncorrect = 5,
        DashLocationIncorrect = 6,
        SubTextureIDNotAlphaNumeric = 7,
        FrameIndexNotWholeNumber = 8,
        FrameIndexNotSequential = 9,
        ValidFormat = 10,
    }
}
