// <copyright file="IImGuiInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1515
#pragma warning disable SA1512
namespace Velaptor.NativeInterop.ImGui;

using System.Numerics;
using ImGuiNET;

/// <summary>
/// Invokes <see cref="ImGui"/> functions.
/// </summary>
public interface IImGuiInvoker
{
    /// <summary>
    /// Gets the IO object for system and <see cref="ImGui"/> related settings and information.
    /// </summary>
    /// <returns>The config/settings/info object.</returns>
    // ReSharper disable once InconsistentNaming
    ImGuiIOPtr GetIO();

    /// <summary>
    /// Creates a new window.
    /// </summary>
    /// <param name="name">The name of the window which becomes the title.</param>
    /// <param name="flags">Various window options.</param>
    /// <returns>True if the window is currently open.</returns>
    /// <remarks>
    ///     Make sure to invoke the <see cref="End"/> method after creating a window.
    /// </remarks>
    bool Begin(string name, ImGuiWindowFlags flags);

    /// <summary>
    /// Signifies the end of the creation of a window.
    /// </summary>
    void End();

    /// <summary>
    /// Creates a test control.
    /// </summary>
    /// <param name="fmt">The text to display.</param>
    /// <remarks>
    ///     The <paramref name="fmt"/> can take syntax for formatting values such as:
    ///     <list type="bullet">
    ///         <item>
    ///             Numerical Formatting
    ///             <para/>
    ///             You can control how numeric values are displayed, including the number of decimal places,
    ///             whether to use scientific notation, whether to include a thousands separator, and more.
    ///             For example, {0:F2} formats the number with two decimal places.
    ///             <para/>
    ///         </item>
    ///         <item>
    ///             Date and Time Formatting
    ///             <para/>
    ///             You can control how DateTime values are displayed, including the date format, time format, and more.
    ///             For example, {0:yyyy-MM-dd} formats the date as year-month-day.
    ///             <para/>
    ///         </item>
    ///         <item>
    ///             String Padding
    ///             <para/>
    ///             You can control the width of the field in which the string is printed, and whether it’s aligned to
    ///             the left or right. For example, {0,10} prints the string in a field of width 10, right-aligned,
    ///             while {0,-10} prints it left-aligned.
    ///             <para/>
    ///         </item>
    ///         <item>
    ///             Custom Formatting
    ///             <para/>
    ///             You can define your own format strings to control how certain types of objects are displayed.
    ///             <para/>
    ///         </item>
    ///     </list>
    /// <para/>
    /// You can escape a brace by adding another brace.
    /// <para/>
    /// Example: {{ and }} will display as { and }.
    /// </remarks>
    void Text(string fmt);

    /// <summary>
    /// Creates an arrow button.
    /// </summary>
    /// <param name="str_id">The id of the button.</param>
    /// <param name="dir">The direction of the text relative to the button.</param>
    /// <returns>True if the button has been clicked.</returns>
    // ReSharper disable once InconsistentNaming
    bool ArrowButton(string str_id, ImGuiDir dir);

    /// <summary>
    /// Creates a slider scope with a float value.
    /// </summary>
    /// <param name="label">The label of the control.</param>
    /// <param name="v">The value of the control.</param>
    /// <param name="v_min">The minimum value.</param>
    /// <param name="v_max">The maximum value.</param>
    /// <returns>True if the button has been clicked.</returns>
    // ReSharper disable InconsistentNaming
    bool SliderFloat(string label, ref float v, float v_min, float v_max);

    /// <summary>
    /// Creates a button with the given <paramref name="label"/>.
    /// </summary>
    /// <param name="label">The button label.</param>
    void Button(string label);

    /// <summary>
    /// Creates a checkbox with the given <paramref name="label"/>.
    /// </summary>
    /// <param name="label">The checkbox label.</param>
    /// <param name="v">The state of the checkbox.</param>
    /// <returns>True if the checkbox state was changed.</returns>
    bool Checkbox(string label, ref bool v);

    /// <summary>
    /// Creates a combo box control.
    /// </summary>
    /// <param name="label">The label of the combo box.</param>
    /// <param name="preview_value">The preview value of the selected item.</param>
    /// <param name="flags">The combobox options.</param>
    /// <returns>True if an item was chosen.</returns>
    bool BeginCombo(string label, string preview_value, ImGuiComboFlags flags);

    /// <summary>
    /// Signifies the end of the creation of a combo box.
    /// </summary>
    /// <remarks>
    ///     This method should be called after <see cref="BeginCombo"/> if and only if <see cref="BeginCombo"/> returns true.
    /// </remarks>
    void EndCombo();

    /// <summary>
    /// Sets up a button to fire repeatedly when held down.
    /// </summary>
    /// <param name="repeat">True to set the button to repeat.</param>
    void PushButtonRepeat(bool repeat);

    /// <summary>
    /// Stops the button from firing repeatedly when held down.
    /// </summary>
    void PopButtonRepeat();

    /// <summary>
    /// Gets the style object for ImGui.
    /// </summary>
    /// <returns>The style object.</returns>
    ImGuiStylePtr GetStyle();

    /// <summary>
    /// Sets the next control to be on the same line as the previous control.
    /// </summary>
    void SameLine();

