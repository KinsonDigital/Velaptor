﻿// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System;
using System.Threading.Tasks;
using Velaptor;

/// <summary>
/// The main program entry point.
/// </summary>
public static class Program
{
    private static MainWindow? gameWindow;

    public static async Task Main(string[] args)
    {
        gameWindow = new MainWindow();

        if (HasDebugConsole())
        {
            Console.WriteLine("Velaptor Starting . . .");

            // ReSharper disable once BadParensLineBreaks
            await gameWindow.ShowAsync(() =>
            {
                var command = string.Empty;
                Console.WriteLine("Velaptor Running");
                Console.WriteLine("Run '--help' for more help.");

                while (command != "--exit")
                {
                    command = Console.ReadLine();

                    if (string.IsNullOrEmpty(command))
                    {
                        continue;
                    }

                    if (command.StartsWith("--help") ||
                        command.StartsWith("-h") ||
                        command.StartsWith("-?"))
                    {
                        ShowHelp(command);
                    }
                    else if (command is "--cls" or "--clear")
                    {
                        Console.Clear();
                        Console.WriteLine("Run '--help' for more help.");
                        EmptyLine();
                    }
                    else if (command.StartsWith("--show"))
                    {
                        var showCommandSections = command.Split(' ');

                        if (showCommandSections.Length >= 2)
                        {
                            switch (showCommandSections[1])
                            {
                                case "glyphs":
                                    Console.WriteLine(AppStats.GetFontGlyphRenderingData());
                                    EmptyLine();
                                    break;
                                case "loaded-fonts":
                                    Console.WriteLine(AppStats.GetLoadedFonts());
                                    EmptyLine();
                                    break;
                                case "loaded-textures":
                                    Console.WriteLine(AppStats.GetLoadedTextures());
                                    EmptyLine();
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"The command '{command}' is invalid.  Type '--help' for help.");
                            EmptyLine();
                        }
                    }
                    else
                    {
                        var prevClr = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Run '--help' for more help.");
                        Console.ForegroundColor = prevClr;
                    }
                }
            },
            () =>
            {
                gameWindow.Dispose();
                Environment.Exit(0);
            });
        }
        else
        {
            // Run the game synchronously without debug console
            RunGame().Wait();
            gameWindow.Dispose();
        }
    }

    private static void ShowHelp(string command)
    {
        EmptyLine();

        // If there is additional information after the help command
        if (command.Length > "--help ".Length && command.Contains(' '))
        {
            var sections = command.Split(' ');

            if (sections.Length >= 2)
            {
                sections[1] = sections[1].Trim().ToLower();

                switch (sections[1])
                {
                    case "--show":
                        WriteLine("Description:", ConsoleColor.DarkCyan);
                        WriteLine("\tDisplays information about the application.");
                        WriteLine("Usage:");
                        Write("\t--show glyphs", ConsoleColor.DarkYellow);
                        WriteLine("\t\tDisplays a list of all the glyphs being rendered on the most current frame.");
                        Write("\t--show loaded-fonts", ConsoleColor.DarkYellow);
                        WriteLine("\tDisplays a list of all the currently loaded fonts.");
                        Write("\t--show loaded-textures", ConsoleColor.DarkYellow);
                        WriteLine("\tDisplays a list of the loaded textures.", true);
                        break;
                }

                return;
            }
        }

        WriteLine("Available Commands:", ConsoleColor.DarkCyan);
        Write("\t--help | -h | -?", ConsoleColor.DarkYellow);
        WriteLine("\tDisplays help.");
        Write("\t--cls | --clear", ConsoleColor.DarkYellow);
        WriteLine("\t\tClears the screen.");
        Write("\t--show\t\t\t", ConsoleColor.DarkYellow);
        WriteLine("Displays data about the application", true);
        WriteLine("Run '--help' [command] for more information on a command.", true);
    }

    /// <summary>
    /// Runs the game asynchronously.
    /// </summary>
    /// <returns>The asynchronous result of the running game.</returns>
    private static async Task RunGame()
    {
        if (gameWindow is null)
        {
            throw new NullReferenceException($"The '{nameof(gameWindow)}' must not be null.");
        }

        await gameWindow.ShowAsync();
    }

    /// <summary>
    /// Prints the given <paramref name="value"/> to the console using the given <paramref name="color"/>.
    /// </summary>
    /// <param name="value">The value to print.</param>
    /// <param name="color">The color of the printed value.</param>
    private static void Write(string value, ConsoleColor color)
    {
        var prevClr = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write(value);
        Console.ForegroundColor = prevClr;
    }

    /// <summary>
    /// Prints the given <paramref name="value"/> to the console with a new line after.
    /// </summary>
    /// <param name="value">The value to print.</param>
    /// <param name="emptyLineAfter">True to add an empty line after the value has been printed.</param>
    private static void WriteLine(string value, bool emptyLineAfter = false)
    {
        Console.WriteLine(value);

        if (emptyLineAfter)
        {
            EmptyLine();
        }
    }

    /// <summary>
    /// Prints the given <paramref name="value"/> to the console using the given <paramref name="color"/> and adds a new line below.
    /// </summary>
    /// <param name="value">The value to print.</param>
    /// <param name="color">The color of the printed value.</param>
    /// <param name="emptyLineAfter">Adds an empty line after the value has been printed.</param>
    private static void WriteLine(string value, ConsoleColor color, bool emptyLineAfter = false)
    {
        var prevClr = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.WriteLine(value);
        Console.ForegroundColor = prevClr;

        if (emptyLineAfter)
        {
            EmptyLine();
        }
    }

    /// <summary>
    /// Prints an empty line to the console.
    /// </summary>
    private static void EmptyLine() => Console.WriteLine();

    /// <summary>
    /// Returns a value indicating if the testing application uses a debug console
    /// to assist with debugging.
    /// </summary>
    /// <returns>True if using a debug console.</returns>
    private static bool HasDebugConsole()
    {
#if DEBUG_CONSOLE || RELEASE_CONSOLE
        return true;
#else
            return false;
#endif
    }
}
