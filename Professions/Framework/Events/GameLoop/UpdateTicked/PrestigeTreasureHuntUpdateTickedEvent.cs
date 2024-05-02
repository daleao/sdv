namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using System.Linq;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeTreasureHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeTreasureHuntUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeTreasureHuntUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Farmer_TreasureHunt.HuntingState.AsEnumerable().All(pair => pair.Value.Value == false))
        {
            this.Disable();
        }

        Game1.gameTimeInterval = 0;
    }
}
