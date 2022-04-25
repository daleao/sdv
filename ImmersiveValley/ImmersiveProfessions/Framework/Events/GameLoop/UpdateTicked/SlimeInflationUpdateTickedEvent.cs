using DaLion.Stardew.Professions.Framework.Extensions;

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
        var uninflated = ModEntry.PlayerState.PipedSlimes.Where(s => !s.ReadDataAs<bool>("DoneInflating")).ToArray();
        if (!uninflated.Any())
        {
            Disable();
            return;
        }

        foreach (var piped in uninflated) piped.Inflate();
    }
}