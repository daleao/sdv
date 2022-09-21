namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlimeInflationUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SlimeInflationUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var uninflated = GreenSlime_Piped.Values.Select(pair => pair.Key).Where(slime => !slime.Get_Inflated())
            .ToArray();
        if (uninflated.Length == 0)
        {
            this.Disable();
            return;
        }

        foreach (var piped in uninflated)
        {
            piped.Inflate();
        }
    }
}
