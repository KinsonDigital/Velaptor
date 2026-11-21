// <copyright file="IGameControllerRumble.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;

internal interface IGameControllerRumble : IDisposable
{
    void Init();

    void ScanForControllers();

    void UpdateControllers(string[] controllers);

    float HeavyIntensity { get; }

    float LightIntensity { get; }

    bool HasRumble(string guid);

    void RumbleLight(float intensity);

    void RumbleLight(float intensity, uint durationMs);

    void RumbleHeavy(float intensity);

    void RumbleHeavy(float intensity, uint durationMs);

    void StopLightRumble();

    void StopHeavyRumble();

    void StopAllRumble();
}
