namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using StardewModdingAPI.Events;

#endregion using directives

internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
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
        Disable();
    }
}