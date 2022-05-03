namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var uninflated = ModEntry.PlayerState.PipedSlimes.Where(s => !s.ReadDataAs<bool>("DoneInflating")).ToArray();
        if (!uninflated.Any())
        {
            this.Disable();
            return;
        }

        foreach (var piped in uninflated) piped.Inflate();
    }
}