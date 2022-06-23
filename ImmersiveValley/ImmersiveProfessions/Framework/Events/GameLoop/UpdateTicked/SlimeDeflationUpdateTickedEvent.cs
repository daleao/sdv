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
internal sealed class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var undeflated = ModEntry.PlayerState.PipedSlimes.Where(c => ModDataIO.ReadDataAs<double>(c, "PipeTimer") <= 0)
            .ToArray();
        foreach (var piped in undeflated)
            piped.Deflate();

        if (!ModEntry.PlayerState.PipedSlimes.Any()) Unhook();
    }
}