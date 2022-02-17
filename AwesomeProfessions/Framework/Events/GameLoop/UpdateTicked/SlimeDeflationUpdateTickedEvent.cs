namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;

#endregion using directives

internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        foreach (var piped in ModEntry.State.Value.SuperfluidSlimes.Where(p => p.BuffTimer <= 0)) piped.Deflate();

        if (!ModEntry.State.Value.SuperfluidSlimes.Any()) Disable();
    }
}