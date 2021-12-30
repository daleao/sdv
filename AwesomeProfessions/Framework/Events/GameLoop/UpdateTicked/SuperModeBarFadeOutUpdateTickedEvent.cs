using System;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeBarFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    private const int FADE_OUT_DELAY_I = 120, FADE_OUT_DURATION_I = 30;

    private int _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;

    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        --_fadeOutTimer;
        if (_fadeOutTimer >= FADE_OUT_DURATION_I) return;

        var ratio = (float) _fadeOutTimer / FADE_OUT_DURATION_I;
        ModState.SuperModeGaugeAlpha = (float) (-1.0 / (1.0 + Math.Exp(12.0 * ratio - 6.0)) + 1.0);

        if (_fadeOutTimer > 0) return;

        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarRenderingHudEvent), GetType());
        ModState.SuperModeGaugeAlpha = 1f;
    }
}