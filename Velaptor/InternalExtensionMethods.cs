// <copyright file="InternalExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Graphics;
using Input;
using OpenGL.Batching;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectF = System.Drawing.RectangleF;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Provides extensions to various things to help make better code.
/// </summary>
internal static class InternalExtensionMethods
{
    private const char WinDirSeparatorChar = '\\';
    private const char CrossPlatDirSeparatorChar = '/';

    /// <summary>
    /// Returns a value indicating whether or not any of the arrow keys are in the down state.
    /// </summary>
    /// <param name="keyboardState">The state of the keyboard.</param>
    /// <returns><c>true</c> if any arrow key is pressed down.</returns>
    public static bool AnyArrowKeysDown(this KeyboardState keyboardState) =>
        keyboardState.IsKeyDown(KeyCode.Left) ||
        keyboardState.IsKeyDown(KeyCode.Right) ||
        keyboardState.IsKeyDown(KeyCode.Up) ||
        keyboardState.IsKeyDown(KeyCode.Down);



    public static void BumpAllToLeft(this List<(char character, NETRectF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.X -= amount;
            charBounds[i] = currItem;
        }
    }

    public static void BumpAllToRight(this List<(char character, NETRectF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.X += amount;
            charBounds[i] = currItem;
        }
    }

    public static void BumpAllUp(this List<(char character, NETRectF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.Y -= amount;
            charBounds[i] = currItem;
        }
    }

    public static void BumpAllDown(this List<(char character, NETRectF bounds)> charBounds, float amount)
    {
        for (var i = 0; i < charBounds.Count; i++)
        {
            var currItem = charBounds[i];
            currItem.bounds.Y += amount;
            charBounds[i] = currItem;
        }
    }

    public static int TextLeft(this List<(char character, NETRectF bounds)> charBounds)
    {
        if (charBounds.Count <= 0)
        {
            return 0;
        }

        return (int)charBounds[0].bounds.Left;
    }

    public static int TextRight(this List<(char character, NETRectF bounds)> charBounds)
    {
        if (charBounds.Count <= 0)
        {
            return 0;
        }

        return (int)charBounds[^1].bounds.Right;
    }

    public static int TextWidth(this List<(char character, NETRectF bounds)> charBounds) => charBounds.TextRight() - charBounds.TextLeft();

    public static float CharLeft(this List<(char character, NETRectF bounds)> charBounds, int index)
    {
        if (index < 0 || index >= charBounds.Count)
        {
            return 0f;
        }

        return charBounds[index].bounds.Left;
    }

    public static float CharRight(this List<(char character, NETRectF bounds)> charBounds, int index)
    {
        if (index < 0 || index >= charBounds.Count)
        {
            return 0f;
        }

        return charBounds[index].bounds.Right;
    }

    public static float CenterPositionX(this List<(char character, NETRectF bounds)>? charBounds)
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

    public static bool GapAtRightEnd(this List<(char character, NETRectF bounds)>? charBounds, float rightEndLimitX) =>
        charBounds is not null && charBounds.Count > 0 && charBounds.TextRight() < rightEndLimitX;

  public static float Half(this float value) => value / 2;

    public static int Half(this int value) => value / 2;

