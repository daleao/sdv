namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Data;
using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SlimeDeflationUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var undeflated = ModEntry.PlayerState.PipedSlimes.Where(c => ModDataIO.ReadDataAs<double>(c, "PipeTimer") <= 0)
            .ToArray();
        foreach (var piped in undeflated)
            piped.Deflate();

        if (!ModEntry.PlayerState.PipedSlimes.Any()) Unhook();
    }
}