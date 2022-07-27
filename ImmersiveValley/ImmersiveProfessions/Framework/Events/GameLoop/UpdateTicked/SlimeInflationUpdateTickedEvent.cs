namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using System.Linq;
using VirtualProperties;

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
        var uninflated = GreenSlime_Piped.Values.Select(pair => pair.Key).Where(slime => !slime.get_Inflated()).ToArray();
        if (uninflated.Length == 0)
        {
            Disable();
            return;
        }

        foreach (var piped in uninflated) piped.Inflate();
    }
}