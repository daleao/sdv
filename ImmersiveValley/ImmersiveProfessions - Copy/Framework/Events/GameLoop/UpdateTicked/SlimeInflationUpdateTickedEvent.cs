namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Common.ModData;
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
        var uninflated = ModEntry.Player.PipedSlimes.Where(c => !ModDataIO.Read<bool>(c, "DoneInflating"))
            .ToArray();
        if (uninflated.Length <= 0)
        {
            Unhook();
            return;
        }

        foreach (var piped in uninflated) piped.Inflate();
    }
}