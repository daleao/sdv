﻿namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateOverlayFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateOverlayFadeOutUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateOverlayFadeOutUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.Get_Ultimate()!.Overlay.FadeOut();
    }
}