namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;

#endregion using directives

internal class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var uninflatedSlimes = ModEntry.State.Value.SuperfluidSlimes.Where(p => !p.DoneInflating).ToArray();
        if (!uninflatedSlimes.Any())
        {
            Disable();
            return;
        }

        foreach (var piped in uninflatedSlimes) piped.Inflate();
    }
}