using StardewModdingAPI.Events;
using System;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        var undeflatedSlimes = ModEntry.State.Value.PipedSlimeScales.Keys.ToList();
        for (var i = undeflatedSlimes.Count - 1; i >= 0; --i)
        {
            undeflatedSlimes[i].Scale = Math.Max(undeflatedSlimes[i].Scale / 1.1f,
                ModEntry.State.Value.PipedSlimeScales[undeflatedSlimes[i]]);
            if (!(undeflatedSlimes[i].Scale <= ModEntry.State.Value.PipedSlimeScales[undeflatedSlimes[i]])) continue;

            undeflatedSlimes[i].willDestroyObjectsUnderfoot = false;
            undeflatedSlimes.RemoveAt(i);
        }

        if (undeflatedSlimes.Any()) return;

        ModEntry.State.Value.PipedSlimeScales.Clear();
        ModEntry.Subscriber.UnsubscribeFrom(GetType());
    }
}