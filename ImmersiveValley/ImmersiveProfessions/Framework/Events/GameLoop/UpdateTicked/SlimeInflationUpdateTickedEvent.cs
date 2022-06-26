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
internal sealed class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SlimeInflationUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
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