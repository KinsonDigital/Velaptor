// <copyright file="FreeTypeErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;

    public class FreeTypeErrorEventArgs : EventArgs
    {
        public FreeTypeErrorEventArgs(string errorMessage)
        {
            ErrorMessage = ErrorMessage;
        }

        public string ErrorMessage { get; set; } = "A FreeType error has occured";
    }
}
