// <copyright file="GameControllerState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1642 // This warning shows it should be described as a class but it is a struct
namespace Velaptor.Input;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents the state of a game controller device.
/// </summary>
public record struct GameControllerState
{
    private static readonly int Capacity = Enum.GetNames<ControllerButton>().Length;

    /// <summary>
    /// Gets the state of the buttons.
    /// </summary>
    internal Dictionary<ControllerButton, bool> ButtonStates { get; private set; } = new (Capacity);

    /// <summary>
    /// Gets the state of the left thumbstick.
    /// </summary>
    internal GameCtrlThumbstickState LeftGameCtrlThumbstick = default;

    /// <summary>
    /// Gets the state of the right thumbstick.
    /// </summary>
    internal GameCtrlThumbstickState RightGameCtrlThumbstick = default;

    /// <summary>
    /// Gets the state of the left trigger.
    /// </summary>
    internal GameCtrlTriggerState LeftGameCtrlTrigger = default;

    /// <summary>
    /// Gets the state of the right trigger.
    /// </summary>
    internal GameCtrlTriggerState RightGameCtrlTrigger = default;

    // TODO: ADD doCS
    public float HeavyRumbleIntensity { get; internal set; }

    public float LightRumbleIntensity { get; internal set; }

    public bool Enabled { get; internal set; }

    public bool IsConnected { get; internal set; }

    // TODO: Look into setting the ID (GUID) into a 'Guid' type.  For the rest of the references in GameController and GameControllerRumble types as well
    // Probably it is best to convert the string to an actual 'Guid' type in the sdl invoker.  This is where it comes from as a source.

    /// <summary>
    /// The game controller device ID.
    /// </summary>
    /// <remarks>Returns -1 if the device state has no ID.</remarks>
    public int Id { get; internal set; }

