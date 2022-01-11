﻿using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class SuperModeGaugeFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperMode.Gauge.FadeOut();
    }
}