    /// <summary>
    /// Sets the next control to be on the same line as the previous control.
    /// </summary>
    /// <param name="offset_from_start_x">The horizontal offset from the start of the previous control.</param>
    /// <param name="spacing">The amount of spacing between the controls.</param>
    // ReSharper disable once InconsistentNaming
    void SameLine(float offset_from_start_x, float spacing);

    /// <summary>
    /// Gets the current position of the cursor.
    /// </summary>
    /// <returns>The cursor position.</returns>
    Vector2 GetCursorPos();

    /// <summary>
    /// Sets the position of the cursor.
    /// </summary>
    /// <param name="local_pos">The position of where to set the cursor.</param>
    // ReSharper disable once InconsistentNaming
    void SetCursorPos(Vector2 local_pos);

    /// <summary>
    /// Creates a new scoped ID for further control operations.
    /// </summary>
    /// <param name="str_id">The id of the scope.</param>
    // ReSharper disable InconsistentNaming
    void PushID(string str_id);
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Ends the current scoped ID.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    void PopID();

    /// <summary>
    /// Gets the current frame height with spacing.
    /// </summary>
    /// <returns>The frame height.</returns>
    float GetFrameHeightWithSpacing();

    /// <summary>
    /// Pushes the color using the given <paramref name="col"/> to the current style described by the given <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The type of area of the style to push the color.</param>
    /// <param name="col">The color to push.</param>
    void PushStyleColor(ImGuiCol idx, Vector4 col);

    /// <summary>
    /// Pops the most recent style color.
    /// </summary>
    void PopStyleColor();

    /// <summary>
    /// Pops the most recent style colors a total number of times by the given <paramref name="count"/>.
    /// </summary>
    /// <param name="count">The total number of recent style color pushes to pop.</param>
    void PopStyleColor(int count);

    /// <summary>
    /// Temporarily modifies the style of <see cref="ImGui"/> by applying a style to a part of
    /// the GUI dictated by the given <paramref name="idx"/> with the given <paramref name="val"/>.
    /// </summary>
    /// <param name="idx">The style to update.</param>
    /// <param name="val">The value to apply.</param>
    /// <remarks>
    ///     Remains in effect until the <see cref="PopStyleVar"/> is invoked.
    /// </remarks>
    void PushStyleVar(ImGuiStyleVar idx, float val);

    /// <summary>
    /// Restores previous updated styles back to their original values.
    /// </summary>
    /// <param name="count">The number of previous applied styles to restore.</param>
    void PopStyleVar(int count);

    /// <summary>
    /// Calculates the size of the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>
    ///     The size of the text described by the X representing the width and the Y representing the height.
    /// </returns>
    Vector2 CalcTextSize(string text);

    /// <summary>
    /// Returns a value indicating whether the mouse is hovering over an item.
    /// </summary>
    /// <returns>True if the mouse is hovering over the item.</returns>
    bool IsItemHovered();

    /// <summary>
    /// Returns a value indicating whether the given mouse <paramref name="button"/> is in the down state.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the mouse is in the down state.</returns>
    bool IsMouseDown(ImGuiMouseButton button);

    /// <summary>
    /// Returns a value indicating whether the given mouse <paramref name="button"/> is in the released state.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the mouse is in the up state.</returns>
    bool IsMouseReleased(ImGuiMouseButton button);

    /// <summary>
    /// Returns a value indicating whether the given item <paramref name="label"/> is selectable.
    /// </summary>
    /// <param name="label">The text to be displayed for the item in the combo box.</param>
    /// <param name="selected">Determines whether the item is selected when the combo box is first displayed.</param>
    /// <returns>True if the item is selected.</returns>
    bool Selectable(string label, bool selected);

    /// <summary>
    /// Sets the default keyboard focus to the item that was previously added.
    /// </summary>
    /// <remarks>This method is typically used in conjunction with combo boxes or list boxes.</remarks>
    void SetItemDefaultFocus();

    /// <summary>
    /// Sets the width of the next item.
    /// </summary>
    /// <param name="item_width">The width.</param>
    // ReSharper disable once InconsistentNaming
    void SetNextItemWidth(float item_width);

    /// <summary>
    /// Sets the position of the window.
    /// </summary>
    /// <param name="pos">The position.</param>
    void SetWindowPos(Vector2 pos);

    /// <summary>
    /// Sets the size of the window.
    /// </summary>
    /// <param name="size">The window size.</param>
    /// <remarks>
    ///     The size of the text described by the X representing the width and the Y representing the height.
    /// </remarks>
    void SetWindowSize(Vector2 size);

    /// <summary>
    /// Gets the position of a window.
    /// </summary>
    /// <returns>The window position.</returns>
    Vector2 GetWindowPos();

    /// <summary>
    /// Gets the size of a window.
    /// </summary>
    /// <returns>The size.</returns>
    /// <remarks>
    ///     The size of the text described by the X representing the width and the Y representing the height.
    /// </remarks>
    Vector2 GetWindowSize();

    /// <summary>
    /// Gets a value indicating whether a window is focused.
    /// </summary>
    /// <returns>True if the window is in focus.</returns>
    bool IsWindowFocused();

    /// <summary>
    /// Gets a value indicating whether a window is being dragged by the given mouse <paramref name="button"/>.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>True if the window is being dragged.</returns>
    bool IsMouseDragging(ImGuiMouseButton button);
}
