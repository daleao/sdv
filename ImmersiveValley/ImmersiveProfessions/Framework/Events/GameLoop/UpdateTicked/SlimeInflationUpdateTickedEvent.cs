namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Data;
using Common.Events;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var uninflated = ModEntry.PlayerState.PipedSlimes.Where(c => !ModDataIO.ReadDataAs<bool>(c, "DoneInflating"))
            .ToArray();
        if (!uninflated.Any())
        {
            Unhook();
            return;
        }

        foreach (var piped in uninflated) piped.Inflate();
    }
}