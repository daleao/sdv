namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Core;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BruteWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BruteWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BruteWarpedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => State.BruteRageCounter > 0;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.IsDungeon() && !CoreMod.State.AreEnemiesNearby)
        {
            State.BruteRageCounter = 0;
        }
    }
}
