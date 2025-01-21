﻿namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ConcertoContactWithSlimeUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ConcertoContactWithSlimeUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (State.LimitBreak is not PiperConcerto { SlimeContactTimer: > 0 } concerto)
        {
            this.Disable();
            return;
        }

        // countdown contact timer
        if (Game1.game1.ShouldTimePass())
        {
            concerto.SlimeContactTimer--;
        }
    }
}