    /// <summary>
    /// Gets or sets the SDL game-controller GUID string that identifies a physical
    /// controller model. SDL uses this value as the lookup key to apply a controller's
    /// button/axis mapping from the embedded game-controller-db resource.
    /// </summary>
    /// <value>
    /// A 32-character lowercase hexadecimal string (for example
    /// <c>03000000c82d00000150000000000000</c>), or the special value
    /// <c>xinput</c>. Take values verbatim from the first comma-separated field of a
    /// record in the <c>Input/game-controller-db.txt</c> resource.
    /// </value>
    /// <remarks>
    /// <para>
    /// <b>This is not a UUID.</b> It only resembles one: 32 hex characters, the same
    /// length as a UUID in compact form, and <see cref="System.Guid.Parse(string)"/>
    /// will even accept it. It is actually SDL's packed device identifier, it carries
    /// no UUID version or variant bits, and <see cref="System.Guid"/> reorders bytes
    /// internally. Treat the value as an opaque string: copy it verbatim and compare
    /// with <see cref="string.Equals(string, string, System.StringComparison)"/> using
    /// <see cref="System.StringComparison.Ordinal"/>.
    /// </para>
    /// <para>
    /// <b>What the GUID is for.</b> When a controller is connected, SDL builds the
    /// same 32-character string from the device's bus type and hardware IDs and then
    /// ordinally matches it against the first field of every registered mapping
    /// record. On a match, that record's <c>a:</c>/<c>b:</c>/<c>leftx:</c>…
    /// assignments define how the device's buttons and axes surface in SDL's standard
    /// game-controller layout. An unknown value simply never matches, and the device
    /// falls back to SDL's built-in default mapping.
    /// </para>
    /// <para>
    /// <b>Standard byte layout.</b> Most records (USB and Bluetooth devices on all
    /// platforms) pack little-endian hardware IDs into the 16 bytes:
    /// </para>
    /// <list type="table">
    ///   <listheader><term>Hex chars</term><description>Content</description></listheader>
    ///   <item><term>1–4</term><description>Bus/transport type: <c>0300</c> = USB, <c>0500</c> = Bluetooth, <c>0600</c>=  virtual device, <c>0000</c> = unknown/legacy.</description></item>
    ///   <item><term>5–8</term><description>Zero padding.</description></item>
    ///   <item><term>9–12</term><description>USB vendor ID, little-endian (e.g. <c>c82d</c> = 0x2dc8 = 8BitDo).</description></item>
    ///   <item><term>13–16</term><description>Zero padding (overlaps where a UUID's version nibble would live - one reason these are never valid UUIDs).</description></item>
    ///   <item><term>17–20</term><description>USB product ID, little-endian.</description></item>
    ///   <item><term>21–24</term><description>Zero padding.</description></item>
    ///   <item><term>25–28</term><description>Device version, little-endian (often zero).</description></item>
    ///   <item><term>29–32</term><description>Zero padding or driver signature.</description></item>
    /// </list>
    /// <para>
    /// <b>Per-platform sections.</b> The database is divided into platform sections,
    /// and each platform encodes GUIDs slightly differently, so the same physical
    /// controller may carry a different GUID in each section. Use the record from the
    /// section matching the OS the app runs on:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     <c>Windows</c> - standard USB layout for practically every record, plus the
    ///     literal value <c>xinput</c>, which is not a device ID at all: it matches
    ///     every XInput controller through the XInput driver, which exposes no
    ///     per-model IDs.
    ///   </description></item>
    ///   <item><description>
    ///     <c>Mac OS X</c> - standard USB layout, plus the Bluetooth layout
    ///     (<c>0500…</c>) for wireless devices.
    ///   </description></item>
    ///   <item><description>
    ///     <c>Linux</c> - standard USB and Bluetooth layouts, plus
    ///     <c>0600…</c> for kernel-virtual devices (emulated pads) and a handful of
    ///     legacy handheld records with nonstandard or unknown bus prefixes
    ///     (<c>0000…</c>, <c>1900…</c>), plus one <c>xinput</c> record.
    ///   </description></item>
    ///   <item><description>
    ///     <c>Android</c> - Bluetooth layout for wireless devices; for the rest, SDL
    ///     on Android frequently cannot read USB vendor/product IDs, so the GUID
    ///     embeds an ASCII device-name signature instead (e.g.
    ///     <c>38426974446f…</c> decodes to the text <c>8BitDo …</c>). These values
    ///     are name-based, not ID-based.
    ///   </description></item>
    ///   <item><description>
    ///     <c>iOS</c> - Bluetooth layout (<c>0500…</c>) for MFi-certified
    ///     controllers, some with an ASCII <c>MF</c> (MFi) signature prefix.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Note that this library registers only the section matching the current desktop
    /// OS (Windows, macOS, or Linux); Android and iOS sections are ignored there.
    /// </para>
    /// </remarks>
    public Guid GUID { get; internal set; }

