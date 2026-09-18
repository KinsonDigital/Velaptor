// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

/// <summary>
/// The main program entry point.
/// </summary>
public static class Program
{
    private static MainWindow? gameWindow;
    public static string DefaultFontBold => "TimesNewRoman-Bold.ttf";

    public static void Main(string[] args)
    {
        gameWindow = new MainWindow();
        gameWindow.Show();
    }
}