    /// <summary>
    /// Builds a name that represents a location of where an execution took place.
    /// </summary>
    /// <param name="unused">The object to enable this extension method to be executed anywhere.</param>
    /// <param name="postFixValue">The value to add to the end of the name.</param>
    /// <param name="memberName">The name of the member invoked this method.</param>
    /// <returns>The formatted member name of where this was invoked.</returns>
    /// <exception cref="Exception">Occurs if the stack frame is null.</exception>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Helper method.")]
    [ExcludeFromCodeCoverage]
    public static string GetExecutionMemberName(
        this object unused,
        string postFixValue = "",
        [CallerMemberName] string memberName = "")
    {
        string callerLocation;
        Type? declaringType;
        var skipFrames = 2;

        do
        {
            var method = new StackFrame(skipFrames, false).GetMethod();

            if (method is null)
            {
                throw new Exception("There was an issue getting the method for stack frame 2.");
            }

            declaringType = method.DeclaringType;

            if (declaringType is null)
            {
                return method.Name;
            }

            skipFrames++;
            callerLocation = declaringType.FullName ?? string.Empty;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        var containsPostFixValue = string.IsNullOrEmpty(postFixValue);

        callerLocation = callerLocation.Contains(".")
            ? callerLocation.Split('.')[^1]
            : callerLocation;

        memberName = memberName == ".ctor"
            ? "Ctor"
            : memberName;

        return $"{callerLocation}.{memberName}{(containsPostFixValue ? string.Empty : " - ")}{postFixValue}";
    }

    /// <summary>
    /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
    /// inherit from <see cref="IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type to suppress against.</typeparam>
    /// <param name="container">The container that the suppression applies to.</param>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public static void SuppressDisposableTransientWarning<T>(this Container container)
    {
        var registration = container.GetRegistration(typeof(T))?.Registration;
        registration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");
    }

    /// <summary>
    ///    Conditionally registers that a new instance of <typeparamref name="TImplementation"/> will be returned
    ///    every time a <typeparamref name="TService"/> is requested (transient) and where the supplied predicate
    ///    returns true. The predicate will only be evaluated a finite number of times;
    ///    the predicate is unsuited for making decisions based on runtime conditions.
    /// </summary>
    /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
    /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="predicate">
    ///     The predicate that determines whether or not the <typeparamref name="TImplementation"/> can be applied for
    ///     the requested service type. This predicate can be used to build a fallback mechanism
    ///     where multiple registrations for the same service type are made. Note that the
    ///     predicate will be called a finite number of times and its result will be cached
    ///     for the lifetime of the container. It can't be used for selecting a type based
    ///     on runtime conditions.
    /// </param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <remarks>
    ///     This method uses the container's LifestyleSelectionBehavior to select the exact
    ///     lifestyle for the specified type. By default this will be transient.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void RegisterConditional<TService, TImplementation>(this Container container, Predicate<PredicateContext> predicate, bool suppressDisposal = false)
        where TService : class
        where TImplementation : class, TService
    {
        container.RegisterConditional<TService, TImplementation>(predicate);

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TService>(container);
        }
    }