    public string Name { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerState"/> struct.
    /// </summary>
    public GameControllerState() => InitButtonStates();

    /// <summary>
    /// Returns a list of all the buttons that are currently in the down position.
    /// </summary>
    /// <returns>The list of buttons.</returns>
    public Span<ControllerButton> GetDownButtons()
    {
        InitButtonStates();

        var results = new List<ControllerButton>();
        foreach ((ControllerButton key, var state) in ButtonStates)
        {
            if (state)
            {
                results.Add(key);
            }
        }

        return results.ToArray().AsSpan();
    }

    /// <summary>
    /// Returns a value indicating if the given <paramref name="button"/> is in the down position.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>The down state of the button.</returns>
    public bool IsButtonDown(ControllerButton button)
    {
        InitButtonStates();

        return ButtonStates.TryGetValue(button, out var value) && value;
    }

    // TODO: Add IsButtonUp method

    /// <summary>
    /// Gets the direction/angle of the given <paramref name="thumbstick"/> in degrees.
    /// </summary>
    /// <param name="thumbstick">The thumbstick to get the angle from.</param>
    /// <returns>The angle in degrees.</returns>
    /// <remarks>
    ///     <para>
    ///         The angle increases in the clockwise direction as follows:
    ///         <list type="bullet">
    ///             <item>A value of <c>0/360</c> represents the stick being pushed up.</item>
    ///             <item>A value of <c>90</c> represents the stick being pushed to the right.</item>
    ///             <item>A value of <c>180</c> represents the stick being pushed down.</item>
    ///             <item>A value of <c>270</c> represents the stick being pushed to the left.</item>
    ///         </list>
    ///     </para>
    /// <para>
    ///     A value of <c>-1</c> represents that the stick at its resting state in the center.
    /// </para>
    /// </remarks>
    public float GetThumbstickDirection(Thumbsticks thumbstick)
    {
        const float deadZone = 0.01f;

        var thumbDirection = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.Direction
            : this.RightGameCtrlThumbstick.Direction;

        var thumbX = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.X
            : this.RightGameCtrlThumbstick.X;

        var thumbY = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.Y
            : this.RightGameCtrlThumbstick.Y;

        var angleDegrees = (thumbDirection + (float)Math.PI) * 180f / (float)Math.PI;

        if (Math.Abs(thumbX) < deadZone && Math.Abs(thumbY) < deadZone)
        {
            return -1f;
        }

        angleDegrees = (angleDegrees - 90) % 360;

        return angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;
    }

    /// <summary>
    /// Gets the position of the <paramref name="trigger"/> as a value between <c>0%</c> and <c>100%</c>.
    /// </summary>
    /// <param name="trigger">The <paramref name="trigger"/> to check.</param>
    /// <returns>The position of the <paramref name="trigger"/>.</returns>
    public float GetTriggerPosition(Triggers trigger)
    {
        var triggerPos = trigger == Triggers.Left
            ? this.LeftGameCtrlTrigger.Position
            : this.RightGameCtrlTrigger.Position;

        return Enabled ? triggerPos.MapValue(0f, 1f, 0f, 100f) : 0f;
    }

    /// <summary>
    /// Gets the X position of the <paramref name="thumbstick"/> as a value between <c>-1</c> and <c>1</c>.
    /// </summary>
    /// <param name="thumbstick">The <paramref name="thumbstick"/> to check.</param>
    /// <remarks>
    ///     The value <c>-1</c> represents the stick pushed to the left of center. <c>1</c> represents
    ///     the stick pushed to the right of center. <c>0</c> represents the stick in the center and in the center.
    /// </remarks>
    /// <returns>The position of the <paramref name="thumbstick"/>.</returns>
    public float GetThumbstickX(Thumbsticks thumbstick)
    {
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                return Enabled ? this.LeftGameCtrlThumbstick.X : 0f;
            case Thumbsticks.Right:
            default:
                return Enabled ? this.RightGameCtrlThumbstick.X : 0f;
        }
    }

    /// <summary>
    /// Gets the Y position of the <paramref name="thumbstick"/> as a value between <c>-1</c> and <c>1</c>.
    /// </summary>
    /// <param name="thumbstick">The <paramref name="thumbstick"/> to check.</param>
    /// <remarks>
    ///     The value <c>-1</c> represents the stick pushed to the up from the center. <c>1</c> represents
    ///     the stick pushed to the down from the center. <c>0</c> represents the stick in the center and in the center.
    /// </remarks>
    /// <returns>The position of the <paramref name="thumbstick"/>.</returns>
    public float GetThumbstickY(Thumbsticks thumbstick)
    {
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                return Enabled ? this.LeftGameCtrlThumbstick.Y : 0f;
            case Thumbsticks.Right:
            default:
                return Enabled ? this.RightGameCtrlThumbstick.Y : 0f;
        }
    }

    /// <summary>
    /// Gets the position of the <paramref name="thumbstick"/> as a value between <c>0</c> and <c>1</c>.
    /// </summary>
    /// <param name="thumbstick">The <paramref name="thumbstick"/> to check.</param>
    /// <returns>
    ///     The position represents the distance from the center pushed to the maximum value
    ///     no matter what direction the thumbstick is pushed.
    /// </returns>
    public float GetThumbstickPosition(Thumbsticks thumbstick)
    {
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                return Enabled ? this.LeftGameCtrlThumbstick.Position : 0f;
            case Thumbsticks.Right:
            default:
                return Enabled ? this.RightGameCtrlThumbstick.Position : 0f;
        }
    }

    /// <summary>
    /// Initializes the button states if they are null.
    /// </summary>
    private void InitButtonStates() => ButtonStates ??= new (Capacity);
}
