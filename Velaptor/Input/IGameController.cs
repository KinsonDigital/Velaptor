namespace Velaptor.Input;

public interface IGameController : IAppInput<GameControllerState>
{
    bool Enabled { get; set; }

    bool HasRumble(string guid);

    void RumbleLight(float intensity);

    void RumbleLight(float intensity, uint durationMs);

    void RumbleHeavy(float intensity);

    void RumbleHeavy(float intensity, uint durationMs);

    void StopLightRumble();

    void StopHeavyRumble();

    void StopAllRumble();
}