    /// <summary>
    /// Registers that a new instance of <typeparamref name="TImplementation"/> will be returned every time
    /// a <typeparamref name="TService"/> is requested (transient).
    /// </summary>
    /// <typeparam name="TService">The interface or base type that can be used to retrieve the instances.</typeparam>
    /// <typeparam name="TImplementation">The concrete type that will be registered.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <remarks>
    ///     This method uses the container's LifestyleSelectionBehavior to select the exact
    ///     lifestyle for the specified type. By default this will be Transient.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference.</exception>
    /// <exception cref="InvalidOperationException">Thrown when this container instance is locked and cannot be altered.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void Register<TService, TImplementation>(this Container container, bool suppressDisposal = false)
        where TService : class
        where TImplementation : class, TService
    {
        container.Register<TService, TImplementation>();

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TService>(container);
        }
    }

    /// <summary>
    /// Registers the specified delegate that allows returning transient instances of
    /// <typeparamref name="TService" />. The delegate is expected to always return a new instance on
    /// each call.
    /// </summary>
    /// <remarks>
    /// This method uses the container's
    /// <see cref="P:SimpleInjector.ContainerOptions.LifestyleSelectionBehavior">LifestyleSelectionBehavior</see> to select
    /// the exact lifestyle for the specified type. By default this will be
    /// <see cref="F:SimpleInjector.Lifestyle.Transient">Transient</see>.
    /// </remarks>
    /// <typeparam name="TService">The interface or base type that can be used to retrieve instances.</typeparam>
    /// <param name="container">The container that the registration applies to.</param>
    /// <param name="instanceCreator">The delegate that allows building or creating new instances.</param>
    /// <param name="suppressDisposal"><c>true</c> to ignore dispose warnings if the original code invokes dispose.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown when this container instance is locked and cannot be altered, or when the
    /// <typeparamref name="TService" /> has already been registered.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown when <paramref name="instanceCreator" /> is a null reference.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static void Register<TService>(this Container container, Func<TService> instanceCreator, bool suppressDisposal = false)
        where TService : class
    {
        container.Register(instanceCreator);

        if (suppressDisposal)
        {
            SuppressDisposableTransientWarning<TService>(container);
        }
    }

    /// <summary>
    /// Converts the given <paramref name="image"/> of type <see cref="ImageData"/>
    /// to the type of <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="image">The image data to convert.</param>
    /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
    public static Image<Rgba32> ToSixLaborImage(this ImageData image)
    {
        var result = new Image<Rgba32>((int)image.Width, (int)image.Height);

        for (var y = 0; y < result.Height; y++)
        {
            var row = y;
            result.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < result.Width; x++)
                {
                    var pixel = image.Pixels[x, row];

                    pixelRowSpan[x] = new Rgba32(
                        pixel.R,
                        pixel.G,
                        pixel.B,
                        pixel.A);
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Converts the given <paramref name="image"/> of type <see cref="Image{Rgba32}"/>
    /// to the type of <see cref="ImageData"/>.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>The image data of type <see cref="ImageData"/>.</returns>
    public static ImageData ToImageData(this Image<Rgba32> image)
    {
        var pixelData = new NETColor[image.Width, image.Height];

        for (var y = 0; y < image.Height; y++)
        {
            var row = y;
            image.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < image.Width; x++)
                {
                    pixelData[x, row] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            });
        }

        return new ImageData(pixelData, (uint)image.Width, (uint)image.Height);
    }

    /// <summary>
    /// Returns the normalized versions of the given <paramref name="paths"/>.
    /// </summary>
    /// <param name="paths">The list of paths to normalize.</param>
    /// <returns>Normalized paths.</returns>
    /// <remarks>
    ///     A normalized path is a path that has all of its directory separators all the same to the value of <c>'/'</c>.
    /// </remarks>
    public static IReadOnlyCollection<string> NormalizePaths(this IEnumerable<string> paths) =>
        paths.Select(p => p.Contains(WinDirSeparatorChar)
                ? p.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar)
                : p).ToArray().AsReadOnly();

    /// <summary>
    /// Scales the length of the given <paramref name="line"/> by the given <paramref name="scale"/> amount.
    /// </summary>
    /// <param name="line">The line to to scale.</param>
    /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
    /// <returns>The scaled line.</returns>
    public static LineBatchItem Scale(this LineBatchItem line, float scale)
    {
        var translatedVector = line.P2 - line.P1;

        var scaledVector = new Vector2(translatedVector.X * scale, translatedVector.Y * scale);

        // Translate the vector back to its original position
        line = line.SetP2(scaledVector + line.P1);

        return line;
    }

    /// <summary>
    /// Flips the end of the <paramref name="line"/> 180 degrees from its current position.
    /// </summary>
    /// <param name="line">The line to flip.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the end flipped.</returns>
    public static LineBatchItem FlipEnd(this LineBatchItem line)
    {
        var translatedStop = line.P2 - line.P1;

        translatedStop *= -1;

        // Translates the end of the line back
        translatedStop += line.P1;

        var result = line.SetP2(translatedStop);

        return result;
    }

    /// <summary>
    /// Clamps all the radius values between the given <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to clamp.</param>
    /// <param name="min">The clamp minimum.</param>
    /// <param name="max">The clamp maximum.</param>
    /// <returns>The result after clamping has been applied.</returns>
    public static CornerRadius Clamp(this CornerRadius cornerRadius, float min, float max)
    {
        var topLeft = Math.Clamp(cornerRadius.TopLeft, min, max);
        var bottomLeft = Math.Clamp(cornerRadius.BottomLeft, min, max);
        var bottomRight = Math.Clamp(cornerRadius.BottomRight, min, max);
        var topRight = Math.Clamp(cornerRadius.TopRight, min, max);

        return new CornerRadius(topLeft, bottomLeft, bottomRight, topRight);
    }

    /// <summary>
    /// Calculates the length of the line.
    /// </summary>
    /// <param name="line">The line to calculate the length from.</param>
    /// <returns>The length of the line.</returns>
    public static float Length(this LineBatchItem line)
        => (float)Math.Sqrt(Math.Pow(line.P2.X - line.P1.X, 2) + Math.Pow(line.P2.Y - line.P1.Y, 2));

    /// <summary>
    /// Creates a rectangle from the given line.  This rectangle is takes the thickness of the line into account with
    /// its length to construct the rectangle.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    ///     The four corners of the rectangle as vectors.
    /// </returns>
    public static IEnumerable<Vector2> CreateRectFromLine(this LineBatchItem line)
    {
        var halfThickness = line.Thickness / 2f;

        // Green
        var lineA = line;
        lineA = lineA.SetP2(lineA.P2.RotateAround(lineA.P1, 90));

        var scale = halfThickness / lineA.Length();
        lineA = lineA.Scale(scale);

        // Blue
        var lineB = lineA;
        lineB = lineB.FlipEnd();

        // Yellow
        var lineC = line;

        lineC = lineC.SetP1(lineC.P1.RotateAround(lineC.P2, 90));

        lineC = lineC.SwapEnds();
        lineC = lineC.Scale(scale);

        // Red
        var lineD = lineC;
        lineD = lineD.FlipEnd();

        var rectPoints = new[]
        {
            lineB.P2,
            lineC.P2,
            lineD.P2,
            lineA.P2,
        };

        return rectPoints;
    }

    /// <summary>
    /// Sets the <see cref="LineBatchItem.P1"/> vector component of the batch item to the given <paramref name="p1"/> vector.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <param name="p1">The line end vector component.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the new line end vector component.</returns>
    public static LineBatchItem SetP1(this LineBatchItem item, Vector2 p1) =>
        new (p1,
            item.P2,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Sets the <see cref="LineBatchItem.P2"/> vector component of the batch item to the given <paramref name="p2"/> vector.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <param name="p2">The line end vector component.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the new line end vector component.</returns>
    public static LineBatchItem SetP2(this LineBatchItem item, Vector2 p2) =>
        new (item.P1,
            p2,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Swaps the <see cref="LineBatchItem"/>.<see cref="LineBatchItem.P1"/> and <see cref="LineBatchItem"/>.<see cref="LineBatchItem.P2"/>
    /// components of the batch item.
    /// </summary>
    /// <param name="item">The batch item.</param>
    /// <returns>The <see cref="LineBatchItem"/> with the components swapped.</returns>
    public static LineBatchItem SwapEnds(this LineBatchItem item) =>
        new (item.P2,
            item.P1,
            item.Color,
            item.Thickness);

    /// <summary>
    /// Converts the given <paramref name="value"/> from the type <see cref="NETPoint"/> to the type <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Vector2"/> result.</returns>
    public static Vector2 ToVector2(this NETPoint value) => new (value.X, value.Y);

    /// <summary>
    /// Converts the given <paramref name="value"/> from the type <see cref="Vector2"/> to the type <see cref="NETPoint"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="NETPoint"/> result.</returns>
    /// <remarks>
    ///     Converting from floating point components of a <see cref="Vector2"/> to
    ///     integer components of a <see cref="NETPoint"/> could result in a loss of information.
    ///     Regular casting rules apply.
    /// </remarks>
    public static NETPoint ToPoint(this Vector2 value) => new ((int)value.X, (int)value.Y);

    public static bool IsEmpty<T>(this IEnumerable<T>? items) => items is null || !items.Any();

    /// <summary>
    /// Dequeues the given <paramref name="queue"/> of items until the <paramref name="untilPredicate"/> returns true.
    /// </summary>
    /// <param name="queue">The <see cref="Queue{T}"/> to process.</param>
    /// <param name="untilPredicate">Stops dequeuing the items if the <see cref="Predicate{T}"/> returns true.</param>
    /// <typeparam name="T">The type of items in the <paramref name="queue"/>.</typeparam>
    public static void DequeueWhile<T>(this Queue<T> queue, Predicate<T> untilPredicate)
    {
        var maxIterations = queue.Count + 1;
        var currentIteration = 0;

        while (currentIteration < maxIterations)
        {
            if (queue.Count <= 0)
            {
                break;
            }

            var peekedItem = queue.Peek();

            if (untilPredicate(peekedItem))
            {
                queue.Dequeue();
            }

            currentIteration += 1;
        }
    }

    /// <summary>
    /// Gets the index of the first item when the given <paramref name="predicate"/> returns true.
    /// </summary>
    /// <param name="items">The items to search.</param>
    /// <param name="predicate">The predicate to execute for each item.</param>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <returns>
    /// The positive index location of the item or <c>-1</c> if the item is not found.
    /// </returns>
    public static int IndexOf<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
        var index = -1;

        foreach (T item in items)
        {
            index++;

            if (predicate(item))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first item that returns <c>true</c> with the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="predicate">The predicate to execute against each item.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>s.</typeparam>
    /// <returns>The index of the item.</returns>
    public static int IndexOf<T>(this Memory<RenderItem<T>> items, Predicate<T> predicate)
    {
        for (var i = 0; i < items.Span.Length; i++)
        {
            if (predicate(items.Span[i].Item))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first occurence of an item that matches the given <paramref name="predicate"/> result.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="predicate">Indicates whether or not if the item index should be returned.</param>
    /// <typeparam name="T">The type of item in the list.</typeparam>
    /// <returns>The index of the item that the <paramref name="predicate"/> returned <c>true</c>.</returns>
    /// <remarks>
    ///     The iterating over the items will stop once the <paramref name="predicate"/> returns true.
    /// </remarks>
    public static int FirstItemIndex<T>(this Memory<T> items, Predicate<T> predicate)
    {
        var index = -1;

        foreach (var item in items.Span)
        {
            index++;

            if (predicate(item))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the first item that matches the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="layer">The layer to check.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>.</typeparam>
    /// <returns>
    ///     The index of the item in the list of <paramref name="items"/>.
    ///     <br/>
    ///     The value of -1 will be returned if the layer cannot be found.
    /// </returns>
    public static int FirstLayerIndex<T>(this Memory<RenderItem<T>> items, int layer)
    {
        for (var i = 0; i < items.Span.Length; i++)
        {
            if (items.Span[i].Layer == layer)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Gets the total number of items that are on the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="items">The items to check.</param>
    /// <param name="layer">The layer to check.</param>
    /// <typeparam name="T">The type of <see cref="RenderItem{T}"/>s.</typeparam>
    /// <returns>
    ///     The total number of items on the <paramref name="layer"/>.
    /// </returns>
    /// <remarks>
    ///     This method assumes that the items are sorted by layer in ascending order to work properly.
    /// </remarks>
    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Left for performance reasons.")]
    public static int TotalOnLayer<T>(this Memory<RenderItem<T>> items, int layer)
    {
        var result = 0;

        for (var i = 0; i < items.Span.Length; i++)
        {
            // If there is no point in checking the rest of the items
            if (items.Span[i].Layer > layer)
            {
                return result;
            }

            if (items.Span[i].Layer == layer)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Increases the total amount of the given <paramref name="items"/> by the given <paramref name="amount"/>.
    /// </summary>
    /// <param name="items">The items to increase its total by the given <paramref name="amount"/>.</param>
    /// <param name="amount">The amount to add to the given <paramref name="items"/>.</param>
    /// <typeparam name="T">The arbitrary data referenced by the <paramref name="items"/> of type <see cref="Memory{T}"/>.</typeparam>
    public static void IncreaseBy<T>(ref this Memory<T> items, uint amount)
    {
        var dataBackup = new Span<T>(new T[items.Length]);

        // Backup the data to not lose it
        items.Span.CopyTo(dataBackup);
        items = new T[items.Length + amount];

        // Copy the backup back to where it came from
        dataBackup.CopyTo(items.Span);
    }

    /// <summary>
    /// Converts the given <paramref name="rect"/> to a <see cref="ShapeBatchItem"/>.
    /// </summary>
    /// <param name="rect">The circle shape to convert.</param>
    /// <returns>The batch item.</returns>
    public static ShapeBatchItem ToBatchItem(this RectShape rect) =>
        new (rect.Position,
            rect.Width,
            rect.Height,
            rect.Color,
            rect.IsSolid,
            rect.BorderThickness,
            rect.CornerRadius,
            rect.GradientType,
            rect.GradientStart,
            rect.GradientStop);

    /// <summary>
    /// Converts the given <paramref name="circle"/> to a <see cref="ShapeBatchItem"/>.
    /// </summary>
    /// <param name="circle">The circle shape to convert.</param>
    /// <returns>The batch item.</returns>
    public static ShapeBatchItem ToBatchItem(this CircleShape circle) =>
        new (circle.Position,
            circle.Diameter,
            circle.Diameter,
            circle.Color,
            circle.IsSolid,
            circle.BorderThickness,
            new CornerRadius(circle.Diameter / 2f),
            circle.GradientType,
            circle.GradientStart,
            circle.GradientStop);

    /// <summary>
    /// Returns the inverse of the color.
    /// </summary>
    /// <param name="value">The color value to invert.</param>
    /// <returns>The invert color of the original color.</returns>
    public static NETColor Inverse(this NETColor value) => NETColor.FromArgb(255, 255 - value.R, 255 - value.G, 255 - value.B);
}
