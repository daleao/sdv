namespace DaLion.Redux.Framework.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeTreasureHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeTreasureHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeTreasureHuntUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.gameTimeInterval = 0;
        if (Farmer_HuntingTreasure.Values.AsEnumerable().All(pair => pair.Value.Value == false))
        {
            this.Disable();
        }
    }
}
