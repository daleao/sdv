using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeBarShakeTimerUpdateTickedEvent : UpdateTickedEvent
{
    private const int TICKS_BETWEEN_SHAKES_I = 126, SHAKE_DURATION_I = 15;

    private int _shakeTimer, _nextShake;

    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        if (_shakeTimer > 0)
        {
            ModState.ShouldShakeSuperModeGauge = true;
            --_shakeTimer;
        }
        else
        {
            ModState.ShouldShakeSuperModeGauge = false;
        }

        --_nextShake;
        if (_nextShake > 0) return;

        _shakeTimer = SHAKE_DURATION_I;
        _nextShake = TICKS_BETWEEN_SHAKES_I;
    }
}